using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Pipeline.Contracts;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.FileUploaded
{
    public class FileUploadedHandler
    {
        private readonly string _notifyTopicArn = Environment.GetEnvironmentVariable("FILE_UPLOADED_TOPIC_ARN");
        
        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");

        private readonly string _conversionJobsTable = Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME");
        
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
        
        private static AmazonS3Client S3Client = new AmazonS3Client();
        
        private static AmazonSimpleNotificationServiceClient _snsClient = new AmazonSimpleNotificationServiceClient();
        
        public async Task OriginalFileUploaded(S3Event @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    try
                    {
                        var jobId = Guid.NewGuid().ToString();
                        var originalFileName = record.S3.Object.Key;
                        var srcFilePath = await MoveToDestinationBucket(jobId, record.S3.Bucket.Name, originalFileName);
                        
                        await _dynamoDbClient.PutItemAsync(_conversionJobsTable, new Dictionary<string, AttributeValue>()
                        {
                            { "id" , new AttributeValue  { S = jobId } },
                            { "fileName" , new AttributeValue { S = record.S3.Object.Key} },
                            { "started" , new AttributeValue { N = DateTime.UtcNow.Ticks.ToString() }}, 
                            { "conversionResults", new AttributeValue() { M = new AutoConstructedDictionary<string, AttributeValue> { }, IsMSet = true }}
                        });
                        
                        LambdaLogger.Log($"Publishing to {_notifyTopicArn}");
                        await PublishFileUploaded(_notifyTopicArn, jobId, srcFilePath, MakeJobFolderPath(jobId, originalFileName));
                    }
                    catch (Exception e)
                    {
                        LambdaLogger.Log("Error during publish message");
                        LambdaLogger.Log(e.ToString());
                        throw;
                    }
                }
            }
        }
        private static string MakeOriginalFileName(string jobId, string key)
        {
            return $"{jobId}/{key}/Original{Path.GetExtension(key)}";
        }
        
        private static string MakeJobFolderPath(string jobId, string key)
        {
            return $"{jobId}/{key}";
        }
        
        private async Task<string> MoveToDestinationBucket(string jobId, string originalBucket, string originalFileName)
        {
            var originalFileKey = MakeOriginalFileName(jobId, originalFileName);
            await S3Client.CopyObjectAsync(
                originalBucket,
                originalFileName,
                _resultBucketName,
                originalFileKey
                );

            await S3Client.DeleteObjectAsync(originalBucket, originalFileName);
            return originalFileKey;
        }
        private async Task PublishFileUploaded(string topic, string jobId, string key, string uploadPath)
        {
            var fileUploadedEvent = new FileUploadedEvent(jobId, _resultBucketName, key, _resultBucketName, uploadPath);

            var publishRequest = new PublishRequest
            {
                Message = JsonSerializer.Serialize(fileUploadedEvent),
                TopicArn = topic
            };

            await _snsClient.PublishAsync(publishRequest, CancellationToken.None)
                .ConfigureAwait(false);
        }
    }
}