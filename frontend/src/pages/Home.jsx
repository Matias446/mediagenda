import { Link } from 'react-router-dom'

function Home() {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center">
      <div className="text-center">
        <h1 className="text-5xl font-bold text-blue-600 mb-4">mediAgenda</h1>
        <p className="text-xl text-gray-600 mb-8">Sistema de gestión de turnos médicos</p>
        <div className="flex gap-4 justify-center">
          <Link to="/especialidades" className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700">
            Ver Especialidades
          </Link>
          <Link to="/medicos" className="bg-white text-blue-600 border border-blue-600 px-6 py-3 rounded-lg hover:bg-blue-50">
            Ver Médicos
          </Link>
        </div>
      </div>
    </div>
  )
}

export default Home