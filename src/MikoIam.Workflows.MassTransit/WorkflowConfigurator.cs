using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Pipeline;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit
{
    public class WorkflowConfigurator : IReceiveEndpointSpecification
    {
        private readonly Workflow _workflow;

        public WorkflowConfigurator(Workflow workflow)
        {
            _workflow = workflow;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_workflow == null)
                yield return this.Failure("Workflow", "must not be null");
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            var connectPipeMethod =
                GetType().GetMethod(nameof(ConnectPipe), BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(connectPipeMethod != null, nameof(connectPipeMethod) + " != null");

            foreach (var type in _workflow.ConsumedMessages)
            {
                connectPipeMethod.MakeGenericMethod(type).Invoke(this, new object[] {builder});
            }
        }

        private void ConnectPipe<T>(IConsumePipeConnector pipeConnector) where T : class
        {
            var pipe = Pipe.New<ConsumeContext<T>>(cfg =>
            {
                cfg.UseExecute(ctx => _workflow.ConsumeMessage(ctx.Message));
            });

            pipeConnector.ConnectConsumePipe(pipe);
        }
    }
}