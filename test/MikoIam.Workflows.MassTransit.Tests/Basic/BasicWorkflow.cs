using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class BasicWorkflow : Workflow
    {
        private WorkflowTask A { get; } = new WorkflowTask("A", () => { });
        private WorkflowTask B { get; } = new WorkflowTask("B", () => { });

        public BasicWorkflow()
        {
            StartOn<StartWorkflowMessage>();
            Initially().Do(A);
            After(A).Do(B);
            After(B).Finish();
        }
    }
}