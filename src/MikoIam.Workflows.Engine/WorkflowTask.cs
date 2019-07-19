using System;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowTask<TWfContext>
    {
        public virtual bool Autocomplete => true;
        public string TaskId { get; }
        public Action<TWfContext> Action { get; }

        public WorkflowTask(string taskId, Action<TWfContext> action)
        {
            TaskId = taskId;
            Action = action;
        }
    }

    public class WorkflowTask<TWfContext, TCompleteMessage> : WorkflowTask<TWfContext>
    {
        public Func<TWfContext, TCompleteMessage, bool> WorkflowRunSelector { get; }
        public Action<TWfContext, TCompleteMessage> AfterMessageReceived { get; }
        
        public override bool Autocomplete => false;

        public WorkflowTask(string taskId, Action<TWfContext> action,
            Func<TWfContext, TCompleteMessage, bool> workflowRunSelector,
            Action<TWfContext, TCompleteMessage> afterMessageReceived) :
            base(taskId, action)
        {
            WorkflowRunSelector = workflowRunSelector;
            AfterMessageReceived = afterMessageReceived;
        }
        
        public WorkflowTask(string taskId, Action<TWfContext> action,
            Func<TWfContext, TCompleteMessage, bool> workflowRunSelector) :
            base(taskId, action)
        {
            WorkflowRunSelector = workflowRunSelector;
        }
    }
}