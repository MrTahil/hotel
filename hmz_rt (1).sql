-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2024. Nov 29. 13:56
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
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `event_id` int(11) NOT NULL,
  `event_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `event_date` date DEFAULT NULL,
  `location` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_added` date DEFAULT NULL,
  `organizer_name` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `contact_info` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

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
  `gender` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

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

--
-- A tábla adatainak kiíratása `notifications`
--

INSERT INTO `notifications` (`notification_id`, `date_sent`, `message`, `status`, `type`, `date_read`, `priority`, `notes`, `user_id`, `category`) VALUES
(880256125, '0001-01-01', 'JÓREGGELT', 'NotSent', 'Room', '0001-01-01', 5, 'A szobád készen áll az átvételre', 369521321, 'Push Notification');

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
  `images` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`images`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

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
  `city` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL
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
  `status` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `date_created` date DEFAULT NULL,
  `last_login` date DEFAULT NULL,
  `date_updated` date DEFAULT NULL,
  `notes` text COLLATE utf8mb4_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `useraccounts`
--

INSERT INTO `useraccounts` (`username`, `user_id`, `password`, `email`, `role`, `status`, `date_created`, `last_login`, `date_updated`, `notes`) VALUES
('mia99', 85140010, 'Mia45678', 'mia99@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '33'),
('scarlettR', 104104515, 'Scarlet1!', 'scarlett.r@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '27'),
('henryL90', 109292344, 'HenryL1!', 'henryl90@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '34'),
('lucasM', 116407224, 'Lucas789', 'lucas.m@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '32'),
('ryanH', 166509636, 'RyanH45!', 'ryanh@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '38'),
('madisonW', 172174168, 'Madison@1', 'madison.w@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '35'),
('loganS', 254642451, 'Logan@10', 'logan.s@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '37'),
('lily_f', 261452192, 'LilyF2021', 'lily.f@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '29'),
('ethanT', 342356854, 'Ethan2022', 'ethan.t@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '27'),
('emilyW', 361652309, 'Emily2023', 'emilyw@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '25'),
('Monostori Róbert', 369521321, '72585631097Gg', 'user@example.com', 'Base', 'Male', '2024-11-29', '2024-11-29', '2024-11-29', '65'),
('alex99', 383424155, 'Alex12345', 'alex99@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '34'),
('danielS', 467731913, 'Daniel88!', 'daniel.s@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '28'),
('graceG', 539321361, 'Grace@22', 'grace.g@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '26'),
('chloe_d', 646536147, 'Chloe@22', 'chloe.d@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '30'),
('jamesBond', 721295255, 'Bond007!', 'james.bond@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '45'),
('jackson55', 729526184, 'JackS5k!', 'jackson55@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '31'),
('sophiaR', 751157442, 'Sophia2024', 'sophiar@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '30'),
('ava.grace', 759131082, 'AvaGrace1', 'ava.grace@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '31'),
('benjaminH', 773538956, 'BenH2024', 'benjamin.h@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '36'),
('zoeyC99', 822745142, 'ZoeyC2021', 'zoeyc99@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '23'),
('harper_m', 826889265, 'Harper@12', 'harper.m@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '24'),
('davidKing', 839486451, 'David123!', 'david.king@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '40'),
('ella_b', 854462919, 'EllaB2021', 'ella.b@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '29'),
('lucy_d', 874466107, 'Lucy4567', 'lucy_d@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '28'),
('noahP77', 878876173, 'Noah1234', 'noahp77@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '29'),
('mike_90', 930618225, 'Mike1980', 'mike90@example.com', 'Base', 'male', '2024-11-29', '2024-11-29', '2024-11-29', '44'),
('evelynG', 987528127, 'Evelyn@5', 'evelyn.g@example.com', 'Base', 'female', '2024-11-29', '2024-11-29', '2024-11-29', '33');

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
  ADD PRIMARY KEY (`guest_id`);

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
  ADD PRIMARY KEY (`tax_rate_id`);

--
-- A tábla indexei `useraccounts`
--
ALTER TABLE `useraccounts`
  ADD PRIMARY KEY (`user_id`);

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `amenities`
--
ALTER TABLE `amenities`
  ADD CONSTRAINT `Amenities_fk5` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`);

--
-- Megkötések a táblához `bookings`
--
ALTER TABLE `bookings`
  ADD CONSTRAINT `Bookings_fk0` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`),
  ADD CONSTRAINT `Bookings_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`);

--
-- Megkötések a táblához `eventbookings`
--
ALTER TABLE `eventbookings`
  ADD CONSTRAINT `EventBookings_fk1` FOREIGN KEY (`event_id`) REFERENCES `events` (`event_id`),
  ADD CONSTRAINT `EventBookings_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`);

--
-- Megkötések a táblához `feedback`
--
ALTER TABLE `feedback`
  ADD CONSTRAINT `Feedback_fk9` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`);

--
-- Megkötések a táblához `invoices`
--
ALTER TABLE `invoices`
  ADD CONSTRAINT `Invoices_fk2` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`booking_id`);

--
-- Megkötések a táblához `notifications`
--
ALTER TABLE `notifications`
  ADD CONSTRAINT `Notifications_fk8` FOREIGN KEY (`user_id`) REFERENCES `useraccounts` (`user_id`);

--
-- Megkötések a táblához `payments`
--
ALTER TABLE `payments`
  ADD CONSTRAINT `Payments_fk1` FOREIGN KEY (`booking_id`) REFERENCES `bookings` (`booking_id`);

--
-- Megkötések a táblához `promotions`
--
ALTER TABLE `promotions`
  ADD CONSTRAINT `Promotions_fk7` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`);

--
-- Megkötések a táblához `reviews`
--
ALTER TABLE `reviews`
  ADD CONSTRAINT `Reviews_fk2` FOREIGN KEY (`guest_id`) REFERENCES `guests` (`guest_id`),
  ADD CONSTRAINT `Reviews_fk3` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`);

--
-- Megkötések a táblához `roominventory`
--
ALTER TABLE `roominventory`
  ADD CONSTRAINT `RoomInventory_fk8` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`);

--
-- Megkötések a táblához `roommaintenance`
--
ALTER TABLE `roommaintenance`
  ADD CONSTRAINT `RoomMaintenance_fk1` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`room_id`),
  ADD CONSTRAINT `RoomMaintenance_fk5` FOREIGN KEY (`staff_id`) REFERENCES `staff` (`staff_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
