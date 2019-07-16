using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class BasicWorkflow : Workflow
    {
        public WorkflowTask A => new WorkflowTask("A", () => { });

        public void Setup()
        {
            StartOn<StartWorkflowMessage>();
            Initially().Do(A);
            After(A).Finish();
        }
    }
}