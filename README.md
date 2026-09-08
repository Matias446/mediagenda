# mediAgenda 🏥

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![React 19](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=black)
![Vite](https://img.shields.io/badge/Vite-8-646CFF?logo=vite&logoColor=white)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-4-06B6D4?logo=tailwindcss&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Supabase-3ECF8E?logo=supabase&logoColor=white)
![Deploy Frontend](https://img.shields.io/badge/Vercel-Frontend-black?logo=vercel&logoColor=white)
![Deploy Backend](https://img.shields.io/badge/Railway-Backend-0B0D0E?logo=railway&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green)

Sistema de gestión de turnos médicos para clínicas con múltiples médicos y especialidades. Permite a **pacientes** reservar turnos online, a **administrativos** gestionar agendas de médicos y a **administradores** gestionar todo el sistema, desde una única plataforma web instalable como PWA.

---

## 🌐 Demo

🔗 **[mediagenda-sand.vercel.app](https://mediagenda-sand.vercel.app)**

📄 API docs (Swagger): [mediagenda-production.up.railway.app/swagger](https://mediagenda-production.up.railway.app/swagger)

---

## ✨ Features

### 👤 Paciente
- Registro público por cédula (valida que no exista como médico)
- Reserva de turnos online con selección de especialidad, médico y sede
- Slots disponibles generados automáticamente según la duración de turno de cada médico
- Selector de médico agrupado por especialidad
- Visualización y cancelación de turnos propios
- Edición de perfil y cambio de contraseña
- Notificaciones por email al reservar y cancelar turnos
- Dashboard personalizado con sus próximos turnos

### 🗂️ Administrativo
- Gestión de médicos, pacientes y turnos
- Confirmación de turnos pendientes
- Buscador y filtros (por nombre, cédula, especialidad, sede)
- Dashboard con los turnos del día

### 🛠️ Admin
- Gestión completa del sistema: especialidades, sedes, médicos, pacientes, turnos y usuarios
- Alta de usuarios Admin y Administrativo
- Dashboard con estadísticas generales del sistema
- Validaciones de integridad (no permite borrar especialidades/sedes con médicos asociados)

### ⚙️ Generales
- Autenticación JWT con roles (Admin / Administrativo / Paciente)
- PWA instalable, con caché offline
- Diseño responsive con menú hamburguesa en mobile
- Paginación y buscadores con `useMemo` en listados extensos
- Modal de confirmación antes de eliminar registros
- Feedback visual con toasts (éxito/error) y estados de carga (spinners, empty states)
- Rate limiting en login (máx. 5 intentos cada 15 minutos)
- Sanitización de inputs contra XSS
- Monitoreo de errores en producción con Sentry (frontend y backend)
- Accesibilidad y SEO optimizados (Lighthouse: Performance 100, Accessibility 100, Best Practices 100)
- Documentación de componentes de UI con Storybook

---

## 🛠️ Stack técnico

**Frontend**
- React 19 + Vite
- TailwindCSS 4
- react-router-dom, react-hot-toast, lucide-react, react-helmet-async
- Storybook (documentación de componentes)
- vite-plugin-pwa

**Backend**
- .NET 8 / ASP.NET Core
- Entity Framework Core + Npgsql
- BCrypt.Net (hash de contraseñas)
- MailKit (envío de emails)
- AspNetCoreRateLimit
- HtmlSanitizer
- Sentry.AspNetCore
- Swashbuckle (Swagger)

**Base de datos**
- PostgreSQL (Supabase, vía connection pooler)

**Deploy**
- Vercel (frontend) + Railway (backend), auto-deploy sobre `master`

**Seguridad**
- JWT (claims: email, rol, usuarioId, pacienteId)
- BCrypt, Rate limiting, HtmlSanitizer, CORS configurado por entorno

---

## 🏗️ Arquitectura

Backend organizado en capas, con inyección de dependencias e interfaces para desacoplar cada capa:

```
mediAgenda.Dominio          → Entidades del negocio (Medico, Paciente, Turno, etc.)
mediAgenda.IDataAccess      → Interfaces de acceso a datos
mediAgenda.DataAccess       → Repositorio genérico IRepositorio<T> + EF Core + Migrations
mediAgenda.ILogicaNegocio   → Interfaces de servicios de negocio
mediAgenda.LogicaNegocio    → Reglas de negocio (validaciones, JWT, email, hashing)
mediAgenda.WebAPI           → Controllers, DTOs, Middleware, punto de entrada
```

- Repositorio genérico `IRepositorio<T>` reutilizado por todas las entidades
- DTOs con Data Annotations para validación de entrada
- `ExceptionMiddleware` centralizado: diferencia errores de negocio (mensaje visible) de errores de sistema (mensaje genérico)
- Los médicos **no** tienen usuario propio en el sistema; son gestionados por Admin/Administrativo

---

## 🚀 Correr localmente

### Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- Cuenta en [Supabase](https://supabase.com/) (para la base de datos PostgreSQL)

### Backend

```bash
cd mediAgenda.WebAPI
# Crear appsettings.json con tu connection string de Supabase y demás config (ver variables de entorno abajo)
dotnet restore
dotnet ef database update --project ../mediAgenda.DataAccess --startup-project .
dotnet run
```

### Frontend

```bash
cd frontend
npm install
# Crear .env.local con VITE_API_URL=http://localhost:5031/api
npm run dev
```

### Storybook

```bash
cd frontend
npm run storybook
```

---

## 📁 Estructura del proyecto

```
mediAgenda/
├── mediAgenda.Dominio/          # Entidades del dominio
├── mediAgenda.IDataAccess/      # Interfaces de acceso a datos
├── mediAgenda.DataAccess/       # Repositorio genérico + EF Core + Migrations
├── mediAgenda.ILogicaNegocio/   # Interfaces de lógica de negocio
├── mediAgenda.LogicaNegocio/    # Servicios de negocio (Auth, Turno, Medico, Paciente, Especialidad, Sede, Email)
├── mediAgenda.WebAPI/           # API REST (Controllers, DTOs, Middleware)
├── frontend/                    # SPA en React
│   ├── src/
│   │   ├── components/          # Componentes reutilizables (Navbar, Paginacion, ModalConfirmacion, etc.)
│   │   ├── pages/                # Páginas de la app (Home, Turnos, Medicos, Pacientes, etc.)
│   │   ├── context/              # Contextos de React (auth)
│   │   ├── services/             # Clientes HTTP hacia la API
│   │   └── stories/              # Historias de Storybook
│   └── public/
├── Dockerfile
└── mediAgenda.sln
```

---

## 🔐 Variables de entorno necesarias

**Backend (Railway)**
```
ConnectionStrings__DefaultConnection
JwtSettings__SecretKey
JwtSettings__Issuer
JwtSettings__Audience
Sentry__Dsn
Email__Username
Email__Password
Email__SmtpHost
Email__SmtpPort
Email__From
```

**Frontend (Vercel)**
```
VITE_API_URL
VITE_SENTRY_DSN
```

> ⚠️ `JwtSettings__SecretKey` debe tener al menos 32 caracteres (generado por ejemplo con `openssl rand -base64 48`). Si se rota, todos los tokens emitidos quedan inválidos.

---

## 📄 Licencia

MIT
