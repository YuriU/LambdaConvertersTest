'use strict'

var ActiveConnectionsTable = require('./lib/ActiveConnectionsTable')
const activeConnectionsTable = new ActiveConnectionsTable(process.env.CONNECTIONS_TABLE_NAME)

const AWS = require('aws-sdk')
const apigatewaymanagementapi = new AWS.ApiGatewayManagementApi({
  apiVersion: '2018-11-29',
  endpoint: process.env.WS_GATEWAY_ENDPOINT,
});

module.exports.connectHandler = async (event, context) => { 

    await activeConnectionsTable.addActiveConnection(event.requestContext.connectionId);

    return {
      statusCode: 200,
    };
};

module.exports.disconnectHandler = async (event, context) => { 

    await activeConnectionsTable.removeActiveConnection(event.requestContext.connectionId);
    
    return {
      statusCode: 200,
    };
};

module.exports.defaultHandler = async (event, context) => { 
    
    return {
      statusCode: 200,
    };
};

module.exports.jobsTableStreamHandler = async (event, context) => {

  const dtos = event.Records
  .filter((r) =>  r.eventName === "INSERT" ||  r.eventName === "MODIFY")
  .map((i) => getConversionJobDto(i.dynamodb.NewImage));

  console.log(JSON.stringify(dtos));

  const connections = await activeConnectionsTable.getActiveConnections();
  for(const connectionId of connections) {
    try {
      await apigatewaymanagementapi
      .postToConnection({
        ConnectionId: connectionId, // connectionId of the receiving ws-client
        Data: JSON.stringify(dtos),
      })
      .promise();
    }
    catch(error) {
      console.error(`Error during sending message to connection: ${connectionId}`, error)
    }
  }
  
  return {
    statusCode: 200,
  };
};

const getConversionJobDto = (newImage) => {
  const dto =   {
    JobId : newImage.id.S,
    Started: +newImage.started.N,
    FileName: newImage.fileName.S,
    ConversionStatuses : {}
  };

  for (const [key, value] of Object.entries(newImage.conversionResults.M)) {
    dto.ConversionStatuses[key] = {
      Successful : value.M.sucessful.BOOL
    }
  }

  return dto;
}