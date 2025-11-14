import { useNavigate } from 'react-router-dom'
import './Header.css'

function Header() {
  const navigate = useNavigate()

  return (
    <header className="header" onClick={() => navigate('/')}>
      <h1 className="header-title">Matthew Micklewright</h1>
    </header>
  )
}

export default Header

