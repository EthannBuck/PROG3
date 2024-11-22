using Microsoft.Azure.Cosmos.Table;
using PROG3.Models;
using System.Threading.Tasks;

namespace PROG3.Helpers
{
    public class TableStorageHelper
    {
        private CloudTable _cloudTable;

        public TableStorageHelper(string connectionString, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _cloudTable = tableClient.GetTableReference(tableName);
        }

        public async Task InsertOrMergeEntityAsync<T>(T entity) where T : TableEntity
        {
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await _cloudTable.ExecuteAsync(insertOrMergeOperation);
        }
        public async Task<IEnumerable<ClaimEntity>> GetClaimsByStatusAsync(string status)
        {
            var filter = TableQuery.GenerateFilterCondition("Status", QueryComparisons.Equal, status);
            var query = new TableQuery<ClaimEntity>().Where(filter);

            var claims = new List<ClaimEntity>();

            var result = _cloudTable.ExecuteQuery(query);

            foreach (var claim in result)
            {
                claims.Add(claim);
            }

            return claims;
        }

        public async Task<T> RetrieveEntityAsync<T>(string partitionKey, string rowKey) where T : TableEntity, new()
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                throw new ArgumentNullException("PartitionKey or RowKey cannot be null or empty");
            }

            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await _cloudTable.ExecuteAsync(retrieveOperation);
            return result.Result as T;
        }





    }
}
