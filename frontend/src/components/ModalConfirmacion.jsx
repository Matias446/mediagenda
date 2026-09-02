function ModalConfirmacion({ abierto, mensaje, textoCancelar = 'Cancelar', textoConfirmar = 'Eliminar', onConfirmar, onCancelar }) {
  if (!abierto) return null

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 px-4">
      <div className="bg-white rounded-lg p-6 shadow-lg w-full max-w-sm">
        <p className="text-gray-800 mb-6">{mensaje}</p>
        <div className="flex gap-3">
          <button onClick={onCancelar}
            className="flex-1 bg-gray-100 text-gray-700 px-4 py-2 rounded-lg font-medium hover:bg-gray-200">
            {textoCancelar}
          </button>
          <button onClick={onConfirmar}
            className="flex-1 bg-red-600 text-white px-4 py-2 rounded-lg font-medium hover:bg-red-700">
            {textoConfirmar}
          </button>
        </div>
      </div>
    </div>
  )
}

export default ModalConfirmacion
