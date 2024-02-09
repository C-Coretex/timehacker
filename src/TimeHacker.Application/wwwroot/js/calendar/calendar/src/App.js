import logo from './logo.svg';
import './App.css';
import Week from './components/week'
import { API_URL, setApiUrl } from './tools/variables'

function App() {
  let api = window.location.href
  api = api.replace(window.location.pathname, "/")
  api += 'api'
  setApiUrl(api)

  return (
    <div className='tw-h-full'>
      <div className='tw-flex tw-flex-row tw-items-center tw-justify-center tw-p-4 tw-h-full'>
        <Week />
      </div>
    </div>
  );
}

export default App;
