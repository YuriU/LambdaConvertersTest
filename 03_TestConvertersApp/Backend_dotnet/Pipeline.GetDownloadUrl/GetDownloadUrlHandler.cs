using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

                var url = S3Client.GetPreSignedURL(new GetPreSignedUrlRequest
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
                    var url = S3Client.GetPreSignedURL(new GetPreSignedUrlRequest
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
    }
}