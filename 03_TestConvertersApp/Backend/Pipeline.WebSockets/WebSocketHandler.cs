using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace Pipeline.WebSockets
{
    public class WebSocketHandler
    {
        private readonly string _connectionTableName = Environment.GetEnvironmentVariable("CONNECTIONS_TABLE_NAME");

        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
        
        public async Task <APIGatewayProxyResponse> Connected(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try 
            {
                var connectionId = request.RequestContext.ConnectionId;
                PutItemRequest ddbRequest = new PutItemRequest {
                    TableName = _connectionTableName,
                    Item = new Dictionary<string, AttributeValue> {{ "connectionId", new AttributeValue { S = connectionId }}}
                };
                PutItemResponse ddbResponse = await _dynamoDbClient.PutItemAsync(ddbRequest);
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = "Connected."
                };
            }
            catch (Exception e) {
                return new APIGatewayProxyResponse {
                    StatusCode = 500,
                    Body = $"Failed to connecting: {e.Message}"
                };
            }
        }
        
        public async Task <APIGatewayProxyResponse> Disconnected(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try 
            {
                var connectionId = request.RequestContext.ConnectionId;
                DeleteItemRequest ddbRequest = new DeleteItemRequest {
                    TableName = _connectionTableName,
                    Key = new Dictionary<string, AttributeValue> {{ "connectionId", new AttributeValue { S = connectionId }}}
                };
                DeleteItemResponse ddbResponse = await _dynamoDbClient.DeleteItemAsync(ddbRequest);
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = "Connected."
                };
            }
            catch (Exception e) {
                return new APIGatewayProxyResponse {
                    StatusCode = 500,
                    Body = $"Failed to connecting: {e.Message}"
                };
            }
        }
    }
}