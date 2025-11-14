import { useNavigate } from 'react-router-dom'
import './Landing.css'

function Landing() {
  const navigate = useNavigate()

  return (
    <div className="landing">
      <div className="landing-content">
        <h1>My name is Matt and this website stands as a testament that I have too much time on my hands</h1>
        <button onClick={() => navigate('/portfolio')} className="portfolio-button">
          Portfolio
        </button>
      </div>
    </div>
  )
}

export default Landing

