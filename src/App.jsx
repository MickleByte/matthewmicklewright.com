import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import Header from './components/Header'
import Landing from './pages/Landing'
import Portfolio from './pages/Portfolio'
import ProjectDetail from './pages/ProjectDetail'

function App() {
  return (
    <Router>
      <Header />
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/portfolio" element={<Portfolio />} />
        <Route path="/portfolio/:projectName" element={<ProjectDetail />} />
      </Routes>
    </Router>
  )
}

export default App

