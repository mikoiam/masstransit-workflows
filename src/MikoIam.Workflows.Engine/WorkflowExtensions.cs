using System;

namespace MikoIam.Workflows.Engine
{
    public static class WorkflowExtensions
    {
        public static WorkflowTask<TWfContext> CreateTask<TWfContext>(
            this Workflow<TWfContext> workflow, string taskId, Action action) where TWfContext : new()
        {
            return new WorkflowTask<TWfContext>(taskId, action);
        }

        public static WorkflowTask<TWfContext, TCompleteMessage> CreateTask<TWfContext, TCompleteMessage>(
            this Workflow<TWfContext> workflow, string taskId, Action action,
            Func<TWfContext, TCompleteMessage, bool> workflowRunSelector) where TWfContext : new()
        {
            return new WorkflowTask<TWfContext, TCompleteMessage>(taskId, action, workflowRunSelector);
        }
    }
}