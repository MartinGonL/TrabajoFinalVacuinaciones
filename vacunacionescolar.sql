-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 06-11-2025 a las 00:00:44
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `vacunacionescolar`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `alumnos`
--

CREATE TABLE `alumnos` (
  `AlumnoID` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `DNI` varchar(20) NOT NULL,
  `FechaNacimiento` date NOT NULL,
  `TelefonoTutor` varchar(50) DEFAULT NULL,
  `EscuelaID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `escuelas`
--

CREATE TABLE `escuelas` (
  `EscuelaID` int(11) NOT NULL,
  `Nombre` varchar(150) NOT NULL,
  `Numero` int(11) DEFAULT NULL,
  `Direccion` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `fotosescuela`
--

CREATE TABLE `fotosescuela` (
  `FotoID` int(11) NOT NULL,
  `FotoURL` varchar(500) NOT NULL,
  `Descripcion` varchar(255) DEFAULT NULL,
  `EscuelaID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `registrosvacunacion`
--

CREATE TABLE `registrosvacunacion` (
  `RegistroID` int(11) NOT NULL,
  `FechaAplicacion` datetime NOT NULL,
  `NumeroDosis` int(11) NOT NULL DEFAULT 1,
  `Observaciones` text DEFAULT NULL,
  `AlumnoID` int(11) NOT NULL,
  `AgenteID` int(11) NOT NULL,
  `VacunaID` int(11) NOT NULL,
  `LugarVacunacion_EscuelaID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `UsuarioID` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `DNI` varchar(20) NOT NULL,
  `Matricula` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Rol` varchar(50) NOT NULL,
  `AvatarURL` varchar(500) DEFAULT NULL,
  `Telefono` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `vacunas`
--

CREATE TABLE `vacunas` (
  `VacunaID` int(11) NOT NULL,
  `NombreVacuna` varchar(100) NOT NULL,
  `Descripcion` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `alumnos`
--
ALTER TABLE `alumnos`
  ADD PRIMARY KEY (`AlumnoID`),
  ADD UNIQUE KEY `DNI` (`DNI`),
  ADD KEY `EscuelaID` (`EscuelaID`);

--
-- Indices de la tabla `escuelas`
--
ALTER TABLE `escuelas`
  ADD PRIMARY KEY (`EscuelaID`),
  ADD UNIQUE KEY `Numero` (`Numero`);

--
-- Indices de la tabla `fotosescuela`
--
ALTER TABLE `fotosescuela`
  ADD PRIMARY KEY (`FotoID`),
  ADD KEY `EscuelaID` (`EscuelaID`);

--
-- Indices de la tabla `registrosvacunacion`
--
ALTER TABLE `registrosvacunacion`
  ADD PRIMARY KEY (`RegistroID`),
  ADD KEY `AlumnoID` (`AlumnoID`),
  ADD KEY `AgenteID` (`AgenteID`),
  ADD KEY `VacunaID` (`VacunaID`),
  ADD KEY `LugarVacunacion_EscuelaID` (`LugarVacunacion_EscuelaID`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`UsuarioID`),
  ADD UNIQUE KEY `DNI` (`DNI`),
  ADD UNIQUE KEY `Matricula` (`Matricula`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `vacunas`
--
ALTER TABLE `vacunas`
  ADD PRIMARY KEY (`VacunaID`),
  ADD UNIQUE KEY `NombreVacuna` (`NombreVacuna`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `alumnos`
--
ALTER TABLE `alumnos`
  MODIFY `AlumnoID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `escuelas`
--
ALTER TABLE `escuelas`
  MODIFY `EscuelaID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `fotosescuela`
--
ALTER TABLE `fotosescuela`
  MODIFY `FotoID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `registrosvacunacion`
--
ALTER TABLE `registrosvacunacion`
  MODIFY `RegistroID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `UsuarioID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `vacunas`
--
ALTER TABLE `vacunas`
  MODIFY `VacunaID` int(11) NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `alumnos`
--
ALTER TABLE `alumnos`
  ADD CONSTRAINT `alumnos_ibfk_1` FOREIGN KEY (`EscuelaID`) REFERENCES `escuelas` (`EscuelaID`);

--
-- Filtros para la tabla `fotosescuela`
--
ALTER TABLE `fotosescuela`
  ADD CONSTRAINT `fotosescuela_ibfk_1` FOREIGN KEY (`EscuelaID`) REFERENCES `escuelas` (`EscuelaID`) ON DELETE CASCADE;

--
-- Filtros para la tabla `registrosvacunacion`
--
ALTER TABLE `registrosvacunacion`
  ADD CONSTRAINT `registrosvacunacion_ibfk_1` FOREIGN KEY (`AlumnoID`) REFERENCES `alumnos` (`AlumnoID`),
  ADD CONSTRAINT `registrosvacunacion_ibfk_2` FOREIGN KEY (`AgenteID`) REFERENCES `usuarios` (`UsuarioID`),
  ADD CONSTRAINT `registrosvacunacion_ibfk_3` FOREIGN KEY (`VacunaID`) REFERENCES `vacunas` (`VacunaID`),
  ADD CONSTRAINT `registrosvacunacion_ibfk_4` FOREIGN KEY (`LugarVacunacion_EscuelaID`) REFERENCES `escuelas` (`EscuelaID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
