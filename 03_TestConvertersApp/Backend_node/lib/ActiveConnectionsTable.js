'use strict'

const AWS = require('aws-sdk')
AWS.config.setPromisesDependency(Promise)


class ActiveConnectionsTable { 
    constructor(table_name) {
        this.table_name = table_name;
    }
    
    async addActiveConnection(connectionId) {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
                params: {TableName: this.table_name}
            })
            
        const dbResult = await dynamoDb.put({
            Item: { connectionId: connectionId }
        }).promise();
        
        return dbResult;
    }

    async removeActiveConnection(connectionId) {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
                params: {TableName: this.table_name}
            })
            
        const dbResult = await dynamoDb.delete({
            Key: { connectionId: connectionId }
        }).promise();
        
        return dbResult;
    }

    async getActiveConnections() {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
                params: {TableName: this.table_name}
            })
            
        const dbResult = await dynamoDb.scan({
            ProjectionExpression: "connectionId"
        }).promise();
        
        const result = dbResult.Items.map((i) => i.connectionId)
        return result;
    }
};

module.exports = ActiveConnectionsTable;
