'use strict'


const JobsTable = require('./lib/JobsTable')
const AWS = require('aws-sdk')
AWS.config.setPromisesDependency(Promise)

module.exports.getJobs = async (event, context) => {
    const jobsTable = new JobsTable(process.env.CONVERSION_JOBS_TABLE_NAME)
    const jobs = await jobsTable.getJobs()
    return generateResponse(200, jobs);
};

module.exports.getUploadUrl = async (event, context) => {
       
    const s3 = new AWS.S3({signatureVersion: 'v4'});
    console.log(event);
    const fileName = event.queryStringParameters.filename;
    
    const uploadKey = uuidv4() + '/' + fileName;

    var params = { Bucket: process.env.UPLOAD_ORIGINAL_BUCKET_NAME, Key: uploadKey, Expires: 60 };
    var url = s3.getSignedUrl('putObject', params);

    return generateResponse(200, ({ Url: url }) );
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