using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Pipeline.GetUploadUrl
{
    public class GetUploadUrlHandler
    {
        private readonly AmazonS3Client _s3Client = new AmazonS3Client();
        
        private readonly string _uploadOriginalBucketName = Environment.GetEnvironmentVariable("UPLOAD_ORIGINAL_BUCKET_NAME");
        
        public APIGatewayProxyResponse GetUploadUrl(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var fileName = request.QueryStringParameters["filename"];
            var uploadKey = Path.Combine(Guid.NewGuid().ToString(), fileName);

            var url = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                Expires = DateTime.UtcNow.AddSeconds(30),
                Key = uploadKey,
                BucketName = _uploadOriginalBucketName,
                Verb = HttpVerb.PUT
            });
            
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(new
                {
                    Url = url
                }),
                Headers = new Dictionary<string, string>
                { 
                    { "Content-Type", "application/json" }, 
                    { "Access-Control-Allow-Origin", "*" }
                },
            };
        }
    }
}