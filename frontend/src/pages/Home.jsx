import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../services/api'
import Spinner from '../components/Spinner'

function esHoy(fechaHoraIso) {
  return new Date(fechaHoraIso).toDateString() === new Date().toDateString()
}

function estadoColor(estado) {
  switch (estado) {
    case 'Pendiente': return 'text-yellow-600'
    case 'Confirmado': return 'text-green-600'
    case 'Cancelado': return 'text-red-500'
    case 'Completado': return 'text-blue-600'
    default: return 'text-gray-600'
  }
}

function TarjetaStat({ valor, etiqueta }) {
  return (
    <div className="bg-white border border-gray-200 rounded-lg p-6 shadow-sm text-center">
      <p className="text-3xl font-bold text-blue-600">{valor}</p>
      <p className="text-sm text-gray-500 mt-1">{etiqueta}</p>
    </div>
  )
}

function DashboardAdmin() {
  const [stats, setStats] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const cargar = async () => {
      try {
        const [medicos, especialidades, sedes, pacientes, turnos] = await Promise.all([
          api.get('/Medico'),
          api.get('/Especialidad'),
          api.get('/Sede'),
          api.get('/Paciente'),
          api.get('/Turno'),
        ])
        setStats({
          medicos: medicos.data.length,
          especialidades: especialidades.data.length,
          sedes: sedes.data.length,
          pacientes: pacientes.data.length,
          turnosHoy: turnos.data.filter(t => esHoy(t.fechaHora)).length,
        })
      } catch (error) {
        console.error(error)
      } finally {
        setLoading(false)
      }
    }
    cargar()
  }, [])

  return (
    <div className="max-w-5xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Panel de administración</h1>
      {loading ? (
        <Spinner />
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4">
          <TarjetaStat valor={stats.medicos} etiqueta="Médicos" />
          <TarjetaStat valor={stats.especialidades} etiqueta="Especialidades" />
          <TarjetaStat valor={stats.sedes} etiqueta="Sedes" />
          <TarjetaStat valor={stats.pacientes} etiqueta="Pacientes" />
          <TarjetaStat valor={stats.turnosHoy} etiqueta="Turnos hoy" />
        </div>
      )}
    </div>
  )
}

function DashboardAdministrativo() {
  const [turnosHoy, setTurnosHoy] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const cargar = async () => {
      try {
        const res = await api.get('/Turno')
        setTurnosHoy(res.data.filter(t => esHoy(t.fechaHora)))
      } catch (error) {
        console.error(error)
      } finally {
        setLoading(false)
      }
    }
    cargar()
  }, [])

  return (
    <div className="max-w-2xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Turnos de hoy</h1>
      {loading ? (
        <Spinner />
      ) : turnosHoy.length === 0 ? (
        <p className="text-gray-500">No hay turnos programados para hoy.</p>
      ) : (
        <ul className="space-y-2">
          {turnosHoy.map(t => (
            <li key={t.id} className="bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <p className="font-medium text-gray-800">{t.nombrePaciente} · {t.nombreMedico}</p>
              <p className="text-sm text-gray-500 mt-1">
                {new Date(t.fechaHora).toLocaleTimeString('es-UY', { hour: '2-digit', minute: '2-digit' })}
              </p>
              <p className={`text-sm font-medium mt-1 ${estadoColor(t.estado)}`}>{t.estado}</p>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

function DashboardPaciente({ pacienteId }) {
  const [turnos, setTurnos] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const cargar = async () => {
      try {
        const res = await api.get(`/Turno/paciente/${pacienteId}`)
        const proximos = res.data
          .filter(t => t.estado === 'Pendiente' || t.estado === 'Confirmado')
          .sort((a, b) => new Date(a.fechaHora) - new Date(b.fechaHora))
        setTurnos(proximos)
      } catch (error) {
        console.error(error)
      } finally {
        setLoading(false)
      }
    }
    cargar()
  }, [pacienteId])

  return (
    <div className="max-w-2xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Mis próximos turnos</h1>
      {loading ? (
        <Spinner />
      ) : turnos.length === 0 ? (
        <p className="text-gray-500">No tenés turnos próximos.</p>
      ) : (
        <ul className="space-y-2">
          {turnos.map(t => (
            <li key={t.id} className="bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <p className="font-medium text-gray-800">{t.nombreMedico}</p>
              <p className="text-sm text-gray-500 mt-1">{new Date(t.fechaHora).toLocaleString('es-UY')}</p>
              <p className={`text-sm font-medium mt-1 ${estadoColor(t.estado)}`}>{t.estado}</p>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

function Bienvenida() {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center px-4">
      <div className="text-center">
        <h1 className="text-4xl md:text-5xl font-bold text-blue-600 mb-4">mediAgenda</h1>
        <p className="text-lg md:text-xl text-gray-600 mb-8">Sistema de gestión de turnos médicos</p>
        <Link to="/medicos" className="bg-white text-blue-600 border border-blue-600 px-6 py-3 rounded-lg hover:bg-blue-50 text-center inline-block">
          Ver Médicos
        </Link>
      </div>
    </div>
  )
}

function Home() {
  const { rol, pacienteId } = useAuth()

  if (rol === 'Admin') return <DashboardAdmin />
  if (rol === 'Administrativo') return <DashboardAdministrativo />
  if (rol === 'Paciente') return <DashboardPaciente pacienteId={pacienteId} />
  return <Bienvenida />
}

export default Home
