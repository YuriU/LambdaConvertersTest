using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Dummy.Converter
{
    public class Function
    {
        private static AmazonS3Client S3Client = new AmazonS3Client();
        
        /// <summary>
        /// A simple function that takes a string and returns both the upper and lower case version of the string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            LambdaLogger.Log("Event handler started");
            LambdaLogger.Log(JsonSerializer.Serialize(sqsEvent));
            LambdaLogger.Log("Event handler completed");
            /*var variables = Environment.GetEnvironmentVariables();
            Environment.GetEnvironmentVariable()
            foreach (DictionaryEntry variable in variables)
            {
                context.Logger.LogLine($"{variable.Key} = ${variable.Value}");
            }

            var serialized = JsonSerializer.Serialize(@event);
            
            LambdaLogger.Log(serialized);
            
            LambdaLogger.Log("Event handler started");
            
            var tokenSource = new CancellationTokenSource();
            foreach (var record in @event.Records)
            {
               var filePath = await DownloadFile(record.S3.Bucket.Name, record.S3.Object.Key, tokenSource.Token);
               
               File.Delete(filePath);
            }
            
            LambdaLogger.Log("Event handler completed");*/
        }

        private static async Task<string> DownloadFile(string bucketName, string key, CancellationToken cancellationToken)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key,
            };

            var tempPath= Path.GetTempPath();
            var extension = Path.GetExtension(key);
            var tempFileName = Guid.NewGuid().ToString();
            var filePath = Path.Combine(tempPath, $"{tempFileName}{extension}");
            
            LambdaLogger.Log($"Downloading {key} to {filePath}");

            using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
            {
                await response.WriteResponseStreamToFileAsync(filePath, false, cancellationToken);
            }
            
            LambdaLogger.Log($"Download completed {key} to {filePath}. File size is {new FileInfo(filePath).Length}");
            
            return filePath;
        }
    }
}
