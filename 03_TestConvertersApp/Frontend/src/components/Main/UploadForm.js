import React, { Component } from "react";

class UploadForm extends Component { 

    constructor(props) {
        super(props);
          this.state = {
            selectedFile: null,
            uploading: false
          }
       
        
        this.onChangeHandler = this.onChangeHandler.bind(this);
        this.onClickHandler = this.onClickHandler.bind(this);
      }

    render() {
        return (<div className="uploadPanel">
                    <div>
                        <input type="file" name="file" onChange={this.onChangeHandler}/>
                    </div>
                    <div>
                        <button type="button" className="btn btn-upload btn-primary" onClick={this.onClickHandler}>
                           { !this.state.uploading && (
                               <div className="bnt-enabled-state">
                                <span>Upload</span>
                               </div>
                             )
                           }

                           {
                             this.state.uploading && (
                                <div className="bnt-disabled-state">
                                    <span className="spinner-grow spinner-grow-sm" role="status" aria-hidden="true"></span><span>Uploading</span>
                                </div>
                             )
                           }
                            
                        </button> 
                    </div>
               </div>)
    }

    async onClickHandler(){
        try {
            if(this.state.selectedFile) {
                this.setState({ uploading: true })
                await this.props.httpClient.uploadFile(this.state.selectedFile);
                this.setState({ uploading: false })
            }
        }
        catch {
            this.setState({ uploading: false })
        }
        
    }

    onChangeHandler(event) {
        this.setState({
        selectedFile: event.target.files[0],
        loaded: 0,
        })
    }
    
}

export default UploadForm;