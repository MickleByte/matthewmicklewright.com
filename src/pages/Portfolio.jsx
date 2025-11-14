import { useNavigate } from 'react-router-dom'
import './Portfolio.css'
import { projects } from '../data/projects'

function Portfolio() {
  const navigate = useNavigate()

  const handleProjectClick = (projectName) => {
    navigate(`/portfolio/${projectName}`)
  }

  return (
    <div className="portfolio">
      <div className="portfolio-container">
        <h1>Portfolio</h1>
        <div className="projects-grid">
          {projects.map((project) => (
            <div
              key={project.id}
              className="project-card"
              onClick={() => handleProjectClick(project.slug)}
            >
              <div className="project-image">
                <img src={project.image} alt={project.title} />
              </div>
              <div className="project-info">
                <h2>{project.title}</h2>
                <p>{project.description}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export default Portfolio

