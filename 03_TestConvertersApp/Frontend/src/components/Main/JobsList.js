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
                return (<JobItem job={item} key={item.JobId} />)
            })}
        </div>);
    }

    onMessage(event){
        console.log(event.data);
        const jobUpdates = JSON.parse(event.data);
        var jobs = this.state.jobs;
        jobUpdates.forEach(update => {
            var existed = jobs.find((v) => {
                return v.JobId === update.JobId
            });

            if(!existed) {
                jobs.push(update);
            } else {

                for (const [key, value] of Object.entries(update.ConversionStatuses)) {
                    console.log(key)
                    console.log(update.ConversionStatuses[key])
                    if(existed.ConversionStatuses[key]){
                        existed.ConversionStatuses[key].Successful = update.ConversionStatuses[key].Successful;
                    }
                    else {
                        existed.ConversionStatuses[key] = update.ConversionStatuses[key];
                    }
                    
                    
                }
            }
        });

        this.setState({
            jobs
        })
    }
}

export default JobsList;