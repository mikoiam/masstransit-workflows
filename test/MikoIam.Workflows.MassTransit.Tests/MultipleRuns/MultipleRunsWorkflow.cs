using System;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.MultipleRuns
{
    public class MultipleRunsWorkflow : Workflow<MultipleRunsWorkflowContext>
    {
        public MultipleRunsWorkflow()
        {
            var a = this.CreateTask("A", ctx => { },
                (MultipleRunsWorkflowContext ctx, CompleteTaskAMessage msg) => ctx.SomeProp == msg.Correlation);

            var b = this.CreateTask("B", ctx => { },
                (MultipleRunsWorkflowContext ctx, CompleteTaskBMessage msg) => ctx.SomeProp == msg.Correlation);

            StartOn<StartWorkflowMessage>(msg => new MultipleRunsWorkflowContext {SomeProp = msg.Correlation});

            Initially().Do(a);
            After(a).Do(b);
            After(b).Finish();
        }
    }
}