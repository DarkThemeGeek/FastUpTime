function UserGreeting(props){
    if(props.isLoggedIn)
        return <h2>Welcome {props.userName}</h2>

}
export default UserGreeting