import Header from "./Components/Header.jsx";
import Footer from "./Components/Footer.jsx";

import Body from "./Components/Body.jsx";
import Register from "./Components/Register.jsx";
import Login from "./Components/Login.jsx";
function mainPage(){
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
            <Login/>
            </main>

        </>
    );
}

export default App
