import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'
import Spinner from '../components/Spinner'

function MiPerfil() {
  const { logout } = useAuth()

  const [perfil, setPerfil] = useState(null)
  const [loading, setLoading] = useState(true)
  const [guardando, setGuardando] = useState(false)
  const [cambiandoPassword, setCambiandoPassword] = useState(false)
  const [form, setForm] = useState({ nombre: '', apellido: '', telefono: '' })
  const [formPassword, setFormPassword] = useState({
    passwordActual: '', passwordNueva: '', confirmarPasswordNueva: ''
  })

  const cargarPerfil = async () => {
    try {
      const res = await api.get('/Paciente/perfil')
      setPerfil(res.data)
      setForm({
        nombre: res.data.nombre,
        apellido: res.data.apellido,
        telefono: res.data.telefono
      })
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    cargarPerfil()
  }, [])

  const guardarPerfil = async () => {
    setGuardando(true)
    try {
      const res = await api.put('/Paciente/perfil', form)
      setPerfil(res.data)
      toast.success('Perfil actualizado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo actualizar el perfil')
    } finally {
      setGuardando(false)
    }
  }

  const cambiarPassword = async () => {
    setCambiandoPassword(true)
    try {
      await api.put('/Paciente/cambiar-password', formPassword)
      setFormPassword({ passwordActual: '', passwordNueva: '', confirmarPasswordNueva: '' })
      toast.success('Contraseña actualizada correctamente. Volvé a iniciar sesión.')
      logout()
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo cambiar la contraseña')
    } finally {
      setCambiandoPassword(false)
    }
  }

  if (loading) {
    return (
      <div className="max-w-2xl mx-auto mt-6 px-4 pb-10">
        <Spinner />
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Mi Perfil</h1>

      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <div>
            <label className="text-sm text-gray-500 mb-1 block">Email</label>
            <input type="email" value={perfil?.email || ''} disabled
              className="w-full border border-gray-200 bg-gray-100 text-gray-500 rounded-lg px-4 py-3" />
          </div>
          <div>
            <label className="text-sm text-gray-500 mb-1 block">Cédula</label>
            <input type="text" value={perfil?.cedula || ''} disabled
              className="w-full border border-gray-200 bg-gray-100 text-gray-500 rounded-lg px-4 py-3" />
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <input type="text" placeholder="Nombre" value={form.nombre}
            onChange={e => setForm({ ...form, nombre: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="text" placeholder="Apellido" value={form.apellido}
            onChange={e => setForm({ ...form, apellido: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>

        <input type="text" placeholder="Teléfono" value={form.telefono}
          onChange={e => setForm({ ...form, telefono: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />

        <button onClick={guardarPerfil} disabled={guardando}
          className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium disabled:opacity-50">
          {guardando ? 'Guardando...' : 'Guardar cambios'}
        </button>
      </div>

      <div className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm space-y-3">
        <h2 className="text-lg font-semibold text-gray-800">Cambiar contraseña</h2>

        <input type="password" placeholder="Contraseña actual" value={formPassword.passwordActual}
          onChange={e => setFormPassword({ ...formPassword, passwordActual: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <input type="password" placeholder="Contraseña nueva" value={formPassword.passwordNueva}
            onChange={e => setFormPassword({ ...formPassword, passwordNueva: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="password" placeholder="Confirmar contraseña nueva" value={formPassword.confirmarPasswordNueva}
            onChange={e => setFormPassword({ ...formPassword, confirmarPasswordNueva: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>

        <button onClick={cambiarPassword} disabled={cambiandoPassword}
          className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium disabled:opacity-50">
          {cambiandoPassword ? 'Actualizando...' : 'Cambiar contraseña'}
        </button>
      </div>
    </div>
  )
}

export default MiPerfil
