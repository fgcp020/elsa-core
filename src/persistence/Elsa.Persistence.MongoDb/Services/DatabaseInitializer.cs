using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services;
using MongoDB.Driver;

namespace Elsa.Persistence.MongoDb.Services
{
    public class DatabaseInitializer : IStartupTask
    {
        private readonly ElsaMongoDbContext _mongoContext;

        public DatabaseInitializer(ElsaMongoDbContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await CreateWorkflowInstancesIndexes(cancellationToken);
            await CreateWorkflowDefinitionsIndexes(cancellationToken);
            await CreateWorkflowExecutionLogIndexes(cancellationToken);
        }

        private async Task CreateWorkflowInstancesIndexes(CancellationToken cancellationToken)
        {
            var tenantKeysDefinition = Builders<WorkflowInstance>.IndexKeys.Ascending(x => x.TenantId);
            var versionKeysDefinition = Builders<WorkflowInstance>.IndexKeys.Ascending(x => x.Version);
            var workflowStatusKeysDefinition = Builders<WorkflowInstance>.IndexKeys.Ascending(x => x.WorkflowStatus);
            var workflowNameKeysDefinition = Builders<WorkflowInstance>.IndexKeys.Ascending(x => x.Name);

            await CreateIndexesAsync(_mongoContext.WorkflowInstances, cancellationToken, tenantKeysDefinition, versionKeysDefinition, workflowStatusKeysDefinition, workflowNameKeysDefinition);
        }

        private async Task CreateWorkflowDefinitionsIndexes(CancellationToken cancellationToken)
        {
            var tenantKeysDefinition = Builders<WorkflowDefinition>.IndexKeys.Ascending(x => x.TenantId);
            var definitionVersionIdKeysDefinition = Builders<WorkflowDefinition>.IndexKeys.Ascending(x => x.DefinitionVersionId);
            var versionKeysDefinition = Builders<WorkflowDefinition>.IndexKeys.Ascending(x => x.Version);
            var nameKeysDefinition = Builders<WorkflowDefinition>.IndexKeys.Ascending(x => x.Name);

            await CreateIndexesAsync(_mongoContext.WorkflowDefinitions, cancellationToken, tenantKeysDefinition, definitionVersionIdKeysDefinition, versionKeysDefinition, nameKeysDefinition);
        }

        private async Task CreateWorkflowExecutionLogIndexes(CancellationToken cancellationToken)
        {
            var tenantKeysDefinition = Builders<WorkflowExecutionLogRecord>.IndexKeys.Ascending(x => x.TenantId);
            var workflowInstanceIdKeysDefinition = Builders<WorkflowExecutionLogRecord>.IndexKeys.Ascending(x => x.WorkflowInstanceId);
            var timestampKeysDefinition = Builders<WorkflowExecutionLogRecord>.IndexKeys.Ascending(x => x.Timestamp);

            await CreateIndexesAsync(_mongoContext.WorkflowExecutionLog, cancellationToken, tenantKeysDefinition, workflowInstanceIdKeysDefinition, timestampKeysDefinition);
        }

        private async Task CreateWorkflowTriggerIndexes(CancellationToken cancellationToken)
        {
            var tenantKeysDefinition = Builders<WorkflowTrigger>.IndexKeys.Ascending(x => x.TenantId);
            var activityTypeKeysDefinition = Builders<WorkflowTrigger>.IndexKeys.Ascending(x => x.ActivityType);
            var workflowDefinitionKeysDefinition = Builders<WorkflowTrigger>.IndexKeys.Ascending(x => x.WorkflowDefinitionId);
            var workflowInstanceIdKeysDefinition = Builders<WorkflowTrigger>.IndexKeys.Ascending(x => x.WorkflowInstanceId);

            await CreateIndexesAsync(_mongoContext.WorkflowTriggers, cancellationToken, tenantKeysDefinition, activityTypeKeysDefinition, workflowDefinitionKeysDefinition, workflowInstanceIdKeysDefinition);
        }

        private async Task CreateIndexesAsync<T>(IMongoCollection<T> collection, CancellationToken cancellationToken, params IndexKeysDefinition<T>[] definitions)
        {
            var models = definitions.Select(x => new CreateIndexModel<T>(x));
            await collection.Indexes.CreateManyAsync(models, cancellationToken);
        }
    }
}