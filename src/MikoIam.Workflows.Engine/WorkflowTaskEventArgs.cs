namespace MikoIam.Workflows.Engine
{
    public class WorkflowTaskEventArgs : WorkflowEventArgs
    {
        public WorkflowTaskEventArgs(string workflowId, string taskId) : base(workflowId)
        {
            TaskId = taskId;
        }

        public string TaskId { get; }
    }
}