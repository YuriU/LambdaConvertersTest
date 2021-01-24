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
namespace Pipeline.DownloadResult
{
    public class DownloadResultHandler
    {
        private readonly JobsTable _jobsTable = new JobsTable(Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME"));
        
        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");
        
        private static AmazonS3Client S3Client = new AmazonS3Client();
        
        public async Task<APIGatewayProxyResponse> DownloadResult(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var id = request.QueryStringParameters["id"];
            var job = await _jobsTable.GetJob(id);

            
            LambdaLogger.Log(_resultBucketName);

            var jobDirectoryPath = Path.Combine(Path.GetTempPath(), id);
            LambdaLogger.Log(jobDirectoryPath);
            Directory.CreateDirectory(jobDirectoryPath);
            

            await DownloadFile(_resultBucketName, job.OriginalKey, Path.Combine(jobDirectoryPath, Path.GetFileName(job.OriginalKey)));
            foreach (var kvp in 
                job.ConversionStatuses
                    .Where(s => s.Value.Successful))
            {
                await DownloadFile(_resultBucketName, kvp.Value.Key, Path.Combine(jobDirectoryPath, Path.GetFileName(kvp.Value.Key)));
            }

            var files = Directory.GetFiles(jobDirectoryPath);
            var inmemoryFiles = files.Select(f => new InMemoryFile
            {
                FileName = f,
                Content = File.ReadAllBytes(f)
            }).ToList();

            var zipArchiveBytes = GetZipArchive(inmemoryFiles);

            var zipFilePath = Path.Combine(jobDirectoryPath, $"{job}.zip");
            File.WriteAllBytes(zipFilePath, zipArchiveBytes);

            await UploadFile(zipFilePath, _resultBucketName, Path.GetFileName(zipFilePath));
            
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = Convert.ToBase64String(zipArchiveBytes),
                IsBase64Encoded = true,
                Headers = new Dictionary<string, string>
                { 
                    { "Content-Type", "application/zip" }, 
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Expose-Headers", "Content-Disposition" },
                    { "Content-Disposition", $"attachment; filename=\"{id}.zip\"" }
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