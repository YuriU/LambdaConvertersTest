using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Pipeline.Contracts;
using Pipeline.Data;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.ProcessResult
{
    public class ProcessResultHandler
    {
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
        
        private readonly JobsTable _jobsTable = new JobsTable(Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME"));

        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");

        public async Task ProcessResult(SQSEvent @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    var result = JsonSerializer.Deserialize<FileProcessedEvent>(record.Body);
                    var jobId = result.JobId;

                    await _jobsTable.SetConversionResult(jobId, result.Converter, result.Sucessful, result.ResultKey);
                }
            }
        }
    }
}