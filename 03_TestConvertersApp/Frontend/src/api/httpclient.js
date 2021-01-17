import Config from '../config'
import { Auth } from 'aws-amplify';


class HttpClient {

    constructor() {
        this.endpoint = Config.BackendHttpEndpoint;
    }

    async getJobsList() {
        let accessToken = await (await Auth.currentSession()).getIdToken().getJwtToken();
        return await this.get('/getJobsList', accessToken)
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
}

export default HttpClient;