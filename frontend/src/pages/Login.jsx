import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'

function Login() {
  const [form, setForm] = useState({ email: '', password: '' })
  const [error, setError] = useState('')
  const navigate = useNavigate()
  const { login } = useAuth()

  const handleLogin = async () => {
    try {
      const res = await api.post('/Auth/login', form)
      login(res.data.token)
      navigate('/')
    } catch {
      setError('Email o contraseña incorrectos')
    }
  }

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') handleLogin()
  }

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="bg-white border border-gray-200 rounded-lg p-8 shadow-sm w-full max-w-md">
        <h1 className="text-3xl font-bold text-blue-600 mb-2 text-center">mediAgenda</h1>
        <p className="text-gray-500 text-center mb-6">Iniciá sesión para continuar</p>

        {error && <p className="text-red-500 text-sm mb-4 text-center">{error}</p>}

        <div className="space-y-3">
          <input type="email" placeholder="Email" value={form.email}
            onChange={e => setForm({ ...form, email: e.target.value })}
            onKeyDown={handleKeyDown}
            className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="password" placeholder="Contraseña" value={form.password}
            onChange={e => setForm({ ...form, password: e.target.value })}
            onKeyDown={handleKeyDown}
            className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <button onClick={handleLogin}
            className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium">
            Ingresar
          </button>
        </div>

        <p className="text-sm text-gray-500 text-center mt-6">
          ¿No tenés cuenta? <Link to="/registro" className="text-blue-600 font-medium hover:underline">Registrate</Link>
        </p>
      </div>
    </div>
  )
}

export default Login