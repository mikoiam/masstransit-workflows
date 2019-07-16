using System;
using System.Collections.Generic;
using System.Linq;

namespace MikoIam.Workflows.Engine
{
    public class Workflow
    {
        private readonly List<Type> _startsOn = new List<Type>();
        private readonly Dictionary<WorkflowTask, Precondition> _tasks = new Dictionary<WorkflowTask, Precondition>();
        private readonly List<Precondition> _finish = new List<Precondition>();

        public event EventHandler<WorkflowEventArgs> WorkflowStarted;
        public event EventHandler<WorkflowEventArgs> WorkflowFinished;
        public event EventHandler<WorkflowTaskEventArgs> TaskStarted;
        public event EventHandler<WorkflowTaskEventArgs> TaskFinished;

        public IEnumerable<Type> StartsOn => _startsOn;

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
            if (!_startsOn.Contains(typeof(T)))
            {
                return;
            }

            var workflowId = Guid.NewGuid().ToString();
            WorkflowStarted?.Invoke(this, new WorkflowEventArgs(workflowId));

            IterateWorkflow(workflowId, new List<WorkflowTask>());
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
            
            foreach (var task in tasksToExecute)
            {
                task.Action();
                TaskStarted?.Invoke(this, new WorkflowTaskEventArgs(workflowId, task.TaskId));
                TaskFinished?.Invoke(this, new WorkflowTaskEventArgs(workflowId, task.TaskId));
            }

            IterateWorkflow(workflowId, new List<WorkflowTask>(finishedTasks.Concat(tasksToExecute)));
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