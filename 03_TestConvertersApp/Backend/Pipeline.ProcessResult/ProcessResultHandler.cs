using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using Pipeline.Contracts;
using Pipeline.Data;
using Pipeline.Storage.Utils;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.ProcessResult
{
    public class ProcessResultHandler
    {
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
        
        private static AmazonS3Client S3Client = new AmazonS3Client();
        
        private readonly JobsTable _jobsTable = new JobsTable(Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME"));

        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");
        
        private readonly string _uploadResultBucketName = Environment.GetEnvironmentVariable("UPLOAD_RESULT_BUCKET_NAME");

        public async Task ProcessResult(SQSEvent @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    var result = JsonSerializer.Deserialize<FileProcessedEvent>(record.Body);
                    var jobId = result.JobId;

                    var fileName = await _jobsTable.GetFileName(jobId);
                    var convertedKey = StorageUtils.MakeConvertedFilePath(jobId, fileName, result.Converter, Path.GetExtension(result.ResultKey));

                    if (result.Sucessful)
                    {
                        await S3Client.CopyObjectAsync(
                            _uploadResultBucketName,
                            result.ResultKey,
                            _resultBucketName,
                            convertedKey
                        );
                        
                        await S3Client.DeleteObjectAsync(_uploadResultBucketName, result.ResultKey);
                    }
                    
                    await _jobsTable.SetConversionResult(jobId, result.Converter, result.Sucessful, convertedKey);
                }
            }
        }
    }
}