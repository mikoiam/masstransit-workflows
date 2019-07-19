using System;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowEventArgs<TWfContext> : EventArgs
    {
        public WorkflowEventArgs(string workflowId, TWfContext context)
        {
            WorkflowId = workflowId;
            Context = context;
        }

        public string WorkflowId { get; }
        public TWfContext Context { get; }
    }
}