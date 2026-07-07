# FoodSave

Aplicación web para gestionar el rescate y aprovechamiento de excedentes alimenticios, conectando **establecimientos** (restaurantes, cafeterías, panaderías) con **consumidores** interesados en adquirir productos aún aptos para consumo.

Este proyecto implementa el **núcleo mínimo funcional** del sistema descrito en el documento de Proyecto Integrador II: publicación de excedentes, consulta/búsqueda de ofertas, reservas y confirmación de retiro.

## Stack tecnológico

- **Backend**: ASP.NET Core Web API (.NET 10) + Entity Framework Core + SQL Server LocalDB
- **Autenticación**: JWT (JSON Web Tokens)
- **Frontend**: React 19 + Vite + React Router + Axios

## Estructura del proyecto

```
FoodSave/
├── FoodSave.Api/        # Backend (API REST)
│   ├── Models/          # Entidades: Rol, Usuario, Establecimiento, OfertaAlimento, Reserva, ConfirmacionRetiro
│   ├── Data/            # DbContext y migraciones de EF Core
│   ├── DTOs/            # Contratos de entrada/salida de la API
│   ├── Services/        # TokenService (generación de JWT)
│   ├── Controllers/     # AuthController, OfertasController, ReservasController, EstablecimientosController
│   └── Program.cs       # Configuración de EF Core, JWT y CORS
├── frontend/            # Frontend (React + Vite)
│   └── src/
│       ├── api/         # Cliente Axios con interceptor de autenticación
│       ├── context/     # AuthContext (sesión del usuario)
│       ├── components/  # NavBar, RutaProtegida
│       └── pages/       # Login, Registro, Ofertas, OfertaDetalle, MisReservas, PanelEstablecimiento
└── FoodSave.sln
```

## Módulos implementados

- ✅ Registro y autenticación de **Establecimientos** y **Consumidores** (JWT)
- ✅ Publicación y edición de excedentes alimenticios (título, categoría, cantidad, precio, tiempo límite)
- ✅ Consulta y búsqueda de ofertas con filtros (categoría, ubicación, precio máximo, tiempo restante)
- ✅ Flujo de reserva → confirmación de retiro por ambas partes (consumidor y establecimiento) → cierre de la oferta
- ✅ Historial de reservas por consumidor y por establecimiento

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (o superior)
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (incluido con Visual Studio en Windows)
- [Node.js](https://nodejs.org/) 18 o superior

## Cómo ejecutar el backend

```bash
cd FoodSave.Api
dotnet restore
dotnet ef database update   # crea la base de datos FoodSaveDb en LocalDB
dotnet run
```

La API quedará disponible en `http://localhost:5080` (ver `Properties/launchSettings.json` para el puerto exacto).

> Si no tienes la herramienta `dotnet-ef` instalada: `dotnet tool install -g dotnet-ef`

## Cómo ejecutar el frontend

```bash
cd frontend
npm install
npm run dev
```

La aplicación quedará disponible en `http://localhost:5173`.

## Flujo de uso

1. **Registrarse** como *Establecimiento* (con nombre comercial, dirección, horario) o como *Consumidor*.
2. Como establecimiento, entrar a **Mi panel** y publicar una oferta de excedente alimenticio.
3. Como consumidor, navegar **Ofertas**, filtrar por categoría/ubicación/precio, y **reservar** una oferta disponible.
4. En **Mis reservas**, el consumidor confirma el retiro.
5. En **Mi panel → Reservas recibidas**, el establecimiento confirma el retiro desde su lado.
6. Cuando ambas partes confirman, la reserva pasa a **Confirmada** y la oferta se cierra automáticamente.

## Notas

- La configuración de conexión a base de datos y la clave JWT están en `FoodSave.Api/appsettings.json`. Cambia la clave JWT (`Jwt:Key`) antes de usar este proyecto en un entorno real.
- Este es el núcleo mínimo funcional; módulos como notificaciones, incidencias, reportes y el rol de Administrador (presentes en el modelo conceptual original) no están implementados en esta versión.
