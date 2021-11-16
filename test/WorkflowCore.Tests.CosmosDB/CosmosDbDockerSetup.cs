using Docker.Testify;
using System;
using System.Collections.Generic;
using Xunit;

namespace WorkflowCore.Tests.CosmosDB
{
    public class CosmosDbDockerSetup : DockerSetup
    {
        public static string ConnectionString { get; set; }

        public override string ImageName => "mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator";

        public override int InternalPort => 8081;

        public override IList<string> EnvironmentVariables => new List<string> {
            "AZURE_COSMOS_EMULATOR_PARTITION_COUNT=3",
            "AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=127.0.0.1"
        };

        public override void PublishConnectionInfo()
        {
            ConnectionString = $"AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        }

        public override bool TestReady()
        {
            System.Threading.Thread.Sleep(10000);
            return true;
        }
    }

    [CollectionDefinition("Cosmos DB collection")]
    public class CosmosDbCollection : ICollectionFixture<CosmosDbDockerSetup>
    {
    }
}
