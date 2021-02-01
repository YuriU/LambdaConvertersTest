'use strict'

const AWS = require('aws-sdk')
AWS.config.setPromisesDependency(Promise)


class JobsTable { 
    constructor(table_name) {
        this.table_name = table_name;
    }
    
    async getJobs() {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
                params: {TableName: this.table_name}
            })
            
        const dbResult = await dynamoDb.scan({
            ProjectionExpression: "id, fileName, started, conversionResults"
        }).promise();
        
        const result = dbResult.Items.map((i) => ({
            JobId: i.id,
            FileName: i.fileName,
            Started: +i.started,
            ConversionStatuses : this.statusesToDictionary(i.conversionResults)
        }))
        
        return result;
    }
    
    statusesToDictionary(conversionResults) {
        var dict = {}
        for (const [key, value] of Object.entries(conversionResults)) {
            dict[key] = ({
               Successful: value["sucessful"]
            })
        }
        return dict
    }
};

module.exports = JobsTable;
