function makeOriginalFilePath(jobId, key) {
    const extension = key.split('.').pop(); 
    return `${jobId}/${key}/Original.${extension}`
}

module.exports = {
    makeOriginalFilePath: makeOriginalFilePath
  }