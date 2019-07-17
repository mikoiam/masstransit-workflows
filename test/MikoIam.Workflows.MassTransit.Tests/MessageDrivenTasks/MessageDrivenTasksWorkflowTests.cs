using System;
using System.Threading.Tasks;
using MassTransit;
using Xunit;

namespace MikoIam.Workflows.MassTransit.Tests.MessageDrivenTasks
{
    public class MessageDrivenTasksWorkflowTests
    {
        private readonly SingleWorkflowObserver _observer;
        private readonly IBusControl _bus;

        public MessageDrivenTasksWorkflowTests()
        {
            var workflow = new MessageDrivenTasksWorkflow();
            _observer = new SingleWorkflowObserver(workflow);
            _bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("message_driven_task_workflow", ep => { ep.Workflow(workflow); });
            });
        }

        [Fact]
        public async Task ShouldExecuteWorkflowWhenTaskCompletionMessagesAreSent()
        {
            // Act
            _bus.Start();

            await _bus.Publish(new StartWorkflowMessage());
            var taskAStarted = _observer.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5)); 
            await _bus.Publish(new CompleteTaskAMessage());
            var taskBStarted = _observer.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5)); 
            await _bus.Publish(new CompleteTaskBMessage());
            var workflowFinished = _observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            _bus.Stop();

            // Assert
            Assert.True(taskAStarted);
            Assert.True(taskBStarted);
            Assert.True(workflowFinished);
            Assert.Equal("@WF-@A-$A-@B-$B-$WF", _observer.EventSequence);
        }
        
        [Fact]
        public async Task ShouldNotCompleteWorkflowWhenTaskCompletionEventsArriveInReverseOrder()
        {
            // Act
            _bus.Start();

            await _bus.Publish(new StartWorkflowMessage());
            var taskAStarted = _observer.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5)); 
            await _bus.Publish(new CompleteTaskBMessage());
            var workflowFinished = _observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            _bus.Stop();

            // Assert
            Assert.True(taskAStarted);
            Assert.False(workflowFinished);
            Assert.Equal("@WF-@A", _observer.EventSequence);
        }
        
        [Fact]
        public async Task ShouldNotFinishWorkflowWhenTaskCompletionMessageIsNotSent()
        {
            // Act
            _bus.Start();

            await _bus.Publish(new StartWorkflowMessage());
            var taskAStarted = _observer.TaskStartedHandle.WaitOne(TimeSpan.FromSeconds(5)); 
            await _bus.Publish(new CompleteTaskAMessage());
            var workflowFinished = _observer.WorkflowFinishedHandle.Wait(TimeSpan.FromSeconds(5));

            _bus.Stop();

            // Assert
            Assert.True(taskAStarted);
            Assert.False(workflowFinished);
            Assert.Equal("@WF-@A-$A-@B", _observer.EventSequence);
        }
    }
}