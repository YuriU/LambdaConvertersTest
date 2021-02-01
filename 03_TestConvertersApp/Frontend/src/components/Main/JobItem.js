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
        return(<div className="jobItem">
            <div className="jobItem-header">
                <strong>FileName:</strong><p>
                    <a href="load">{this.props.job.FileName}</a>
                </p>
                { this.converters.map((converter, index) => {
                   return(<button className={"btn-download " + this.computeStyle(converter)} key={converter}>{converter}</button>)
                })}
                <button className="btn-download success" onClick={(evt) => this.downloadResult()}>Download</button>
                <button className="btn-download success" onClick={(evt) => this.deleteJob()}>Delete</button>
            </div>
        </div>);
    }
}

export default JobItem;