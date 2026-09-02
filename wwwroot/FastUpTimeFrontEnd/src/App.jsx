import Header from "./Components/Header.jsx";
import Footer from "./Components/Footer.jsx";

import Body from "./Components/Body.jsx";
import Register from "./Components/Register.jsx";
import Login from "./Components/Login.jsx";
import {BrowserRouter, Routes, Route, Link} from 'react-router-dom';
import UpTimePage from "./Components/UpTimePage.jsx";
import ProtectedPath from "./Components/ProtectedPath.jsx";

function mainPage() {
    return (<>
        <title>FastUpTime</title>
        <Header/>
        <Body/>
        <Register/>
        <Footer/>
    </>)
}


function App() {
    
    return (
        <>

            <main className="App">
                <BrowserRouter>
                    <Routes>
                        <Route path="/" element={<Login/>}/>
                        <Route path="/register" element={<Register/>}/>
                        <Route path="/login" element={<Login/>}/>
                        <Route path="/UpTimePage"
                               element={
                                   //<ProtectedPath>
                                       <UpTimePage/>
                                   //<ProtectedPath>
                               }

                        />

                    </Routes>

                </BrowserRouter>
            </main>

        </>
    );
}

export default App
