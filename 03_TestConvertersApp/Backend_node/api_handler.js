'use strict'

var JobsTable = require('./lib/JobsTable')

module.exports.getJobs = async (event, context) => {

    const jobsTable = new JobsTable(process.env.CONVERSION_JOBS_TABLE_NAME)
    console.log(JobsTable);
    const jobs = await jobsTable.getJobs()
    
    return {
        statusCode: 200,
        headers: { 'Access-Control-Allow-Origin': '*' },
        body: JSON.stringify(jobs)
      }
  }