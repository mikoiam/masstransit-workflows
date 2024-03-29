using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.MessageDrivenTasks
{
    public class MessageDrivenTasksWorkflow : Workflow<EmptyContext>
    {
        public MessageDrivenTasksWorkflow()
        {
            var a = this.CreateTask("A", ctx => { }, (EmptyContext ctx, CompleteTaskAMessage msg) => true);
            var b = this.CreateTask("B", ctx => { }, (EmptyContext ctx, CompleteTaskBMessage msg) => true);

            StartOn<StartWorkflowMessage>();
            Initially().Do(a);
            After(a).Do(b);
            After(b).Finish();
        }
    }
}