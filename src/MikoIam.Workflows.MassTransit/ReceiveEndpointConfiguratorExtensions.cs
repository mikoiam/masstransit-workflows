using MassTransit;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit
{
    public static class ReceiveEndpointConfiguratorExtensions
    {
        public static void Workflow(this IReceiveEndpointConfigurator configurator, Workflow workflow)
        {
            configurator.AddEndpointSpecification(new WorkflowConfigurator(workflow));
        }
    }
}