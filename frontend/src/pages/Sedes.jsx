import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { MapPin } from 'lucide-react'
import api from '../services/api'
import ModalConfirmacion from '../components/ModalConfirmacion'
import Spinner from '../components/Spinner'
import EmptyState from '../components/EmptyState'

function Sedes() {
  const [sedes, setSedes] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState({ nombre: '', direccion: '', telefono: '' })
  const [aEliminar, setAEliminar] = useState(null)

  const cargarSedes = async () => {
    try {
      const res = await api.get('/Sede')
      setSedes(res.data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const fetchData = async () => { await cargarSedes() }
    fetchData()
  }, [])

  const crear = async () => {
    try {
      await api.post('/Sede', form)
      setForm({ nombre: '', direccion: '', telefono: '' })
      await cargarSedes()
      toast.success('Sede creada correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo crear la sede')
    }
  }

  const eliminar = async (id) => {
    try {
      await api.delete(`/Sede/${id}`)
      await cargarSedes()
      toast.success('Sede eliminada correctamente')
    } catch (error) {
      toast.error(error.response?.data?.mensaje || 'No se pudo eliminar la sede')
    } finally {
      setAEliminar(null)
    }
  }

  return (
    <div className="max-w-2xl mx-auto mt-6 px-4 pb-10">
      <h1 className="text-2xl md:text-3xl font-bold text-blue-600 mb-6">Sedes</h1>

      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
        <input type="text" placeholder="Nombre" value={form.nombre}
          onChange={e => setForm({ ...form, nombre: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        <input type="text" placeholder="Dirección" value={form.direccion}
          onChange={e => setForm({ ...form, direccion: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        <input type="text" placeholder="Teléfono" value={form.telefono}
          onChange={e => setForm({ ...form, telefono: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        <button onClick={crear}
          className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium">
          Agregar Sede
        </button>
      </div>

      {loading ? (
        <Spinner />
      ) : sedes.length === 0 ? (
        <EmptyState icono={MapPin} mensaje="No hay sedes registradas." />
      ) : (
        <ul className="space-y-2">
          {sedes.map(s => (
            <li key={s.id} className="flex justify-between items-center bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <div>
                <p className="font-medium text-gray-800">{s.nombre}</p>
                <p className="text-sm text-gray-500">{s.direccion} · {s.telefono}</p>
              </div>
              <button onClick={() => setAEliminar(s)}
                className="text-red-500 hover:text-red-700 text-sm font-medium ml-4">
                Eliminar
              </button>
            </li>
          ))}
        </ul>
      )}

      <ModalConfirmacion
        abierto={!!aEliminar}
        mensaje={`¿Estás seguro que querés eliminar ${aEliminar?.nombre}? Esta acción no se puede deshacer.`}
        onConfirmar={() => eliminar(aEliminar.id)}
        onCancelar={() => setAEliminar(null)}
      />
    </div>
  )
}

export default Sedes