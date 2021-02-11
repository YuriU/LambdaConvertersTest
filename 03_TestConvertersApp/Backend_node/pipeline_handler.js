'use strict'

const JobsTable = require('./lib/JobsTable')
const AWS = require('aws-sdk')
AWS.config.setPromisesDependency(Promise)
const storageUtils = require('./lib/storageUtils')
const s3 = new AWS.S3({apiVersion: '2006-03-01'})
const sns = new AWS.SNS({apiVersion: '2010-03-31'})

module.exports.fileUploaded = async (event, context) => { 
    
    for(const record of event.Records) {

        try {
            await processFileUploaded(record.s3);
        }
        catch(error) {
            console.error(`Error during processing file upload: '${JSON.stringify(record)}'`, error)
        }
    }
    return {
      statusCode: 200,
    };
};

module.exports.processResult = async (event, context) => {

    const jobsTable = new JobsTable(process.env.CONVERSION_JOBS_TABLE_NAME)
    for(const record of event.Records) { 
        
        const result = JSON.parse(record.body)
        console.log(record.body)
        var jobId = result.JobId;
        const fileName = await jobsTable.getFileName(jobId)
        console.log('File name ' + fileName)

        if(result.Sucessful) 
        {
            const extension = result.ResultKey.split('.').pop(); 
            const destinationKey = storageUtils.makeConvertedFilePath(jobId, fileName, result.Converter, '.' + extension)

            var params = {
                Bucket: process.env.RESULT_BUCKET_NAME, 
                CopySource: `${process.env.UPLOAD_RESULT_BUCKET_NAME}/${result.ResultKey}`, 
                Key: destinationKey
               };

            try 
            {
                await s3.copyObject(params).promise()
                await s3.deleteObject({
                    Bucket: process.env.UPLOAD_RESULT_BUCKET_NAME,
                    Key: result.ResultKey
                }).promise()
                await jobsTable.setConversionResult(jobId, result.Converter, result.Sucessful, destinationKey, null)
            }
            catch(error) {
                await jobsTable.setConversionResult(jobId, result.Converter, false, "", JSON.stringify(error))
            }
        }
        else 
        {
            await jobsTable.setConversionResult(jobId, result.Converter, result.Sucessful, "", null)
        }
    }   

    return {
        statusCode: 200,
      };
};

const processFileUploaded =  async (record) => {
    const originalFileKey = record.object.key;
    const originalBucket = record.bucket.name;
    const jobId = uuidv4();
    const fileName = originalFileKey.split(/(\\|\/)/g).pop()

    const jobsTable = new JobsTable(process.env.CONVERSION_JOBS_TABLE_NAME)
    await jobsTable.addJob(jobId, fileName, new Date().getTime())

    const srcFilePath = await moveToDestinationBucket(jobId, originalBucket, originalFileKey)

    await jobsTable.setOriginalFileName(jobId, srcFilePath, null)

    await publishFileUploaded(process.env.FILE_UPLOADED_TOPIC_ARN, jobId, srcFilePath)
}



const moveToDestinationBucket = async (jobId, originalBucket, originalFileKey) => {
    const fileName = originalFileKey.split(/(\\|\/)/g).pop()
    const destinationKey = storageUtils.makeOriginalFilePath(jobId, fileName)

    var params = {
        Bucket: process.env.RESULT_BUCKET_NAME, 
        CopySource: `${originalBucket}/${originalFileKey}`, 
        Key: destinationKey
       };

    await s3.copyObject(params).promise()
    await s3.deleteObject({
        Bucket: originalBucket,
        Key: originalFileKey
    }).promise()

    return destinationKey;
}

const moveToResultBucket = async (jobId, originalBucket, originalFileKey) => {
    const fileName = originalFileKey.split(/(\\|\/)/g).pop()
    const destinationKey = storageUtils.makeOriginalFilePath(jobId, fileName)

   
    await s3.deleteObject({
        Bucket: originalBucket,
        Key: originalFileKey
    }).promise()

    return destinationKey;
}

const publishFileUploaded = async (topic, jobId, key) => {
    const fileUploadedEvent = {
        JobId : jobId,
        OriginalBucketName: process.env.RESULT_BUCKET_NAME,
        Key: key,
        ResultBucketName: process.env.UPLOAD_RESULT_BUCKET_NAME
    }

    console.log(JSON.stringify(fileUploadedEvent));

    var params = {
        Message: JSON.stringify(fileUploadedEvent),
        TopicArn: topic
      };

    await sns.publish(params).promise()
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
}