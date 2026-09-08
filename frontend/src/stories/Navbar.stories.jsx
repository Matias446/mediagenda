import { MemoryRouter } from 'react-router-dom'
import Navbar from '../components/Navbar.jsx'
import { AuthContext } from '../context/AuthContext.jsx'

function mockAuthValue({ rol, isAuthenticated }) {
  return {
    token: isAuthenticated ? 'fake-token' : null,
    login: () => {},
    logout: () => {},
    isAuthenticated,
    rol,
    pacienteId: null,
  }
}

export default {
  title: 'Componentes/Navbar',
  component: Navbar,
  decorators: [
    (Story, context) => (
      <MemoryRouter initialEntries={['/']}>
        <AuthContext.Provider value={mockAuthValue(context.args)}>
          <Story />
        </AuthContext.Provider>
      </MemoryRouter>
    ),
  ],
}

export const Admin = {
  args: {
    rol: 'Admin',
    isAuthenticated: true,
  },
}

export const Paciente = {
  args: {
    rol: 'Paciente',
    isAuthenticated: true,
  },
}

export const SinLogin = {
  args: {
    rol: null,
    isAuthenticated: false,
  },
}
