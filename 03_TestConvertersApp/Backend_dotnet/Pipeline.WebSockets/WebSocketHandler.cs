using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Pipeline.Data;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Pipeline.WebSockets
{
    public class WebSocketHandler
    {
        private readonly ActiveConnectionsTable _activeConnectionsTable = new ActiveConnectionsTable(Environment.GetEnvironmentVariable("CONNECTIONS_TABLE_NAME"));

        private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
        
        public async Task <APIGatewayProxyResponse> Connected(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try 
            {
                var connectionId = request.RequestContext.ConnectionId;
                await _activeConnectionsTable.AddActiveConnection(connectionId);
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = "Connected."
                };
            }
            catch (Exception e) 
            {
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
                await _activeConnectionsTable.RemoveActiveConnection(connectionId);
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = "Disconnected."
                };
            }
            catch (Exception e) 
            {
                return new APIGatewayProxyResponse {
                    StatusCode = 500,
                    Body = $"Failed to disconnect: {e.Message}"
                };
            }
        }
        
        public async Task <APIGatewayProxyResponse> Default(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return new APIGatewayProxyResponse {
                StatusCode = 200,
                Body = "Default."
            };
        }
    }
}