import React, { Component } from "react";
import Config from '../../config'

class JobItem extends Component {
    constructor(props) {
        super(props);
        this.converters = Config.Converters;
        this.computeStyle = this.computeStyle.bind(this);
        this.downloadResult = this.downloadResult.bind(this);
    }

    computeStyle(converter) {
        if(this.props.job.ConversionStatuses && this.props.job.ConversionStatuses[converter]) {
            const status = this.props.job.ConversionStatuses[converter].Successful;
            if(status == true){
                return "success";
            }
            else{
                return "failed";
            }
        }
        else {
            return "unknown";
        }
    }

    async downloadResult() {
       const result = await this.props.httpClient.download(this.props.job.JobId);
       console.log(result);
    }

    async deleteJob(){
        try{
            this.props.httpClient.deleteJob(this.props.job.JobId);
            this.props.onDeleted(this.props.job.JobId)
        }
        catch {
        }
    }

    render() {
        return(<tr className="jobItem">
            <td>{this.props.job.FileName}</td>
            <td>{new Date(+this.props.job.Started).toUTCString()}</td>
            <td><button className="btn-download" onClick={(evt) => this.downloadResult()}><i class="fas fa-download fa-fw"></i></button>
                <button className="btn-download" onClick={(evt) => this.deleteJob()}><i class="fas fa-trash fa-fw"></i></button>
            </td>
            <td>
                {
                    Object.entries(this.props.job.ConversionStatuses)
                    .map((v) => v[0])
                    .sort()
                    .map((converter) => {
                        return(<button className={"btn-download " + this.computeStyle(converter)} key={converter}>{converter}</button>)
                    })
                 }
            </td>
        </tr>);
    }
}

export default JobItem;