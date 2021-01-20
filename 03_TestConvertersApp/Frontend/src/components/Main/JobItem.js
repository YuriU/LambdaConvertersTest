import React, { Component } from "react";

class JobItem extends Component {

    constructor(props) {
        super(props);
    }

    render() {
        return(<div className="jobItem">
            <div class="jobItem-header">
                <strong>Id:</strong><p>{this.props.job.JobId}</p>
                <strong>FileName:</strong><p>
                    <a href="load">{this.props.job.FileName}</a>
                </p>
                <button>Copy</button>
                <button>Copy</button>
                <button>Copy</button>
                <button>Copy</button>
                <button>Copy</button>
            </div>
        </div>);
        //return(<div><span>{this.props.job.JobId}</span>&nbsp;&nbsp;<span>{this.props.job.FileName}</span></div>);
    }
}

export default JobItem;