using System;
using System.Threading.Tasks;
using FluentAssertions;
using WorkflowCore.Exceptions;
using WorkflowCore.Models;
using WorkflowCore.Services;
using Xunit;

namespace WorkflowCore.UnitTests.MemoryPersistenceProviderTests;

public class MemoryPersistenceProviderTests
{
    private readonly MemoryPersistenceProvider _provider = new();

    [Fact]
    public void Should_Throw_If_Workflow_With_Same_Reference_Already_Exist()
    {
        var testWorkflow = new WorkflowInstance()
        {
            Reference = Guid.NewGuid().ToString()
        };
        
        var act = async () =>
        {
            await _provider.CreateNewWorkflow(testWorkflow);
            await _provider.CreateNewWorkflow(testWorkflow);
        };

        act.ShouldThrow<WorkflowExistsException>();
    }
    
    [Fact]
    public void Should_not_throw_if_reference_is_null()
    {
        var testWorkflow = new WorkflowInstance()
        {
            Reference = null
        };
        
        var act = async () =>
        {
            await _provider.CreateNewWorkflow(testWorkflow);
            await _provider.CreateNewWorkflow(testWorkflow);
        };

        act.ShouldNotThrow();
    }
}