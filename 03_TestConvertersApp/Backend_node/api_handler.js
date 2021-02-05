'use strict'


const JobsTable = require('./lib/JobsTable')
const AWS = require('aws-sdk')
const storageUtils = require('aws-sdk')
const s3 = new AWS.S3({signatureVersion: 'v4'});
AWS.config.setPromisesDependency(Promise)

module.exports.getJobs = async (event, context) => {
    const jobsTable = new JobsTable(process.env.CONVERSION_JOBS_TABLE_NAME)
    const jobs = await jobsTable.getJobs()
    return generateResponse(200, jobs);
};

module.exports.getUploadUrl = async (event, context) => {
    const fileName = event.queryStringParameters.filename;
    const uploadKey = uuidv4() + '/' + fileName;
    var url = s3.getSignedUrl('putObject', { Bucket: process.env.UPLOAD_ORIGINAL_BUCKET_NAME, Key: uploadKey, Expires: 60 });

    return generateResponse(200, ({ Url: url }) );
}

module.exports.getDownloadUrl = async (event, context) => {
  const jobsTable = new JobsTable(process.env.CONVERSION_JOBS_TABLE_NAME)
  const jobId = event.queryStringParameters.id;
  const job = await jobsTable.getJob(jobId);

  const converter = event.queryStringParameters.converter;

  const getDownloadPresigned = (key) => {
    var url = s3.getSignedUrl('getObject', { Bucket: process.env.RESULT_BUCKET_NAME, Key: key, Expires: 60 });
    return url;
  }

  const result = [];
  if(converter) {
    if(job.ConversionStatuses[converter]) {
        const key = job.ConversionStatuses[converter].Key
        result.push(getDownloadPresigned(key))
    }
  }
  else {
    for (const [key, value] of Object.entries(job.ConversionStatuses)) {
      result.push(getDownloadPresigned(value.Key))
    }
  }

  return generateResponse(200, result);
}

const generateResponse = (status, message) => {
    return {
      statusCode: status,
      headers: { 
          'Access-Control-Allow-Origin': '*', 
          'Content-Type' : 'application/json',
        },
      body : JSON.stringify(message)
    }
};

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }