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
namespace Pipeline.GetFilesList
{
    public class GetFileListHandler
    {
        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();

        private readonly string FilesTableName = Environment.GetEnvironmentVariable("FILES_TABLE_NAME");
        
        public async Task<APIGatewayProxyResponse> GetFilesList(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var fileNames = await _dynamoDbClient.ScanAsync(FilesTableName, new List<string>() { "fileName" });
            var result = fileNames.Items.Select(i => i["fileName"]).ToArray();
            
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