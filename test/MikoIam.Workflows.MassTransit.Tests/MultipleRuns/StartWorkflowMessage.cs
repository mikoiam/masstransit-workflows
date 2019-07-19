namespace MikoIam.Workflows.MassTransit.Tests.MultipleRuns
{
    public class StartWorkflowMessage
    {
        public string Correlation { get; }

        public StartWorkflowMessage(string correlation)
        {
            Correlation = correlation;
        }
    }
}