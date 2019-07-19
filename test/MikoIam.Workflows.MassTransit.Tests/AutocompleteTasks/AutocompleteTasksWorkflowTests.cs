using System;
using System.Threading.Tasks;
using MassTransit;
using Xunit;

namespace MikoIam.Workflows.MassTransit.Tests.AutocompleteTasks
{
    public class AutocompleteTasksWorkflowTests
    {
        [Fact]
        public async Task ShouldExecuteWorkflow()
        {
            // Arrange
            var aCalled = false;
            var bCalled = false;
            var workflow = new AutocompleteTasksWorkflow(() => { aCalled = true; }, () => { bCalled = true; });
            var observer = new SingleWorkflowObserver<EmptyContext>(workflow, context => true);

            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("auto_complete_task_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            bus.Start();

            await bus.Publish(new StartWorkflowMessage());
            var workflowFinished = observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            bus.Stop();

            // Assert
            Assert.True(workflowFinished);
            Assert.Equal("@WF-@A-$A-@B-$B-$WF", observer.EventSequence);
            Assert.True(aCalled);
            Assert.True(bCalled);
        }
    }
}