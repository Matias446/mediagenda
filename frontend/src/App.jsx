import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Toaster } from 'react-hot-toast'
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
import Registro from './pages/Registro'
import MiPerfil from './pages/MiPerfil'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Toaster
          toastOptions={{
            duration: 3000,
            success: { style: { background: '#16a34a', color: '#fff' } },
            error: { style: { background: '#dc2626', color: '#fff' } },
          }}
        />
        <Navbar />
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/registro" element={<Registro />} />
          <Route path="/" element={<RutaProtegida><Home /></RutaProtegida>} />
          <Route path="/especialidades" element={<RutaProtegida roles={['Admin']}><Especialidades /></RutaProtegida>} />
          <Route path="/medicos" element={<RutaProtegida roles={['Admin', 'Administrativo', 'Paciente']}><Medicos /></RutaProtegida>} />
          <Route path="/sedes" element={<RutaProtegida roles={['Admin']}><Sedes /></RutaProtegida>} />
          <Route path="/pacientes" element={<RutaProtegida roles={['Admin', 'Administrativo']}><Pacientes /></RutaProtegida>} />
          <Route path="/turnos" element={<RutaProtegida roles={['Admin', 'Administrativo', 'Paciente']}><Turnos /></RutaProtegida>} />
          <Route path="/mi-perfil" element={<RutaProtegida roles={['Paciente']}><MiPerfil /></RutaProtegida>} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App