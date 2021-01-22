using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Pipeline.Data
{
    public class ActiveConnectionsTable
    {
        private readonly string _tableName;
        
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();

        public ActiveConnectionsTable(string tableName)
        {
            _tableName = tableName;
        }

        public async Task AddActiveConnection(string connectionId)
        {
            PutItemRequest ddbRequest = new PutItemRequest {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue> {{ "connectionId", new AttributeValue { S = connectionId }}}
            };

            PutItemResponse ddbResponse = await _dynamoDbClient.PutItemAsync(ddbRequest);
        }
        
        public async Task RemoveActiveConnection(string connectionId)
        {
            DeleteItemRequest ddbRequest = new DeleteItemRequest {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue> {{ "connectionId", new AttributeValue { S = connectionId }}},
            };

            DeleteItemResponse ddbResponse = await _dynamoDbClient.DeleteItemAsync(ddbRequest);
        }
        
        public async Task<List<string>> GetActiveConnections()
        {
            var connections = await _dynamoDbClient.ScanAsync(_tableName, 
                new List<string>() { "connectionId" });

            return connections.Items.Select(c => c["connectionId"].S).ToList();
        }
    }
}