import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { Home, Stethoscope, MapPin, UserRound, Users, Calendar, LogOut, UserCog } from 'lucide-react'
import { useAuth } from '../context/AuthContext.jsx'

function Navbar() {
  const location = useLocation()
  const navigate = useNavigate()
  const { isAuthenticated, rol, logout } = useAuth()
  const [menuAbierto, setMenuAbierto] = useState(false)

  const linksPorRol = {
    Admin: [
      { to: '/', label: 'Inicio', icono: Home },
      { to: '/especialidades', label: 'Especialidades', icono: Stethoscope },
      { to: '/sedes', label: 'Sedes', icono: MapPin },
      { to: '/medicos', label: 'Médicos', icono: UserRound },
      { to: '/pacientes', label: 'Pacientes', icono: Users },
      { to: '/turnos', label: 'Turnos', icono: Calendar },
    ],
    Administrativo: [
      { to: '/', label: 'Inicio', icono: Home },
      { to: '/medicos', label: 'Médicos', icono: UserRound },
      { to: '/turnos', label: 'Turnos', icono: Calendar },
      { to: '/pacientes', label: 'Pacientes', icono: Users },
    ],
    Paciente: [
      { to: '/', label: 'Inicio', icono: Home },
      { to: '/turnos', label: 'Mis Turnos', icono: Calendar },
      { to: '/medicos', label: 'Médicos', icono: UserRound },
      { to: '/mi-perfil', label: 'Mi Perfil', icono: UserCog },
    ],
  }

  const links = linksPorRol[rol] || []

  const handleLogout = () => {
    logout()
    navigate('/login')
    setMenuAbierto(false)
  }

  return (
    <nav className="bg-blue-600 text-white shadow-md">
      <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
        <Link to="/" className="text-xl font-bold tracking-tight">
          mediAgenda
        </Link>

        {/* Desktop */}
        <div className="hidden md:flex items-center gap-6">
          {isAuthenticated && links.map(link => (
            <Link key={link.to} to={link.to}
              className={`flex items-center gap-1.5 text-sm font-medium transition-opacity hover:opacity-80 ${
                location.pathname === link.to ? 'underline underline-offset-4' : 'opacity-90'
              }`}>
              <link.icono size={16} />
              {link.label}
            </Link>
          ))}
          {isAuthenticated ? (
            <button onClick={handleLogout}
              className="flex items-center gap-1.5 text-sm font-medium bg-white text-blue-600 px-4 py-1 rounded-lg hover:bg-blue-50">
              <LogOut size={16} />
              Cerrar sesión
            </button>
          ) : (
            <Link to="/login"
              className="text-sm font-medium bg-white text-blue-600 px-4 py-1 rounded-lg hover:bg-blue-50">
              Iniciar sesión
            </Link>
          )}
        </div>

        {/* Mobile burger */}
        <button className="md:hidden flex flex-col gap-1.5" onClick={() => setMenuAbierto(!menuAbierto)}>
          <span className={`block w-6 h-0.5 bg-white transition-transform ${menuAbierto ? 'rotate-45 translate-y-2' : ''}`} />
          <span className={`block w-6 h-0.5 bg-white transition-opacity ${menuAbierto ? 'opacity-0' : ''}`} />
          <span className={`block w-6 h-0.5 bg-white transition-transform ${menuAbierto ? '-rotate-45 -translate-y-2' : ''}`} />
        </button>
      </div>

      {/* Mobile menu */}
      {menuAbierto && (
        <div className="md:hidden px-6 pb-4 flex flex-col gap-3">
          {isAuthenticated && links.map(link => (
            <Link key={link.to} to={link.to}
              onClick={() => setMenuAbierto(false)}
              className={`flex items-center gap-2 text-sm font-medium py-2 border-b border-blue-500 ${
                location.pathname === link.to ? 'font-bold' : 'opacity-90'
              }`}>
              <link.icono size={16} />
              {link.label}
            </Link>
          ))}
          {isAuthenticated ? (
            <button onClick={handleLogout}
              className="flex items-center gap-2 text-sm font-medium bg-white text-blue-600 px-4 py-2 rounded-lg hover:bg-blue-50 mt-2">
              <LogOut size={16} />
              Cerrar sesión
            </button>
          ) : (
            <Link to="/login" onClick={() => setMenuAbierto(false)}
              className="text-sm font-medium bg-white text-blue-600 px-4 py-2 rounded-lg hover:bg-blue-50 mt-2">
              Iniciar sesión
            </Link>
          )}
        </div>
      )}
    </nav>
  )
}

export default Navbar