import React, { Component } from "react";

class FilesList extends Component {

    componentDidMount() {
        this.props.httpClient.getFilesList().then(r => console.log(r));
    }

    render() {
        return(<h1>File list</h1>);
    }
}

export default FilesList;