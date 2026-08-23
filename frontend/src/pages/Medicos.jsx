import { useEffect, useState } from 'react'
import api from '../services/api'

function Medicos() {
  const [medicos, setMedicos] = useState([])
  const [especialidades, setEspecialidades] = useState([])
  const [sedes, setSedes] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState({
    nombre: '', apellido: '', email: '', especialidadId: '', sedeId: ''
  })

  const cargarDatos = async () => {
    try {
      const [medicosRes, especialidadesRes, sedesRes] = await Promise.all([
        api.get('/Medico'),
        api.get('/Especialidad'),
        api.get('/Sede')
      ])
      setMedicos(medicosRes.data)
      setEspecialidades(especialidadesRes.data)
      setSedes(sedesRes.data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const fetchData = async () => { await cargarDatos() }
    fetchData()
  }, [])

  const crear = async () => {
    try {
      await api.post('/Medico', {
        ...form,
        especialidadId: parseInt(form.especialidadId),
        sedeId: parseInt(form.sedeId)
      })
      setForm({ nombre: '', apellido: '', email: '', especialidadId: '', sedeId: '' })
      await cargarDatos()
    } catch (error) {
      console.error(error)
    }
  }

  const eliminar = async (id) => {
    try {
      await api.delete(`/Medico/${id}`)
      await cargarDatos()
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <div className="max-w-3xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Médicos</h1>

      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
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
          <select value={form.especialidadId}
            onChange={e => setForm({ ...form, especialidadId: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500">
            <option value="">Especialidad</option>
            {especialidades.map(e => <option key={e.id} value={e.id}>{e.nombre}</option>)}
          </select>
          <select value={form.sedeId}
            onChange={e => setForm({ ...form, sedeId: e.target.value })}
            className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500">
            <option value="">Sede</option>
            {sedes.map(s => <option key={s.id} value={s.id}>{s.nombre}</option>)}
          </select>
        </div>
        <button onClick={crear}
          className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium">
          Agregar Médico
        </button>
      </div>

      {loading ? (
        <p className="text-gray-500">Cargando...</p>
      ) : (
        <ul className="space-y-2">
          {medicos.map(m => (
            <li key={m.id} className="flex justify-between items-center bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <div>
                <p className="font-medium text-gray-800">{m.nombre} {m.apellido}</p>
                <p className="text-sm text-gray-500">{m.email}</p>
              </div>
              <button onClick={() => eliminar(m.id)}
                className="text-red-500 hover:text-red-700 text-sm font-medium ml-4">
                Eliminar
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default Medicos