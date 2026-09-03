import Header from "./Header.jsx";
import Footer from "./Footer.jsx";
import {useEffect, useRef, useState} from "react";
import "./ServerState.css"
import api from "./api.jsx";
import axios from "axios";

//props for this link,name
const GET_LAST_PING_FOR_SITES_URL = "https://localhost:8443/sites/get_last";
const ADD_SITE_TO_ACCOUNT_URL = "https://localhost:8443/sites/add";
const LOGGED_IN_CHECK_URL = "https://localhost:8443/auth/me";


function UpTimePage() {
    const errRef = useRef();
    const [errMsg, setErrMsg] = useState('');

    const [link, setLink] = useState("");
    const [name, setName] = useState("");

    const [sites, setSites] = useState([])
    const [isServerUp, setServers] = useState([])
    const [lastChecks, setLastChecks] = useState([])
    const AddSiteToDb = async () => {
        try {
            const result = await axios.get(LOGGED_IN_CHECK_URL, {withCredentials: true});
            if (!result) return
        } catch (err) {
            setErrMsg("Auth error")
            return;
        }
        try {
            const response = axios.post(ADD_SITE_TO_ACCOUNT_URL, {
                    Url: link,

                }, {withCredentials: true}
            );


        } catch (err) {
            if (!err?.response) {
                setErrMsg('No Server Response');

            } else if (err.response?.status === 409) {
                setErrMsg('Site already exists Taken');
            } else {
                setErrMsg('Site adding Failed');
            }
        }

    }
    const UpdateSites = async () => {

        try {
            const respose = await axios.get(GET_LAST_PING_FOR_SITES_URL, {withCredentials: true})
            if (!respose) {
                setErrMsg("NoSiteAdded")
            }
            setSites(respose.data);
        } catch (err) {
            if (!err?.response) {
                setErrMsg('No Server Response');

            } else if (err.response?.status === 401) {
                setErrMsg('Not logged');
            } else {
                setErrMsg('Gettin sites data Failed');
            }
        }
    }

    useEffect(() => {


        setErrMsg('')
    }, [name, link]);


    useEffect(() => {
        const intervalId = setInterval(() => {
            UpdateSites()
        }, 5000);
        return () => clearInterval(intervalId);
    }, []);

    console.log("sites:", sites);
    console.log("is array:", Array.isArray(sites))
    return (

        <div className="page">
            <Header/>

            <p ref={errRef} className={errMsg ? "errMsg" : "offscreen"} aria-live={"assertive"}>{errMsg}</p>
            <main className="main" aria-live={"assertive"}>
                <button className="button" onClick={AddSiteToDb}>
                    Add
                </button>
                <br/>
                <table>
                    <thead>
                    <tr>
                        <th>Link</th>
                        <th>Name</th>
                        <th>LastChecked</th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr>
                        <td>
                            <input
                                type="text"
                                id="link"
                                autoComplete="off"
                                onChange={(e) => setLink(e.target.value)}
                            />

                        </td>
                        {/*<td>*/}
                        {/*    <input*/}
                        {/*        type="text"*/}
                        {/*        id="Name"*/}
                        {/*        autoComplete="off"*/}
                        {/*        onChange={(e) => setName(e.target.value)}*/}
                        {/*    />*/}
                        {/*</td>*/}

                    </tr>
                    {!sites ? setErrMsg("awaiting server response") : sites.map(site => (
                        <tr>
                            <td>{site.url}</td>
                            <td>
                                <label
                                    className={site.pings[0].success ? "server-status online" : "server-status offline"}>
                                    {site.pings[0].success ? `Server Online: ${site.pings[0].timestamp}` : `Server Offline: ${site.pings[0].timestamp}`}
                                </label>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </main>

            <Footer/>
        </div>

    );

}

export default UpTimePage