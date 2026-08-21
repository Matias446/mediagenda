import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'
import { useAuth } from '../context/AuthContext.jsx'

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

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center">
      <div className="bg-white border border-gray-200 rounded-lg p-8 shadow-sm w-full max-w-md">
        <h1 className="text-3xl font-bold text-blue-600 mb-6 text-center">mediAgenda</h1>
        <h2 className="text-xl font-semibold text-gray-700 mb-4">Iniciar sesión</h2>

        {error && <p className="text-red-500 text-sm mb-4">{error}</p>}

        <div className="space-y-3">
          <input type="email" placeholder="Email" value={form.email}
            onChange={e => setForm({ ...form, email: e.target.value })}
            className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="password" placeholder="Contraseña" value={form.password}
            onChange={e => setForm({ ...form, password: e.target.value })}
            className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <button onClick={handleLogin}
            className="w-full bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700">
            Ingresar
          </button>
        </div>
      </div>
    </div>
  )
}

export default Login