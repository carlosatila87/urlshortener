import React, { Component } from 'react';
import './Home.css';

const baseUrl = window.location.href;

export class Home extends Component {
    constructor() {
        super();
        this.state = { urlRegister: [], loading: true };
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleErrors = this.handleErrors.bind(this);
    }

    handleErrors(response) {
        if (!response.ok) {
            this.setState({ urlRegister: [], loading: true });
            alert('Error: Invalid URI');
            throw Error(response.statusText);
        }
        return response.json();
    }

    handleSubmit(event) {
        event.preventDefault();
        const data = new FormData(event.target);

        fetch('api/ShortUrl/Shorten', {
            method: 'POST',
            body: data
        })
            .then(this.handleErrors)
            .then(data => {
                this.setState({ urlRegister: data, loading: false });
                document.getElementById('uriList').value = '';
            }).catch(function (error) {
                console.log(error);
            });
    }

    static renderUrlTable(urlList) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>Url</th>
                        <th>Short Url</th>                        
                    </tr>
                </thead>
                <tbody>
                    {urlList.map(u =>
                        <tr key={u.token}>
                            <td>{u.longUrl}</td>
                            <td><input id={u.token} class="form-control" value={baseUrl + u.token} name={u.token} type="text" /></td>                            
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <br />
            : Home.renderUrlTable(this.state.urlRegister);
        return (
            <div id="wrapper">
                <form onSubmit={this.handleSubmit}>
                    <h1>Url Shortener</h1>
                    <div class="form-group">
                        <label htmlFor="uriList">You can insert one or more URIs separating by space ex.:(http://hello.com http://holla.com)</label>
                        <input id="uriList" class="form-control" name="uriList" type="text" />                
                        <button class="btn btn-primary btn-block">Make your URI Shorter!</button>
                        {contents}
                    </div>
                </form>                    
            </div>
        );
    }
}





