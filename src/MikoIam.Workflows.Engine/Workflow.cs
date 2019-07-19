using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MikoIam.Workflows.Engine
{
    public class Workflow<TWfContext> where TWfContext : new()
    {
        private readonly Dictionary<Type, Delegate> _startsOn = new Dictionary<Type, Delegate>();

        private readonly Dictionary<WorkflowTask<TWfContext>, Type> _continuesOn =
            new Dictionary<WorkflowTask<TWfContext>, Type>();

        private readonly Dictionary<WorkflowTask<TWfContext>, Precondition> _tasks =
            new Dictionary<WorkflowTask<TWfContext>, Precondition>();

        private readonly List<Precondition> _finish = new List<Precondition>();
        private readonly ConcurrentBag<WorkflowRun<TWfContext>> _runs = new ConcurrentBag<WorkflowRun<TWfContext>>();

        public event EventHandler<WorkflowEventArgs<TWfContext>> WorkflowStarted;
        public event EventHandler<WorkflowEventArgs<TWfContext>> WorkflowFinished;
        public event EventHandler<WorkflowTaskEventArgs<TWfContext>> TaskStarted;
        public event EventHandler<WorkflowTaskEventArgs<TWfContext>> TaskFinished;

        public IEnumerable<Type> ConsumedMessages =>
            _startsOn.Select(kv => kv.Key).Concat(_continuesOn.Select(kv => kv.Value)).Distinct();

        protected Precondition After(WorkflowTask<TWfContext> workflowTask)
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
            if (_startsOn.ContainsKey(typeof(TMessage)))
            {
                var run = new WorkflowRun<TWfContext>(
                    ((Func<TMessage, TWfContext>) _startsOn[typeof(TMessage)])(message));
                _runs.Add(run);
                WorkflowStarted?.Invoke(this, new WorkflowEventArgs<TWfContext>(run.RunId, run.Context));
                IterateWorkflow(run);
                return;
            }

            var runs = _runs.ToArray();
            var tasksToFinish = _continuesOn.Where(kv => kv.Value == typeof(TMessage)).Select(kv => kv.Key);
            foreach (var workflowTask in tasksToFinish)
            {
                var messageBasedTask = (WorkflowTask<TWfContext, TMessage>) workflowTask;
                var runsToProcess = runs.Where(r =>
                        (messageBasedTask).WorkflowRunSelector(r.Context, message))
                    .Where(r => r.HasStarted(workflowTask));
                foreach (var run in runsToProcess)
                {
                    messageBasedTask.AfterMessageReceived?.Invoke(run.Context, message);
                    TaskFinished?.Invoke(this,
                        new WorkflowTaskEventArgs<TWfContext>(run.RunId, workflowTask.TaskId, run.Context));
                    run.FinishTask(workflowTask);
                    IterateWorkflow(run);
                }
            }
        }

        private void IterateWorkflow(WorkflowRun<TWfContext> run)
        {
            if (_finish.Exists(precondition => precondition.Met(run.FinishedTasks)))
            {
                WorkflowFinished?.Invoke(this, new WorkflowEventArgs<TWfContext>(run.RunId, run.Context));
                return;
            }

            var tasksToExecute = _tasks.Where(kv => kv.Value.Met(run.FinishedTasks) && !run.HasFinished(kv.Key))
                .Select(kv => kv.Key).ToList();

            var anythingFinished = false;
            foreach (var task in tasksToExecute)
            {
                task.Action(run.Context);
                run.StartTask(task);
                TaskStarted?.Invoke(this, new WorkflowTaskEventArgs<TWfContext>(run.RunId, task.TaskId, run.Context));
                if (task.Autocomplete)
                {
                    TaskFinished?.Invoke(this,
                        new WorkflowTaskEventArgs<TWfContext>(run.RunId, task.TaskId, run.Context));
                    run.FinishTask(task);
                    anythingFinished = true;
                }
            }

            if (anythingFinished)
            {
                IterateWorkflow(run);
            }
        }
        
        protected class Precondition
        {
            private readonly Workflow<TWfContext> _workflow;
            private readonly WorkflowTask<TWfContext> _workflowTask;

            public Precondition(Workflow<TWfContext> workflow)
            {
                _workflow = workflow;
            }

            public Precondition(Workflow<TWfContext> workflow, WorkflowTask<TWfContext> workflowTask) : this(workflow)
            {
                _workflowTask = workflowTask;
            }

            public void Do(WorkflowTask<TWfContext> workflowTask)
            {
                _workflow._tasks.Add(workflowTask, this);
            }

            public void Do<TCompleteMessage>(WorkflowTask<TWfContext, TCompleteMessage> workflowTask)
            {
                _workflow._tasks.Add(workflowTask, this);
                _workflow._continuesOn.Add(workflowTask, typeof(TCompleteMessage));
            }

            public void Finish()
            {
                _workflow._finish.Add(this);
            }

            public bool Met(IEnumerable<WorkflowTask<TWfContext>> finishedTasks)
            {
                return _workflowTask == null || finishedTasks.Contains(_workflowTask);
            }
        }
    }
}