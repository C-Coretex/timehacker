import logo from './logo.svg';
import './App.css';
import Week from './components/week'

function App() {
  return (
    <div className='h-full'>
      <div className='flex flex-row items-center justify-center p-4 h-full'>
        <Week />
      </div>
    </div>
  );
}

export default App;
