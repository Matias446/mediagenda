function EmptyState({ icono: Icono, mensaje }) {
  return (
    <div className="flex flex-col items-center justify-center text-center py-12 text-gray-400">
      <Icono size={40} strokeWidth={1.5} className="mb-3" />
      <p className="text-gray-500">{mensaje}</p>
    </div>
  )
}

export default EmptyState
