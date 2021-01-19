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
        
        public async Task SetOriginalFile(string jobId, string originalFileKey = null, Exception exception = null)
        {
            var originalFileResult = exception == null 
                ? new AttributeValue() { 
                        M = new Dictionary<string, AttributeValue>
                        {
                            { "key", new AttributeValue { S = originalFileKey }}
                        }
                  }
                : new AttributeValue() { 
                        M = new Dictionary<string, AttributeValue>
                        {
                            { "error", new AttributeValue { S = exception.Message }}
                        }
                  };
            
            await _dynamoDbClient.UpdateItemAsync(new UpdateItemRequest()
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue() { S = jobId }}},
                UpdateExpression = "SET #original = :result",
                ExpressionAttributeNames =
                {
                    { "#original", "original" },
                },
                ExpressionAttributeValues =
                {
                    { ":result", originalFileResult }
                }
            });
        }

        public async Task SetConversionResult(string jobId, string convertor, bool successful, string key)
        {
            await _dynamoDbClient.UpdateItemAsync(new UpdateItemRequest()
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue() { S = jobId }}},
                UpdateExpression = "SET #converters.#converter = :result",
                ConditionExpression = "if_not_exists(#converters, #converter)",
                ExpressionAttributeNames =
                {
                    { "#converters", "conversionResults" },
                    { "#converter", convertor },
                },
                ExpressionAttributeValues =
                {
                    { ":result", new AttributeValue() { M = new Dictionary<string, AttributeValue>()
                    {
                        { "sucessful", new AttributeValue() { BOOL = successful }},
                        { "key", new AttributeValue() { S = key }}
                    }}}
                }
            });
        }
    }
}