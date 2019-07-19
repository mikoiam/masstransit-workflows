using System;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.ContextProcessing
{
    public class ContextProcessingWorkflow : Workflow<Context>
    {
        public ContextProcessingWorkflow(Action<int> requestMultiplication, Action<int> reportResult)
        {
            var multiply = this.CreateTask("Multiply", ctx => requestMultiplication(ctx.Result),
                (Context ctx, NumberMultipliedMessage msg) => true,
                (context, message) => context.Result = message.MultipliedNumber);
            
            var report = this.CreateTask("Report", ctx => reportResult(ctx.Result));

            StartOn<StartWorkflowMessage>(msg => new Context {Result = msg.Number});
            Initially().Do(multiply);
            After(multiply).Do(report);
            After(report).Finish();
        }
    }
}