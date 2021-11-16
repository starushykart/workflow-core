using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.IntegrationTests.Scenarios;
using WorkflowCore.Tests.CosmosDB;
using Xunit;

namespace WorkflowCore.Tests.DynamoDB.Scenarios
{
    [Collection("Cosmos DB collection")]
    public class CosmosDbBasicScenario : BasicScenario
    {        
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddWorkflow(x => x.UseCosmosDbPersistence(CosmosDbDockerSetup.ConnectionString, "tests"));
        }
    }
}
