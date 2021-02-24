'use strict'

const AWS = require('aws-sdk')
AWS.config.setPromisesDependency(Promise)


class JobsTable { 
    constructor(table_name) {
        this.table_name = table_name;
    }

    async addJob(jobId, fileName, started) {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
            params: {TableName: this.table_name}
        })

        await dynamoDb.put({
            Item: { 
                id: jobId,
                fileName: fileName,
                started: started,
                conversionResults : {}
            }
        }).promise();
    }

    async setOriginalFileName(jobId, originalFileKey, exception) {
        const originalFileResult = exception == null 
                ? { key : originalFileKey }
                : { error : exception };

        const dynamoDb = new AWS.DynamoDB.DocumentClient({
            params: {TableName: this.table_name}
        })

        await dynamoDb.update({
            Key: { id: jobId },
            UpdateExpression: 'SET #original = :result',
            ExpressionAttributeNames: { '#original' : 'original' },
            ExpressionAttributeValues: { ':result' : originalFileResult }
        }).promise();
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

    async getJob(jobId) {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
            params: {TableName: this.table_name}
        })

        const job = await dynamoDb.get({
            Key: { id: jobId },
            AttributesToGet: ['id', 'fileName', 'started', 'original', 'conversionResults']
        }).promise()

        if(job.Item) {
            return {
                JobId : job.Item.id,
                FileName: job.Item.fileName,
                Started: +job.Item.started,
                OriginalKey: job.Item["original"]["key"],
                ConversionStatuses : this.statusesWithFilesToDictionary(job.Item.conversionResults)
            }
        }
        else {
            return null;
        }
    }


    async getFileName(jobId) {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
            params: {TableName: this.table_name}
        })

        const job = await dynamoDb.get({
            Key: { id: jobId },
            AttributesToGet: ['id', 'fileName']
        }).promise()

        if(job && job.Item){
            return job.Item.fileName
        }
        
        return null;
    }

    async deleteJob(jobId) {
        const dynamoDb = new AWS.DynamoDB.DocumentClient({
                params: {TableName: this.table_name}
            })
            
        const dbResult = await dynamoDb.delete({
            Key: { id: jobId }
        }).promise();
        
        return dbResult;
    }

    async setConversionResult(jobId, converter, success, convertedKey, additionalFiles, error) {

        const dynamoDb = new AWS.DynamoDB.DocumentClient({
            params: {TableName: this.table_name}
        })

        await dynamoDb.update({
            Key: { id: jobId },
            UpdateExpression: 'SET #converters.#converter = :result',
            //ConditionExpression : "attribute_not_exists(#converters.#converter)",
            ExpressionAttributeNames: { 
                '#converters' : 'conversionResults',
                '#converter' : converter 
            },
            ExpressionAttributeValues: { ':result' : {
                sucessful : success,
                key: convertedKey,
                additionalFiles: additionalFiles
            }}
        }).promise();
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

    statusesWithFilesToDictionary(conversionResults) {
        var dict = {}
        for (const [key, value] of Object.entries(conversionResults)) {
            dict[key] = ({
               Successful: value["sucessful"],
               Key: value["key"]
            });
            
            if(value["additionalFiles"]) {
                dict[key].AdditionalFiles = {};
                for (const [fileName, filePath] of Object.entries(value["additionalFiles"])) {
                    dict[key].AdditionalFiles[fileName] = filePath;
                }
            }
        }

        return dict
    }
};

module.exports = JobsTable;