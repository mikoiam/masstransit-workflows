using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.Empty
{
    public class EmptyWorkflow : Workflow
    {
        public EmptyWorkflow()
        {
            StartOn<StartWorkflowMessage>();
            Initially().Finish();
        }
    }
}