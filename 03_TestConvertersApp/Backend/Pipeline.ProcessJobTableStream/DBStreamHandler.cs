using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Newtonsoft.Json;
using Pipeline.Contracts;
using Pipeline.Data;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Pipeline.ProcessJobTableStream
{
    public class DBStreamHandler
    {
        private readonly ActiveConnectionsTable _activeConnectionsTable = new ActiveConnectionsTable(Environment.GetEnvironmentVariable("CONNECTIONS_TABLE_NAME"));
        
        private readonly AmazonApiGatewayManagementApiClient _apiClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig()
        {
            ServiceURL = Environment.GetEnvironmentVariable("WS_GATEWAY_ENDPOINT"),
        });
        
        public async Task ProcessEvent(DynamoDBEvent dynamoDbEvent, ILambdaContext context)
        {
            List<ConversionJobDto> dtos = dynamoDbEvent.Records
                .Where(r => r.EventName == "INSERT" || r.EventName == "MODIFY")
                .Select(r => GetConversionJobDto(r.Dynamodb.NewImage))
                .ToList();
            
            var activeConnections = await _activeConnectionsTable.GetActiveConnections();
            var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dtos)));
            foreach (var connectionId in activeConnections)
            {
                stream.Position = 0;
                await _apiClient.PostToConnectionAsync(new PostToConnectionRequest
                {
                    ConnectionId = connectionId,
                    Data = stream
                });
            }
        }

        private ConversionJobDto GetConversionJobDto(Dictionary<string, AttributeValue> record)
        {
            var jobDto = new ConversionJobDto
            {
                JobId = record["id"].S,
                Started = long.Parse(record["started"].N),
                FileName = record["fileName"].S,
                ConversionStatuses = new Dictionary<string, ConversionStatus>()
            };

            foreach (var kvp in record["conversionResults"].M)
            {
                var key = kvp.Key;
                jobDto.ConversionStatuses[key] = new ConversionStatus
                {
                    Successful = kvp.Value.M["sucessful"].BOOL
                };
            }

            return jobDto;
        }
    }
}