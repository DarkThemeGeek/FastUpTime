import Header from "./Header.jsx";
import Footer from "./Footer.jsx";
import {useEffect, useRef, useState} from "react";
import "./ServerState.css"
import api from "./api.jsx";

//props for this link,name


function UpTimePage() {
    const errRef = useRef();
    const [errMsg, setErrMsg] = useState('');

    const [link, setLink] = useState("");
    const [name, setName] = useState("");

    const [sites, setSites] = useState([])
    const [isServerUp, setServers] = useState([])
    const [lastChecks, setLastChecks] = useState([])
    const AddSiteToDb = async (prop) => {
        try {
            const result = await api.get("/auth/me");
            if (!result) return
        } catch (err) {
            console.log("error")
            setErrMsg("Auth error")
        }
        try {
            api().post("sites/add", {
                    Site: prop.site,
                    Name: prop.name
                },
                {withCredentials: true}
            );
            const sites = api().get("sites/",{},{withCredentials: true});
            setSites(sites);
        } catch (err) {
            if (!err?.response) {
                setErrMsg('No Server Response' + err?.respose?.data);

                // } else if (err.response?.status === 409) {
                //     setErrMsg('UserName Taken' + err?.respose?.data); b
            } else {
                setErrMsg('Site adding Failed' + err?.respose?.data);
            }
        }

    }

    useEffect(() => {
        setErrMsg('')
    }, [name, link]);

    return (

        <div className="page">
            <Header/>

            <p ref={errRef} className={errMsg ? "errMsg" : "offscreen"} aria-live={"assertive"}>{errMsg}</p>
            <main className="main" aria-live={"assertive"}>
                <button className="button">
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
                        <td>
                            <input
                                type="text"
                                id="Name"
                                autoComplete="off"
                                onChange={(e) => setName(e.target.value)}
                            />
                        </td>

                    </tr>
                    {sites.map(site => (
                        <tr key={site.id}>
                            <td>{site.link}</td>
                            <td>{site.name}</td>
                            <td>
                                <label
                                    className={isServerUp[site.id] ? "server-status online" : "server-status offline"}>
                                    {isServerUp[site.id] ? `Server Online: ${lastChecks[site.id]}` : `Server Offline: ${lastChecks[site.id]}`}
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