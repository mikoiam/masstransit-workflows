using System;
using System.Threading.Tasks;
using MassTransit;
using Xunit;

namespace MikoIam.Workflows.MassTransit.Tests.ContextProcessing
{
    public class ContextProcessingWorkflowTests
    {
        private IBusControl _bus;
        const int InitialNumber = 3;
        const int Multiplier = 5;

        [Fact]
        public async Task ShouldExecuteWorkflow()
        {
            // Arrange
            var result = 0;

            var workflow = new ContextProcessingWorkflow(Multiply, number => result = number);
            var observer = new SingleWorkflowObserver<Context>(workflow, context => true);

            _bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("context_processing_workflow", ep => { ep.Workflow(workflow); });
            });

            // Act
            _bus.Start();

            await _bus.Publish(new StartWorkflowMessage(InitialNumber));
            var workflowFinished = observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            _bus.Stop();

            // Assert
            Assert.True(workflowFinished);
            Assert.Equal(InitialNumber * Multiplier, result);
        }

        private void Multiply(int number)
        {
            _bus.Publish(new NumberMultipliedMessage(number * Multiplier));
        }
    }
}