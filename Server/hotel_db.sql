-- phpMyAdmin SQL Dump
-- version 4.0.3deb1
-- http://www.phpmyadmin.net
--
-- Servidor: localhost
-- Tiempo de generación: 16-07-2013 a las 13:23:19
-- Versión del servidor: 5.5.31-1
-- Versión de PHP: 5.4.4-15.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de datos: `hotel`
--
CREATE DATABASE IF NOT EXISTS `hotel` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `hotel`;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estado_hotel`
--

DROP TABLE IF EXISTS `estado_hotel`;
CREATE TABLE IF NOT EXISTS `estado_hotel` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_partida` int(11) DEFAULT NULL,
  `nombre` varchar(9) NOT NULL,
  `num_fases_construidas` int(11) NOT NULL,
  `entrada_comprada_ultimo_turno` tinyint(1) NOT NULL,
  `suelo_comprado` tinyint(1) NOT NULL,
  `posiciones_de_entradas` varchar(40) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_partida` (`id_partida`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=17 ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `estado_jugador`
--

DROP TABLE IF EXISTS `estado_jugador`;
CREATE TABLE IF NOT EXISTS `estado_jugador` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_partida` int(11) DEFAULT NULL,
  `nombre` varchar(20) NOT NULL,
  `numero` int(11) NOT NULL,
  `posicion` int(11) NOT NULL,
  `pago_ultimo_turno` tinyint(1) NOT NULL,
  `n_billetes_50` int(11) NOT NULL,
  `n_billetes_100` int(11) NOT NULL,
  `n_billetes_500` int(11) NOT NULL,
  `n_billetes_1000` int(11) NOT NULL,
  `n_billetes_5000` int(11) NOT NULL,
  `hoteles_poseidos` varchar(70) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_partida` (`id_partida`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=5 ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `partida`
--

DROP TABLE IF EXISTS `partida`;
CREATE TABLE IF NOT EXISTS `partida` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `password` varchar(50) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `nombre_creador` varchar(20) NOT NULL,
  `fecha_creacion` datetime NOT NULL,
  `fecha_salvado` datetime NOT NULL,
  `finalizada` tinyint(1) NOT NULL DEFAULT '0',
  `num_turnos` int(11) NOT NULL,
  `num_jugadores` int(11) NOT NULL,
  `num_jugadores_activos` int(11) NOT NULL,
  `jugador_inicial` int(11) NOT NULL,
  `jugador_actual` int(11) NOT NULL,
  `ultimo_res_dado` int(11) NOT NULL,
  `ultimo_avance_auto` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `estado_hotel`
--
ALTER TABLE `estado_hotel`
  ADD CONSTRAINT `estado_hotel_ibfk_1` FOREIGN KEY (`id_partida`) REFERENCES `partida` (`id`) ON DELETE SET NULL ON UPDATE SET NULL;

--
-- Filtros para la tabla `estado_jugador`
--
ALTER TABLE `estado_jugador`
  ADD CONSTRAINT `estado_jugador_ibfk_1` FOREIGN KEY (`id_partida`) REFERENCES `partida` (`id`) ON DELETE SET NULL ON UPDATE SET NULL;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
