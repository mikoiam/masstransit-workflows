using System;
using System.Collections.Generic;
using System.Linq;

namespace MikoIam.Workflows.Engine
{
    public class Workflow<TWfContext> where TWfContext : new()
    {
        private readonly Dictionary<Type, Delegate> _startsOn = new Dictionary<Type, Delegate>();
        private readonly Dictionary<WorkflowTask, Type> _continuesOn = new Dictionary<WorkflowTask, Type>();
        private readonly Dictionary<WorkflowTask, Precondition> _tasks = new Dictionary<WorkflowTask, Precondition>();
        private readonly List<Precondition> _finish = new List<Precondition>();
        private string _workflowId;
        private List<WorkflowTask> _finishedTasks = new List<WorkflowTask>();
        private List<WorkflowTask> _startedTasks = new List<WorkflowTask>();
        private TWfContext _context;

        public event EventHandler<WorkflowEventArgs<TWfContext>> WorkflowStarted;
        public event EventHandler<WorkflowEventArgs<TWfContext>> WorkflowFinished;
        public event EventHandler<WorkflowTaskEventArgs<TWfContext>> TaskStarted;
        public event EventHandler<WorkflowTaskEventArgs<TWfContext>> TaskFinished;

        public IEnumerable<Type> ConsumedMessages =>
            _startsOn.Select(kv => kv.Key).Concat(_continuesOn.Select(kv => kv.Value)).Distinct();

        protected Precondition After(WorkflowTask workflowTask)
        {
            return new Precondition(this, workflowTask);
        }

        protected Precondition Initially()
        {
            return new Precondition(this);
        }

        protected void StartOn<TMessage>()
        {
            StartOn<TMessage>(msg => new TWfContext());
        }

        protected void StartOn<TMessage>(Func<TMessage, TWfContext> initializeContext)
        {
            _startsOn.Add(typeof(TMessage), initializeContext);
        }

        public void ConsumeMessage<TMessage>(TMessage message)
        {
            var iterate = false;
            if (_startsOn.ContainsKey(typeof(TMessage)))
            {
                _workflowId = Guid.NewGuid().ToString();
                _finishedTasks = new List<WorkflowTask>();
                _startedTasks = new List<WorkflowTask>();
                _context = ((Func<TMessage, TWfContext>) _startsOn[typeof(TMessage)])(message);
                WorkflowStarted?.Invoke(this, new WorkflowEventArgs<TWfContext>(_workflowId, _context));
                iterate = true;
            }

            var tasks = _continuesOn.Where(kv => kv.Value == typeof(TMessage)).Select(kv => kv.Key)
                .Where(task => _startedTasks.Contains(task));
            foreach (var workflowTask in tasks)
            {
                TaskFinished?.Invoke(this,
                    new WorkflowTaskEventArgs<TWfContext>(_workflowId, workflowTask.TaskId, _context));
                _finishedTasks.Add(workflowTask);
                iterate = true;
            }

            if (iterate)
            {
                IterateWorkflow(_workflowId, _finishedTasks);
            }
        }

        private void IterateWorkflow(string workflowId, IReadOnlyCollection<WorkflowTask> finishedTasks)
        {
            if (_finish.Exists(precondition => precondition.Met(finishedTasks)))
            {
                WorkflowFinished?.Invoke(this, new WorkflowEventArgs<TWfContext>(workflowId, _context));
                return;
            }

            var tasksToExecute = _tasks.Where(kv => kv.Value.Met(finishedTasks) && !finishedTasks.Contains(kv.Key))
                .Select(kv => kv.Key).ToList();

            var anythingFinished = false;
            foreach (var task in tasksToExecute)
            {
                task.Action();
                _startedTasks.Add(task);
                TaskStarted?.Invoke(this, new WorkflowTaskEventArgs<TWfContext>(workflowId, task.TaskId, _context));
                if (task.Autocomplete)
                {
                    TaskFinished?.Invoke(this,
                        new WorkflowTaskEventArgs<TWfContext>(workflowId, task.TaskId, _context));
                    _finishedTasks.Add(task);
                    anythingFinished = true;
                }
            }

            if (anythingFinished)
            {
                IterateWorkflow(workflowId, _finishedTasks);
            }
        }

        protected class Precondition
        {
            private readonly Workflow<TWfContext> _workflow;
            private readonly WorkflowTask _workflowTask;

            public Precondition(Workflow<TWfContext> workflow)
            {
                _workflow = workflow;
            }

            public Precondition(Workflow<TWfContext> workflow, WorkflowTask workflowTask) : this(workflow)
            {
                _workflowTask = workflowTask;
            }

            public void Do(WorkflowTask workflowTask)
            {
                _workflow._tasks.Add(workflowTask, this);
            }

            public void Do<TCompleteMessage>(WorkflowTask<TCompleteMessage> workflowTask)
            {
                _workflow._tasks.Add(workflowTask, this);
                _workflow._continuesOn.Add(workflowTask, typeof(TCompleteMessage));
            }

            public void Finish()
            {
                _workflow._finish.Add(this);
            }

            public bool Met(IEnumerable<WorkflowTask> finishedTasks)
            {
                return _workflowTask == null || finishedTasks.Contains(_workflowTask);
            }
        }
    }

    public class Workflow : Workflow<EmptyContext>
    {
    }
}