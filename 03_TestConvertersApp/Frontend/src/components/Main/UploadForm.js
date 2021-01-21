import React, { Component } from "react";

class UploadForm extends Component { 
    render() {
        return (<div className="uploadPanel">
                    <div>
                        <input type="file" name="file" onChange={this.onChangeHandler}/>
                    </div>
                    <div>
                        <button type="button" className="btn btn-success btn-block" onClick={this.onClickHandler}>Upload</button> 
                    </div>
               </div>)
    }
}

export default UploadForm;