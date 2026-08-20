import { useEffect, useState } from 'react'
import api from '../services/api'

function Pacientes() {
  const [pacientes, setPacientes] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState({
    nombre: '', apellido: '', email: '', password: '',
    cedula: '', telefono: '', fechaNacimiento: ''
  })

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
    } catch (error) {
      console.error(error)
    }
  }

  const eliminar = async (id) => {
    try {
      await api.delete(`/Paciente/${id}`)
      await cargarPacientes()
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <div className="max-w-3xl mx-auto mt-10 p-6">
      <h1 className="text-3xl font-bold text-blue-600 mb-6">Pacientes</h1>

      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
        <div className="flex gap-2">
          <input type="text" placeholder="Nombre" value={form.nombre}
            onChange={e => setForm({ ...form, nombre: e.target.value })}
            className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="text" placeholder="Apellido" value={form.apellido}
            onChange={e => setForm({ ...form, apellido: e.target.value })}
            className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>
        <div className="flex gap-2">
          <input type="email" placeholder="Email" value={form.email}
            onChange={e => setForm({ ...form, email: e.target.value })}
            className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="password" placeholder="Contraseña" value={form.password}
            onChange={e => setForm({ ...form, password: e.target.value })}
            className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>
        <div className="flex gap-2">
          <input type="text" placeholder="Cédula" value={form.cedula}
            onChange={e => setForm({ ...form, cedula: e.target.value })}
            className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          <input type="text" placeholder="Teléfono" value={form.telefono}
            onChange={e => setForm({ ...form, telefono: e.target.value })}
            className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>
        <input type="date" value={form.fechaNacimiento}
          onChange={e => setForm({ ...form, fechaNacimiento: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        <button onClick={crear}
          className="w-full bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700">
          Agregar Paciente
        </button>
      </div>

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
              <button onClick={() => eliminar(p.id)} className="text-red-500 hover:text-red-700 text-sm">
                Eliminar
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default Pacientes