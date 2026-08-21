import { Link, useLocation } from 'react-router-dom'

function Navbar() {
  const location = useLocation()

  const links = [
    { to: '/', label: 'Inicio' },
    { to: '/especialidades', label: 'Especialidades' },
    { to: '/sedes', label: 'Sedes' },
    { to: '/medicos', label: 'Médicos' },
    { to: '/pacientes', label: 'Pacientes' },
    { to: '/turnos', label: 'Turnos' },
  ]

  return (
    <nav className="bg-blue-600 text-white shadow-md">
      <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
        <Link to="/" className="text-xl font-bold tracking-tight">
          mediAgenda
        </Link>
        <div className="flex gap-6">
          {links.map(link => (
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
        </div>
      </div>
    </nav>
  )
}

export default Navbar