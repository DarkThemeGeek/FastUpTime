import Header from "./Components/Header.jsx";
import Footer from "./Components/Footer.jsx";

import Body from "./Components/Body.jsx";
import Register from "./Components/Register.jsx";
import Login from "./Components/Login.jsx";
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
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
                        <Route path="/" element={<Login />} />
                        <Route path="/register" element={<Register/>} />
                        <Route path="/login" element={<Login/>} />
                    </Routes>
                </BrowserRouter>
            </main>

        </>
    );
}

export default App
