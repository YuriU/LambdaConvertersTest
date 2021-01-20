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
        return(<div>
            {this.state.jobs.map((item, index) => {
                return (<JobItem job={item} key={item.id} />)
            })}
        </div>);
    }
}

export default JobsList;