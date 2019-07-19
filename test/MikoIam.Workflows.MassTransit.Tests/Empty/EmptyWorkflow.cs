using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.Empty
{
    public class EmptyWorkflow : Workflow<EmptyContext>
    {
        public EmptyWorkflow()
        {
            StartOn<StartWorkflowMessage>();
            Initially().Finish();
        }
    }
}