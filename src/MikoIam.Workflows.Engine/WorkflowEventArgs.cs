using System;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowEventArgs : EventArgs
    {
        public WorkflowEventArgs(string workflowId)
        {
            WorkflowId = workflowId;
        }

        public string WorkflowId { get; }
    }
}