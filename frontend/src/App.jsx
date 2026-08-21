import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext.jsx'
import Navbar from './components/Navbar'
import RutaProtegida from './components/RutaProtegida'
import Home from './pages/Home'
import Especialidades from './pages/Especialidades'
import Medicos from './pages/Medicos'
import Sedes from './pages/Sedes'
import Pacientes from './pages/Pacientes'
import Turnos from './pages/Turnos'
import Login from './pages/Login'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Navbar />
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<RutaProtegida><Home /></RutaProtegida>} />
          <Route path="/especialidades" element={<RutaProtegida><Especialidades /></RutaProtegida>} />
          <Route path="/medicos" element={<RutaProtegida><Medicos /></RutaProtegida>} />
          <Route path="/sedes" element={<RutaProtegida><Sedes /></RutaProtegida>} />
          <Route path="/pacientes" element={<RutaProtegida><Pacientes /></RutaProtegida>} />
          <Route path="/turnos" element={<RutaProtegida><Turnos /></RutaProtegida>} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App