import React, { Component } from 'react';
import { Auth } from 'aws-amplify';
import Login from './Login';
import '../../styles/Main.css'
import JobsList from './JobsList'

import {
  Router as Router,
  Switch,
  Route,
  Link,
  Redirect
} 
from "react-router-dom";
import UploadForm from './UploadForm';

class MainScreen extends Component {

    constructor(props) {
      super(props);

      this.state = {
        authenticated: false,
        userName: null,
        password: null
      };

      this.updateLoginState = this.updateLoginState.bind(this);
      this.renderContent = this.renderContent.bind(this);
    }

    async componentDidMount() {
      await this.updateLoginState();
    }

    async logout() {
        await Auth.signOut();
        await this.updateLoginState();
    }

    async updateLoginState() {
      try {
        const currentSession = await Auth.currentSession();
        if(currentSession) {
          this.setState({
            authenticated: true
          })
        }
        else {
          this.setState({
            authenticated: false
          })
        }
      }
      catch(error) {
        this.setState({
          authenticated: false
        })
      }
    }

    renderContent(authenticated) {
        return (<Router history={this.props.history}>
          <Switch>
            <Route path="/login">
              <Login updateLoginState={this.updateLoginState} history={this.props.history} />
            </Route>
            <Route path="/">
              { authenticated &&
                 <div className="container">
                  <JobsList httpClient={this.props.httpClient}/>
                  <UploadForm httpClient={this.props.httpClient}/>
                </div>}
              {
                !authenticated &&
                <Login updateLoginState={this.updateLoginState} history={this.props.history} />
              }
            </Route>
          </Switch>
        </Router>)
    }

    render() {
        return (<div className="mainscreen">
          <nav className="navbar navbar-dark bg-dark">
            <ul>
              <li>
                <Link to="/">Home</Link>
              </li>  
              {
                this.state.authenticated && <li>
                  <Link to="/" onClick={(evt) => this.logout()}>Logout</Link>
                </li>
              }
              {
                !this.state.authenticated && <li>
                  <Link to="/login">Login</Link>
                </li>
              }
            </ul>
          </nav>
          {
            this.renderContent(this.state.authenticated)
          }
          <footer className="footer">
              <span className="text-muted">Yurii Ulianets 2020. Powered by <a href="https://aws.amazon.com/">AWS</a>,<a href="https://serverless.com/">Serverless</a>,<a href="https://reactjs.org/">React</a> and <a href="https://aws-amplify.github.io/docs/js/authentication">AmplifyJS</a></span>
          </footer>
        </div>)
    }
}

export default MainScreen;