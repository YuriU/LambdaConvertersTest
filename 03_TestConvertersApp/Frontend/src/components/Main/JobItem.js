import React, { Component } from "react";

class JobItem extends Component {

    constructor(props) {
        super(props);
    }

    render() {
        return(<div><span>{this.props.job.id}</span>&nbsp;&nbsp;<span>{this.props.job.fileName}</span></div>);
    }
}

export default JobItem;