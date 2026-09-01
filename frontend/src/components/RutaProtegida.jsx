import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

function RutaProtegida({ children, roles }) {
  const { isAuthenticated, rol } = useAuth()
  if (!isAuthenticated) return <Navigate to="/login" />
  if (roles && !roles.includes(rol)) return <Navigate to="/" />
  return children
}

export default RutaProtegida
