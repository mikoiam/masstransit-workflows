using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Pipeline;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit
{
    public class WorkflowConfigurator<TWfContext> : IReceiveEndpointSpecification where TWfContext : new()
    {
        private readonly Workflow<TWfContext> _workflow;

        public WorkflowConfigurator(Workflow<TWfContext> workflow)
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

        private void ConnectPipe<TMessage>(IConsumePipeConnector pipeConnector) where TMessage : class
        {
            var pipe = Pipe.New<ConsumeContext<TMessage>>(cfg =>
            {
                cfg.UseExecute(ctx => _workflow.ConsumeMessage(ctx.Message));
            });

            pipeConnector.ConnectConsumePipe(pipe);
        }
    }
}