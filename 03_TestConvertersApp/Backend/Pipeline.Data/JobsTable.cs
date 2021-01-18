using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal;

namespace Pipeline.Data
{
    public class JobsTable
    {
        private readonly string _tableName;
        
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();

        public JobsTable(string tableName)
        {
            _tableName = tableName;
        }

        public async Task AddJob(string jobId, string fileName, DateTime started)
        {
            await _dynamoDbClient.PutItemAsync(_tableName, new Dictionary<string, AttributeValue>()
            {
                { "id" , new AttributeValue  { S = jobId } },
                { "fileName" , new AttributeValue { S = fileName } },
                { "started" , new AttributeValue { N = DateTime.UtcNow.Ticks.ToString() }}, 
                { "conversionResults", new AttributeValue() { M = new AutoConstructedDictionary<string, AttributeValue> { }, IsMSet = true }}
            });
        }
    }
}