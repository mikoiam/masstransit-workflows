using System;
using System.Collections.Generic;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowRun<TWfContext> where TWfContext : new()
    {
        private readonly List<WorkflowTask<TWfContext>> _finishedTasks = new List<WorkflowTask<TWfContext>>();
        private readonly List<WorkflowTask<TWfContext>> _startedTasks = new List<WorkflowTask<TWfContext>>();

        public string RunId { get; }
        public TWfContext Context { get; }
        public IReadOnlyCollection<WorkflowTask<TWfContext>> FinishedTasks => _finishedTasks;
        public IReadOnlyCollection<WorkflowTask<TWfContext>> StartedTasks => _startedTasks;


        public WorkflowRun(TWfContext context)
        {
            Context = context;
            RunId = Guid.NewGuid().ToString();
        }

        public void StartTask(WorkflowTask<TWfContext> task)
        {
            _startedTasks.Add(task);
        }

        public bool HasStarted(WorkflowTask<TWfContext> task) => _startedTasks.Contains(task);

        public void FinishTask(WorkflowTask<TWfContext> task)
        {
            _finishedTasks.Add(task);
        }

        public bool HasFinished(WorkflowTask<TWfContext> task) => _finishedTasks.Contains(task);
    }
}