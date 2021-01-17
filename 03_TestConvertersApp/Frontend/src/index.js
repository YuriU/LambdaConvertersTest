import React from "react";
import ReactDOM from "react-dom";
import App from "./components/App.js";
import Config from './config'

console.log('Config: ' + JSON.stringify(Config) );

ReactDOM.render(<App />, document.getElementById("root"));

