using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Pipeline.Contracts;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.ProcessResult
{
    public class ProcessResultHandler
    {
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
        
        private readonly string _conversionJobsTable = Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME");

        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");

        public async Task ProcessResult(SQSEvent @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    var result = JsonSerializer.Deserialize<FileProcessedEvent>(record.Body);
                    var jobId = result.JobId;

                    await _dynamoDbClient.UpdateItemAsync(new UpdateItemRequest()
                    {
                        TableName = _conversionJobsTable,
                        Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue() { S = jobId }}},
                        UpdateExpression = "SET #converters.#converter = :result",
                        ExpressionAttributeNames =
                        {
                            { "#converters", "conversionResults" },
                            { "#converter", result.Converter },
                        },
                        ExpressionAttributeValues =
                        {
                            { ":result", new AttributeValue() { M = new Dictionary<string, AttributeValue>()
                            {
                                { "sucessful", new AttributeValue() { BOOL = result.Sucessful }},
                                { "key", new AttributeValue() { S = result.ResultKey }}
                            }}}
                        }
                    });

                    /*Table table = Table.LoadTable(_dynamoDbClient, _resultBucketName);
                    
                    var resultDoc = new Document();
                    resultDoc["Success"] = result.Sucessful;
                    if (result.Sucessful)
                    {
                        resultDoc["Key"] = result.ResultKey;
                    }
                    
                    var book = new Document();
                    book["id"] = jobId;
                    book["ConversionResult"] = resultDoc;
                    
                    UpdateItemOperationConfig config = new UpdateItemOperationConfig
                    {
                        // Get updated item in response.
                        ReturnValues = ReturnValues.None
                    };
                    await table.UpdateItemAsync(book, config);*/
                }
            }
        }
    }
}