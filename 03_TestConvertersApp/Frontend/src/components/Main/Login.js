import React, { Component } from 'react';
import { Auth } from 'aws-amplify';

class Login extends Component {

    constructor(props) {
        super(props);
  
        this.state = {
          userName: null,
          password: null,
          message: null
        };
  
        this.login = this.login.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangePassword = this.handleChangePassword.bind(this);
        this.clearMessage = this.clearMessage.bind(this);
        this.changePassword = this.changePassword.bind(this);
        this.handleNewPasswordChange = this.handleNewPasswordChange.bind(this);
        this.handleNewPassword2Change = this.handleNewPassword2Change.bind(this);
    }

    async login() {
      try 
      {
        const user = await Auth.signIn(this.state.userName, this.state.password);
        if (user.challengeName === 'NEW_PASSWORD_REQUIRED') {
          this.setState({
            message: 'New password required',
            newPasswordRequest: { user: user }
          })
        }

        await this.props.updateLoginState();
        if(!user.challengeName && user.username){
          this.props.history.push('/');
        }
      }
      catch (err) 
      {
        if (err.code === 'UserNotConfirmedException') {
            // The error happens if the user didn't finish the confirmation step when signing up
            // In this case you need to resend the code and confirm the user
            // About how to resend the code and confirm the user, please check the signUp part
            this.setState({
              message: 'User is not confirmed'
            })
        } else if (err.code === 'PasswordResetRequiredException') {
            // The error happens when the password is reset in the Cognito console
            // In this case you need to call forgotPassword to reset the password
            // Please check the Forgot Password part.
            this.setState({
              message: 'Password reset is required'
            })
        } else if (err.code === 'NotAuthorizedException') {
            // The error happens when the incorrect password is provided
            this.setState({
              message: 'Invalid login or password'
            })
        } else if (err.code === 'UserNotFoundException') {
            // The error happens when the supplied username/email does not exist in the Cognito user pool
            this.setState({
              message: 'Invalid login or password'
            })
        } else {
          this.setState({
            message: err
          })
        }
      }
    }

    async changePassword() {      

      if(this.state.newPasswordRequest.newPassword != this.state.newPasswordRequest.newPassword2) {
        this.setState({
          message: 'Passwords dont match'
        })

        return;
      }

      try {
        const user = await Auth.completeNewPassword(
          this.state.newPasswordRequest.user,
          this.state.newPasswordRequest.newPassword,
        );

        await this.props.updateLoginState();
        if(!user.challengeName && user.username){
          this.props.history.push('/');
        }
      }
      catch(err) {
        const message = err.message ? err.message : err;
        this.setState({
          message: message
        })
      }
    }

    handleChangeName(event){
        this.setState({
          userName: event.target.value
        });
    }
  
    handleChangePassword(event){
      this.setState({
        password: event.target.value
      });
    }

    handleNewPasswordChange(event){
      const state = this.state;
      state.newPasswordRequest.newPassword = event.target.value;
      this.setState({
        state
      });
    }

    handleNewPassword2Change(event){
      const state = this.state;
      state.newPasswordRequest.newPassword2 = event.target.value;
      this.setState({
        state
      });
    }

    clearMessage(event){
      this.setState({
        message: undefined
      })
    }

    render() {
        return (<div className="loginScreen" >
            <div className="loginformContainer">
              {this.state.message && <div className ="errorBox">
                {this.state.message}
                <button onClick={this.clearMessage}>&times;</button>
              </div>}
              {this.state.newPasswordRequest == null && <div className="loginBox">
                <div>
                    <label><b>UserName:</b><input type="text" onChange={this.handleChangeName}></input></label>
                </div>
                <div>
                  <label><b>Password:</b><input type="password" onChange={this.handleChangePassword}></input></label>
                </div>
                <button onClick={(evt) => this.login()}>Login</button>
              </div>}
              {this.state.newPasswordRequest != null && <div className="loginBox">
                <div>
                    <label><b>New password:</b><input type="password" onChange={this.handleNewPasswordChange}></input></label>
                </div>
                <div>
                  <label><b>Retype password:</b><input type="password" onChange={this.handleNewPassword2Change}></input></label>
                </div>
                <button onClick={(evt) => this.changePassword()}>Change password</button>
              </div>}
            </div>
          </div>)
    }
}

export default Login;