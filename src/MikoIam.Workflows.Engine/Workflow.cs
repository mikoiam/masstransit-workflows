using System;
using System.Collections.Generic;
using System.Linq;

namespace MikoIam.Workflows.Engine
{
    public class Workflow
    {
        private readonly List<Type> _startsOn = new List<Type>();
        private readonly Dictionary<WorkflowTask, Type> _continuesOn = new Dictionary<WorkflowTask, Type>();
        private readonly Dictionary<WorkflowTask, Precondition> _tasks = new Dictionary<WorkflowTask, Precondition>();
        private readonly List<Precondition> _finish = new List<Precondition>();
        private string _workflowId;
        private List<WorkflowTask> _finishedTasks = new List<WorkflowTask>();
        private List<WorkflowTask> _startedTasks = new List<WorkflowTask>();

        public event EventHandler<WorkflowEventArgs> WorkflowStarted;
        public event EventHandler<WorkflowEventArgs> WorkflowFinished;
        public event EventHandler<WorkflowTaskEventArgs> TaskStarted;
        public event EventHandler<WorkflowTaskEventArgs> TaskFinished;

        public IEnumerable<Type> ConsumedMessages => _startsOn.Concat(_continuesOn.Select(kv => kv.Value)).Distinct();

        protected Precondition After(WorkflowTask workflowTask)
        {
            return new Precondition(this, workflowTask);
        }

        protected Precondition Initially()
        {
            return new Precondition(this);
        }

        protected void StartOn<T>()
        {
            _startsOn.Add(typeof(T));
        }

        public void ConsumeMessage<T>(T message)
        {
            var iterate = false;
            if (_startsOn.Contains(typeof(T)))
            {
                _workflowId = Guid.NewGuid().ToString();
                _finishedTasks = new List<WorkflowTask>();
                _startedTasks = new List<WorkflowTask>();
                WorkflowStarted?.Invoke(this, new WorkflowEventArgs(_workflowId));
                iterate = true;
            }

            var tasks = _continuesOn.Where(kv => kv.Value == typeof(T)).Select(kv => kv.Key)
                .Where(task => _startedTasks.Contains(task));
            foreach (var workflowTask in tasks)
            {
                TaskFinished?.Invoke(this, new WorkflowTaskEventArgs(_workflowId, workflowTask.TaskId));
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
                WorkflowFinished?.Invoke(this, new WorkflowEventArgs(workflowId));
                return;
            }

            var tasksToExecute = _tasks.Where(kv => kv.Value.Met(finishedTasks) && !finishedTasks.Contains(kv.Key))
                .Select(kv => kv.Key).ToList();

            var anythingFinished = false;
            foreach (var task in tasksToExecute)
            {
                task.Action();
                _startedTasks.Add(task);
                TaskStarted?.Invoke(this, new WorkflowTaskEventArgs(workflowId, task.TaskId));
                if (task.Autocomplete)
                {
                    TaskFinished?.Invoke(this, new WorkflowTaskEventArgs(workflowId, task.TaskId));
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
            private readonly Workflow _workflow;
            private readonly WorkflowTask _workflowTask;

            public Precondition(Workflow workflow)
            {
                _workflow = workflow;
            }

            public Precondition(Workflow workflow, WorkflowTask workflowTask) : this(workflow)
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
}