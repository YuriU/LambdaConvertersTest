using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using Pipeline.Data;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Pipeline.GetDownloadUrl

{
    public class GetDownloadUrlHandler
    {
        private readonly JobsTable _jobsTable = new JobsTable(Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME"));
        
        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");
        
        private static AmazonS3Client S3Client = new AmazonS3Client();
        
        public async Task<APIGatewayProxyResponse> GetDownloadUrl(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var id = request.QueryStringParameters["id"];
            
            var job = await _jobsTable.GetJob(id);
            List<string> urls = new List<string>();
            if (request.QueryStringParameters.ContainsKey("converter"))
            {
                var converterName = request.QueryStringParameters["converter"];
                // Add check for errors
                var converterFileKey = job.ConversionStatuses[converterName].Key;

                var url = S3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
                {
                    Expires = DateTime.Now.AddSeconds(30),
                    BucketName = _resultBucketName,
                    Key = converterFileKey,
                });
                urls.Add(url);
            }
            else
            {
                var keys = job.ConversionStatuses
                    .Where(s => s.Value.Successful)
                    .Select(r => r.Value.Key)
                    .ToList();

                foreach (var key in keys)
                {
                    var url = S3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
                    {
                        Expires = DateTime.Now.AddSeconds(60),
                        BucketName = _resultBucketName,
                        Key = key,
                    });
                    
                    urls.Add(url);
                }
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(urls),
                IsBase64Encoded = true,
                Headers = new Dictionary<string, string>
                { 
                    { "Content-Type", "application/json" }, 
                    { "Access-Control-Allow-Origin", "*" },
                }
            };
        }
        
        private static async Task DownloadFile(string srcBucket, string srcKey, string destFileName)
        {
            var request = new GetObjectRequest { BucketName = srcBucket, Key = srcKey };
            using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
            {
                await response.WriteResponseStreamToFileAsync(destFileName, false, CancellationToken.None);
            }
        }
        
        public static byte[] GetZipArchive(List<InMemoryFile> files)
        {
            byte[] archiveFile;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(file.Content, 0, file.Content.Length);
                    }
                }

                archiveFile = archiveStream.ToArray();
            }

            return archiveFile;
        }

        public class InMemoryFile
        {
            public string FileName { get; set; }
            public byte[] Content { get; set; }
        }
        
        private static async Task UploadFile(string srcFileName, string destBucket, string destFileName)
        {
            var request = new PutObjectRequest { BucketName = destBucket, Key = destFileName, FilePath = srcFileName };
            var response = await S3Client.PutObjectAsync(request, CancellationToken.None);
        }
    }
}