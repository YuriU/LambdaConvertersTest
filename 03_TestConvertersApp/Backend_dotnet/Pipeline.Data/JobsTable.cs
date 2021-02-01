using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal;
using Pipeline.Contracts;

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
                ConditionExpression = "attribute_not_exists(#converters.#converter)",
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

        public async Task<string> GetFileName(string jobId)
        {
            var item = await _dynamoDbClient.GetItemAsync(_tableName,
                new Dictionary<string, AttributeValue>() {{"id", new AttributeValue() {S = jobId}}});
            
            return item.Item["fileName"].S;
        }
        
        public async Task<ConversionJobFullInfo> GetJob(string jobId)
        {
            var job = await _dynamoDbClient.GetItemAsync(new GetItemRequest
            {
                Key = new Dictionary<string, AttributeValue> {{"id", new AttributeValue { S = jobId }}},
                TableName = _tableName,
                AttributesToGet = new List<string> {"id", "fileName", "started", "original", "conversionResults"}
            });

            if (job.Item != null)
            {
                return new ConversionJobFullInfo
                {
                    JobId = job.Item["id"].S,
                    FileName = job.Item["fileName"].S,
                    Started = long.Parse(job.Item["started"].N),
                    OriginalKey = job.Item["original"].M["key"].S,
                    ConversionStatuses = job.Item["conversionResults"].M.ToDictionary(
                        p => p.Key,
                        p => new ConversionResult
                        {
                            Successful = p.Value.M["sucessful"].BOOL,
                            Key = p.Value.M["key"].S
                        })
                };
            }
            else
            {
                return null;
            }
        }
        
        public async Task DeleteJob(string jobId)
        {
            var job = await _dynamoDbClient.DeleteItemAsync(new DeleteItemRequest
            {
                Key = new Dictionary<string, AttributeValue> {{"id", new AttributeValue { S = jobId }}},
                TableName = _tableName,
            });
        }

        public async Task<List<ConversionJobDto>> GetJobDtos()
        {
            var fileNames = await _dynamoDbClient.ScanAsync(_tableName, 
                new List<string>() { "id", "fileName", "started", "conversionResults" });
            var result = fileNames.Items.Select(i => new ConversionJobDto
            {
                JobId = i["id"].S,
                FileName = i["fileName"].S,
                Started = long.Parse(i["started"].N),
                ConversionStatuses = i["conversionResults"].M.ToDictionary(
                    p => p.Key, 
                    p => new ConversionStatus
                    {
                        Successful = p.Value.M["sucessful"].BOOL
                    })
            }).ToList();
            return result;
        }
    }
}