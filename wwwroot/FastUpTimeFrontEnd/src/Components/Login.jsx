import "./Register.css"
import {faCheck, faClock, faCircleInfo,} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {useContext, useEffect, useRef, useState} from "react";
import {faTimes} from "@fortawesome/free-solid-svg-icons";
import axios from "axios";
import AuthContext from "../context/AuthProvider.jsx";

const USER_REGEX = /^[a-zA-Z][a-zA-Z0-9-_]{3,23}$/
const PWD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%]).{8,24}$/;


const LOGIN_URL = "https://localhost:8443/auth/login";


function Login() {
    const {SetAuth} = useContext(AuthContext);
    const userRef = useRef();
    const errRef = useRef();


    const [user, SetUser] = useState('');
    const [pwd, setPwd] = useState('');

    const [errMsg, setErrMsg] = useState('');
    const [success, setSuccess] = useState(false);

    useEffect(() => {
        userRef.current.focus();
    }, [])

    useEffect(() => {
        setErrMsg('');
    }, [user, pwd])

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(LOGIN_URL, {
                UserName: user,
                Password: pwd
                
            }, {headers: {"Content-Type": "application/json"}, withCredentials: true});
            console.log(response?.data)
            
            const accessToken = response?.data?.accessToken;
            const roles = response?.data?.roles;
            
            SetAuth({user,pwd,roles,accessToken});
            SetUser('');
            setPwd('');
            setSuccess(true);
            
        } catch (err) {
            if(!err?.response){
                setErrMsg('No server response');
            }
            else if(err.response?.status === 400){
                setErrMsg("Missing username or password")
            }
            else if(err.response?.status === 401){
                setErrMsg("Unauthorized")
            }
            else {
                setErrMsg("LoginFailed")
            }
            errRef.current.focus();
            
        }


        
    }
    return (

        <>
            {success ? (
                <section>
                    <h1>You are logged in!</h1><br/>
                    <p>
                        <a href="/UpTimePage">Go to Home</a>
                    </p>
                </section>
            ) : (
                <section>
                    <p ref={errRef} className={errMsg ? "errMsg" : "offscreen"} aria-live={"assertive"}>{errMsg}</p>
                    <h1>Sign In</h1>
                    <form onSubmit={handleSubmit}>
                        <label htmlFor="username">
                            Username:
                        </label>
                        <input
                            type="text"
                            id="username"
                            ref={userRef}
                            autoComplete="off"
                            onChange={(e) => SetUser(e.target.value)}
                            required
                            value={user}
                        />
                        <label htmlFor="password">
                            Password:
                        </label>
                        <input
                            type="password"
                            id="password"
                            onChange={(e) => setPwd(e.target.value)}
                            required
                            value={pwd}
                        />
                        <button>Sign in</button>
                    </form>
                    <p>
                        Need and Account?<br/>
                        <span className={"line"}>
                            <a href="/register">Sign Up</a>
                        </span>
                    </p>
                </section>
            )}
        </>
    )


}

export default Login