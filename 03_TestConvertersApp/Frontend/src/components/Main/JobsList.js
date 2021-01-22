import React, { Component } from "react";
import JobItem from './JobItem'
import WSClient from '../../api/wsclient'

class JobsList extends Component {

    constructor(props) {
        super(props);
  
        this.socket = new WSClient();
        this.state = {
          jobs: []
        };

        this.onMessage = this.onMessage.bind(this);
        this.socket.onMessage(this.onMessage)
    }

    async componentDidMount() {
        const jobList = await this.props.httpClient.getJobsList();
        this.socket.connect();
        this.setState({
            jobs: jobList
        });
    }

    componentWillUnmount() {
        this.socket.disconnect()
    }

    render() {
        return(<div>
            {this.state.jobs.map((item, index) => {
                return (<JobItem job={item} key={item.id} />)
            })}
        </div>);
    }

    onMessage(event){
        console.log(event);
        const jobUpdates = JSON.parse(event.data);
        var jobs = this.state.jobs;
        jobUpdates.forEach(update => {
            var existed = jobs.find((v) => {
                return v.JobId === update.JobId
            });

            if(!existed) {
                jobs.push(update);
            } else {
                existed.ConversionStatuses = update.ConversionStatuses;
            }
        });

        this.setState({
            jobs
        })
    }
}

export default JobsList;