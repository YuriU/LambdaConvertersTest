import React, { Component } from "react";

class JobItem extends Component {
    constructor(props) {
        super(props);
        this.converters = [
            'Copy', 'One', 'Two', 'Three'
        ];

        this.computeStyle = this.computeStyle.bind(this);
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

    render() {
        return(<div className="jobItem">
            <div className="jobItem-header">
                <strong>Id:</strong><p>{this.props.job.JobId}</p>
                <strong>FileName:</strong><p>
                    <a href="load">{this.props.job.FileName}</a>
                </p>
                { this.converters.map((converter, index) => {
                   return(<button className={"btn-download " + this.computeStyle(converter)} key={converter}>{converter}</button>)
                })}
                <button className="btn-download success">Download</button>
                <button className="btn-download success">Delete</button>
            </div>
        </div>);
    }
}

export default JobItem;