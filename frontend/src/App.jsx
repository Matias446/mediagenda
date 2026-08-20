import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import Especialidades from './pages/Especialidades'
import Medicos from './pages/Medicos'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/especialidades" element={<Especialidades />} />
        <Route path="/medicos" element={<Medicos />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App