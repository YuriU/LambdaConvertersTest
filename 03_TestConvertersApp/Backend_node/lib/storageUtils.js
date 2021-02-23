function makeOriginalFilePath(jobId, key) {
    const extension = key.split('.').pop(); 
    return `${jobId}/${key}/Original.${extension}`
}

function makeConvertedFilePath(jobId, key, converter, convertedExtension) {
    return `${jobId}/${key}/${converter}${convertedExtension}`
}

function makeConvertedAdditionalFilePath(jobId, key, converter, fileName) {
    return `${jobId}/${key}/${converter}_Additional/${fileName}`
}

module.exports = {
    makeOriginalFilePath: makeOriginalFilePath,
    makeConvertedFilePath: makeConvertedFilePath,
    makeConvertedAdditionalFilePath: makeConvertedAdditionalFilePath
  }