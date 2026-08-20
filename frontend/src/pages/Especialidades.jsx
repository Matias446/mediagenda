import { useEffect, useState } from 'react'
import api from '../services/api'

function Especialidades() {
  const [especialidades, setEspecialidades] = useState([])
  const [loading, setLoading] = useState(true)
  const [nombre, setNombre] = useState('')

  const cargarEspecialidades = async () => {
    try {
      const res = await api.get('/Especialidad')
      setEspecialidades(res.data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const fetchData = async () => {
      await cargarEspecialidades()
    }
    fetchData()
  }, [])

  const crear = async () => {
    if (!nombre.trim()) return
    try {
      await api.post('/Especialidad', { nombre })
      setNombre('')
      await cargarEspecialidades()
    } catch (error) {
      console.error(error)
    }
  }

  const eliminar = async (id) => {
    try {
      await api.delete(`/Especialidad/${id}`)
      await cargarEspecialidades()
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <div className="max-w-2xl mx-auto mt-10 p-6">
      <h1 className="text-3xl font-bold text-blue-600 mb-6">Especialidades</h1>

      <div className="flex gap-2 mb-6">
        <input
          type="text"
          value={nombre}
          onChange={e => setNombre(e.target.value)}
          placeholder="Nueva especialidad"
          className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <button
          onClick={crear}
          className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700"
        >
          Agregar
        </button>
      </div>

      {loading ? (
        <p className="text-gray-500">Cargando...</p>
      ) : (
        <ul className="space-y-2">
          {especialidades.map(e => (
            <li key={e.id} className="flex justify-between items-center bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
              <span className="text-gray-800">{e.nombre}</span>
              <button
                onClick={() => eliminar(e.id)}
                className="text-red-500 hover:text-red-700 text-sm"
              >
                Eliminar
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default Especialidades