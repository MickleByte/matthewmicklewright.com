import { useParams, useNavigate } from 'react-router-dom'
import './ProjectDetail.css'
import { projects } from '../data/projects'

function ProjectDetail() {
  const { projectName } = useParams()
  const navigate = useNavigate()
  
  const project = projects.find(p => p.slug === projectName)

  if (!project) {
    return (
      <div className="project-detail">
        <div className="project-detail-container">
          <h1>Project Not Found</h1>
          <button onClick={() => navigate('/portfolio')}>Back to Portfolio</button>
        </div>
      </div>
    )
  }

  return (
    <div className="project-detail">
      <div className="project-detail-container">
        <button onClick={() => navigate('/portfolio')} className="back-button">
          ← Back to Portfolio
        </button>
        <div className="project-header">
          <h1>{project.title}</h1>
        </div>
        <div className="project-image-large">
          <img src={project.image} alt={project.title} />
        </div>
        <div className="project-content">
          <p className="project-description">{project.description}</p>
          {project.fullDescription && (
            <div className="project-full-description">
              {project.fullDescription}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default ProjectDetail

