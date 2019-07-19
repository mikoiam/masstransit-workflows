namespace MikoIam.Workflows.Engine
{
    public class WorkflowTaskEventArgs<TWfContext> : WorkflowEventArgs<TWfContext>
    {
        public WorkflowTaskEventArgs(string workflowId, string taskId, TWfContext context) : base(workflowId, context)
        {
            TaskId = taskId;
        }

        public string TaskId { get; }
    }
}