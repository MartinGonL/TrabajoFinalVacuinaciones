# 💉 Sistema de Vacunación Escolar

Aplicación Web de Gestión de Vacunación Escolar desarrollada en **ASP.NET Core 9.0 MVC**, utilizando el **Patrón Repositorio** clásico con **ADO.NET** (sin EF Core), base de datos **MySQL**, frontend reactivo en **Vue.js** y APIs REST securizadas con **JWT**.

---

## 📋 Requerimientos y Funcionalidades

- **Tablas Relacionadas (1:N)**: `Escuelas`, `Alumnos`, `Vacunas`, `RegistrosVacunacion`, `Usuarios`, `FotosEscuela`.
- **Seguridad y Roles**: Autenticación por cookies y roles (`Administrador` y `Agente`). Gestión y carga de Avatares por usuario.
- **Manejo de Archivos**: Galería de fotos por escuela almacenadas en `wwwroot/uploads/escuelas/`.
- **ABM con Vue.js**: CRUD interactivo de Alumnos implementado con Vue.js (CDN) de forma 100% asíncrona vía AJAX.
- **Paginación Real**: Paginación en servidor con SQL `LIMIT` y `OFFSET` en Registros de Vacunación y Alumnos.
- **Búsqueda AJAX**: Buscador predictivo en tiempo real para selección de alumnos al registrar vacunaciones.
- **APIs REST con JWT**: Endpoints organizados en la carpeta `Api/` securizados mediante tokens JWT Bearer.

---

## 👤 Usuarios de Prueba

| Rol | Email | Contraseña |
| :--- | :--- | :--- |
| **Administrador** | `admin@vacunas.com` | `admin` |
| **Agente** | `agente@vacunas.com` | `agente` |

---

## ⚙️ Ejecución Local

1. Importar la base de datos `vacunacionescolar.sql` en MySQL / MariaDB (XAMPP).
2. Verificar la cadena de conexión en `appsettings.json`.
3. Ejecutar `dotnet run` en la terminal (escucha en `http://localhost:5006`).
