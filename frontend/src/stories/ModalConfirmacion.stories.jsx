import ModalConfirmacion from '../components/ModalConfirmacion.jsx'

export default {
  title: 'Componentes/ModalConfirmacion',
  component: ModalConfirmacion,
}

export const Default = {
  args: {
    abierto: true,
    mensaje: '¿Estás seguro de que querés eliminar Especialidad?',
    onConfirmar: () => {},
    onCancelar: () => {},
  },
}

export const Turno = {
  args: {
    abierto: true,
    mensaje: '¿Estás seguro de que querés eliminar turno del Dr. García?',
    onConfirmar: () => {},
    onCancelar: () => {},
  },
}
