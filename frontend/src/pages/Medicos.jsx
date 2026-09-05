import { useEffect, useMemo, useState } from 'react'
import toast from 'react-hot-toast'
import { UserRound, Pencil, Trash2, Mail } from 'lucide-react'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'
import ModalConfirmacion from '../components/ModalConfirmacion'
import Spinner from '../components/Spinner'
import EmptyState from '../components/EmptyState'
import Paginacion from '../components/Paginacion'

const POR_PAGINA = 10

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
  const [filtroEspecialidad, setFiltroEspecialidad] = useState('')
  const [filtroSede, setFiltroSede] = useState('')
  const [pagina, setPagina] = useState(1)

  const hayFiltrosActivos = filtroEspecialidad !== '' || filtroSede !== ''

  const medicosFiltrados = useMemo(() => {
    return medicos.filter(m => {
      const matchEspecialidad = !filtroEspecialidad || m.especialidadId === parseInt(filtroEspecialidad)
      const matchSede = !filtroSede || m.sedeId === parseInt(filtroSede)
      return matchEspecialidad && matchSede
    })
  }, [medicos, filtroEspecialidad, filtroSede])

  const totalPaginas = Math.max(1, Math.ceil(medicosFiltrados.length / POR_PAGINA))
  const medicosPaginados = useMemo(() => {
    const inicio = (pagina - 1) * POR_PAGINA
    return medicosFiltrados.slice(inicio, inicio + POR_PAGINA)
  }, [medicosFiltrados, pagina])

  const handleFiltroEspecialidad = (valor) => {
    setFiltroEspecialidad(valor)
    setPagina(1)
  }

  const handleFiltroSede = (valor) => {
    setFiltroSede(valor)
    setPagina(1)
  }

  const limpiarFiltros = () => {
    setFiltroEspecialidad('')
    setFiltroSede('')
    setPagina(1)
  }

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
    <div className="max-w-5xl mx-auto mt-6 px-4 pb-10">
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
        <>
          <div className="flex flex-col sm:flex-row gap-3 mb-4">
            <select value={filtroEspecialidad}
              onChange={e => handleFiltroEspecialidad(e.target.value)}
              className="flex-1 border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Todas las especialidades</option>
              {especialidades.map(e => <option key={e.id} value={e.id}>{e.nombre}</option>)}
            </select>
            <select value={filtroSede}
              onChange={e => handleFiltroSede(e.target.value)}
              className="flex-1 border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Todas las sedes</option>
              {sedes.map(s => <option key={s.id} value={s.id}>{s.nombre}</option>)}
            </select>
            {hayFiltrosActivos && (
              <button onClick={limpiarFiltros}
                className="px-4 py-3 rounded-lg border border-gray-300 text-gray-600 hover:bg-gray-50 text-sm font-medium whitespace-nowrap">
                Limpiar filtros
              </button>
            )}
          </div>

          {medicosFiltrados.length === 0 ? (
            <p className="text-center text-gray-500 py-6">No se encontraron médicos con esos filtros.</p>
          ) : (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {medicosPaginados.map(m => (
                  <div key={m.id} className="bg-white border border-gray-200 rounded-lg shadow-sm p-5 relative">
                    {esAdmin && editandoId !== m.id && (
                      <div className="absolute top-3 right-3 flex gap-1">
                        <button onClick={() => empezarEdicion(m)} title="Editar"
                          className="text-blue-600 hover:text-blue-800 hover:bg-blue-50 p-1.5 rounded-lg transition-colors">
                          <Pencil size={16} />
                        </button>
                        <button onClick={() => setAEliminar(m)} title="Eliminar"
                          className="text-red-500 hover:text-red-700 hover:bg-red-50 p-1.5 rounded-lg transition-colors">
                          <Trash2 size={16} />
                        </button>
                      </div>
                    )}

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
                        <input type="email" placeholder="Email" value={formEdicion.email}
                          onChange={e => setFormEdicion({ ...formEdicion, email: e.target.value })}
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                        <input type="text" placeholder="Cédula (dejar vacío para no cambiar)" value={formEdicion.cedula}
                          onChange={e => setFormEdicion({ ...formEdicion, cedula: e.target.value })}
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                        <select value={formEdicion.especialidadId}
                          onChange={e => setFormEdicion({ ...formEdicion, especialidadId: e.target.value })}
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                          {especialidades.map(e => <option key={e.id} value={e.id}>{e.nombre}</option>)}
                        </select>
                        <select value={formEdicion.sedeId}
                          onChange={e => setFormEdicion({ ...formEdicion, sedeId: e.target.value })}
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                          {sedes.map(s => <option key={s.id} value={s.id}>{s.nombre}</option>)}
                        </select>
                        <div className="flex gap-2 pt-1">
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
                      <div className="flex flex-col items-center text-center">
                        <div className="bg-blue-50 text-blue-600 rounded-full p-3 mb-3">
                          <UserRound size={28} />
                        </div>
                        <p className="font-semibold text-gray-800 text-lg">{m.nombre} {m.apellido}</p>
                        <p className="text-blue-600 text-sm font-medium mt-1">{m.especialidadNombre}</p>
                        <p className="text-gray-500 text-sm">{m.sedeNombre}</p>
                        <p className="flex items-center gap-1 text-gray-400 text-xs mt-3">
                          <Mail size={12} /> {m.email}
                        </p>
                      </div>
                    )}
                  </div>
                ))}
              </div>
              <Paginacion
                paginaActual={pagina}
                totalPaginas={totalPaginas}
                totalResultados={medicosFiltrados.length}
                porPagina={POR_PAGINA}
                onCambiarPagina={setPagina}
              />
            </>
          )}
        </>
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
