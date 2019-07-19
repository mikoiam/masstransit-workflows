using MassTransit;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit
{
    public static class ReceiveEndpointConfiguratorExtensions
    {
        public static void Workflow<TWfContext>(this IReceiveEndpointConfigurator configurator, Workflow<TWfContext> workflow) where TWfContext : new()
        {
            configurator.AddEndpointSpecification(new WorkflowConfigurator<TWfContext>(workflow));
        }
    }
}