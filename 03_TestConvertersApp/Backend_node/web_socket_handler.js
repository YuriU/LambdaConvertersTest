'use strict'

var ActiveConnectionsTable = require('./lib/ActiveConnectionsTable')
const activeConnectionsTable = new ActiveConnectionsTable(process.env.CONNECTIONS_TABLE_NAME)

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