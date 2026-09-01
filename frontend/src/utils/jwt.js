const CLAIM_ROL = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'
const CLAIM_EMAIL = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'

export function decodeToken(token) {
  if (!token) return null
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')
    const padded = base64.padEnd(base64.length + (4 - (base64.length % 4)) % 4, '=')
    const payload = JSON.parse(atob(padded))
    return {
      email: payload[CLAIM_EMAIL],
      rol: payload[CLAIM_ROL],
      usuarioId: payload['usuarioId'] || null,
      pacienteId: payload['pacienteId'] || null,
    }
  } catch {
    return null
  }
}
