import {useEffect, useState} from "react";

function GetLastUp(props) {
    
    const [sites, setSites]=useState([]);
    
    useEffect(() => {
        const fetchSiteUp = async () => {
            try {
                const response = await fetch(`http://localhost:57057/site/${props.site}`);
                const data = await response.json();

                setSites(data);
            } catch (error) {
                console.error("failed to fetch data about site from api: ", error);
            }
        }

        fetchSiteUp();
        
        //1 second fetch
        const intervalId = setInterval(fetchSiteUp, 1000);
        
        //clear on exit
        return () => {
            clearInterval(intervalId);
        };
    }, []);
    
    //getting the component
    return (
        <div>
            Hello world
            {sites.map(site => ( 
                <div key={site.id}>{site.url}</div>
            ))}
        </div>
    );
}
export  default  GetLastUp