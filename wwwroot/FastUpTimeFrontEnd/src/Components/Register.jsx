import "./Register.css"
import {faCheck, faClock, faCircleInfo,} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {useEffect, useRef, useState} from "react";
import {faTimes} from "@fortawesome/free-solid-svg-icons";
import axios from "axios";

const USER_REGEX = /^[a-zA-Z][a-zA-Z0-9-_]{3,23}$/
const PWD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%]).{8,24}$/;

const REGISTER_URL = "https://localhost:8443/auth/register";


function Register() {

    const userRef = useRef();
    const errRef = useRef();


    const [user, SetUser] = useState('');
    const [validName, setValidName] = useState(false);
    const [userFocus, setUserFocus] = useState(false);

    const [pwd, setPwd] = useState('');
    const [validPwd, setValidPwd] = useState(false);
    const [pwdFocus, setPwdFocus] = useState(false);

    const [matchPwd, setMatchPwd] = useState('');
    const [validMatch, setValidMatch] = useState(false);
    const [matchFocus, setMatchFocus] = useState(false);

    const [errMsg, setErrMsg] = useState('');
    const [success, setSuccess] = useState(false);


    useEffect(() => {
        userRef.current.focus();
    }, [])
    //validating username
    useEffect(() => {
        const result = USER_REGEX.test(user);
        setValidName(result);

    }, [user]);
    //validating pwd
    useEffect(() => {
        const result = PWD_REGEX.test(pwd);

        setValidPwd(result);

        const match = pwd === matchPwd
        setValidMatch(match);

    }, [pwd, matchPwd])

    useEffect(() => {
        setErrMsg('')

    }, [user, pwd, matchPwd]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        const v1 = USER_REGEX.test(user);
        const v2 = PWD_REGEX.test(pwd);
        if (!v1 || !v2) {
            setErrMsg("Invalid Entry")
        }

        try {
            const response = await axios.post(REGISTER_URL, {
                UserName: user, Password: pwd

            });

            console.log(response);

            setSuccess(true);
            //clear input fields
        } catch (err) {
            if (!err?.response) {
                setErrMsg('No Server Response' + err?.respose?.data);
            } else if (err.response?.status === 409) {
                setErrMsg('UserName Taken' + err?.respose?.data);
            } else {
                setErrMsg('Registration Failed' + err?.respose?.data);
            }
            errRef.current.focus();
        }


    }
    return (

        <>
            {success ? (
                <section>
                    <h1>Success!</h1>
                    <p>
                        <a href="/login">Sign In</a>
                    </p>
                </section>
            ) : (
                <section>
                    <p ref={errRef} className={errMsg ? "errMsg" : "offscreen"} aria-live={"assertive"}>{errMsg}</p>
                    <h1>Register</h1>
                    <form onSubmit={handleSubmit}>

                        <label htmlFor="username">
                            Username:
                            <span className={validName ? "valid" : "hide"}>
                       <FontAwesomeIcon icon={faCheck}/>
                   </span>
                            <span className={validName || !user ? "hide" : "invalid"}>
                       <FontAwesomeIcon icon={faTimes}/>
                   </span>

                        </label>
                        <input
                            type="text"
                            id="username"
                            ref={userRef}
                            autoComplete="off"
                            onChange={(e) => SetUser(e.target.value)}
                            required
                            aria-invalid={validName ? "false" : "true"}
                            aria-describedby="namenote"
                            onFocus={() => setUserFocus(true)}
                            onBlur={() => setUserFocus(false)}
                        />
                        <p id="namenote" className={userFocus && user && !validName ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faCircleInfo}/>
                            4 to 24 characters.<br/>
                            Must Begin with a letter.<br/>
                            Letters, numbers, underscores, hyphens allowed.
                        </p>


                        <label htmlFor="password">
                            Password:
                            <span className={validPwd ? "valid" : "hide"}>
                       <FontAwesomeIcon icon={faCheck}/>
                   </span>
                            <span className={validPwd || !pwd ? "hide" : "invalid"}>
                       <FontAwesomeIcon icon={faTimes}/>
                   </span>
                        </label>
                        <input
                            type="password"
                            id="password"
                            onChange={(e) => setPwd(e.target.value)}
                            required
                            aria-invalid={validPwd ? "false" : "true"}
                            aria-describedby="pwdnote"
                            onFocus={() => setPwdFocus(true)}
                            onBlur={() => setPwdFocus(false)}
                        />
                        <p id="pwdnote" className={pwdFocus && !validPwd ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faCircleInfo}/>
                            8 to 24 characters.<br/>
                            Must include uppercase and lowercase letters, a number and a special character<br/>
                            Allowed Special characters:
                            <span aria-label="exclamation">!</span>
                            <span aria-label="at symbol">@</span>
                            <span aria-label="hashtag">#</span>
                            <span aria-label="dollar sign">$</span>
                            <span aria-label="percent">%</span>

                        </p>


                        <label htmlFor="confirm_password">
                            Confirm password:
                            <span className={validMatch && matchPwd ? "valid" : "hide"}>
                       <FontAwesomeIcon icon={faCheck}/>
                   </span>
                            <span className={validMatch || !matchPwd ? "hide" : "invalid"}>
                       <FontAwesomeIcon icon={faTimes}/>
                   </span>
                        </label>
                        <input
                            type="password"
                            id="confirm_password"
                            onChange={(e) => setMatchPwd(e.target.value)}
                            required
                            aria-invalid={validPwd ? "false" : "true"}
                            aria-describedby="confirmnote"
                            onFocus={() => setMatchFocus(true)}
                            onBlur={() => setMatchFocus(false)}
                        />
                        <p id="confirmnote" className={matchFocus && !validMatch ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faCircleInfo}/>
                            Must match the first password inputted

                        </p>
                        <button disabled={!validName || !validPwd || !validMatch}>
                            Sign Up
                        </button>

                    </form>
                    <p>
                        Already registered?<br/>
                        <span className={"line"}>
                            <a href="/login">Sign in</a>
                </span>
                    </p>
                </section>
            )}
        </>
    )


}

export default Register