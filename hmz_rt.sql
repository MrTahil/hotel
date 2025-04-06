-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Apr 06, 2025 at 05:27 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `hmz_rt`
--
CREATE DATABASE IF NOT EXISTS `hmz_rt` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `hmz_rt`;

-- --------------------------------------------------------

--
-- Table structure for table `amenities`
--

CREATE TABLE `amenities` (
  `amenity_name` varchar(255) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `amenity_id` int(11) NOT NULL,
  `availability` varchar(255) DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `room_id` int(11) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `icon` varchar(255) DEFAULT NULL,
  `category` varchar(255) DEFAULT NULL,
  `priority` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `amenities`
--

INSERT INTO `amenities` (`amenity_name`, `description`, `amenity_id`, `availability`, `date_added`, `room_id`, `status`, `icon`, `category`, `priority`) VALUES
('TV', 'Szép lapos', 3, 'Nem elérhető', NULL, 10, 'Javítás alatt', 'Tv', 'Technológia', 3),
('Törölközők', 'fürdő és kéztörlők', 4, 'Elérhető', NULL, 10, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 6, 'Elérhető', NULL, 10, 'Rendben', 'Wifi', 'Technológia', 5),
('Minibár', 'italok, snackek', 7, 'Elérhető', NULL, 10, 'Rendben', 'Liquor', 'Technológia', 2),
('Szobai széf', 'Széf', 8, 'Elérhető', NULL, 10, 'Rendben', 'Lock', 'Technológia', 3),
('Szauna', 'Törölközők wellness részlegekhez', 10, 'Elérhető', NULL, 10, 'Rendben', 'Spa', 'Wellness', 2),
('Különleges párnák', 'memóriahabos, anatómiai', 11, 'Elérhető', NULL, 10, 'Rendben', 'Bed', 'Extra', 1),
('TV', 'Szép lapos', 18, 'Elérhető', NULL, 3, 'Rendben', 'Tv', 'Technológia', 3),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 19, 'Elérhető', NULL, 3, 'Rendben', 'Wifi', 'Technológia', 5),
('Törölközők', 'fürdő és kéztörlők', 20, 'Elérhető', NULL, 3, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('TV', 'Szép lapos', 21, 'Nem elérhető', NULL, 4, 'Javítás alatt', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 22, 'Elérhető', NULL, 5, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 23, 'Elérhető', NULL, 6, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 24, 'Elérhető', NULL, 9, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 25, 'Elérhető', NULL, 10, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 26, 'Elérhető', NULL, 11, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 27, 'Nem elérhető', NULL, 12, 'Javítás alatt', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 28, 'Elérhető', NULL, 13, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 29, 'Elérhető', NULL, 14, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 30, 'Elérhető', NULL, 15, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 31, 'Elérhető', NULL, 16, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 32, 'Elérhető', NULL, 17, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 33, 'Elérhető', NULL, 18, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 34, 'Elérhető', NULL, 19, 'Rendben', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 35, 'Nem Elérhető', NULL, 20, 'Javítás alatt', 'Tv', 'Technológia', 3),
('TV', 'Szép lapos', 36, 'Elérhető', NULL, 21, 'Rendben', 'Tv', 'Technológia', 3),
('Törölközők', 'fürdő és kéztörlők', 37, 'Elérhető', NULL, 4, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 38, 'Nem Elérhető', NULL, 5, 'Mosás alatt', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 39, 'Elérhető', NULL, 6, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 40, 'Elérhető', NULL, 9, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 41, 'Elérhető', NULL, 10, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 42, 'Elérhető', NULL, 11, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 43, 'Elérhető', NULL, 12, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 44, 'Elérhető', NULL, 13, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 45, 'Nem Elérhető', NULL, 14, 'Mosás alatt', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 46, 'Elérhető', NULL, 15, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 47, 'Elérhető', NULL, 16, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 48, 'Elérhető', NULL, 17, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 49, 'Elérhető', NULL, 18, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 50, 'Elérhető', NULL, 19, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 51, 'Elérhető', NULL, 20, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Törölközők', 'fürdő és kéztörlők', 52, 'Elérhető', NULL, 21, 'Rendben', 'Shower', 'Alapvető kényelem', 3),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 53, 'Elérhető', NULL, 4, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 54, 'Elérhető', NULL, 5, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 55, 'Elérhető', NULL, 6, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 56, 'Elérhető', NULL, 9, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 57, 'Elérhető', NULL, 10, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 58, 'Elérhető', NULL, 11, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 59, 'Elérhető', NULL, 12, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 60, 'Elérhető', NULL, 13, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 61, 'Elérhető', NULL, 14, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 62, 'Elérhető', NULL, 15, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 63, 'Elérhető', NULL, 16, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 64, 'Elérhető', NULL, 17, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 65, 'Elérhető', NULL, 18, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 66, 'Elérhető', NULL, 19, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 67, 'Elérhető', NULL, 20, 'Rendben', 'Wifi', 'Technológia', 5),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 68, 'Elérhető', NULL, 21, 'Rendben', 'Wifi', 'Technológia', 5),
('Minibár', 'italok, snackek', 69, 'Elérhető', NULL, 18, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 70, 'Elérhető', NULL, 17, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 71, 'Elérhető', NULL, 4, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 72, 'Elérhető', NULL, 13, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 73, 'Elérhető', NULL, 16, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 74, 'Elérhető', NULL, 21, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 75, 'Elérhető', NULL, 10, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 76, 'Elérhető', NULL, 6, 'Rendben', 'Liquor', 'Technológia', 2),
('Minibár', 'italok, snackek', 77, 'Elérhető', NULL, 11, 'Rendben', 'Liquor', 'Technológia', 2),
('Szobai széf', 'Széf', 78, 'Elérhető', NULL, 11, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 79, 'Elérhető', NULL, 6, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 80, 'Elérhető', NULL, 10, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 81, 'Elérhető', NULL, 21, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 82, 'Elérhető', NULL, 16, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 83, 'Elérhető', NULL, 13, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 84, 'Elérhető', NULL, 4, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 85, 'Elérhető', NULL, 17, 'Rendben', 'Lock', 'Technológia', 3),
('Szobai széf', 'Széf', 86, 'Elérhető', NULL, 16, 'Rendben', 'Lock', 'Technológia', 3),
('Szauna', 'Törölközők wellness részlegekhez', 87, 'Elérhető', NULL, 18, 'Rendben', 'Spa', 'Wellness', 4),
('Szauna', 'Törölközők wellness részlegekhez', 88, 'Elérhető', NULL, 17, 'Rendben', 'Spa', 'Wellness', 4),
('Szauna', 'Törölközők wellness részlegekhez', 89, 'Elérhető', NULL, 13, 'Rendben', 'Spa', 'Wellness', 4),
('Szauna', 'Törölközők wellness részlegekhez', 90, 'Elérhető', NULL, 16, 'Rendben', 'Spa', 'Wellness', 4),
('Szauna', 'Törölközők wellness részlegekhez', 91, 'Elérhető', NULL, 21, 'Rendben', 'Spa', 'Wellness', 4),
('Különleges párnák', 'memóriahabos, anatómiai', 92, 'Elérhető', NULL, 10, 'Rendben', 'Spa', 'Extra', 1),
('Különleges párnák', 'memóriahabos, anatómiai', 93, 'Elérhető', NULL, 6, 'Rendben', 'Bed', 'Extra', 1),
('Különleges párnák', 'memóriahabos, anatómiai', 94, 'Elérhető', NULL, 11, 'Rendben', 'Bed', 'Extra', 1),
('Különleges párnák', 'memóriahabos, anatómiai', 95, 'Elérhető', NULL, 9, 'Rendben', 'Bed', 'Extra', 1),
('Különleges párnák', 'memóriahabos, anatómiai', 96, 'Elérhető', NULL, 12, 'Rendben', 'Bed', 'Extra', 1),
('Különleges párnák', 'memóriahabos, anatómiai', 97, 'Elérhető', NULL, 15, 'Rendben', 'Bed', 'Extra', 1),
('Különleges párnák', 'memóriahabos, anatómiai', 98, 'Elérhető', NULL, 20, 'Rendben', 'Bed', 'Extra', 1),
('Különleges párnák', 'memóriahabos, anatómiai', 99, 'Elérhető', NULL, 3, 'Rendben', 'Bed', 'Extra', 1);

-- --------------------------------------------------------

--
-- Table structure for table `bookings`
--

CREATE TABLE `bookings` (
  `room_id` int(11) NOT NULL,
  `booking_id` int(11) NOT NULL,
  `guest_id` int(11) NOT NULL,
  `check_in_date` date DEFAULT NULL,
  `check_out_date` date DEFAULT NULL,
  `number_of_guests` int(11) DEFAULT NULL,
  `total_price` decimal(10,0) DEFAULT NULL,
  `booking_date` date DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `payment_status` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `bookings`
--

INSERT INTO `bookings` (`room_id`, `booking_id`, `guest_id`, `check_in_date`, `check_out_date`, `number_of_guests`, `total_price`, `booking_date`, `status`, `payment_status`) VALUES
(20, 4, 3, '2025-03-12', '2025-03-15', 1, 195, '2025-03-10', 'Finished', 'Fizetve'),
(10, 6, 3, '2025-03-17', '2025-03-20', 1, 70000, '2025-03-17', 'Finished', 'Fizetve'),
(18, 7, 3, '2025-03-25', '2025-03-29', 4, 800000, '2025-03-23', 'Finished', 'Fizetve'),
(5, 11, 4, '2025-03-25', '2025-03-28', 1, 15000, '2025-03-24', 'Jóváhagyva', 'Fizetésre vár'),
(3, 16, 40, '2025-03-06', '2025-03-11', 1, 35000, '2025-03-29', 'Jóváhagyva', 'Fizetve'),
(21, 17, 40, '2025-03-12', '2025-03-14', 4, 400000, '2025-03-29', 'Jóváhagyva', 'Fizetve'),
(5, 21, 45, '2025-04-26', '2025-05-10', 1, 15000, '2025-04-02', 'Jóváhagyva', 'Fizetésre vár'),
(3, 23, 4, '2025-04-03', '2025-04-11', 2, 70000, '2025-04-02', 'Jóváhagyva', 'Fizetésre vár'),
(3, 29, 40, '2025-05-04', '2025-05-25', 1, 35000, '2025-04-03', 'Jóváhagyva', 'Fizetésre vár'),
(19, 30, 40, '2025-05-04', '2025-05-08', 1, 18000, '2025-04-03', 'Jóváhagyva', 'Fizetésre vár'),
(4, 31, 40, '2025-05-23', '2025-05-25', 1, 100000, '2025-04-03', 'Jóváhagyva', 'Fizetésre vár'),
(18, 32, 3, '1990-11-11', '1995-11-11', 6, 2191200000, '2025-04-03', 'Jóváhagyva', 'Fizetésre vár'),
(6, 33, 40, '2025-04-04', '2025-04-08', 5, 1200000, '2025-04-03', 'Jóváhagyva', 'Fizetésre vár'),
(11, 34, 4, '2025-04-06', '2025-04-18', 3, 2160000, '2025-04-05', 'Jóváhagyva', 'Fizetésre vár');

-- --------------------------------------------------------

--
-- Table structure for table `eventbookings`
--

CREATE TABLE `eventbookings` (
  `event_booking_id` int(11) NOT NULL,
  `event_id` int(11) NOT NULL,
  `guest_id` int(11) NOT NULL,
  `booking_date` date DEFAULT NULL,
  `number_of_tickets` int(11) DEFAULT NULL,
  `total_price` decimal(10,0) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `payment_status` varchar(255) DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `notes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `eventbookings`
--

INSERT INTO `eventbookings` (`event_booking_id`, `event_id`, `guest_id`, `booking_date`, `number_of_tickets`, `total_price`, `status`, `payment_status`, `date_added`, `notes`) VALUES
(33, 4, 40, '2025-04-03', 5, 12500, 'Foglalt', 'Fizetésre vár', '2025-04-03', ''),
(34, 5, 4, '2025-04-05', 1, 5000, 'Foglalt', 'Fizetésre vár', '2025-04-05', '');

-- --------------------------------------------------------

--
-- Table structure for table `events`
--

CREATE TABLE `events` (
  `capacity` int(11) DEFAULT NULL,
  `price` decimal(10,0) NOT NULL,
  `status` varchar(255) DEFAULT NULL,
  `event_id` int(11) NOT NULL,
  `event_name` varchar(255) DEFAULT NULL,
  `event_date` date DEFAULT NULL,
  `location` varchar(255) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `organizer_name` varchar(255) DEFAULT NULL,
  `contact_info` varchar(255) DEFAULT NULL,
  `images` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `events`
--

INSERT INTO `events` (`capacity`, `price`, `status`, `event_id`, `event_name`, `event_date`, `location`, `description`, `date_added`, `organizer_name`, `contact_info`, `images`) VALUES
(50000, 10000, 'Available', 1, 'Hotel Fesztivál', '2025-05-15', 'Margit-sziget', 'Haverok? Buli? Hangos zene? Mi kell még?! Gyere el minden hétvégén megrendezett fesztiválunkra, biztos élvezni fogod!', '2025-03-12', 'Sike Domonkos', 'siked@kkszki.hu', '../img/fesztival.png'),
(15, 5000, 'Available', 2, 'Főző tanfolyam', '2025-05-14', 'Recepciónál kell jelentkezni és abban a pillanatban vezetnek a konyhánkba!', 'Szeretnél belekóstolni a michelin csillagos éttermek titkaiba? Tarts velünk egy 2 órás tanfolyam keretein belül sok sok tapasztalatot szerezhetsz!', '2025-03-12', 'Séf Kovács Szabolcs', 'kovacssz@kkszki.hu', '../img/fozo.png'),
(10, 7500, 'Available', 3, 'Bortúra', '2025-05-25', 'Tokaj szőlőhegyek', 'Szereted a természetet, a madárcsicsergést, és netán a bort? Ezesetben gyere kóstoljatok bele a Tokaji szőlőhegyek termésébe!', '2025-03-12', 'Zelenák Zalán Noel', '+36 70 323 7436', '../img/bor.png'),
(5, 2500, 'Available', 4, 'Csillagászat', '2025-05-16', 'Hotelünk B2 épületének 15. emeletén található csillagászati szoba', 'Éjjeli bagoly vagy? Szereted az égitesteket? Gyere nézd meg őket!', '2025-03-12', 'Dr. Pozsgai Marcell TT.', 'pozsgaim@kkszki.hu', '../img/csillag.png'),
(15, 5000, 'Available', 5, 'Fotótúra', '2025-05-19', 'Csanyik', 'Szeretnél megtanulni profi módon kamerával bánni? Gyere, megtanítjuk!', '2025-03-12', 'Ágoston Attila', 'agostona@kkszki.hu', '../img/foto.png'),
(2, 10000, 'Available', 6, 'Horgászat', '2025-05-30', 'Csorba tó', 'Szeretnél egy kicsit elszakadni a világtól? Kapsz felszerelést, oktatást, mi kell még? Hal? Az is lesz....', '2025-03-12', 'Kovács Szabolcs', 'kovacssz@kszki.hu', '../img/horgaszat.png'),
(20, 5000, 'Available', 7, 'Jóga hegyen', '2025-05-28', 'Kékes', 'Szeretsz jógázni? Gyere a PHD jógaoktatónnkkal egy spirituális utazásra!', '2025-03-12', 'Jánosi Marcell PHD , Pozsgai Marcell PHD', 'janosim@kkszki.hu , pozsgaim@kkszki.hu', '../img/jogahegyen.png'),
(50, 8500, 'Available', 8, 'Múzeum túra', '2025-05-31', 'Pannon Tenger Múzeum', 'A világ egyik legelismertebb MD Doktorával körbenézhetsz és tanulhatsz a történelemről!', '2025-03-12', 'Mr. Dr. Jánosi MD. Marcell jr. sr.', 'janosim@kkszki.hu', '../img/muzeum.png'),
(200, 17500, 'Available', 9, 'Téli Sport', '2025-05-14', 'Kékes', 'Minden Decembertől Februárig elérhető köreinkben az olimpia aranyérmes magyar nyrtesünkkel a sielés, snowboardozás, és ami még van!', '2025-03-12', 'Bódi Balázs 🥇', 'bodib@kkszki.hu', '../img/sieles.png');

-- --------------------------------------------------------

--
-- Table structure for table `feedback`
--

CREATE TABLE `feedback` (
  `feedback_id` int(11) NOT NULL,
  `feedback_date` date DEFAULT NULL,
  `comments` text DEFAULT NULL,
  `category` varchar(255) DEFAULT NULL,
  `rating` decimal(10,0) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `response` text DEFAULT NULL,
  `response_date` date DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `guest_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `feedback`
--

INSERT INTO `feedback` (`feedback_id`, `feedback_date`, `comments`, `category`, `rating`, `status`, `response`, `response_date`, `date_added`, `guest_id`) VALUES
(1, '2025-03-28', '', 'Panasz', 5, 'Feldolgozva', 'nem volt jo', '2025-03-28', '2025-03-28', 3);

-- --------------------------------------------------------

--
-- Table structure for table `guests`
--

CREATE TABLE `guests` (
  `first_name` varchar(255) DEFAULT NULL,
  `guest_id` int(11) NOT NULL,
  `last_name` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `phone_number` varchar(255) DEFAULT NULL,
  `address` varchar(255) DEFAULT NULL,
  `city` varchar(255) DEFAULT NULL,
  `country` varchar(255) DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `gender` varchar(255) DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `guests`
--

INSERT INTO `guests` (`first_name`, `guest_id`, `last_name`, `email`, `phone_number`, `address`, `city`, `country`, `date_of_birth`, `gender`, `user_id`) VALUES
('Róbert', 3, 'Monostori', 'monostorir@kkszki.hu', '06707026565', 'Nagyi', 'Miskolc', 'Magyarország', '2006-02-23', 'Férfi', 5),
('Tamás', 4, 'Hilóczki', 'hiloczkit@kkszki.hu', '065254587', 'Nagy Lajos utca 12', 'Szeged', 'Magyarország', '2005-03-06', 'Férfi', 3),
('Omni', 40, 'Man', 'omniman@gmail.com', '+36703214586', 'Viltrum', 'Viltrum', 'Viltrum', '1999-01-08', 'Férfi', 22),
('Spongyabob', 43, 'Kockanadrág', 'spongya@gmail.com', '+366969696', 'Bikini utca 2', 'Bikini fenék', 'Kalifornia', '1987-12-31', 'Férfi', 22),
('Jason', 45, 'Momoa', 'jasonmomoa@gmail.com', '+37658789876', 'kjahhohoi', 'hoihfaohoi', 'oiahfgoiihaf', '1990-01-01', 'Férfi', 19);

-- --------------------------------------------------------

--
-- Table structure for table `invoices`
--

CREATE TABLE `invoices` (
  `status` varchar(255) DEFAULT NULL,
  `invoice_id` int(11) NOT NULL,
  `booking_id` int(11) NOT NULL,
  `invoice_date` date DEFAULT NULL,
  `total_amount` decimal(10,0) DEFAULT NULL,
  `payment_status` varchar(255) DEFAULT NULL,
  `due_date` date DEFAULT NULL,
  `notes` text DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `currency` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `loyaltyprograms`
--

CREATE TABLE `loyaltyprograms` (
  `program_name` varchar(255) DEFAULT NULL,
  `loyalty_program_id` int(11) NOT NULL,
  `description` text DEFAULT NULL,
  `points_required` int(11) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `benefits` text DEFAULT NULL,
  `expiration_period` int(11) DEFAULT NULL,
  `terms_conditions` text DEFAULT NULL,
  `category` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `marketing`
--

CREATE TABLE `marketing` (
  `marketing_id` int(11) NOT NULL,
  `campaign_name` varchar(255) DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `budget` decimal(10,0) DEFAULT 10,
  `status` varchar(255) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `target_audience` varchar(255) DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `notes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `newsletter`
--

CREATE TABLE `newsletter` (
  `newsid` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `userid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `newsletter`
--

INSERT INTO `newsletter` (`newsid`, `email`, `userid`) VALUES
(2, 'monostorir@kkszki.hu', 5);

-- --------------------------------------------------------

--
-- Table structure for table `notifications`
--

CREATE TABLE `notifications` (
  `notification_id` int(11) NOT NULL,
  `date_sent` date DEFAULT NULL,
  `message` text DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `type` varchar(255) DEFAULT NULL,
  `date_read` date DEFAULT NULL,
  `priority` int(11) DEFAULT NULL,
  `notes` text DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `category` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `payments`
--

CREATE TABLE `payments` (
  `payment_id` int(11) NOT NULL,
  `booking_id` int(11) NOT NULL,
  `payment_date` date DEFAULT NULL,
  `amount` decimal(10,0) DEFAULT NULL,
  `payment_method` varchar(255) DEFAULT NULL,
  `transaction_id` varchar(255) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `currency` varchar(255) DEFAULT NULL,
  `payment_notes` text DEFAULT NULL,
  `date_added` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `payments`
--

INSERT INTO `payments` (`payment_id`, `booking_id`, `payment_date`, `amount`, `payment_method`, `transaction_id`, `status`, `currency`, `payment_notes`, `date_added`) VALUES
(3, 4, '0001-01-01', 195, 'Készpénz', '0', 'Fizetve', 'Huf', '', '2025-03-10'),
(5, 6, '0001-01-01', 70000, 'Készpénz', '0', 'Fizetve', 'Huf', '', '2025-03-17'),
(6, 7, '0001-01-01', 800000, 'Készpénz', '0', 'Fizetve', 'Huf', '', '2025-03-23'),
(10, 11, '0001-01-01', 15000, 'Fizetés érkezéskor', '0', 'Fizetésre vár', 'Huf', '', '2025-03-24'),
(15, 16, '0001-01-01', 35000, 'Készpénz', '0', 'Fizetve', 'Huf', '', '2025-03-29'),
(16, 17, '0001-01-01', 400000, 'Átutalás', '0', 'Fizetve', 'Huf', '', '2025-03-29'),
(20, 21, '0001-01-01', 15000, 'Bankkártya', '0', 'Fizetésre vár', 'Huf', '', '2025-04-02'),
(22, 23, '0001-01-01', 70000, 'Bankkártya', '0', 'Fizetésre vár', 'Huf', '', '2025-04-02'),
(28, 29, '0001-01-01', 35000, 'Fizetés érkezéskor', '0', 'Fizetésre vár', 'Huf', '', '2025-04-03'),
(29, 30, '0001-01-01', 18000, 'Bankkártya', '0', 'Fizetésre vár', 'Huf', '', '2025-04-03'),
(30, 31, '0001-01-01', 100000, 'Bankkártya', '0', 'Fizetésre vár', 'Huf', '', '2025-04-03'),
(31, 32, '0001-01-01', 2191200000, 'Fizetés érkezéskor', '0', 'Fizetésre vár', 'Huf', '', '2025-04-03'),
(32, 33, '0001-01-01', 1200000, 'Bankkártya', '0', 'Fizetésre vár', 'Huf', '', '2025-04-03'),
(33, 34, '0001-01-01', 2160000, 'Fizetés érkezéskor', '0', 'Fizetésre vár', 'Huf', '', '2025-04-05');

-- --------------------------------------------------------

--
-- Table structure for table `promotions`
--

CREATE TABLE `promotions` (
  `promotion_id` int(11) NOT NULL,
  `promotion_name` varchar(255) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `terms_conditions` text DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `discount_percentage` decimal(10,0) DEFAULT NULL,
  `room_id` int(11) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `date_added` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `promotions`
--

INSERT INTO `promotions` (`promotion_id`, `promotion_name`, `description`, `start_date`, `terms_conditions`, `end_date`, `discount_percentage`, `room_id`, `status`, `date_added`) VALUES
(3, 'Tavaszi Szállás Kedvezmény', '15% kedvezmény a szállásokra a tavaszi időszakban', '2025-03-24', 'A kedvezmény csak a honlapunkon foglalható szobákra érvényes', '2025-04-23', 15, 10, 'Aktív', '2025-03-24'),
(4, 'Hétvégi Pihenés', '20% kedvezmény a hétvégi szállásokra', '2025-03-24', 'Csak szombat és vasárnap érvényes, minimum 2 éjszaka', '2025-12-31', 20, 3, 'Aktív', '2025-03-24'),
(5, 'Deluxe Szoba Kedvezmény', '30% kedvezmény a deluxe szobákra', '2025-03-24', 'Csak a deluxe szobákra érvényes', '2025-05-31', 30, 12, 'Aktív', '2025-03-24'),
(6, 'Deluxe Szoba Kedvezmény', '30% kedvezmény a deluxe szobákra', '2025-03-24', 'Csak a deluxe szobákra érvényes', '2025-05-31', 30, 15, 'Aktív', '2025-03-24'),
(7, 'Deluxe Szoba Kedvezmény', '30% kedvezmény a deluxe szobákra', '2025-03-24', 'Csak a deluxe szobákra érvényes', '2025-04-23', 30, 20, 'Aktív', '2025-03-24'),
(8, 'Nyári Szezon Kedvezmény', '18% kedvezmény a nyári szállásokra', '2025-06-01', 'A kedvezmény csak a honlapunkon foglalható szobákra érvényes', '2025-08-31', 18, 12, 'Tervezett', '2025-03-24');

-- --------------------------------------------------------

--
-- Table structure for table `reviews`
--

CREATE TABLE `reviews` (
  `review_date` date DEFAULT NULL,
  `review_id` int(11) NOT NULL,
  `guest_id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `rating` decimal(10,0) DEFAULT NULL,
  `comment` text DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `response` text DEFAULT NULL,
  `response_date` date DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `booking_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `reviews`
--

INSERT INTO `reviews` (`review_date`, `review_id`, `guest_id`, `room_id`, `rating`, `comment`, `status`, `response`, `response_date`, `date_added`, `booking_id`) VALUES
('2025-03-31', 5, 40, 21, 5, 'Ki lakik odalenn, kit rejt a víz', 'OK', '', '0001-01-01', '2025-03-31', 17),
('2025-04-01', 9, 40, 21, 5, 'Tetszett a szoba, tágas illatos kényelmes', 'OK', '', '0001-01-01', '2025-04-01', 17);

-- --------------------------------------------------------

--
-- Table structure for table `roominventory`
--

CREATE TABLE `roominventory` (
  `item_name` varchar(255) DEFAULT NULL,
  `quantity` int(11) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `last_updated` date DEFAULT NULL,
  `notes` text DEFAULT NULL,
  `supplier` varchar(255) DEFAULT NULL,
  `inventory_id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `cost_per_item` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `roommaintenance`
--

CREATE TABLE `roommaintenance` (
  `maintenance_id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `maintenance_date` date DEFAULT NULL,
  `description` text DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `staff_id` int(11) DEFAULT NULL,
  `date_reported` date DEFAULT NULL,
  `resolution_date` date DEFAULT NULL,
  `cost` decimal(10,0) DEFAULT NULL,
  `notes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `roommaintenance`
--

INSERT INTO `roommaintenance` (`maintenance_id`, `room_id`, `maintenance_date`, `description`, `status`, `staff_id`, `date_reported`, `resolution_date`, `cost`, `notes`) VALUES
(1, 5, '2025-02-21', 'Büdi van', 'In Progress', 2, '2025-02-21', '0001-01-01', 0, 'büdös'),
(4, 6, '2025-03-22', 'sadasd', 'Resolved', 2, '2025-03-20', '2025-03-25', 10000, 'asdada');

-- --------------------------------------------------------

--
-- Table structure for table `rooms`
--

CREATE TABLE `rooms` (
  `room_type` varchar(255) DEFAULT NULL,
  `room_id` int(11) NOT NULL,
  `room_number` varchar(255) DEFAULT NULL,
  `capacity` int(11) DEFAULT NULL,
  `price_per_night` decimal(10,0) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `floor_number` int(11) DEFAULT NULL,
  `amenities` text DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `images` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `rooms`
--

INSERT INTO `rooms` (`room_type`, `room_id`, `room_number`, `capacity`, `price_per_night`, `status`, `description`, `floor_number`, `amenities`, `date_added`, `images`) VALUES
('Standard', 3, '102', 2, 35000, 'Occupied', 'Kényelmes szoba queen-size ággyal és modern kényelmi szolgáltatásokkal.', 1, NULL, '2025-01-14', '../img/standard_room.png'),
('Suite', 4, '201', 4, 100000, 'Available', 'Tágas lakosztály külön nappali résszel, ideális családok számára.', 2, NULL, '2025-01-14', '../img/suite_room.png'),
('Single', 5, '301', 1, 15000, 'Under Maintenance', 'Kényelmes egyágyas szoba alapvető kényelmi szolgáltatásokkal, ideális egyedül utazók számára.', 3, NULL, '2025-01-14', '../img/single_room.png'),
('Family', 6, '302', 5, 60000, 'Available', 'Családi szoba több ággyal és játszótérrel a gyermekek számára.', 3, NULL, '2025-01-14', '../img/family_room.png'),
('Queen', 9, '106', 2, 50000, 'Available', 'Kényelmes queen-size ágyas szoba modern felszereltséggel.', 1, NULL, '2025-01-14', '../img/queen_room.png'),
('King', 10, '207', 2, 70000, 'Occupied', 'Tágas szoba luxus king-size ággyal és panorámás kilátással.', 2, NULL, '2025-01-14', '../img/king_room.png'),
('Family', 11, '520', 5, 60000, 'Available', 'Családi szoba két hálótérrel és gyerekbarát felszereléssel.', 5, NULL, '2025-03-06', '../img/family_room.png\n'),
('Deluxe', 12, '202', 3, 45000, 'Occupied', 'Tágas szoba extra kényelmi szolgáltatásokkal és erkéllyel.', 2, NULL, '2025-03-06', '../img/deluxe_room.png'),
('Suite', 13, '305', 4, 100000, 'Available', 'Luxus lakosztály panorámás kilátással és privát jacuzzival.', 3, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Single', 14, '410', 1, 15000, 'Under Maintenance', 'Egyszerű, de kényelmes egyágyas szoba üzleti utazók számára.', 4, NULL, '2025-03-06', '../img/single_room.png\n'),
('Deluxe', 15, '203', 3, 45000, 'Available', 'Tágas szoba erkéllyel és luxus felszereltséggel.', 2, NULL, '2025-03-06', '../img/deluxe_room.png'),
('Suite', 16, '306', 4, 100000, 'Occupied', 'Luxus lakosztály privát jacuzzival és panorámás kilátással.', 3, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Penthouse Suite', 17, '601', 6, 150000, 'Occupied', 'Luxus penthouse lakosztály privát terasszal és jacuzzival.', 6, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Presidential Suite', 18, '702', 6, 200000, 'Available', 'Elegáns elnöki lakosztály hatalmas nappalival és privát szaunával.', 7, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Economy', 19, '110', 2, 18000, 'Occupied', 'Egyszerű, de kényelmes szoba alacsonyabb árkategóriában.', 1, NULL, '2025-03-06', '../img/default_image.png\n'),
('Deluxe', 20, '205', 3, 45000, 'Under Maintance', 'Deluxe szoba extra kényelmi szolgáltatásokkal és kilátással a városra.', 2, NULL, '2025-03-06', '../img/deluxe_room.png'),
('Suite', 21, '307', 4, 100000, 'Available', 'Luxus lakosztály tágas nappalival és jacuzzival.', 3, NULL, '2025-03-06', '../img/suite_room.png\n');

-- --------------------------------------------------------

--
-- Table structure for table `services`
--

CREATE TABLE `services` (
  `service_name` varchar(255) DEFAULT NULL,
  `service_id` int(11) NOT NULL,
  `description` text DEFAULT NULL,
  `price` decimal(10,0) DEFAULT NULL,
  `service_type` varchar(255) DEFAULT NULL,
  `availability` varchar(255) DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `staff_id` int(11) DEFAULT NULL,
  `rating` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `staff`
--

CREATE TABLE `staff` (
  `first_name` varchar(255) DEFAULT NULL,
  `staff_id` int(11) NOT NULL,
  `last_name` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `phone_number` varchar(255) DEFAULT NULL,
  `position` varchar(255) DEFAULT NULL,
  `salary` decimal(10,0) DEFAULT NULL,
  `date_hired` date DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `department` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `staff`
--

INSERT INTO `staff` (`first_name`, `staff_id`, `last_name`, `email`, `phone_number`, `position`, `salary`, `date_hired`, `status`, `department`) VALUES
('Pozsgai', 1, 'Marcell', 'pozsgaim@kkszki.hu', '+36 2013747285', 'Tomo arus', -1000, '2025-02-21', 'Aktív', 'Tomo king'),
('Jánosi', 2, 'Marcell', 'janosim@kkszki.hu', '+36 2019855222', 'Hotel Manager', 10000000, '2025-02-21', 'Inaktív', 'Manager'),
('Zalán', 5, 'Zelenák', 'zelenakz@kkszki.hu', '+36 209548099', 'Karbantartó', 100000000, '2025-02-25', 'Szabadságon', 'IT');

-- --------------------------------------------------------

--
-- Table structure for table `useraccounts`
--

CREATE TABLE `useraccounts` (
  `username` varchar(255) DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `password` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `role` varchar(255) DEFAULT NULL,
  `RefreshToken` varchar(255) DEFAULT NULL,
  `RefreshTokenExpiryTime` date DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `date_created` date DEFAULT NULL,
  `last_login` date DEFAULT NULL,
  `date_updated` date DEFAULT NULL,
  `notes` text DEFAULT NULL,
  `authenticationcode` varchar(255) DEFAULT NULL,
  `authenticationexpire` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `useraccounts`
--

INSERT INTO `useraccounts` (`username`, `user_id`, `password`, `email`, `role`, `RefreshToken`, `RefreshTokenExpiryTime`, `status`, `date_created`, `last_login`, `date_updated`, `notes`, `authenticationcode`, `authenticationexpire`) VALUES
('Tahil', 3, 'hte+RnleAunUji+Bx3f7EPd8Nd2nOw82PS50E6kPBYOBYQ+8JUKlArMigzjZ1CM3', 'hiloczkit@kkszki.hu', 'System', 'l2Yx+hjgM/qmqD8Rx/bJUB24ioCH1uB9Z0nn/vI1e1k=', '2025-04-12', 'string', '2025-01-16', '2025-01-16', '2025-01-16', 'string', '111111', '2025-02-13'),
('a_Beto', 5, 'u04oCPhO+K7Y9IBD+zsk/QP/jWnVhlEdpOyaWFAzwQjPvc0kubpehqBt15MLXuVv', 'monostorir@kkszki.hu', 'System', 'll17xCgdnQQBLgpYR0CpM0BcWjIQpwlVxbPHzPiP8GI=', '2025-04-13', 'string', '2025-02-14', '2025-02-14', '2025-02-14', 'string', 'activated', '2025-02-20'),
('Bozsgai', 6, 'qcFvcMo+qVpNxTezQhvnNO1acWKoZ1LEuvTFPrpsF48g/IL7vNu9jxt9epGzeLWA', 'monostorir@kkszki.hu', 'Base', 'gAuQy6/FuHKA0O1HEhZ4/d4iecZg8P88EprvucvItbA=', '2025-03-31', NULL, '2025-02-21', '2025-02-21', '2025-02-21', NULL, '000000', '2025-03-20'),
('LoriAttila', 19, 'Vfc5k4Hhfh9g6fkBRrImBSi5/sDtEGfThpMVysBdNY2+JeOtPH4p4AUD9GDHTJQC', 'nokivagyok69420@gmail.com', 'Base', 'C6/fE84d7yOHcLQYIX2pi+0mUAbGP3d6bZCRBnkbv3U=', '2025-04-09', NULL, '2025-03-24', '2025-03-24', '2025-03-24', NULL, 'activated', '2025-03-29'),
('ZuZuu', 22, 'L2FGPfANZJWC27IMMahtkimizz8eRTEKCt7qVStIioAr2M8haZ2FSiQ7h2V0M9Su', 'zelenakz@kkszki.hu', 'System', 'aYjrinBnogr+90lg0FjGemB4zDYFfkDouNbr+i1Lxok=', '2025-04-10', NULL, '2025-03-29', '2025-03-29', '2025-03-29', NULL, 'activated', NULL),
('Nemethb', 24, 'RXnhRqaxVPUb/x+xUmy6I+22oHLyKcrDhxr3nq0fy5bsTGdl5VS/lQQBjOl1kr0v', 'mindegy@kkszki.hu', 'System', 'mo1Dm6bTtVqMshqImakn7EpMaG0HBDx3g1rbKGY8yys=', '2025-04-11', NULL, '2025-04-04', '2025-04-04', '2025-04-04', NULL, 'activated', NULL);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `amenities`
--
ALTER TABLE `amenities`
  ADD PRIMARY KEY (`amenity_id`),
  ADD KEY `Amenities_fk5` (`room_id`);

--
-- Indexes for table `bookings`
--
ALTER TABLE `bookings`
  ADD PRIMARY KEY (`booking_id`),
  ADD KEY `Bookings_fk0` (`room_id`),
  ADD KEY `Bookings_fk2` (`guest_id`);

--
-- Indexes for table `eventbookings`
--
ALTER TABLE `eventbookings`
  ADD PRIMARY KEY (`event_booking_id`),
  ADD KEY `EventBookings_fk1` (`event_id`),
  ADD KEY `EventBookings_fk2` (`guest_id`);

--
-- Indexes for table `events`
--
ALTER TABLE `events`
  ADD PRIMARY KEY (`event_id`);

--
-- Indexes for table `feedback`
--
ALTER TABLE `feedback`
  ADD PRIMARY KEY (`feedback_id`),
  ADD KEY `Feedback_fk9` (`guest_id`);

--
-- Indexes for table `guests`
--
ALTER TABLE `guests`
  ADD PRIMARY KEY (`guest_id`),
  ADD KEY `fk_guests_useraccounts` (`user_id`);

--
-- Indexes for table `invoices`
--
ALTER TABLE `invoices`
  ADD PRIMARY KEY (`invoice_id`),
  ADD KEY `Invoices_fk2` (`booking_id`);

--
-- Indexes for table `loyaltyprograms`
--
ALTER TABLE `loyaltyprograms`
  ADD PRIMARY KEY (`loyalty_program_id`);

--
-- Indexes for table `marketing`
--
ALTER TABLE `marketing`
  ADD PRIMARY KEY (`marketing_id`);

--
-- Indexes for table `newsletter`
--
ALTER TABLE `newsletter`
  ADD PRIMARY KEY (`newsid`),
  ADD KEY `userid` (`userid`);

--
-- Indexes for table `notifications`
--
ALTER TABLE `notifications`
  ADD PRIMARY KEY (`notification_id`),
  ADD KEY `Notifications_fk8` (`user_id`);

--
-- Indexes for table `payments`
--
ALTER TABLE `payments`
  ADD PRIMARY KEY (`payment_id`),
  ADD KEY `Payments_fk1` (`booking_id`);

--
-- Indexes for table `promotions`
--
ALTER TABLE `promotions`
  ADD PRIMARY KEY (`promotion_id`),
  ADD KEY `Promotions_fk7` (`room_id`);

--
-- Indexes for table `reviews`
--
ALTER TABLE `reviews`
  ADD PRIMARY KEY (`review_id`),
  ADD KEY `Reviews_fk2` (`guest_id`),
  ADD KEY `Reviews_fk3` (`room_id`),
  ADD KEY `Booking_id` (`booking_id`);

--
-- Indexes for table `roominventory`
--
ALTER TABLE `roominventory`
  ADD PRIMARY KEY (`inventory_id`),
  ADD KEY `RoomInventory_fk8` (`room_id`);

--
-- Indexes for table `roommaintenance`
--
ALTER TABLE `roommaintenance`
  ADD PRIMARY KEY (`maintenance_id`),
  ADD KEY `RoomMaintenance_fk1` (`room_id`),
  ADD KEY `RoomMaintenance_fk5` (`staff_id`);

--
-- Indexes for table `rooms`
--
ALTER TABLE `rooms`
  ADD PRIMARY KEY (`room_id`);

--
-- Indexes for table `services`
--
ALTER TABLE `services`
  ADD PRIMARY KEY (`service_id`);

--
-- Indexes for table `staff`
--
ALTER TABLE `staff`
  ADD PRIMARY KEY (`staff_id`);

--
-- Indexes for table `useraccounts`
--
ALTER TABLE `useraccounts`
  ADD PRIMARY KEY (`user_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `amenities`
--
ALTER TABLE `amenities`
  MODIFY `amenity_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=100;

--
-- AUTO_INCREMENT for table `bookings`
--
ALTER TABLE `bookings`
  MODIFY `booking_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;

--
-- AUTO_INCREMENT for table `eventbookings`
--
ALTER TABLE `eventbookings`
  MODIFY `event_booking_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;

--
-- AUTO_INCREMENT for table `events`
--
ALTER TABLE `events`
  MODIFY `event_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `feedback`
--
ALTER TABLE `feedback`
  MODIFY `feedback_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `guests`
--
ALTER TABLE `guests`
  MODIFY `guest_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=47;

--
-- AUTO_INCREMENT for table `invoices`
--
ALTER TABLE `invoices`
  MODIFY `invoice_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `loyaltyprograms`
--
ALTER TABLE `loyaltyprograms`
  MODIFY `loyalty_program_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `marketing`
--
ALTER TABLE `marketing`
  MODIFY `marketing_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `newsletter`
--
ALTER TABLE `newsletter`
  MODIFY `newsid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `notifications`
--
ALTER TABLE `notifications`
  MODIFY `notification_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `payments`
--
ALTER TABLE `payments`
  MODIFY `payment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- AUTO_INCREMENT for table `promotions`
--
ALTER TABLE `promotions`
  MODIFY `promotion_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `reviews`
--
ALTER TABLE `reviews`
  MODIFY `review_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `roominventory`
--
ALTER TABLE `roominventory`
  MODIFY `inventory_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `roommaintenance`
--
ALTER TABLE `roommaintenance`
  MODIFY `maintenance_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `rooms`
--
ALTER TABLE `rooms`
  MODIFY `room_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT for table `services`
--
ALTER TABLE `services`
  MODIFY `service_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `staff`
--
ALTER TABLE `staff`
  MODIFY `staff_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `useraccounts`
--
ALTER TABLE `useraccounts`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `amenities`
--
ALTER TABLE `amenities`
  ADD CONSTRAINT `Amenities_fk5` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `bookings`
--
ALTER TABLE `bookings`
  ADD CONSTRAINT `Bookings_fk0` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `Bookings_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `eventbookings`
--
ALTER TABLE `eventbookings`
  ADD CONSTRAINT `EventBookings_fk1` FOREIGN KEY (`event_id`) REFERENCES `events` (`event_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `EventBookings_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `feedback`
--
ALTER TABLE `feedback`
  ADD CONSTRAINT `Feedback_fk9` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `guests`
--
ALTER TABLE `guests`
  ADD CONSTRAINT `fk_guests_useraccounts` FOREIGN KEY (`user_id`) REFERENCES `useraccounts` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `invoices`
--
ALTER TABLE `invoices`
  ADD CONSTRAINT `Invoices_fk2` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`booking_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `newsletter`
--
ALTER TABLE `newsletter`
  ADD CONSTRAINT `userfk` FOREIGN KEY (`userid`) REFERENCES `useraccounts` (`user_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `notifications`
--
ALTER TABLE `notifications`
  ADD CONSTRAINT `Notifications_fk8` FOREIGN KEY (`user_id`) REFERENCES `useraccounts` (`user_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `payments`
--
ALTER TABLE `payments`
  ADD CONSTRAINT `Payments_fk1` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`booking_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `promotions`
--
ALTER TABLE `promotions`
  ADD CONSTRAINT `Promotions_fk7` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `reviews`
--
ALTER TABLE `reviews`
  ADD CONSTRAINT `Reviews_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `Reviews_fk3` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `reviews_ibfk_1` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`booking_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `roominventory`
--
ALTER TABLE `roominventory`
  ADD CONSTRAINT `RoomInventory_fk8` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `roommaintenance`
--
ALTER TABLE `roommaintenance`
  ADD CONSTRAINT `RoomMaintenance_fk1` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `RoomMaintenance_fk5` FOREIGN KEY (`staff_id`) REFERENCES `staff` (`staff_id`) ON DELETE CASCADE ON UPDATE NO ACTION;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
