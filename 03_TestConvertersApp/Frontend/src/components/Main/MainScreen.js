import React, { Component } from 'react';
import { Auth } from 'aws-amplify';
import Login from './Login';
import '../../styles/Main.css'
import FilesList from './FilesList'

import {
  Router as Router,
  Switch,
  Route,
  Link,
  Redirect
} 
from "react-router-dom";

class MainScreen extends Component {

    constructor(props) {
      super(props);

      this.state = {
        authenticated: false,
        userName: null,
        password: null
      };

      this.updateLoginState = this.updateLoginState.bind(this);
      this.onCreateSession = this.onCreateSession.bind(this);
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

    renderContent() {
        return (<Router history={this.props.history}>
          <Switch>
            <Route path="/login">
              <Login updateLoginState={this.updateLoginState} history={this.props.history} />
            </Route>
            <Route path="/">
              { this.state.authenticated ? <FilesList/> : <Redirect to="/login" /> }
            </Route>
          </Switch>
        </Router>)
    }

    async onCreateSession(items) {
      const result = await this.props.httpClient.startSession(items);
      this.props.history.push('/session/' + result.SessionId)        
  }

    render() {
        return (<div className="mainscreen">
          <nav className="navbar">
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
            this.renderContent()
          }
          <footer className="footer">
              <span className="text-muted">Yurii Ulianets 2020. Powered by <a href="https://aws.amazon.com/">AWS</a>,<a href="https://serverless.com/">Serverless</a>,<a href="https://reactjs.org/">React</a> and <a href="https://aws-amplify.github.io/docs/js/authentication">AmplifyJS</a></span>
          </footer>
        </div>)
    }
}

export default MainScreen;