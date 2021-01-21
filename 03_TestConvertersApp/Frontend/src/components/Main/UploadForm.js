import React, { Component } from "react";

class UploadForm extends Component { 

    constructor(props) {
        super(props);
          this.state = {
            selectedFile: null
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
                        <button type="button" className="btn btn-upload" onClick={this.onClickHandler}>Upload</button> 
                    </div>
               </div>)
    }

    async onClickHandler(){
        if(this.state.selectedFile){
            await this.props.httpClient.uploadFile(this.state.selectedFile);
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