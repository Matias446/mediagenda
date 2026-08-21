import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function Navbar() {
  const location = useLocation()
  const navigate = useNavigate()
  const { isAuthenticated, logout } = useAuth()

  const links = [
    { to: '/', label: 'Inicio' },
    { to: '/especialidades', label: 'Especialidades' },
    { to: '/sedes', label: 'Sedes' },
    { to: '/medicos', label: 'Médicos' },
    { to: '/pacientes', label: 'Pacientes' },
    { to: '/turnos', label: 'Turnos' },
  ]

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <nav className="bg-blue-600 text-white shadow-md">
      <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
        <Link to="/" className="text-xl font-bold tracking-tight">
          mediAgenda
        </Link>
        <div className="flex items-center gap-6">
          {isAuthenticated && links.map(link => (
            <Link
              key={link.to}
              to={link.to}
              className={`text-sm font-medium transition-opacity hover:opacity-80 ${
                location.pathname === link.to ? 'underline underline-offset-4' : 'opacity-90'
              }`}
            >
              {link.label}
            </Link>
          ))}
          {isAuthenticated ? (
            <button onClick={handleLogout}
              className="text-sm font-medium bg-white text-blue-600 px-4 py-1 rounded-lg hover:bg-blue-50">
              Cerrar sesión
            </button>
          ) : (
            <Link to="/login"
              className="text-sm font-medium bg-white text-blue-600 px-4 py-1 rounded-lg hover:bg-blue-50">
              Iniciar sesión
            </Link>
          )}
        </div>
      </div>
    </nav>
  )
}

export default Navbar