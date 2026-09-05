import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { Calendar, CalendarX } from 'lucide-react'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'
import ModalConfirmacion from '../components/ModalConfirmacion'
import Spinner from '../components/Spinner'
import EmptyState from '../components/EmptyState'

function Turnos() {
  const { rol, pacienteId } = useAuth()
  const esPaciente = rol === 'Paciente'
  const puedeGestionar = rol === 'Admin' || rol === 'Administrativo'

  const [turnos, setTurnos] = useState([])
  const [medicos, setMedicos] = useState([])
  const [pacientes, setPacientes] = useState([])
  const [loading, setLoading] = useState(true)
  const [slotsDisponibles, setSlotsDisponibles] = useState([])
  const [form, setForm] = useState({
    pacienteId: '', medicoId: '', fecha: '', slotSeleccionado: ''
  })
  const [aCancelar, setACancelar] = useState(null)

  const cargarDatos = async () => {
    try {
      const turnosPromise = esPaciente
        ? api.get(`/Turno/paciente/${pacienteId}`)
        : api.get('/Turno')

      const requests = [turnosPromise, api.get('/Medico')]
      if (!esPaciente) requests.push(api.get('/Paciente'))

      const [turnosRes, medicosRes, pacientesRes] = await Promise.all(requests)
      setTurnos(turnosRes.data)
      setMedicos(medicosRes.data)
      if (!esPaciente) setPacientes(pacientesRes.data)
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

  const cargarSlots = async (medicoId, fecha) => {
    if (!medicoId || !fecha) return
    try {
      const res = await api.get(`/Turno/disponibles?medicoId=${medicoId}&fecha=${fecha}`)
      setSlotsDisponibles(res.data)
      setForm(prev => ({ ...prev, slotSeleccionado: '' }))
    } catch (error) {
      console.error(error)
    }
  }

  const handleMedicoChange = (e) => {
    const medicoId = e.target.value
    setForm(prev => ({ ...prev, medicoId, slotSeleccionado: '' }))
    cargarSlots(medicoId, form.fecha)
  }

  const handleFechaChange = (e) => {
    const fecha = e.target.value
    setForm(prev => ({ ...prev, fecha, slotSeleccionado: '' }))
    cargarSlots(form.medicoId, fecha)
  }

  const crear = async () => {
    const idPaciente = esPaciente ? pacienteId : form.pacienteId
    if (!idPaciente || !form.medicoId || !form.slotSeleccionado) return
    try {
      await api.post('/Turno', {
        pacienteId: parseInt(idPaciente),
        medicoId: parseInt(form.medicoId),
        fechaHora: new Date(form.slotSeleccionado).toISOString()
      })
      setForm({ pacienteId: '', medicoId: '', fecha: '', slotSeleccionado: '' })
      setSlotsDisponibles([])
      await cargarDatos()
      toast.success('Turno reservado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo reservar el turno')
    }
  }

  const confirmar = async (id) => {
    try {
      await api.put(`/Turno/${id}/confirmar`)
      await cargarDatos()
      toast.success('Turno confirmado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo confirmar el turno')
    }
  }

  const cancelar = async (id) => {
    try {
      await api.put(`/Turno/${id}/cancelar`)
      await cargarDatos()
      toast.success('Turno cancelado correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo cancelar el turno')
    } finally {
      setACancelar(null)
    }
  }

  const estadoColor = (estado) => {
    switch (estado) {
      case 'Pendiente': return 'text-yellow-600'
      case 'Confirmado': return 'text-green-600'
      case 'Cancelado': return 'text-red-500'
      case 'Completado': return 'text-blue-600'
      default: return 'text-gray-600'
    }
  }

  const formatHora = (isoString) => {
    const date = new Date(isoString)
    return date.toLocaleTimeString('es-UY', { hour: '2-digit', minute: '2-digit' })
  }

  const esFinDeSemana = (fechaStr) => {
    if (!fechaStr) return false
    const dia = new Date(`${fechaStr}T00:00:00Z`).getUTCDay()
    return dia === 0 || dia === 6
  }

  const puedeCrear = esPaciente
    ? (form.medicoId && form.slotSeleccionado)
    : (form.pacienteId && form.medicoId && form.slotSeleccionado)

  return (
    <div className="max-w-3xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">
        {esPaciente ? 'Mis Turnos' : 'Turnos'}
      </h1>

      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
        {!esPaciente && (
          <select value={form.pacienteId}
            onChange={e => setForm({ ...form, pacienteId: e.target.value })}
            className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500">
            <option value="">Seleccioná un paciente</option>
            {pacientes.map(p => <option key={p.id} value={p.id}>{p.nombre} {p.apellido}</option>)}
          </select>
        )}

        <select value={form.medicoId} onChange={handleMedicoChange}
          className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">Seleccioná un médico</option>
          {medicos.map(m => <option key={m.id} value={m.id}>{m.nombre} {m.apellido}</option>)}
        </select>

        <div>
          <label className="text-sm text-gray-500 mb-1 block">Fecha del turno</label>
          <input type="date" value={form.fecha} onChange={handleFechaChange}
            min={new Date().toISOString().split('T')[0]}
            className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        </div>

        {slotsDisponibles.length > 0 && (
          <div>
            <p className="text-sm text-gray-500 mb-2">Horarios disponibles:</p>
            <div className="grid grid-cols-3 sm:grid-cols-4 gap-2">
              {slotsDisponibles.map(slot => (
                <button key={slot}
                  onClick={() => setForm(prev => ({ ...prev, slotSeleccionado: slot }))}
                  className={`py-2 px-3 rounded-lg text-sm border transition-colors ${
                    form.slotSeleccionado === slot
                      ? 'bg-blue-600 text-white border-blue-600'
                      : 'bg-white text-gray-700 border-gray-300 hover:border-blue-400'
                  }`}>
                  {formatHora(slot)}
                </button>
              ))}
            </div>
          </div>
        )}

        {slotsDisponibles.length === 0 && form.medicoId && form.fecha && (
          <div className="flex items-center gap-2 text-sm text-red-500">
            <CalendarX size={18} />
            <p>
              {esFinDeSemana(form.fecha)
                ? 'No se atiende los fines de semana. Elegí un día hábil.'
                : 'No hay horarios disponibles para este día.'}
            </p>
          </div>
        )}

        <button onClick={crear}
          disabled={!puedeCrear}
          className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium disabled:opacity-50 disabled:cursor-not-allowed">
          Reservar Turno
        </button>
      </div>

      {loading ? (
        <Spinner />
      ) : turnos.length === 0 ? (
        <EmptyState icono={Calendar} mensaje="No hay turnos registrados." />
      ) : (
        <ul className="space-y-2">
          {turnos.map(t => (
            <li key={t.id} className="bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <div className="flex justify-between items-start">
                <div>
                  <p className="font-medium text-gray-800">
                    {esPaciente
                      ? (t.nombreMedico || `Médico #${t.medicoId}`)
                      : `${t.nombrePaciente || `Paciente #${t.pacienteId}`} · ${t.nombreMedico || `Médico #${t.medicoId}`}`}
                  </p>
                  <p className="text-sm text-gray-500 mt-1">
                    {new Date(t.fechaHora).toLocaleString('es-UY')}
                  </p>
                  <p className={`text-sm font-medium mt-1 ${estadoColor(t.estado)}`}>{t.estado}</p>
                </div>
                <div className="flex items-center gap-3 ml-4">
                  {t.estado === 'Pendiente' && puedeGestionar && (
                    <button onClick={() => confirmar(t.id)}
                      className="text-green-600 hover:text-green-700 text-sm font-medium">
                      Confirmar
                    </button>
                  )}
                  {t.estado === 'Pendiente' && (
                    <button onClick={() => setACancelar(t)}
                      className="text-red-500 hover:text-red-700 text-sm font-medium">
                      Cancelar
                    </button>
                  )}
                </div>
              </div>
            </li>
          ))}
        </ul>
      )}

      <ModalConfirmacion
        abierto={!!aCancelar}
        mensaje="¿Estás seguro que querés cancelar este turno? Esta acción no se puede deshacer."
        textoCancelar="Volver"
        textoConfirmar="Sí, cancelar turno"
        onConfirmar={() => cancelar(aCancelar.id)}
        onCancelar={() => setACancelar(null)}
      />
    </div>
  )
}

export default Turnos
