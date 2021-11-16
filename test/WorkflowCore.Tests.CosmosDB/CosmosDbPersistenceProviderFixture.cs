using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Providers.Azure.Services;
using WorkflowCore.UnitTests;
using Xunit;

namespace WorkflowCore.Tests.CosmosDB
{
    [Collection("Cosmos DB collection")]
    public class CosmosDbPersistenceProviderFixture : BasePersistenceFixture
    {
        CosmosDbDockerSetup _dockerSetup;
        private IPersistenceProvider _subject;

        public CosmosDbPersistenceProviderFixture(CosmosDbDockerSetup dockerSetup)
        {
            _dockerSetup = dockerSetup;
        }

        protected override IPersistenceProvider Subject
        {
            get
            {
                if (_subject == null)
                {
                    var cosmosDbStorageOptions = new CosmosDbStorageOptions();
                    var clientFactory = new CosmosClientFactory(CosmosDbDockerSetup.ConnectionString);
                    var provisioner = new CosmosDbProvisioner(clientFactory, cosmosDbStorageOptions);
                    var client = new CosmosDbPersistenceProvider(clientFactory, "tests", provisioner, cosmosDbStorageOptions);
                    
                    client.EnsureStoreExists();
                    _subject = client;
                }
                return _subject;
            }
        }
    }
}
