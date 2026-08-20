import { useEffect, useState } from 'react'
import api from '../services/api'

function Sedes() {
  const [sedes, setSedes] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState({ nombre: '', direccion: '', telefono: '' })

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
    } catch (error) {
      console.error(error)
    }
  }

  const eliminar = async (id) => {
    try {
      await api.delete(`/Sede/${id}`)
      await cargarSedes()
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <div className="max-w-2xl mx-auto mt-10 p-6">
      <h1 className="text-3xl font-bold text-blue-600 mb-6">Sedes</h1>

      <div className="bg-white border border-gray-200 rounded-lg p-4 mb-6 shadow-sm space-y-3">
        <input type="text" placeholder="Nombre" value={form.nombre}
          onChange={e => setForm({ ...form, nombre: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        <input type="text" placeholder="Dirección" value={form.direccion}
          onChange={e => setForm({ ...form, direccion: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        <input type="text" placeholder="Teléfono" value={form.telefono}
          onChange={e => setForm({ ...form, telefono: e.target.value })}
          className="w-full border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
        <button onClick={crear}
          className="w-full bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700">
          Agregar Sede
        </button>
      </div>

      {loading ? (
        <p className="text-gray-500">Cargando...</p>
      ) : (
        <ul className="space-y-2">
          {sedes.map(s => (
            <li key={s.id} className="flex justify-between items-center bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <div>
                <p className="font-medium text-gray-800">{s.nombre}</p>
                <p className="text-sm text-gray-500">{s.direccion} · {s.telefono}</p>
              </div>
              <button onClick={() => eliminar(s.id)} className="text-red-500 hover:text-red-700 text-sm">
                Eliminar
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default Sedes