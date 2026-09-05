import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { UserRound } from 'lucide-react'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'
import ModalConfirmacion from '../components/ModalConfirmacion'
import Spinner from '../components/Spinner'
import EmptyState from '../components/EmptyState'

function Medicos() {
  const { rol } = useAuth()
  const esAdmin = rol === 'Admin'

  const [medicos, setMedicos] = useState([])
  const [especialidades, setEspecialidades] = useState([])
  const [sedes, setSedes] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState({
    nombre: '', apellido: '', email: '', cedula: '', especialidadId: '', sedeId: ''
  })
  const [editandoId, setEditandoId] = useState(null)
  const [formEdicion, setFormEdicion] = useState(null)
  const [aEliminar, setAEliminar] = useState(null)

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
      setForm({ nombre: '', apellido: '', email: '', cedula: '', especialidadId: '', sedeId: '' })
      await cargarDatos()
      toast.success('Médico creado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo crear el médico')
    }
  }

  const eliminar = async (id) => {
    try {
      await api.delete(`/Medico/${id}`)
      await cargarDatos()
      toast.success('Médico eliminado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo eliminar el médico')
    } finally {
      setAEliminar(null)
    }
  }

  const empezarEdicion = (m) => {
    setEditandoId(m.id)
    setFormEdicion({
      nombre: m.nombre, apellido: m.apellido, email: m.email,
      cedula: '', especialidadId: m.especialidadId, sedeId: m.sedeId
    })
  }

  const guardarEdicion = async (id) => {
    try {
      await api.put(`/Medico/${id}`, {
        ...formEdicion,
        especialidadId: parseInt(formEdicion.especialidadId),
        sedeId: parseInt(formEdicion.sedeId)
      })
      setEditandoId(null)
      setFormEdicion(null)
      await cargarDatos()
      toast.success('Médico actualizado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo actualizar el médico')
    }
  }

  return (
    <div className="max-w-3xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Médicos</h1>

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
            <input type="text" placeholder="Cédula" value={form.cedula}
              onChange={e => setForm({ ...form, cedula: e.target.value })}
              className="border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>
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
      )}

      {loading ? (
        <Spinner />
      ) : medicos.length === 0 ? (
        <EmptyState icono={UserRound} mensaje="No hay médicos registrados." />
      ) : (
        <ul className="space-y-2">
          {medicos.map(m => (
            <li key={m.id} className="bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              {editandoId === m.id ? (
                <div className="space-y-2">
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                    <input type="text" placeholder="Nombre" value={formEdicion.nombre}
                      onChange={e => setFormEdicion({ ...formEdicion, nombre: e.target.value })}
                      className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                    <input type="text" placeholder="Apellido" value={formEdicion.apellido}
                      onChange={e => setFormEdicion({ ...formEdicion, apellido: e.target.value })}
                      className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                    <input type="email" placeholder="Email" value={formEdicion.email}
                      onChange={e => setFormEdicion({ ...formEdicion, email: e.target.value })}
                      className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                    <input type="text" placeholder="Cédula (dejar vacío para no cambiar)" value={formEdicion.cedula}
                      onChange={e => setFormEdicion({ ...formEdicion, cedula: e.target.value })}
                      className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                  </div>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                    <select value={formEdicion.especialidadId}
                      onChange={e => setFormEdicion({ ...formEdicion, especialidadId: e.target.value })}
                      className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                      {especialidades.map(e => <option key={e.id} value={e.id}>{e.nombre}</option>)}
                    </select>
                    <select value={formEdicion.sedeId}
                      onChange={e => setFormEdicion({ ...formEdicion, sedeId: e.target.value })}
                      className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                      {sedes.map(s => <option key={s.id} value={s.id}>{s.nombre}</option>)}
                    </select>
                  </div>
                  <div className="flex gap-2">
                    <button onClick={() => guardarEdicion(m.id)}
                      className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700">
                      Guardar
                    </button>
                    <button onClick={() => { setEditandoId(null); setFormEdicion(null) }}
                      className="flex-1 bg-gray-100 text-gray-700 px-4 py-2 rounded-lg text-sm font-medium hover:bg-gray-200">
                      Cancelar
                    </button>
                  </div>
                </div>
              ) : (
                <div className="flex justify-between items-center">
                  <div>
                    <p className="font-medium text-gray-800">{m.nombre} {m.apellido}</p>
                    <p className="text-sm text-gray-500">{m.email}</p>
                  </div>
                  {esAdmin && (
                    <div className="flex gap-3 ml-4">
                      <button onClick={() => empezarEdicion(m)}
                        className="text-blue-600 hover:text-blue-800 text-sm font-medium">
                        Editar
                      </button>
                      <button onClick={() => setAEliminar(m)}
                        className="text-red-500 hover:text-red-700 text-sm font-medium">
                        Eliminar
                      </button>
                    </div>
                  )}
                </div>
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

export default Medicos
