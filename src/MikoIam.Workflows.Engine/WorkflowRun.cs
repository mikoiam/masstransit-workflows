using System;
using System.Collections.Generic;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowRun<TWfContext> where TWfContext : new()
    {
        private readonly List<WorkflowTask> _finishedTasks = new List<WorkflowTask>();
        private readonly List<WorkflowTask> _startedTasks = new List<WorkflowTask>();

        public string RunId { get; }
        public TWfContext Context { get; }
        public IReadOnlyCollection<WorkflowTask> FinishedTasks => _finishedTasks;
        public IReadOnlyCollection<WorkflowTask> StartedTasks => _startedTasks;


        public WorkflowRun(TWfContext context)
        {
            Context = context;
            RunId = Guid.NewGuid().ToString();
        }

        public void StartTask(WorkflowTask task)
        {
            _startedTasks.Add(task);
        }

        public bool HasStarted(WorkflowTask task) => _startedTasks.Contains(task);

        public void FinishTask(WorkflowTask task)
        {
            _finishedTasks.Add(task);
        }

        public bool HasFinished(WorkflowTask task) => _finishedTasks.Contains(task);
    }
}