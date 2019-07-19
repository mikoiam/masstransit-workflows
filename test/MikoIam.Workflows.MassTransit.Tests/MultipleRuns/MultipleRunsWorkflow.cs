using MikoIam.Workflows.Engine;
using MikoIam.Workflows.MassTransit.Tests.MessageDrivenTasks;

namespace MikoIam.Workflows.MassTransit.Tests.MultipleRuns
{
    public class MultipleRunsWorkflow : Workflow<MultipleRunsWorkflowContext>
    {
        public MultipleRunsWorkflow()
        {
            var a = new WorkflowTask<CompleteTaskAMessage>("A", () => { });
            var b = new WorkflowTask<CompleteTaskBMessage>("B", () => { });

            StartOn<StartWorkflowMessage>(msg => new MultipleRunsWorkflowContext { SomeProp = msg.Correlation});
            
            Initially().Do(a);
            After(a).Do(b);
            After(b).Finish();
        }
    }
}