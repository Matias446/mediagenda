import { useEffect, useState } from 'react'
import api from '../services/api'

function Turnos() {
  const [turnos, setTurnos] = useState([])
  const [medicos, setMedicos] = useState([])
  const [pacientes, setPacientes] = useState([])
  const [slotsDisponibles, setSlotsDisponibles] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState({
    pacienteId: '', medicoId: '', fecha: '', slotSeleccionado: ''
  })

  const cargarDatos = async () => {
    try {
      const [turnosRes, medicosRes, pacientesRes] = await Promise.all([
        api.get('/Turno'),
        api.get('/Medico'),
        api.get('/Paciente')
      ])
      setTurnos(turnosRes.data)
      setMedicos(medicosRes.data)
      setPacientes(pacientesRes.data)
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
    if (!form.pacienteId || !form.medicoId || !form.slotSeleccionado) return
    try {
      await api.post('/Turno', {
        pacienteId: parseInt(form.pacienteId),
        medicoId: parseInt(form.medicoId),
        fechaHora: new Date(form.slotSeleccionado).toISOString()
      })
      setForm({ pacienteId: '', medicoId: '', fecha: '', slotSeleccionado: '' })
      setSlotsDisponibles([])
      await cargarDatos()
    } catch (error) {
      console.error(error)
    }
  }

  const cancelar = async (id) => {
    try {
      await api.put(`/Turno/${id}/cancelar`)
      await cargarDatos()
    } catch (error) {
      console.error(error)
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

  return (
    <div className="max-w-3xl mx-auto mt-10 p-6">
      <h1 className="text-3xl font-bold text-blue-600 mb-6">Turnos</h1>

      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
        <select value={form.pacienteId}
          onChange={e => setForm({ ...form, pacienteId: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">Seleccioná un paciente</option>
          {pacientes.map(p => <option key={p.id} value={p.id}>{p.nombre} {p.apellido}</option>)}
        </select>

        <select value={form.medicoId} onChange={handleMedicoChange}
          className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">Seleccioná un médico</option>
          {medicos.map(m => <option key={m.id} value={m.id}>{m.nombre} {m.apellido}</option>)}
        </select>

        <input type="date" value={form.fecha} onChange={handleFechaChange}
          min={new Date().toISOString().split('T')[0]}
          className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />

        {slotsDisponibles.length > 0 && (
          <div>
            <p className="text-sm text-gray-500 mb-2">Horarios disponibles:</p>
            <div className="grid grid-cols-4 gap-2">
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
          <p className="text-sm text-red-500">No hay horarios disponibles para este día.</p>
        )}

        <button onClick={crear}
          disabled={!form.pacienteId || !form.medicoId || !form.slotSeleccionado}
          className="w-full bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed">
          Reservar Turno
        </button>
      </div>

      {loading ? (
        <p className="text-gray-500">Cargando...</p>
      ) : (
        <ul className="space-y-2">
          {turnos.map(t => (
            <li key={t.id} className="flex justify-between items-center bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <div>
                <p className="font-medium text-gray-800">
                  Paciente #{t.pacienteId} · Médico #{t.medicoId}
                </p>
                <p className="text-sm text-gray-500">
                  {new Date(t.fechaHora).toLocaleString('es-UY')}
                </p>
                <p className={`text-sm font-medium ${estadoColor(t.estado)}`}>{t.estado}</p>
              </div>
              {t.estado === 'Pendiente' && (
                <button onClick={() => cancelar(t.id)} className="text-red-500 hover:text-red-700 text-sm">
                  Cancelar
                </button>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default Turnos