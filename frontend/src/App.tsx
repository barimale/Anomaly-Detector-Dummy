import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";
import {useEffect, useState }from "react"
import FilesUpload from "./components/FilesUpload";
import { HubConnectionBuilder, LogLevel, IHttpConnectionOptions } from '@microsoft/signalr';

const App: React.FC = () => {
  const userToken = "adsjalksdjalkdsjadlka";//useContext(AuthContext);
  const [messageLengthOver0, setMessageLengthOver0] = useState<boolean>(false);
  const [localesInProgress, setLocalesInProgress] = useState<boolean | null>(null);
  const options: IHttpConnectionOptions = {
    accessTokenFactory: () => `${userToken}`,
    withCredentials: false,
  };
  const connection = new HubConnectionBuilder()
    .withUrl(`http://localhost:53654/localesHub`, options)
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect()
    .build();

  connection.on('OnFinishAsync', (id: string) => {
    console.log(`${id} finished.`);
    // alert(`${id} finished.`);
    setLocalesInProgress(false);
  });

  connection.on('OnStartAsync', (id: string) => {
    console.log(`${id} started.`);
        // alert(`${id} started.`);
    setLocalesInProgress(true);
  });

  useEffect(() => {
    connection.start()
      .then(() => {
        console.log('connection started');
      }).catch((error: any) => {
        console.log(error);
      });

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <div className="container" style={{ width: "600px" }}>
      <div className="my-3">
        <h3>HackNation 2025</h3>
        <h4>Wyszukiwarka anomalii</h4>
      </div>

      <FilesUpload OnMessageChanged={() => {setMessageLengthOver0(true)}}/>
      <p style={{
        color: localesInProgress != null ? (localesInProgress ? 'green' : 'red') : 'blue'
      }}>{localesInProgress != null && messageLengthOver0? (localesInProgress ? 
        'Brak anomalii!' : 
        'Wykryto anomalie!') : 
        'Praca w toku'}
      </p>
    </div>
  );
}

export default App;
