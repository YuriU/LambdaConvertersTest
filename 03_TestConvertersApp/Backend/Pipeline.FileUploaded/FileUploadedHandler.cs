﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Pipeline.Contracts;
using Pipeline.Data;
using Pipeline.Storage.Utils;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.FileUploaded
{
    public class FileUploadedHandler
    {
        private readonly string _notifyTopicArn = Environment.GetEnvironmentVariable("FILE_UPLOADED_TOPIC_ARN");
        
        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");
        
        private readonly string _uploadResultBucketName = Environment.GetEnvironmentVariable("UPLOAD_RESULT_BUCKET_NAME");

        private readonly JobsTable _jobsTable = new JobsTable(Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME"));
        
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
                        var originalFileName = record.S3.Object.Key;
                        var jobId = Guid.NewGuid().ToString();
                        
                        // Creating job in DB
                        await _jobsTable.AddJob(jobId, originalFileName, DateTime.UtcNow);
                        
                        // Move file to dedicated job table
                        var srcFilePath = await MoveToDestinationBucket(jobId, record.S3.Bucket.Name, originalFileName);

                        // Set original file path
                        await _jobsTable.SetOriginalFile(jobId, srcFilePath);
                        
                        // Publish job started
                        LambdaLogger.Log($"Publishing to {_notifyTopicArn}");
                        await PublishFileUploaded(_notifyTopicArn, jobId, srcFilePath);
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
    
        private async Task<string> MoveToDestinationBucket(string jobId, string originalBucket, string originalFileName)
        {
            var originalFileKey = StorageUtils.MakeOriginalFilePath(jobId, originalFileName);
            await S3Client.CopyObjectAsync(
                originalBucket,
                originalFileName,
                _resultBucketName,
                originalFileKey
                );

            await S3Client.DeleteObjectAsync(originalBucket, originalFileName);
            return originalFileKey;
        }
        private async Task PublishFileUploaded(string topic, string jobId, string key)
        {
            var fileUploadedEvent = new FileUploadedEvent(jobId, _resultBucketName, key, _uploadResultBucketName);

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