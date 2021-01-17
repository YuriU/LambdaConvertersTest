using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Pipeline.GetJobsList
{
    public class GetJobsListHandler
    {
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();

        private readonly string JobsListTable = Environment.GetEnvironmentVariable("CONVERSION_JOBS_TABLE_NAME");
        
        public async Task<APIGatewayProxyResponse> GetJobsList(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var fileNames = await _dynamoDbClient.ScanAsync(JobsListTable, new List<string>() { "id", "fileName", "started" });
            var result = fileNames.Items.Select(i => new
            {
                id = i["id"].S,
                fileName = i["fileName"].S,
                started = i["started"].N
            }).ToArray();
            
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(result),
                Headers = new Dictionary<string, string>
                { 
                    { "Content-Type", "application/json" }, 
                    { "Access-Control-Allow-Origin", "*" }
                },
            };
        }
    }
}