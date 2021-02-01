'use strict'

var JobsTable = require('./lib/JobsTable')
const jobsTable = new JobsTable(process.env.CONVERSION_JOBS_TABLE_NAME)

module.exports.getJobs = async (event, context) => {
    const jobs = await jobsTable.getJobs()
    
    return {
        statusCode: 200,
        headers: { 
            'Content-Type' : 'application/json',
            'Access-Control-Allow-Origin': '*' 
        },
        body: JSON.stringify(jobs)
      }
  }