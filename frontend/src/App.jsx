import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Navbar from './components/Navbar'
import Home from './pages/Home'
import Especialidades from './pages/Especialidades'
import Medicos from './pages/Medicos'
import Sedes from './pages/Sedes'
import Pacientes from './pages/Pacientes'
import Turnos from './pages/Turnos'

function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/especialidades" element={<Especialidades />} />
        <Route path="/medicos" element={<Medicos />} />
        <Route path="/sedes" element={<Sedes />} />
        <Route path="/pacientes" element={<Pacientes />} />
        <Route path="/turnos" element={<Turnos />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App