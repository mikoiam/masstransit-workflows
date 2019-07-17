using System.Threading.Tasks;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.MessageDrivenTasks
{
    public class MessageDrivenTasksWorkflow : Workflow
    {
        private WorkflowTask A { get; } = new WorkflowTask("A", () => { }).CompleteOn<CompleteTaskAMessage>();
        private WorkflowTask B { get; } = new WorkflowTask("B", () => { }).CompleteOn<CompleteTaskBMessage>();

        public MessageDrivenTasksWorkflow()
        {
            StartOn<StartWorkflowMessage>();
            Initially().Do(A);
            After(A).Do(B);
            After(B).Finish();
        }
    }
}