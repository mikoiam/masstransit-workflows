namespace MikoIam.Workflows.MassTransit.Tests.ContextProcessing
{
    public class StartWorkflowMessage
    {
        public int Number { get; }

        public StartWorkflowMessage(int number)
        {
            Number = number;
        }
    }
}