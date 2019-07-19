using System;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowTask<TWfContext>
    {
        public virtual bool Autocomplete => true;
        public string TaskId { get; }
        public Action Action { get; }

        public WorkflowTask(string taskId, Action action)
        {
            TaskId = taskId;
            Action = action;
        }
    }

    public class WorkflowTask<TWfContext, TCompleteMessage > : WorkflowTask<TWfContext>
    {
        public Func<TWfContext, TCompleteMessage, bool> WorkflowRunSelector { get; }
        public override bool Autocomplete => false;

        public WorkflowTask(string taskId, Action action, Func<TWfContext, TCompleteMessage, bool> workflowRunSelector) :
            base(taskId, action)
        {
            WorkflowRunSelector = workflowRunSelector;
        }
    }
}