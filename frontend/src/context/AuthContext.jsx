import { createContext, useContext, useState } from 'react'
import { decodeToken } from '../utils/jwt'

const AuthContext = createContext()

export function AuthProvider({ children }) {
  const [token, setToken] = useState(localStorage.getItem('token'))
  const [claims, setClaims] = useState(() => decodeToken(localStorage.getItem('token')))

  const login = (newToken) => {
    localStorage.setItem('token', newToken)
    setToken(newToken)
    setClaims(decodeToken(newToken))
  }

  const logout = () => {
    localStorage.removeItem('token')
    setToken(null)
    setClaims(null)
  }

  const isAuthenticated = !!token
  const rol = claims?.rol || null
  const pacienteId = claims?.pacienteId || null

  return (
    <AuthContext.Provider value={{ token, login, logout, isAuthenticated, rol, pacienteId }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  return useContext(AuthContext)
}
