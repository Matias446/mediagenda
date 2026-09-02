import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'
import ModalConfirmacion from '../components/ModalConfirmacion'

function Pacientes() {
  const { rol } = useAuth()
  const esAdmin = rol === 'Admin'

  const [pacientes, setPacientes] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState({
    nombre: '', apellido: '', email: '', password: '',
    cedula: '', telefono: '', fechaNacimiento: ''
  })
  const [aEliminar, setAEliminar] = useState(null)

  const cargarPacientes = async () => {
    try {
      const res = await api.get('/Paciente')
      setPacientes(res.data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const fetchData = async () => { await cargarPacientes() }
    fetchData()
  }, [])

  const crear = async () => {
    try {
      await api.post('/Paciente', form)
      setForm({ nombre: '', apellido: '', email: '', password: '', cedula: '', telefono: '', fechaNacimiento: '' })
      await cargarPacientes()
      toast.success('Paciente creado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo crear el paciente')
    }
  }

  const eliminar = async (id) => {
    try {
      await api.delete(`/Paciente/${id}`)
      await cargarPacientes()
      toast.success('Paciente eliminado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo eliminar el paciente')
    } finally {
      setAEliminar(null)
    }
  }

  return (
    <div className="max-w-3xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Pacientes</h1>

      {esAdmin && (
      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <input type="text" placeholder="Nombre" value={form.nombre}
            onChange={e => setForm({ ...form, nombre: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="text" placeholder="Apellido" value={form.apellido}
            onChange={e => setForm({ ...form, apellido: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <input type="email" placeholder="Email" value={form.email}
            onChange={e => setForm({ ...form, email: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="password" placeholder="Contraseña" value={form.password}
            onChange={e => setForm({ ...form, password: e.target.value })}
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
        <button onClick={crear}
          className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium">
          Agregar Paciente
        </button>
      </div>
      )}

      {loading ? (
        <p className="text-gray-500">Cargando...</p>
      ) : (
        <ul className="space-y-2">
          {pacientes.map(p => (
            <li key={p.id} className="flex justify-between items-center bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <div>
                <p className="font-medium text-gray-800">{p.nombre} {p.apellido}</p>
                <p className="text-sm text-gray-500">{p.email} · {p.cedula}</p>
              </div>
              {esAdmin && (
                <button onClick={() => setAEliminar(p)}
                  className="text-red-500 hover:text-red-700 text-sm font-medium ml-4">
                  Eliminar
                </button>
              )}
            </li>
          ))}
        </ul>
      )}

      <ModalConfirmacion
        abierto={!!aEliminar}
        mensaje={`¿Estás seguro que querés eliminar ${aEliminar?.nombre} ${aEliminar?.apellido}? Esta acción no se puede deshacer.`}
        onConfirmar={() => eliminar(aEliminar.id)}
        onCancelar={() => setAEliminar(null)}
      />
    </div>
  )
}

export default Pacientes