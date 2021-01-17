import React, { Component } from "react";
import JobItem from './JobItem'

class JobsList extends Component {

    constructor(props) {
        super(props);
  
        this.state = {
          jobs: []
        };
    }

    async componentDidMount() {
        const jobList = await this.props.httpClient.getJobsList();
        this.setState({
            jobs: jobList
        });
    }

    render() {
        return(<ul>
            {this.state.jobs.map((item, index) => {
                return (<li><JobItem job={item} id={item.id} /></li>)
            })}
        </ul>);
    }
}

export default JobsList;