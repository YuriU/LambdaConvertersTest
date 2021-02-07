function makeOriginalFilePath(jobId, key) {
    const extension = key.split('.').pop(); 
    return `${jobId}/${key}/Original.${extension}`
}

function makeConvertedFilePath(jobId, key, converter, convertedExtension) {
    return `${jobId}/${key}/${converter}${convertedExtension}`
}

module.exports = {
    makeOriginalFilePath: makeOriginalFilePath,
    makeConvertedFilePath: makeConvertedFilePath
  }