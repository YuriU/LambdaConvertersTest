import React, { Component } from "react";
import MainScreen from './Main/MainScreen'
import '../styles/App.css';
import { getAllUrlParams } from '../utils/urlutils'
import { createBrowserHistory } from "history";
import HttpClient from '../api/httpclient'
import {
    Router as Router,
    Switch,
    Route,
    Link
  } 
from "react-router-dom";
import Amplify, { Auth } from 'aws-amplify';
import Config from '../config'


class App extends Component {

    constructor(props) {
        super(props)
        this.urlParams = getAllUrlParams();

        this.httpClient = new HttpClient();
        this.history = createBrowserHistory();

        Amplify.configure({
          Auth: {
            region : Config.Region,
            userPoolId: Config.UserPoolId,
            userPoolWebClientId: Config.UserPoolClientId
          }
        });
    }

    render() {
        return (
            <Router history={this.history}>
              <Switch>
                <Route path="/">
                  <MainScreen history={this.history} httpClient={this.httpClient} />
                </Route>
              </Switch>
            </Router>
        );
    }   
}

export default App;