-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Már 17. 09:27
-- Kiszolgáló verziója: 10.4.20-MariaDB
-- PHP verzió: 7.3.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `hmz_rt`
--
CREATE DATABASE IF NOT EXISTS `hmz_rt` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `hmz_rt`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `amenities`
--

CREATE TABLE `amenities` (
  `amenity_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `amenity_id` int(11) NOT NULL,
  `availability` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `room_id` int(11) DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `icon` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `category` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `priority` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `amenities`
--

INSERT INTO `amenities` (`amenity_name`, `description`, `amenity_id`, `availability`, `date_added`, `room_id`, `status`, `icon`, `category`, `priority`) VALUES
('TV', 'Szép lapos', 3, 'Nem elérhető', NULL, 10, 'Javítás alatt', '<FaTv />', 'Technológia', 3),
('Törölközők', 'fürdő és kéztörlők', 4, 'Elérhető', NULL, 10, 'Rendben', '<FaTshirt />', 'Alapvető kényelem', 3),
('Ingyenes Wi-Fi', 'A szobához modern kornak megfelelő sebességű internet tartozik', 6, 'Elérhető', NULL, 10, 'Rendben', '<FaWifi />', 'Technológia', 5),
('Minibár', 'italok, snackek', 7, 'Elérhető', NULL, 10, 'Rendben', '<FaGlassMartiniAlt />', 'Technológia', 2),
('Szobai széf', 'Széf', 8, 'Elérhető', NULL, 10, 'Rendben', '<FaLock />', 'Technológia', 3),
('Szauna', 'Törölközők wellness részlegekhez', 10, 'Elérhető', NULL, 10, 'Rendben', '<FaSpa />', 'Wellness', 2),
('Különleges párnák', 'memóriahabos, anatómiai', 11, 'Elérhető', NULL, 10, 'Rendben', '<FaBed />', 'Extra', 1);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `bookings`
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
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `payment_status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `bookings`
--

INSERT INTO `bookings` (`room_id`, `booking_id`, `guest_id`, `check_in_date`, `check_out_date`, `number_of_guests`, `total_price`, `booking_date`, `status`, `payment_status`) VALUES
(20, 4, 3, '2025-03-12', '2025-03-15', 1, '195', '2025-03-10', 'Checked In', 'Fizetésre vár'),
(10, 6, 3, '2025-03-17', '2025-03-20', 1, '70000', '2025-03-17', 'Finished', 'Fizetve');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `eventbookings`
--

CREATE TABLE `eventbookings` (
  `event_booking_id` int(11) NOT NULL,
  `event_id` int(11) NOT NULL,
  `guest_id` int(11) NOT NULL,
  `booking_date` date DEFAULT NULL,
  `number_of_tickets` int(11) DEFAULT NULL,
  `total_price` decimal(10,0) DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `payment_status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `events`
--

CREATE TABLE `events` (
  `capacity` int(11) DEFAULT NULL,
  `price` decimal(10,0) NOT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `event_id` int(11) NOT NULL,
  `event_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `event_date` date DEFAULT NULL,
  `location` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `organizer_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `contact_info` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `images` longtext COLLATE utf8mb4_hungarian_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `events`
--

INSERT INTO `events` (`capacity`, `price`, `status`, `event_id`, `event_name`, `event_date`, `location`, `description`, `date_added`, `organizer_name`, `contact_info`, `images`) VALUES
(50000, '0', 'Available', 1, 'Hotel Fesztivál', '2025-03-15', 'Margit-sziget', 'Haverok? Buli? Hangos zene? Mi kell még?! Gyere el minden hétvégén megrendezett fesztiválunkra, biztos élvezni fogod!', '2025-03-12', 'Sike Domonkos', 'siked@kkszki.hu', '../img/fesztival.png'),
(15, '0', 'Available', 2, 'Főző tanfolyam', '2025-03-14', 'Recepciónál kell jelentkezni és abban a pillanatban vezetnek a konyhánkba!', 'Szeretnél belekóstolni a michelin csillagos éttermek titkaiba? Tarts velünk egy 2 órás tanfolyam keretein belül sok sok tapasztalatot szerezhetsz!', '2025-03-12', 'Séf Kovács Szabolcs', 'kovacssz@kkszki.hu', '../img/fozo.png'),
(10, '0', 'Available', 3, 'Bortúra', '2025-03-25', 'Tokaj szőlőhegyek', 'Szereted a természetet, a madárcsicsergést, és netán a bort? Ezesetben gyere kóstoljatok bele a Tokaji szőlőhegyek termésébe!', '2025-03-12', 'Zelenák Zalán Noel', '+36 70 323 7436', '../img/bor.png'),
(5, '0', 'Available', 4, 'Csillagászat', '2025-03-16', 'Hotelünk B2 épületének 15. emeletén található csillagászati szoba', 'Éjjeli bagoly vagy? Szereted az égitesteket? Gyere nézd meg őket!', '2025-03-12', 'Dr. Pozsgai Marcell TT.', 'pozsgaim@kkszki.hu', '../img/csillag.png'),
(15, '0', 'Available', 5, 'Fotótúra', '2025-03-19', 'Csanyik', 'Szeretnél megtanulni profi módon kamerával bánni? Gyere, megtanítjuk!', '2025-03-12', 'Ágoston Attila', 'agostona@kkszki.hu', '../img/foto.png'),
(2, '0', 'Available', 6, 'Horgászat', '2025-03-30', 'Csorba tó', 'Szeretnél egy kicsit elszakadni a világtól? Kapsz felszerelést, oktatást, mi kell még? Hal? Az is lesz....', '2025-03-12', 'Kovács Szabolcs', 'kovacssz@kszki.hu', '../img/horgaszat.png'),
(20, '0', 'Available', 7, 'Jóga hegyen', '2025-03-28', 'Kékes', 'Szeretsz jógázni? Gyere a PHD jógaoktatónnkkal egy spirituális utazásra!', '2025-03-12', 'Jánosi Marcell PHD , Pozsgai Marcell PHD', 'janosim@kkszki.hu , pozsgaim@kkszki.hu', '../img/jogahegyen.png'),
(50, '0', 'Available', 8, 'Múzeum túra', '2025-03-31', 'Pannon Tenger Múzeum', 'A világ egyik legelismertebb MD Doktorával körbenézhetsz és tanulhatsz a történelemről!', '2025-03-12', 'Mr. Dr. Jánosi MD. Marcell jr. sr.', 'janosim@kkszki.hu', '../img/muzeum.png'),
(200, '0', 'Available', 9, 'Téli Sport', '2025-03-14', 'Kékes', 'Minden Decembertől Februárig elérhető köreinkben az olimpia aranyérmes magyar nyrtesünkkel a sielés, snowboardozás, és ami még van!', '2025-03-12', 'Bódi Balázs 🥇', 'bodib@kkszki.hu', '../img/sieles.png');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `feedback`
--

CREATE TABLE `feedback` (
  `feedback_id` int(11) NOT NULL,
  `feedback_date` date DEFAULT NULL,
  `comments` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `category` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `rating` decimal(10,0) DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `response` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `response_date` date DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `guest_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `guests`
--

CREATE TABLE `guests` (
  `first_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `guest_id` int(11) NOT NULL,
  `last_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `email` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `phone_number` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `address` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `city` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `country` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `gender` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `guests`
--

INSERT INTO `guests` (`first_name`, `guest_id`, `last_name`, `email`, `phone_number`, `address`, `city`, `country`, `date_of_birth`, `gender`, `user_id`) VALUES
('Róbert', 3, 'Monostori', 'monostorir@kkszki.hu', '06707026565', 'Josika utca 17', 'Miskolc', 'Hungary', '2025-02-24', 'Férfi', 5),
('Tamás', 4, 'Hilóczki', 'hiloczkit@kkszki.hu', '065254587', 'Nagy Lajos utca 12', 'Szeged', 'Hungary', '2005-03-06', 'Férfi', 3);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `invoices`
--

CREATE TABLE `invoices` (
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `invoice_id` int(11) NOT NULL,
  `booking_id` int(11) NOT NULL,
  `invoice_date` date DEFAULT NULL,
  `total_amount` decimal(10,0) DEFAULT NULL,
  `payment_status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `due_date` date DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `currency` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `loyaltyprograms`
--

CREATE TABLE `loyaltyprograms` (
  `program_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `loyalty_program_id` int(11) NOT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `points_required` int(11) DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `benefits` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `expiration_period` int(11) DEFAULT NULL,
  `terms_conditions` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `category` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `marketing`
--

CREATE TABLE `marketing` (
  `marketing_id` int(11) NOT NULL,
  `campaign_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `budget` decimal(10,0) DEFAULT 10,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `target_audience` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `notifications`
--

CREATE TABLE `notifications` (
  `notification_id` int(11) NOT NULL,
  `date_sent` date DEFAULT NULL,
  `message` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `type` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_read` date DEFAULT NULL,
  `priority` int(11) DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `category` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `payments`
--

CREATE TABLE `payments` (
  `payment_id` int(11) NOT NULL,
  `booking_id` int(11) NOT NULL,
  `payment_date` date DEFAULT NULL,
  `amount` decimal(10,0) DEFAULT NULL,
  `payment_method` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `transaction_id` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `currency` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `payment_notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `payments`
--

INSERT INTO `payments` (`payment_id`, `booking_id`, `payment_date`, `amount`, `payment_method`, `transaction_id`, `status`, `currency`, `payment_notes`, `date_added`) VALUES
(3, 4, '0001-01-01', '195', 'Készpénz', '0', 'Fizetve', 'Huf', '', '2025-03-10'),
(5, 6, '0001-01-01', '70000', 'Készpénz', '0', 'Fizetve', 'Huf', '', '2025-03-17');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `promotions`
--

CREATE TABLE `promotions` (
  `promotion_id` int(11) NOT NULL,
  `promotion_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `terms_conditions` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `discount_percentage` decimal(10,0) DEFAULT NULL,
  `room_id` int(11) DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `reviews`
--

CREATE TABLE `reviews` (
  `review_date` date DEFAULT NULL,
  `review_id` int(11) NOT NULL,
  `guest_id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `rating` decimal(10,0) DEFAULT NULL,
  `comment` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `response` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `response_date` date DEFAULT NULL,
  `date_added` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `roominventory`
--

CREATE TABLE `roominventory` (
  `item_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `quantity` int(11) DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `last_updated` date DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `supplier` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `inventory_id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `cost_per_item` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `roommaintenance`
--

CREATE TABLE `roommaintenance` (
  `maintenance_id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `maintenance_date` date DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `staff_id` int(11) DEFAULT NULL,
  `date_reported` date DEFAULT NULL,
  `resolution_date` date DEFAULT NULL,
  `cost` decimal(10,0) DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `roommaintenance`
--

INSERT INTO `roommaintenance` (`maintenance_id`, `room_id`, `maintenance_date`, `description`, `status`, `staff_id`, `date_reported`, `resolution_date`, `cost`, `notes`) VALUES
(1, 5, '2025-02-21', 'Büdi van', 'Not resolved yet', NULL, '2025-02-21', '0001-01-01', '0', 'büdös');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `rooms`
--

CREATE TABLE `rooms` (
  `room_type` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `room_id` int(11) NOT NULL,
  `room_number` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `capacity` int(11) DEFAULT NULL,
  `price_per_night` decimal(10,0) DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `floor_number` int(11) DEFAULT NULL,
  `amenities` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `images` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `rooms`
--

INSERT INTO `rooms` (`room_type`, `room_id`, `room_number`, `capacity`, `price_per_night`, `status`, `description`, `floor_number`, `amenities`, `date_added`, `images`) VALUES
('Standard', 3, '102', 2, '35000', 'Occupied', 'Kényelmes szoba queen-size ággyal és modern kényelmi szolgáltatásokkal.', 1, NULL, '2025-01-14', '../img/standard_room.png'),
('Suite', 4, '201', 4, '100000', 'Available', 'Tágas lakosztály külön nappali résszel, ideális családok számára.', 2, NULL, '2025-01-14', '../img/suite_room.png'),
('Single', 5, '301', 1, '15000', 'Under Maintenance', 'Kényelmes egyágyas szoba alapvető kényelmi szolgáltatásokkal, ideális egyedül utazók számára.', 3, NULL, '2025-01-14', '../img/single_room.png'),
('Family', 6, '302', 5, '60000', 'Available', 'Családi szoba több ággyal és játszótérrel a gyermekek számára.', 3, NULL, '2025-01-14', '../img/family_room.png'),
('Queen', 9, '106', 2, '50000', 'Available', 'Kényelmes queen-size ágyas szoba modern felszereltséggel.', 1, NULL, '2025-01-14', '../img/queen_room.png'),
('King', 10, '207', 2, '70000', 'Occupied', 'Tágas szoba luxus king-size ággyal és panorámás kilátással.', 2, NULL, '2025-01-14', '../img/king_room.png'),
('Family', 11, '520', 5, '60000', 'Available', 'Családi szoba két hálótérrel és gyerekbarát felszereléssel.', 5, NULL, '2025-03-06', '../img/family_room.png\n'),
('Deluxe', 12, '202', 3, '45000', 'Occupied', 'Tágas szoba extra kényelmi szolgáltatásokkal és erkéllyel.', 2, NULL, '2025-03-06', '../img/deluxe_room.png'),
('Suite', 13, '305', 4, '100000', 'Available', 'Luxus lakosztály panorámás kilátással és privát jacuzzival.', 3, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Single', 14, '410', 1, '15000', 'Under Maintenance', 'Egyszerű, de kényelmes egyágyas szoba üzleti utazók számára.', 4, NULL, '2025-03-06', '../img/single_room.png\n'),
('Deluxe', 15, '203', 3, '45000', 'Available', 'Tágas szoba erkéllyel és luxus felszereltséggel.', 2, NULL, '2025-03-06', '../img/deluxe_room.png'),
('Suite', 16, '306', 4, '100000', 'Occupied', 'Luxus lakosztály privát jacuzzival és panorámás kilátással.', 3, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Penthouse Suite', 17, '601', 6, '150000', 'Occupied', 'Luxus penthouse lakosztály privát terasszal és jacuzzival.', 6, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Presidential Suite', 18, '702', 6, '200000', 'Available', 'Elegáns elnöki lakosztály hatalmas nappalival és privát szaunával.', 7, NULL, '2025-03-06', '../img/suite_room.png\n'),
('Economy', 19, '110', 2, '18000', 'Occupied', 'Egyszerű, de kényelmes szoba alacsonyabb árkategóriában.', 1, NULL, '2025-03-06', '../img/default_image.png\n'),
('Deluxe', 20, '205', 3, '45000', 'Under Maintance', 'Deluxe szoba extra kényelmi szolgáltatásokkal és kilátással a városra.', 2, NULL, '2025-03-06', '../img/deluxe_room.png'),
('Suite', 21, '307', 4, '100000', 'Available', 'Luxus lakosztály tágas nappalival és jacuzzival.', 3, NULL, '2025-03-06', '../img/suite_room.png\n');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `roomtypes`
--

CREATE TABLE `roomtypes` (
  `type_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `room_type_id` int(11) NOT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `base_price` decimal(10,0) DEFAULT NULL,
  `max_capacity` int(11) DEFAULT NULL,
  `amenities` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `image_url` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `priority` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `services`
--

CREATE TABLE `services` (
  `service_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `service_id` int(11) NOT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `price` decimal(10,0) DEFAULT NULL,
  `service_type` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `availability` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `staff_id` int(11) DEFAULT NULL,
  `rating` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `staff`
--

CREATE TABLE `staff` (
  `first_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `staff_id` int(11) NOT NULL,
  `last_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `email` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `phone_number` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `position` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `salary` decimal(10,0) DEFAULT NULL,
  `date_hired` date DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `department` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `staff`
--

INSERT INTO `staff` (`first_name`, `staff_id`, `last_name`, `email`, `phone_number`, `position`, `salary`, `date_hired`, `status`, `department`) VALUES
('Pozsgai', 1, 'Marcell', 'pozsgaim@kkszki.hu', '+36 2013747285', 'Tomo arus', '-1000', '2025-02-21', 'Aktív', 'Tomo king'),
('Jánosi', 2, 'Marcell', 'janosim@kkszki.hu', '+36 2019855222', 'Hotel Manager', '10000000', '2025-02-21', 'Inaktív', 'Manager'),
('Zalán', 5, 'Zelenák', 'zelenakz@kkszki.hu', '+36 209548099', 'Karbantartó', '100000000', '2025-02-25', 'Szabadságon', 'IT');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `taxrates`
--

CREATE TABLE `taxrates` (
  `tax_rate_id` int(11) NOT NULL,
  `tax_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `rate` decimal(10,0) DEFAULT NULL,
  `effective_date` date DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `country` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `state` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `city` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `payment_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `useraccounts`
--

CREATE TABLE `useraccounts` (
  `username` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `password` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `email` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `role` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `RefreshToken` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `RefreshTokenExpiryTime` date DEFAULT NULL,
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_created` date DEFAULT NULL,
  `last_login` date DEFAULT NULL,
  `date_updated` date DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `authenticationcode` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `authenticationexpire` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `useraccounts`
--

INSERT INTO `useraccounts` (`username`, `user_id`, `password`, `email`, `role`, `RefreshToken`, `RefreshTokenExpiryTime`, `status`, `date_created`, `last_login`, `date_updated`, `notes`, `authenticationcode`, `authenticationexpire`) VALUES
('Tahil', 3, 'hte+RnleAunUji+Bx3f7EPd8Nd2nOw82PS50E6kPBYOBYQ+8JUKlArMigzjZ1CM3', 'hiloczkit@kkszki.hu', 'System', 'OMPldv1Ly9+hEyF2hIEoAXEHpWJhBg2gtr216ykVIJM=', '2025-02-21', 'string', '2025-01-16', '2025-01-16', '2025-01-16', 'string', '111111', '2025-02-13'),
('asdasdasd', 4, '3S4JbEmjI0P69HENXf0Wp+u8teCyLgUPrKSYOMLRv91+ixql4MlfM4TWgeaJLIU/', 'hiloczkit12@kkszki.hu', 'Base', 'qiNaBxchix/fw5p2I6Bq0odyQmSo0CmQawB6MiPqVFc=', '2025-01-23', 'string', '2025-01-16', '2025-01-16', '2025-01-16', 'string', '111111', '2025-02-13'),
('a_Beto', 5, 'u04oCPhO+K7Y9IBD+zsk/QP/jWnVhlEdpOyaWFAzwQjPvc0kubpehqBt15MLXuVv', 'monostorir@kkszki.hu', 'System', 'GYmdQY9OZuKtolofd3QfQD5ausXAkECx3+6CJqUGRFM=', '2025-03-24', 'string', '2025-02-14', '2025-02-14', '2025-02-14', 'string', 'activated', '2025-02-20'),
('Bozsgai', 6, '48RL9zaxFrXNx3WQWwmErDml2gjVS/8N8ess8G65a4mMWb4VBkZqDJDsEki62YpY', 'monostori@kkszki.hu', 'Base', NULL, NULL, NULL, '2025-02-21', '2025-02-21', '2025-02-21', NULL, 'activated', '2025-02-21'),
('LoriAttila', 7, '92SGRkfIVMvNl6lr+bRfxUldVRoplekiF+3r/jgeepFxB/Ocq9d9egROEekm38P/', 'nokivagyok69420@gmail.com', 'Base', '6IAshvkZE4nL86EQQXMhOzTJCKchbWQtHGbU9AD4Ebk=', '2025-03-19', NULL, '2025-03-12', '2025-03-12', '2025-03-12', NULL, 'activated', NULL);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `amenities`
--
ALTER TABLE `amenities`
  ADD PRIMARY KEY (`amenity_id`),
  ADD KEY `Amenities_fk5` (`room_id`);

--
-- A tábla indexei `bookings`
--
ALTER TABLE `bookings`
  ADD PRIMARY KEY (`booking_id`),
  ADD KEY `Bookings_fk0` (`room_id`),
  ADD KEY `Bookings_fk2` (`guest_id`);

--
-- A tábla indexei `eventbookings`
--
ALTER TABLE `eventbookings`
  ADD PRIMARY KEY (`event_booking_id`),
  ADD KEY `EventBookings_fk1` (`event_id`),
  ADD KEY `EventBookings_fk2` (`guest_id`);

--
-- A tábla indexei `events`
--
ALTER TABLE `events`
  ADD PRIMARY KEY (`event_id`);

--
-- A tábla indexei `feedback`
--
ALTER TABLE `feedback`
  ADD PRIMARY KEY (`feedback_id`),
  ADD KEY `Feedback_fk9` (`guest_id`);

--
-- A tábla indexei `guests`
--
ALTER TABLE `guests`
  ADD PRIMARY KEY (`guest_id`),
  ADD KEY `fk_guests_useraccounts` (`user_id`);

--
-- A tábla indexei `invoices`
--
ALTER TABLE `invoices`
  ADD PRIMARY KEY (`invoice_id`),
  ADD KEY `Invoices_fk2` (`booking_id`);

--
-- A tábla indexei `loyaltyprograms`
--
ALTER TABLE `loyaltyprograms`
  ADD PRIMARY KEY (`loyalty_program_id`);

--
-- A tábla indexei `marketing`
--
ALTER TABLE `marketing`
  ADD PRIMARY KEY (`marketing_id`);

--
-- A tábla indexei `notifications`
--
ALTER TABLE `notifications`
  ADD PRIMARY KEY (`notification_id`),
  ADD KEY `Notifications_fk8` (`user_id`);

--
-- A tábla indexei `payments`
--
ALTER TABLE `payments`
  ADD PRIMARY KEY (`payment_id`),
  ADD KEY `Payments_fk1` (`booking_id`);

--
-- A tábla indexei `promotions`
--
ALTER TABLE `promotions`
  ADD PRIMARY KEY (`promotion_id`),
  ADD KEY `Promotions_fk7` (`room_id`);

--
-- A tábla indexei `reviews`
--
ALTER TABLE `reviews`
  ADD PRIMARY KEY (`review_id`),
  ADD KEY `Reviews_fk2` (`guest_id`),
  ADD KEY `Reviews_fk3` (`room_id`);

--
-- A tábla indexei `roominventory`
--
ALTER TABLE `roominventory`
  ADD PRIMARY KEY (`inventory_id`),
  ADD KEY `RoomInventory_fk8` (`room_id`);

--
-- A tábla indexei `roommaintenance`
--
ALTER TABLE `roommaintenance`
  ADD PRIMARY KEY (`maintenance_id`),
  ADD KEY `RoomMaintenance_fk1` (`room_id`),
  ADD KEY `RoomMaintenance_fk5` (`staff_id`);

--
-- A tábla indexei `rooms`
--
ALTER TABLE `rooms`
  ADD PRIMARY KEY (`room_id`);

--
-- A tábla indexei `roomtypes`
--
ALTER TABLE `roomtypes`
  ADD PRIMARY KEY (`room_type_id`);

--
-- A tábla indexei `services`
--
ALTER TABLE `services`
  ADD PRIMARY KEY (`service_id`);

--
-- A tábla indexei `staff`
--
ALTER TABLE `staff`
  ADD PRIMARY KEY (`staff_id`);

--
-- A tábla indexei `taxrates`
--
ALTER TABLE `taxrates`
  ADD PRIMARY KEY (`tax_rate_id`),
  ADD KEY `payment` (`payment_id`);

--
-- A tábla indexei `useraccounts`
--
ALTER TABLE `useraccounts`
  ADD PRIMARY KEY (`user_id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `amenities`
--
ALTER TABLE `amenities`
  MODIFY `amenity_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT a táblához `bookings`
--
ALTER TABLE `bookings`
  MODIFY `booking_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT a táblához `eventbookings`
--
ALTER TABLE `eventbookings`
  MODIFY `event_booking_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `events`
--
ALTER TABLE `events`
  MODIFY `event_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT a táblához `feedback`
--
ALTER TABLE `feedback`
  MODIFY `feedback_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `guests`
--
ALTER TABLE `guests`
  MODIFY `guest_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT a táblához `invoices`
--
ALTER TABLE `invoices`
  MODIFY `invoice_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `loyaltyprograms`
--
ALTER TABLE `loyaltyprograms`
  MODIFY `loyalty_program_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `marketing`
--
ALTER TABLE `marketing`
  MODIFY `marketing_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `notifications`
--
ALTER TABLE `notifications`
  MODIFY `notification_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `payments`
--
ALTER TABLE `payments`
  MODIFY `payment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `promotions`
--
ALTER TABLE `promotions`
  MODIFY `promotion_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT a táblához `reviews`
--
ALTER TABLE `reviews`
  MODIFY `review_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `roominventory`
--
ALTER TABLE `roominventory`
  MODIFY `inventory_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `roommaintenance`
--
ALTER TABLE `roommaintenance`
  MODIFY `maintenance_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT a táblához `rooms`
--
ALTER TABLE `rooms`
  MODIFY `room_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT a táblához `roomtypes`
--
ALTER TABLE `roomtypes`
  MODIFY `room_type_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `services`
--
ALTER TABLE `services`
  MODIFY `service_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `staff`
--
ALTER TABLE `staff`
  MODIFY `staff_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `taxrates`
--
ALTER TABLE `taxrates`
  MODIFY `tax_rate_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `useraccounts`
--
ALTER TABLE `useraccounts`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `amenities`
--
ALTER TABLE `amenities`
  ADD CONSTRAINT `Amenities_fk5` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `bookings`
--
ALTER TABLE `bookings`
  ADD CONSTRAINT `Bookings_fk0` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `Bookings_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `eventbookings`
--
ALTER TABLE `eventbookings`
  ADD CONSTRAINT `EventBookings_fk1` FOREIGN KEY (`event_id`) REFERENCES `events` (`event_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `EventBookings_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `feedback`
--
ALTER TABLE `feedback`
  ADD CONSTRAINT `Feedback_fk9` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Megkötések a táblához `guests`
--
ALTER TABLE `guests`
  ADD CONSTRAINT `fk_guests_useraccounts` FOREIGN KEY (`user_id`) REFERENCES `useraccounts` (`user_id`) ON DELETE SET NULL ON UPDATE NO ACTION;

--
-- Megkötések a táblához `invoices`
--
ALTER TABLE `invoices`
  ADD CONSTRAINT `Invoices_fk2` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`booking_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `notifications`
--
ALTER TABLE `notifications`
  ADD CONSTRAINT `Notifications_fk8` FOREIGN KEY (`user_id`) REFERENCES `useraccounts` (`user_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `payments`
--
ALTER TABLE `payments`
  ADD CONSTRAINT `Payments_fk1` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`booking_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `promotions`
--
ALTER TABLE `promotions`
  ADD CONSTRAINT `Promotions_fk7` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `reviews`
--
ALTER TABLE `reviews`
  ADD CONSTRAINT `Reviews_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `Reviews_fk3` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `roominventory`
--
ALTER TABLE `roominventory`
  ADD CONSTRAINT `RoomInventory_fk8` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `roommaintenance`
--
ALTER TABLE `roommaintenance`
  ADD CONSTRAINT `RoomMaintenance_fk1` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `RoomMaintenance_fk5` FOREIGN KEY (`staff_id`) REFERENCES `staff` (`staff_id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Megkötések a táblához `taxrates`
--
ALTER TABLE `taxrates`
  ADD CONSTRAINT `taxrates_ibfk_1` FOREIGN KEY (`payment_id`) REFERENCES `payments` (`payment_id`) ON DELETE CASCADE ON UPDATE NO ACTION;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
