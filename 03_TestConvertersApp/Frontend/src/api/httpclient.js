import Config from '../config'
import { Auth } from 'aws-amplify';
import JSZip from 'jszip'


class HttpClient {

    constructor() {
        this.endpoint = Config.BackendHttpEndpoint;
    }

    async getJobsList() {
        let accessToken = await (await Auth.currentSession()).getIdToken().getJwtToken();
        return await this.get('/getJobsList', accessToken)
    }

    async getDownloadUrl(jobId,converter) {
        let accessToken = await (await Auth.currentSession()).getIdToken().getJwtToken();
        if(converter){
            return await this.get('/getDownloadUrl?id=' + jobId + '&converter=' + converter, accessToken)
        }
        else {
            return await this.get('/getDownloadUrl?id=' + jobId, accessToken)
        }
    }


    async download(jobId, converter) {
        const self = this;
        const urls = await this.getDownloadUrl(jobId, converter);
        Promise.all(
            urls.map(url =>
              fetch(url)
                .then(res => res.blob())
                .then(res => Promise.resolve({ fileName: self.getFileName(url), blob : res }))
            )
          ).then(members => {
                if(members.length == 1) {
                    self.downloadBlob(members[0].blob, members[0].fileName);
                }
                else {
                    var zip = new JSZip();
                    var count = 0;
                    console.log(members)
                    members.forEach((p) => {
                        self.blobToBase64(p.blob, function (binaryData) {

                            zip.file(p.fileName, binaryData, {base64: true});

                            if(count < members.length - 1) {
                                count++;
                            }
                            else {
                                zip.generateAsync({type:"blob"})
                                .then(function(content) {
                                    self.downloadBlob(content, 'file.zip');
                                });
                            }
                        })
                    })
                }
          });
    }

    blobToBase64(blob, callback) {
        var reader = new FileReader();
        reader.onload = function() {
            var dataUrl = reader.result;
            var base64 = dataUrl.split(',')[1];
            callback(base64);
        };
        reader.readAsDataURL(blob);
    }

    downloadBlob(blob, fileName){
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    getFileName(url) {
        const indexOfQuestionMark = url.indexOf('?');
        var path =  url.substring(0, indexOfQuestionMark);
        var indexOfLastSlash = path.lastIndexOf('/');
        return path.substring(indexOfLastSlash + 1);
    }

    async deleteJob(jobId) {
        let accessToken = await (await Auth.currentSession()).getIdToken().getJwtToken();
        return await this.delete('/deleteJob?id=' + jobId, accessToken)
    }

    async getUploadUrl(fileName) {
        let accessToken = await (await Auth.currentSession()).getIdToken().getJwtToken();
        return await this.get('/getUploadUrl?filename=' + fileName, accessToken)
    }

    async uploadFile(file) {
        var fileName = file.name;
        var url = await this.getUploadUrl(fileName);

        const headers = {
            'Content-Type': file.type,
            'Content-Length': file.size
        };

        try {
            await fetch(url.Url, {
                headers: headers,    
                method: 'PUT',
                body: file
            });
        }
        catch(error) {
            console.log(console.error())
        }
    }

    async post(method, data, accessToken) {

        const url = this.endpoint + method;
        const headers = {
                'Content-Type': 'application/json;charset=utf-8'
        };

        if(accessToken) {
            headers['Authorization'] = accessToken;
        }

        let response = await fetch(url, {
            method: 'POST',
            headers: headers,    
            body: JSON.stringify(data)
        })

        const result = await response.json();
        return result;
    }

    async get(method, accessToken) {

        const url = this.endpoint + method;

        const headers = {};
        if(accessToken) {
            headers['Authorization'] = accessToken;
        }

        let response = await fetch(url, {
            method: 'GET',
            headers: headers,    
        })

        const result = await response.json();
        return result;
    }

    async delete(method, accessToken) {

        const url = this.endpoint + method;

        const headers = {};
        if(accessToken) {
            headers['Authorization'] = accessToken;
        }

        let response = await fetch(url, {
            method: 'DELETE',
            headers: headers,    
        })

        const result = await response.json();
        return result;
    }
}

export default HttpClient;