import Paginacion from '../components/Paginacion.jsx'

export default {
  title: 'Componentes/Paginacion',
  component: Paginacion,
}

export const PrimeraPagina = {
  args: {
    paginaActual: 1,
    totalPaginas: 5,
    totalResultados: 50,
    porPagina: 10,
    onCambiarPagina: () => {},
  },
}

export const UltimaPagina = {
  args: {
    paginaActual: 5,
    totalPaginas: 5,
    totalResultados: 50,
    porPagina: 10,
    onCambiarPagina: () => {},
  },
}

export const UnaSolaPagina = {
  args: {
    paginaActual: 1,
    totalPaginas: 1,
    totalResultados: 5,
    porPagina: 10,
    onCambiarPagina: () => {},
  },
}
