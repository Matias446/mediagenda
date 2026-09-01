import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

function Home() {
  const { rol } = useAuth()

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center px-4">
      <div className="text-center">
        <h1 className="text-4xl md:text-5xl font-bold text-blue-600 mb-4">mediAgenda</h1>
        <p className="text-lg md:text-xl text-gray-600 mb-8">Sistema de gestión de turnos médicos</p>
        <div className="flex flex-col md:flex-row gap-4 justify-center">
          {rol === 'Admin' && (
            <Link to="/especialidades" className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 text-center">
              Ver Especialidades
            </Link>
          )}
          <Link to="/medicos" className="bg-white text-blue-600 border border-blue-600 px-6 py-3 rounded-lg hover:bg-blue-50 text-center">
            Ver Médicos
          </Link>
        </div>
      </div>
    </div>
  )
}

export default Home