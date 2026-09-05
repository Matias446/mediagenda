import { ChevronLeft, ChevronRight } from 'lucide-react'

function Paginacion({ paginaActual, totalPaginas, totalResultados, porPagina, onCambiarPagina }) {
  if (totalPaginas <= 1) return null

  const inicio = (paginaActual - 1) * porPagina + 1
  const fin = Math.min(paginaActual * porPagina, totalResultados)

  return (
    <div className="flex items-center justify-between mt-4 flex-wrap gap-3">
      <p className="text-sm text-gray-500">
        Mostrando {inicio}-{fin} de {totalResultados} resultados
      </p>
      <div className="flex items-center gap-3">
        <button
          onClick={() => onCambiarPagina(paginaActual - 1)}
          disabled={paginaActual === 1}
          className="flex items-center gap-1 px-3 py-2 rounded-lg border border-gray-300 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
          <ChevronLeft size={16} />
          Anterior
        </button>
        <span className="text-sm text-gray-600">
          Página {paginaActual} de {totalPaginas}
        </span>
        <button
          onClick={() => onCambiarPagina(paginaActual + 1)}
          disabled={paginaActual === totalPaginas}
          className="flex items-center gap-1 px-3 py-2 rounded-lg border border-gray-300 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
          Siguiente
          <ChevronRight size={16} />
        </button>
      </div>
    </div>
  )
}

export default Paginacion
