-- phpMyAdmin SQL Dump
-- version 4.0.5deb1
-- http://www.phpmyadmin.net
--
-- Servidor: localhost
-- Tiempo de generación: 26-08-2013 a las 16:28:00
-- Versión del servidor: 5.5.31-1
-- Versión de PHP: 5.5.1-2

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

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
  `dueno` int(11) DEFAULT NULL,
  `nombre` varchar(9) NOT NULL,
  `num_fases_construidas` int(11) NOT NULL,
  `entrada_comprada_ultimo_turno` tinyint(1) NOT NULL,
  `suelo_comprado` tinyint(1) NOT NULL,
  `posiciones_de_entradas` varchar(40) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_partida_idx` (`id_partida`),
  KEY `dueno_idx` (`dueno`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=17 ;

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
  PRIMARY KEY (`id`),
  KEY `id_partida_idx` (`id_partida`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=5 ;

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
  ADD CONSTRAINT FOREIGN KEY (`dueno`) REFERENCES `estado_jugador` (`id`) ON DELETE SET NULL ON UPDATE SET NULL,
  ADD CONSTRAINT FOREIGN KEY (`id_partida`) REFERENCES `partida` (`id`) ON DELETE SET NULL ON UPDATE SET NULL;

--
-- Filtros para la tabla `estado_jugador`
--
ALTER TABLE `estado_jugador`
  ADD CONSTRAINT FOREIGN KEY (`id_partida`) REFERENCES `partida` (`id`) ON DELETE SET NULL ON UPDATE SET NULL;