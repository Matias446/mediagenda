import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5031/api',
})

export default api