
function Footer(){
    return(
        <footer className="footer">
            <p>&copy;{new Date().getFullYear()} FastUpTime</p>
            <nav className="nav">
                <button>Home</button>
                <button>About</button>
                <button>Contact</button>
            </nav>
        </footer>
    );
}
export default Footer