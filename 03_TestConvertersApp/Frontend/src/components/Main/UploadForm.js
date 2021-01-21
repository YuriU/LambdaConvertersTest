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
        const data = new FormData() 
        data.append('file', this.state.selectedFile);

        console.log(this.props);
        const res = await this.props.httpClient.getUploadUrl("file.txt");
    }

    onChangeHandler(event) {
        this.setState({
        selectedFile: event.target.files[0],
        loaded: 0,
        })
    }
    
}

export default UploadForm;