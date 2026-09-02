import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'

function Registro() {
  const [form, setForm] = useState({
    email: '', password: '', confirmPassword: '',
    cedula: '', nombre: '', apellido: '', telefono: '', fechaNacimiento: ''
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()
  const { login } = useAuth()

  const handleRegister = async () => {
    setError('')
    setLoading(true)
    try {
      await api.post('/Auth/register', form)
      const res = await api.post('/Auth/login', { email: form.email, password: form.password })
      login(res.data.token)
      navigate('/')
    } catch (err) {
      setError(err.response?.data?.mensaje || 'No se pudo completar el registro')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4 py-10">
      <div className="bg-white border border-gray-200 rounded-lg p-8 shadow-sm w-full max-w-md">
        <h1 className="text-3xl font-bold text-blue-600 mb-2 text-center">mediAgenda</h1>
        <p className="text-gray-500 text-center mb-6">Creá tu cuenta de paciente</p>

        {error && <p className="text-red-500 text-sm mb-4 text-center">{error}</p>}

        <div className="space-y-3">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <input type="text" placeholder="Nombre" value={form.nombre}
              onChange={e => setForm({ ...form, nombre: e.target.value })}
              className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
            <input type="text" placeholder="Apellido" value={form.apellido}
              onChange={e => setForm({ ...form, apellido: e.target.value })}
              className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>

          <input type="email" placeholder="Email" value={form.email}
            onChange={e => setForm({ ...form, email: e.target.value })}
            className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <input type="password" placeholder="Contraseña" value={form.password}
              onChange={e => setForm({ ...form, password: e.target.value })}
              className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
            <input type="password" placeholder="Confirmar contraseña" value={form.confirmPassword}
              onChange={e => setForm({ ...form, confirmPassword: e.target.value })}
              className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <input type="text" placeholder="Cédula" value={form.cedula}
              onChange={e => setForm({ ...form, cedula: e.target.value })}
              className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
            <input type="text" placeholder="Teléfono" value={form.telefono}
              onChange={e => setForm({ ...form, telefono: e.target.value })}
              className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>

          <div>
            <label className="text-sm text-gray-500 mb-1 block">Fecha de nacimiento</label>
            <input type="date" value={form.fechaNacimiento}
              onChange={e => setForm({ ...form, fechaNacimiento: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>

          <button onClick={handleRegister} disabled={loading}
            className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium disabled:opacity-50">
            {loading ? 'Creando cuenta...' : 'Registrarme'}
          </button>
        </div>

        <p className="text-sm text-gray-500 text-center mt-6">
          ¿Ya tenés cuenta? <Link to="/login" className="text-blue-600 font-medium hover:underline">Iniciá sesión</Link>
        </p>

      </div>
    </div>
  )
}

export default Registro
