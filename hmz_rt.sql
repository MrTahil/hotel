-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Már 10. 11:31
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
  `contact_info` varchar(255) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `images` longtext COLLATE utf8mb4_hungarian_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `events`
--

INSERT INTO `events` (`capacity`, `status`, `event_id`, `event_name`, `event_date`, `location`, `description`, `date_added`, `organizer_name`, `contact_info`, `images`) VALUES
(12, 'Available', 1, 'Lovaglás', '2025-03-15', 'Hotel melletti Remény farm', 'Betanított szelíd lovakkal megtapasztalhatják mint kicsik mint nagyok a lovaglást.', '2025-03-10', 'Vályi István', '+36703237436', '../img/lo.png'),
(2, 'Available', 2, 'Csillagászat', '2025-03-15', 'Hotel 10. emelete, Csillagászati szoba', 'Nagy értékű távcsvünkkel megtekinthetik a bolygónkat körülvevő égitesteket. ', '2025-03-10', 'Bartos Cs. István', '+36 70 323 7436', '../img/csillag.png'),
(1000000, 'Available', 3, 'Fesztivál', '2025-03-15', 'Budapest park', 'Haverokkal családdal zenésen szórakoznál? Akkor itt a helyed', '2025-03-10', 'Hilóczki Tamás', 'hiloczkit@kkszki.hu', '../img/fesztival.png'),
(15, 'Available', 4, 'Fotós Túra', '2025-03-13', 'Csanyik', 'Szeretsz túrázni és közben fotózni való látványosságokatlátni? akkor itt a helyed a túránkon, 10km séta, fizetett étkezés, folyadékpótlás elintézve, neked csak sétálni kell!', '2025-03-10', 'Monostori Róbert', 'monostorir@kkszki.hu', '../img/foto.png'),
(6, 'Available', 5, 'Főzőtanfolyam', '2025-03-11', 'Hotel földszint, pultnál bejelentkezik és elvezetik a konyhához', 'Meg szeretnéd a főzés csínyját bínyját tanulni? Vagy csak belekóstolnál jópár ország ízeibe? Gyere főzzünk eggyütt! ', '2025-03-10', 'Kovács Szabolcs', 'kovacssz@kkszki.hu', '../img/fozo.png'),
(4, 'Available', 6, 'Horgászat', '2025-03-22', 'Csorba-tó', 'Ha a barátaiddal vagy szeretteiddel szeretnél egy festői környezetben kikapcsolni, és a mesterséges tavunkban a nagyobbnál nagyobb fenevadakat szeretnéd kifogni, akkor bátran jelentkezz! Felszereléssel ellátunk, amit kérsz, kapsz!', '2025-03-10', 'Kovács Szabolcs', 'kovacssz@kkszki.hu', '../img/horgaszat.png'),
(15, 'Available', 7, 'Jóga Hegyen', '2025-03-11', 'Kékestető', 'Nyugott környezetben kiengedheted a csakrád egy igen képzett oktatóval, egy nagy lélegzetvétel magyarország legmagasabb pontján mindent jobbá tesz', '2025-03-10', 'Jánosi Marcell', 'janosim@kkszki.hu', '../img/jogahegyen.png'),
(10, 'Available', 8, 'Kalandpark', '2025-03-12', 'Factory Aréna', 'Eggyaránt kicsiknek és nagyoknak igen merész programmal álltunk elő melyben fák között ugrálhat mint egy mókus vagy nezán csúszhat le a hegyről mint egy pingvin', '2025-03-10', 'Sike Domonkos', 'siked@kkszki.hu', '../img/kalandpark.png'),
(50, 'Available', 9, 'Kézműves Piac', '2025-03-16', 'Zsarnai Piac', 'Minden hónap minden hetének vasárnapján kézműves piac van a Zsarnai piac szívében ahol ézileg elkészített kincseket lehet vásárolni, érdemes meglesni!', '2025-03-10', 'Ágoston Attila', 'agostona@kkszki.hu', '../img/kezmuves.png'),
(10, 'Available', 11, 'Múzeum Túra', '2025-03-11', 'Pannon Tenger Múzeum', 'Szeretnél betekintést nyerni a kőkorszakba, vagy netán a jégkorszakba, szó eshet minden kornak minden faja, élőlénye, környezete, mind egy helyen, lesd meg te is!', '2025-03-10', 'Pozsgai Marcell', 'pozsgaim@kkszki.hu', '../img/muzeum.png'),
(200, 'Available', 12, 'Téli Sportok', '2025-12-01', 'Kékestető', 'Téli Sport? Igen van... sielnél? snowboard? minden jöhet, jelentkezz, felszereléssel oktatással ellátunk, aztán suhanj!', '2025-03-10', 'Zelenák Zalán Noel', '+36 70 323 7436', '../img/sieles.png');

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `events`
--
ALTER TABLE `events`
  ADD PRIMARY KEY (`event_id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `events`
--
ALTER TABLE `events`
  MODIFY `event_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
