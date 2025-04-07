import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import DetailItem from "./DetailItem";

const ProfilePage = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [deletePassword, setDeletePassword] = useState("");
  const [showGuestModal, setShowGuestModal] = useState(false);
  const [guests, setGuests] = useState([]);
  const [guestData, setGuestData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phoneNumber: "",
    address: "",
    city: "",
    country: "",
    dateOfBirth: "",
    gender: "",
  });
  const [editGuestId, setEditGuestId] = useState(null);
  const [message, setMessage] = useState(null);
  const [bookings, setBookings] = useState([]);
  const [eventBookings, setEventBookings] = useState([]);
  const [events, setEvents] = useState([]); // Added to store event details
  const [selectedBooking, setSelectedBooking] = useState(null);
  const [showCommentModal, setShowCommentModal] = useState(false);
  const [commentData, setCommentData] = useState({
    rating: 5,
    comment: "",
  });
  const [passwordData, setPasswordData] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [passwordErrors, setPasswordErrors] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [showCurrentPassword, setShowCurrentPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [forgotEmail, setForgotEmail] = useState("");
  const [verificationCode, setVerificationCode] = useState("");
  const [newPasswordData, setNewPasswordData] = useState({
    newPassword: "",
    confirmPassword: "",
  });
  const [forgotStep, setForgotStep] = useState(1);
  const [showNewPasswordForgot, setShowNewPasswordForgot] = useState(false);
  const [showConfirmPasswordForgot, setShowConfirmPasswordForgot] = useState(false);

  const fetchRoomIdByNumber = async (roomNumber) => {
    try {
      const token = localStorage.getItem("authToken");
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Rooms/GetRoomWith`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error(`Nem sikerült lekérni a szobák adatait: ${response.status}`);
      }
      const rooms = await response.json();
      const room = rooms.find((r) => r.roomNumber === roomNumber);
      return room?.roomId || null;
    } catch (err) {
      console.error("Hiba a szoba azonosító lekérdezésekor:", err.message);
      return null;
    }
  };

  const fetchBookingDetails = async (bookingId) => {
    try {
      const token = localStorage.getItem("authToken");
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Bookings/GetBookingDetails/${bookingId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error("Nem sikerült lekérni a foglalás részleteit");
      }
      const data = await response.json();
      return data.roomId || data.room?.id;
    } catch (err) {
      console.error("Hiba a foglalás részleteinek lekérésekor:", err.message);
      return null;
    }
  };

  useEffect(() => {
    if (message) {
      const timer = setTimeout(() => setMessage(null), 3000);
      return () => clearTimeout(timer);
    }
  }, [message]);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const token = localStorage.getItem("authToken");
        if (!token) {
          throw new Error("Nincs token elmentve! Jelentkezz be újra.");
        }
        const response = await fetch(
          `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/GetOneUserData/${localStorage.getItem("username")}`,
          {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`,
              "Content-Type": "application/json",
            },
          }
        );
        if (response.status === 401) {
          throw new Error("Token érvénytelen vagy lejárt!");
        }
        if (!response.ok) {
          throw new Error(`HTTP hiba! Státusz: ${response.status}`);
        }
        const data = await response.json();
        setUser({
          username: data.username,
          email: data.email,
          profileImage:
            data.profileImage || "https://randomuser.me/api/portraits/men/42.jpg",
          userId: data.userId,
          role: data.role,
          dateCreated: new Date(data.dateCreated).toLocaleDateString("hu-HU"),
        });
      } catch (error) {
        console.error("Hiba történt:", error.message);
        setError(error.message);
        if (error.message.includes("token") || error.message.includes("401")) {
          navigate("/login");
        }
      } finally {
        setLoading(false);
      }
    };
    fetchUserData();
  }, [navigate]);

  const fetchBookings = async () => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token || !user?.userId) {
        setError("Hiányzó autentikációs adatok vagy felhasználó ID");
        return;
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Bookings/BookingsByUser/${user.userId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        if (response.status === 400) {
          setBookings([]);
          return;
        }
        throw new Error(`Nem sikerült lekérni a foglalásokat: ${response.status}`);
      }
      const data = await response.json();
      setBookings(data);
    } catch (err) {
      console.error("Hiba a foglalások lekérésekor:", err.message);
      setError(err.message);
      setBookings([]);
    }
  };

  const fetchEventBookings = async () => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Feedback/GetEventBookings`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error(`Nem sikerült lekérni a program foglalásokat: ${response.status}`);
      }
      const data = await response.json();
      console.log("Event Bookings Response:", data); // Log for debugging
      setEventBookings(data);
    } catch (err) {
      console.error("Hiba a program foglalások lekérésekor:", err.message);
      setError(err.message);
      setEventBookings([]);
    }
  };

  const fetchEvents = async () => {
    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Events/Geteents`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error(`Nem sikerült lekérni az eseményeket: ${response.status}`);
      }
      const data = await response.json();
      console.log("Events Response:", data); // Log for debugging
      setEvents(data);
    } catch (err) {
      console.error("Hiba az események lekérésekor:", err.message);
      setError(err.message);
      setEvents([]);
    }
  };

  useEffect(() => {
    if (user?.userId) {
      const loadData = async () => {
        await Promise.all([fetchBookings(), fetchEvents(), fetchEventBookings()]);
      };
      loadData();
    }
  }, [user?.userId]);

  const handleOpenDetails = async (booking) => {
    let roomId = booking.room?.id || booking.roomId || booking.RoomId || booking.room?.RoomId;
    if (!roomId && booking.roomNumber) {
      roomId = await fetchRoomIdByNumber(booking.roomNumber);
    }
    if (!roomId && (booking.id || booking.bookingId)) {
      roomId = await fetchBookingDetails(booking.id || booking.bookingId);
    }
    if (!roomId) {
      setMessage({ type: "error", text: "Nem található szoba azonosító a foglalásban!" });
      return;
    }
    setSelectedBooking({
      ...booking,
      BookingId: booking.id || booking.bookingId,
      RoomId: roomId,
      RoomNumber: booking.room?.number || booking.roomNumber || "N/A",
      CheckInDate: booking.startDate || booking.checkInDate || booking.dateFrom || "N/A",
      CheckOutDate: booking.endDate || booking.checkOutDate || booking.dateTo || "N/A",
      FirstName: booking.guest?.firstName || booking.firstName || "N/A",
      LastName: booking.guest?.lastName || booking.lastName || "N/A",
      FloorNumber: booking.room?.floor || booking.floorNumber || "N/A",
      RoomType: booking.room?.type || booking.roomType || "N/A",
      NumberOfGuests: booking.guestCount || booking.numberOfGuests || "N/A",
      TotalPrice: booking.price || booking.totalPrice || "N/A",
      PaymentStatus: booking.status || booking.paymentStatus || "N/A",
    });
  };

  const handleCloseDetails = () => {
    setSelectedBooking(null);
  };

  const handleDeleteBooking = async (bookingId) => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Bookings/DeleteBooking/${bookingId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error(`Hiba a foglalás törlése során: ${response.status}`);
      }
      setMessage({ type: "success", text: "Foglalás sikeresen törölve!" });
      fetchBookings();
    } catch (err) {
      console.error("Hiba a foglalás törlése közben:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleDeleteEventBooking = async (id) => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Feedback/DeleteBookingById/${id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error(`Hiba a program foglalás törlése során: ${response.status}`);
      }
      setMessage({ type: "success", text: "Program foglalás sikeresen törölve!" });
      fetchEventBookings();
    } catch (err) {
      console.error("Hiba a program foglalás törlése közben:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const fetchGuests = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem("authToken");
      const userId = user?.userId;
      if (!token || !userId) {
        setError("Hiányzó autentikációs adatok");
        return;
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Guests/GetGuestData/${user.username}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        if (response.status === 404) {
          setGuests([]);
          return;
        }
        throw new Error(`Nem sikerült lekérni a vendégeket: ${response.status}`);
      }
      const data = await response.json();
      setGuests(Array.isArray(data) ? data : [data]);
    } catch (err) {
      console.error("Hiba a vendéglista lekérésekor:", err.message);
      setError(err.message);
      setGuests([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user?.userId) {
      fetchGuests();
    }
  }, [user?.userId]);

  const handleChangePassword = async (e) => {
    e.preventDefault();
    setPasswordErrors({ currentPassword: "", newPassword: "", confirmPassword: "" });
    setMessage(null);
    try {
      const token = localStorage.getItem("authToken");
      const username = localStorage.getItem("username");
      const passwordRegex =
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{}|;:,.<>?/~`])[A-Za-z\d!@#$%^&*()_+\-=\[\]{}|;:,.<>?/~`]{8,}$/;
      if (!token || !username) {
        throw new Error("Hiányzó autentikációs adatok");
      }
      if (passwordData.newPassword !== passwordData.confirmPassword) {
        setPasswordErrors((prev) => ({
          ...prev,
          confirmPassword: "Az új jelszavak nem egyeznek!",
        }));
        return;
      }
      if (!passwordRegex.test(passwordData.newPassword)) {
        setPasswordErrors((prev) => ({
          ...prev,
          newPassword:
            "Az új jelszónak legalább 8 karakterből kell állnia, és tartalmaznia kell kisbetűt, nagybetűt, számot és speciális karaktert (!@#$%^&*?)!",
        }));
        return;
      }
      const { response, data } = await handleApiCall(
        `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/Newpasswithknownpass/${username}`,
        "PUT",
        {
          OldPassword: passwordData.currentPassword,
          Password: passwordData.newPassword,
        }
      );
      if (!response.ok) {
        if (
          data.message &&
          (data.message.includes("jelenlegi jelszó") ||
            data.message.includes("Incorrect password"))
        ) {
          setPasswordErrors((prev) => ({
            ...prev,
            currentPassword: "Helytelen jelenlegi jelszó!",
          }));
        } else {
          throw new Error(
            data.message || `Hiba a jelszó változtatásakor: ${response.status}`
          );
        }
        return;
      }
      setMessage({ type: "success", text: "Jelszó sikeresen megváltoztatva!" });
      setPasswordData({
        currentPassword: "",
        newPassword: "",
        confirmPassword: "",
      });
    } catch (err) {
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleForgotPasswordStep1 = async (e) => {
    e.preventDefault();
    setMessage(null);
    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/ForgotPasswordsendemail/${forgotEmail}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      if (response.status === 418) {
        throw new Error("Ez az email cím nem szerepel az adatbázisban!");
      }
      if (!response.ok) {
        throw new Error("Hiba történt a jelszó-visszaállítási kérelem során!");
      }
      setMessage({
        type: "success",
        text: "Ellenőrizze email fiókját a 6 számjegyű kódért!",
      });
      setForgotStep(2);
    } catch (err) {
      console.error("Hiba a kód küldése során:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleForgotPasswordStep2 = async (e) => {
    e.preventDefault();
    setMessage(null);
    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/VerifyTheforgotpass`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            Email: forgotEmail,
            Code: verificationCode,
          }),
        }
      );
      if (response.status === 202) {
        throw new Error("Hibás kód vagy email!");
      }
      if (!response.ok) {
        throw new Error(`Hiba az ellenőrzés során: ${response.status}`);
      }
      setMessage({ type: "success", text: "Kód elfogadva!" });
      setForgotStep(3);
    } catch (err) {
      console.error("Hiba az ellenőrzés során:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleForgotPasswordStep3 = async (e) => {
    e.preventDefault();
    setPasswordErrors({ newPassword: "", confirmPassword: "" });
    setMessage(null);
    try {
      const passwordRegex =
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{}|;:,.<>?/~`])[A-Za-z\d!@#$%^&*()_+\-=\[\]{}|;:,.<>?/~`]{8,}$/;
      if (newPasswordData.newPassword !== newPasswordData.confirmPassword) {
        setPasswordErrors((prev) => ({
          ...prev,
          confirmPassword: "Az új jelszavak nem egyeznek!",
        }));
        return;
      }
      if (!passwordRegex.test(newPasswordData.newPassword)) {
        setPasswordErrors((prev) => ({
          ...prev,
          newPassword:
            "Az új jelszónak legalább 8 karakterből kell állnia, és tartalmaznia kell kisbetűt, nagybetűt, számot és speciális karaktert (!@#$%^&*?)!",
        }));
        return;
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/SetNewPassword`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            Email: forgotEmail,
            Password: newPasswordData.newPassword,
          }),
        }
      );
      const data = await response.json();
      if (!response.ok) {
        throw new Error(
          data.message || "Hiba történt az új jelszó beállításakor!"
        );
      }
      setMessage({
        type: "success",
        text:
          data.message ||
          "Jelszó sikeresen visszaállítva! Jelentkezzen be az új jelszóval.",
      });
      setForgotEmail("");
      setVerificationCode("");
      setNewPasswordData({ newPassword: "", confirmPassword: "" });
      setForgotStep(1);
    } catch (err) {
      console.error("Hiba az új jelszó beállításakor:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleDeleteAccount = async () => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      const username = localStorage.getItem("username");
      if (!username) {
        throw new Error("Nincs felhasználónév elmentve!");
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/DeleteUserByUsername/${username}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ Password: deletePassword }),
        }
      );
      if (response.status === 401) {
        throw new Error("Token érvénytelen vagy lejárt!");
      }
      if (response.status === 404) {
        throw new Error("Felhasználó nem található!");
      }
      if (!response.ok) {
        const errorData = await response.text();
        throw new Error(
          errorData || `HTTP hiba! Státusz: ${response.status}`
        );
      }
      localStorage.removeItem("authToken");
      localStorage.removeItem("username");
      navigate("/");
    } catch (error) {
      console.error("Hiba történt a fiók törlése közben:", error.message);
      setError(error.message);
    }
  };

  const handleApiCall = async (url, method, body, requiresAuth = true) => {
    const headers = {
      "Content-Type": "application/json",
    };
    if (requiresAuth) {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Authentication token missing");
      }
      headers["Authorization"] = `Bearer ${token}`;
    }
    const response = await fetch(url, {
      method,
      headers,
      body: body ? JSON.stringify(body) : undefined,
    });
    const responseClone = response.clone();
    try {
      const data = await response.json();
      return { response, data };
    } catch (error) {
      const text = await responseClone.text();
      return { response, data: { message: text } };
    }
  };

  const handleAddGuest = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        setMessage({ type: "error", text: "Nincs token elmentve!" });
        return;
      }
      if (!user?.userId) {
        setMessage({ type: "error", text: "Felhasználó ID hiányzik!" });
        return;
      }
      if (!guestData.firstName || !guestData.lastName || !guestData.dateOfBirth) {
        setMessage({ type: "error", text: "Kötelező mezők hiányoznak!" });
        return;
      }
      const payload = {
        ...guestData,
        userId: user.userId,
        dateOfBirth: guestData.dateOfBirth
          ? new Date(guestData.dateOfBirth).toISOString()
          : null,
      };
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Guests/Addnewguest`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify(payload),
        }
      );
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Hiba a vendég hozzáadása során: ${errorText}`);
      }
      const responseText = await response.text();
      if (responseText.includes("Sikeres")) {
        await fetchGuests();
        setShowGuestModal(false);
        setGuestData({
          firstName: "",
          lastName: "",
          email: "",
          phoneNumber: "",
          address: "",
          city: "",
          country: "",
          dateOfBirth: "",
          gender: "",
        });
        setMessage({ type: "success", text: "Vendég sikeresen hozzáadva!" });
      } else {
        throw new Error(`Váratlan válasz a szervertől: ${responseText}`);
      }
    } catch (err) {
      console.error("Hiba a vendég hozzáadása közben:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleDeleteGuest = async (guestId) => {
    try {
      if (!guestId) {
        throw new Error("A vendég azonosítója (guestId) hiányzik!");
      }
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Guests/DeleteGuest/${guestId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        let errorText;
        try {
          errorText = await response.text();
        } catch (e) {
          errorText = `Hiba a válasz olvasása közben: ${response.status}`;
        }
        throw new Error(`Hiba a vendég törlése során: ${errorText}`);
      }
      await fetchGuests();
      setMessage({ type: "success", text: "Vendég sikeresen törölve!" });
    } catch (err) {
      console.error("Hiba a vendég törlése közben:", err.message);
      setError(err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleEditGuest = (guest) => {
    setGuestData({
      firstName: guest.firstName,
      lastName: guest.lastName,
      email: guest.email || "",
      phoneNumber: guest.phoneNumber || "",
      address: guest.address || "",
      city: guest.city || "",
      country: guest.country || "",
      dateOfBirth: guest.dateOfBirth
        ? new Date(guest.dateOfBirth).toISOString().split("T")[0]
        : "",
      gender: guest.gender || "",
    });
    setEditGuestId(guest.guestId);
    setShowGuestModal(true);
  };

  const handleUpdateGuest = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      if (!editGuestId) {
        throw new Error("Nincs kiválasztott vendég a szerkesztéshez!");
      }
      const payload = {
        ...guestData,
        dateOfBirth: guestData.dateOfBirth
          ? new Date(guestData.dateOfBirth).toISOString()
          : null,
      };
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Guests/UpdateGuest/${editGuestId}`,
        {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify(payload),
        }
      );
      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Hiba a vendég módosítása során: ${errorText}`);
      }
      const responseText = await response.text();
      if (responseText.includes("Sikeres frissítés")) {
        await fetchGuests();
        setShowGuestModal(false);
        setGuestData({
          firstName: "",
          lastName: "",
          email: "",
          phoneNumber: "",
          address: "",
          city: "",
          country: "",
          dateOfBirth: "",
          gender: "",
        });
        setEditGuestId(null);
        setMessage({ type: "success", text: "Vendég sikeresen módosítva!" });
      } else {
        throw new Error(`Váratlan válasz a szervertől: ${responseText}`);
      }
    } catch (err) {
      console.error("Hiba a vendég módosítása közben:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  const handleLogout = () => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("username");
    setUser(null);
    navigate("/");
  };

  const handleSubmitComment = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      if (guests.length === 0) {
        throw new Error("Nincs vendég hozzáadva! Kérjük, adjon hozzá egy vendéget először.");
      }
      const roomId = selectedBooking.RoomId;
      const bookingId = selectedBooking.BookingId;
      if (!roomId) {
        throw new Error("A szoba azonosítója (RoomId) hiányzik az adatokból!");
      }
      if (!bookingId) {
        throw new Error("A foglalás azonosítója (BookingId) hiányzik az adatokból!");
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Reviews/NewComment/${roomId}`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            BookingId: bookingId,
            GuestId: guests[0].guestId,
            Rating: commentData.rating,
            Comment: commentData.comment,
          }),
        }
      );
      if (!response.ok) {
        let errorMessage = "Hiba történt az értékelés küldése közben";
        try {
          const errorData = await response.json();
          errorMessage = errorData.message || errorMessage;
        } catch (jsonError) {
          console.error("JSON parsing error:", jsonError);
        }
        throw new Error(`${errorMessage} (Státusz: ${response.status})`);
      }
      setMessage({ type: "success", text: "Értékelés sikeresen elküldve!" });
      setShowCommentModal(false);
      setCommentData({ rating: 5, comment: "" });
    } catch (err) {
      console.error("Hiba az értékelés küldése közben:", err.message);
      setMessage({ type: "error", text: err.message });
    }
  };

  if (loading) {
    return <div className="bg-blue-50 min-h-screen p-6 sm:p-8">Betöltés...</div>;
  }

  return (
    <div className="bg-blue-50 min-h-screen p-6 sm:p-8 relative">
      {message && (
        <div
          className={`fixed top-4 right-4 p-4 rounded-lg shadow-lg text-white ${message.type === "success" ? "bg-green-500" : "bg-red-500"}`}
        >
          {message.text}
        </div>
      )}
      <div className="max-w-4xl mx-auto">
        <div className="flex flex-col sm:flex-row sm:items-center mb-6 gap-4">
          <button
            onClick={() => window.history.back()}
            className="flex items-center bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            <span className="material-symbols-outlined mr-2">arrow_back</span>
            Vissza a főoldalra
          </button>
          <h1 className="text-2xl sm:text-3xl font-bold text-blue-800">
            Profil kezelése
          </h1>
          <button
            onClick={handleLogout}
            className="flex items-center bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors"
          >
            <span className="material-symbols-outlined mr-2">logout</span>
            Kijelentkezés
          </button>
        </div>
        {error && <p className="text-red-600 mb-4">{error}</p>}
        {user && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6 md:gap-8 mb-8">
            <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 border border-blue-100">
              <div className="flex items-center mb-4">
                <span className="material-symbols-outlined text-3xl sm:text-4xl mr-3 sm:mr-4 bg-blue-100 p-2 sm:p-3 rounded-full text-blue-600">
                  person
                </span>
                <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">
                  Profilom
                </h2>
              </div>
              <div className="flex flex-col items-center mb-6">
                <div className="relative group mb-4">
                  <div className="w-24 h-24 sm:w-32 sm:h-32 rounded-full overflow-hidden border-4 border-blue-100 group-hover:border-blue-300 transition-all duration-300">
                    <img
                      src={user.profileImage}
                      alt="Profilkép"
                      className="w-full h-full object-cover"
                    />
                  </div>
                </div>
                <h3 className="text-lg sm:text-xl font-semibold text-blue-800 mb-1">
                  {user.username}
                </h3>
                <p className="text-xs sm:text-sm text-blue-600 mb-4">
                  Regisztrált: {user.dateCreated}
                </p>
              </div>
              <div className="space-y-4">
                <div className="border-t border-blue-100 pt-4">
                  <label className="block text-sm font-medium mb-1 text-blue-800">
                    Felhasználónév
                  </label>
                  <div className="px-3 sm:px-4 py-2 bg-blue-50 rounded-lg text-blue-700 text-sm sm:text-base">
                    {user.username}
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1 text-blue-800">
                    Email cím
                  </label>
                  <div className="px-3 sm:px-4 py-2 bg-blue-50 rounded-lg text-blue-700 text-sm sm:text-base break-all">
                    {user.email}
                  </div>
                </div>
              </div>
            </div>
            <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 border border-blue-100">
              <div className="flex flex-col gap-3 mb-4">
                <div className="flex items-center">
                  <span className="material-symbols-outlined text-3xl sm:text-4xl mr-3 sm:mr-4 bg-cyan-100 p-2 sm:p-3 rounded-full text-cyan-600">
                    bookmark
                  </span>
                  <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">
                    Elmentett vendég adatok
                  </h2>
                </div>
                <button
                  onClick={() => setShowGuestModal(true)}
                  className="flex items-center justify-center bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transform hover:scale-105 transition-all duration-200 w-full md:w-auto"
                >
                  <span className="material-symbols-outlined mr-2">
                    person_add
                  </span>
                  Vendég hozzáadása
                </button>
              </div>
              <div className="space-y-4">
                {guests.length > 0 ? (
                  guests.map((guest) => (
                    <div
                      key={guest.guestId}
                      className="border border-blue-100 rounded-lg p-3 sm:p-4 hover:border-blue-300 transition-all bg-blue-50/50 hover:shadow-md"
                    >
                      <h3 className="font-semibold mb-2 text-blue-800">
                        {guest.firstName} {guest.lastName}
                      </h3>
                      <p className="text-xs sm:text-sm text-blue-700 mb-1">
                        Email: {guest.email || "Nincs megadva"}
                      </p>
                      <p className="text-xs sm:text-sm text-blue-700 mb-3">
                        Telefon: {guest.phoneNumber || "Nincs megadva"}
                      </p>
                      <div className="flex flex-wrap gap-2">
                        <button
                          onClick={() => handleEditGuest(guest)}
                          className="px-3 py-1 bg-blue-100 text-blue-600 rounded hover:bg-blue-200 transition-colors flex items-center text-sm"
                        >
                          <span className="material-symbols-outlined text-sm mr-1">
                            edit
                          </span>
                          Szerkesztés
                        </button>
                        <button
                          onClick={() => handleDeleteGuest(guest.guestId)}
                          className="px-3 py-1 bg-red-100 text-red-600 rounded hover:bg-red-200 transition-colors flex items-center text-sm"
                        >
                          <span className="material-symbols-outlined text-sm mr-1">
                            delete
                          </span>
                          Törlés
                        </button>
                      </div>
                    </div>
                  ))
                ) : (
                  <p className="text-blue-700">Nincsenek mentett vendégek.</p>
                )}
              </div>
            </div>
          </div>
        )}
        {user && (
          <div className="bg-white p-4 sm:p-6 rounded-xl shadow-lg mb-6 sm:mb-8 hover:shadow-2xl transition-all duration-300 border border-blue-200 transform hover:-translate-y-1">
            <div className="flex items-center mb-6">
              <span className="material-symbols-outlined text-4xl sm:text-5xl mr-4 bg-teal-100 p-3 rounded-full text-teal-600 animate-pulse">
                event
              </span>
              <h2 className="text-2xl sm:text-3xl font-bold text-blue-800 bg-gradient-to-r from-blue-600 to-teal-600 bg-clip-text text-transparent">
                Foglalásaim
              </h2>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
              {bookings.length > 0 ? (
                bookings.map((booking, index) => {
                  const roomNumber =
                    booking.room?.number || booking.roomNumber || "N/A";
                  const checkInDate =
                    booking.startDate ||
                    booking.checkInDate ||
                    booking.dateFrom ||
                    "N/A";
                  const checkOutDate =
                    booking.endDate ||
                    booking.checkOutDate ||
                    booking.dateTo ||
                    "N/A";
                  const firstName =
                    booking.guest?.firstName || booking.firstName || "N/A";
                  const lastName =
                    booking.guest?.lastName || booking.lastName || "N/A";
                  const floorNumber =
                    booking.room?.floor || booking.floorNumber || "N/A";
                  const roomType =
                    booking.room?.type || booking.roomType || "N/A";
                  const numberOfGuests =
                    booking.guestCount || booking.numberOfGuests || "N/A";
                  const totalPrice = booking.price || booking.totalPrice || "N/A";
                  const paymentStatus =
                    booking.status || booking.paymentStatus || "N/A";
                  return (
                    <div
                      key={booking.id || booking.bookingísmoId || `booking-${index}`}
                      className="border border-blue-2 rounded-xl p-4 bg-gradient-to-br from-blue-50 to-teal-50 hover:border-teal-400 transition-all duration-300 shadow-md hover:shadow-xl"
                    >
                      <div className="flex justify-between items-center mb-3">
                        <div>
                          <p className="text-sm font-medium text-blue-700">
                            Szobaszám:{" "}
                            <span className="font-bold text-teal-600">
                              {roomNumber}
                            </span>
                          </p>
                          <p className="text-sm font-medium text-blue-700">
                            Időpont:{" "}
                            <span className="font-bold text-teal-600">
                              {checkInDate !== "N/A" && checkOutDate !== "N/A"
                                ? `${new Date(
                                  checkInDate
                                ).toLocaleDateString("hu-HU")} - ${new Date(
                                  checkOutDate
                                ).toLocaleDateString("hu-HU")}`
                                : "N/A"}
                            </span>
                          </p>
                        </div>
                        <button
                          onClick={() =>
                            handleOpenDetails({
                              ...booking,
                              RoomNumber: roomNumber,
                              CheckInDate: checkInDate,
                              CheckOutDate: checkOutDate,
                              FirstName: firstName,
                              LastName: lastName,
                              FloorNumber: floorNumber,
                              RoomType: roomType,
                              NumberOfGuests: numberOfGuests,
                              TotalPrice: totalPrice,
                              PaymentStatus: paymentStatus,
                            })
                          }
                          className="bg-blue-500 text-white px-3 py-1 rounded-full text-sm font-semibold hover:bg-blue-600 transform hover:scale-105 transition-all duration-200 shadow-md"
                        >
                          Részletek
                        </button>
                      </div>
                    </div>
                  );
                })
              ) : (
                <p className="text-blue-700 col-span-full">
                  Nincsenek foglalásaid.
                </p>
              )}
            </div>
          </div>
        )}
        {user && (
          <div className="bg-white p-4 sm:p-6 rounded-xl shadow-lg mb-6 sm:mb-8 hover:shadow-2xl transition-all duration-300 border border-blue-200 transform hover:-translate-y-1">
            <div className="flex items-center mb-6">
              <span className="material-symbols-outlined text-4xl sm:text-5xl mr-4 bg-purple-100 p-3 rounded-full text-purple-600 animate-pulse">
                celebration
              </span>
              <h2 className="text-2xl sm:text-3xl font-bold text-blue-800 bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent">
                Program foglalásaim
              </h2>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
              {eventBookings.length > 0 ? (
                eventBookings.map((booking) => {
                  const event = events.find((e) => e.eventId === booking.eventId); // Match booking to event
                  return (
                    <div
                      key={booking.eventBookingId}
                      className="border border-blue-200 rounded-xl p-4 bg-gradient-to-br from-purple-50 to-blue-50 hover:border-purple-400 transition-all duration-300 shadow-md hover:shadow-xl"
                    >
                      <div className="flex justify-between items-center mb-3">
                        <div>
                          <p className="text-sm font-medium text-blue-700">
                            Program neve:{" "}
                            <span className="font-bold text-purple-600">
                              {event ? event.eventName : `Esemény #${booking.eventId}`}
                            </span>
                          </p>
                          <p className="text-sm font-medium text-blue-700">
                            Dátum:{" "}
                            <span className="font-bold text-purple-600">
                              {event && event.eventDate
                                ? new Date(event.eventDate).toLocaleDateString("hu-HU")
                                : "Nincs adat"}
                            </span>
                          </p>
                          <p className="text-sm font-medium text-blue-700">
                            Helyszín:{" "}
                            <span className="font-bold text-purple-600">
                              {event ? event.location : "Nincs adat"}
                            </span>
                          </p>
                          <p className="text-sm font-medium text-blue-700">
                            Státusz:{" "}
                            <span className="font-bold text-purple-600">
                              {booking.status || "Nincs adat"}
                            </span>
                          </p>
                          <p className="text-sm font-medium text-blue-700">
                            Jegyek száma:{" "}
                            <span className="font-bold text-purple-600">
                              {booking.numberOfTickets || "Nincs adat"}
                            </span>
                          </p>
                        </div>
                        <button
                          onClick={() => handleDeleteEventBooking(booking.eventBookingId)}
                          className="bg-red-500 text-white px-3 py-1 rounded-full text-sm font-semibold hover:bg-red-600 transform hover:scale-105 transition-all duration-200 shadow-md"
                        >
                          Törlés
                        </button>
                      </div>
                    </div>
                  );
                })
              ) : (
                <p className="text-blue-700 col-span-full">
                  Nincsenek program foglalásaid.
                </p>
              )}
            </div>
          </div>
        )}
        {selectedBooking && (
          <div
            className="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center z-50 p-2 sm:p-4 backdrop-blur-sm"
            onClick={() => setSelectedBooking(null)}
          >
            <div
              className="bg-white rounded-xl sm:rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto transform transition-all duration-300 ease-out"
              onClick={(e) => e.stopPropagation()}
            >
              <div className="bg-gradient-to-r from-teal-500 to-blue-600 p-4 sm:p-6 text-white sticky top-0 z-10">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="text-xl sm:text-2xl font-bold">Foglalás részletei</h3>
                    <p className="text-teal-100 mt-1 text-sm sm:text-base">
                      #{selectedBooking.BookingId}
                    </p>
                  </div>
                  <button
                    onClick={() => setSelectedBooking(null)}
                    className="text-white hover:text-teal-200 transition-colors p-1"
                    aria-label="Bezárás"
                  >
                    <span className="material-symbols-outlined text-2xl sm:text-3xl">
                      close
                    </span>
                  </button>
                </div>
              </div>

              <div className="p-4 sm:p-6 space-y-4 sm:space-y-5">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4 sm:gap-5">
                  <div className="space-y-3">
                    <DetailItem
                      label="Szobaszám"
                      value={selectedBooking.RoomNumber || "N/A"}
                      icon="meeting_room"
                    />
                    <DetailItem
                      label="Időpont"
                      value={
                        selectedBooking.CheckInDate && selectedBooking.CheckOutDate
                          ? `${new Date(selectedBooking.CheckInDate).toLocaleDateString("hu-HU")} - ${new Date(selectedBooking.CheckOutDate).toLocaleDateString("hu-HU")}`
                          : "N/A"
                      }
                      icon="date_range"
                    />
                    <DetailItem
                      label="Vendégek száma"
                      value={`${selectedBooking.NumberOfGuests || "N/A"} fő`}
                      icon="group"
                    />
                  </div>
                  <div className="space-y-3">
                    <DetailItem
                      label="Név"
                      value={
                        selectedBooking.FirstName && selectedBooking.LastName
                          ? `${selectedBooking.FirstName} ${selectedBooking.LastName}`
                          : "N/A"
                      }
                      icon="person"
                    />
                    <DetailItem
                      label="Fizetési státusz"
                      value={selectedBooking.PaymentStatus || "N/A"}
                      icon="payments"
                      highlight={selectedBooking.PaymentStatus === "Fizetve"}
                    />
                    <DetailItem
                      label="Összeg"
                      value={`${selectedBooking.TotalPrice?.toLocaleString("hu-HU") || "N/A"} HUF`}
                      icon="attach_money"
                    />
                  </div>
                </div>

                <div className="border-t border-gray-200 pt-3 sm:pt-4">
                  <DetailItem
                    label="Szobatípus"
                    value={selectedBooking.RoomType || "N/A"}
                    icon="king_bed"
                  />
                  <DetailItem
                    label="Emelet"
                    value={selectedBooking.FloorNumber || "N/A"}
                    icon="floor"
                  />
                </div>

                <div className="border-t border-gray-200 pt-3 sm:pt-4">
                  <details className="group">
                    <summary className="flex items-center justify-between cursor-pointer list-none">
                      <div className="flex items-center text-blue-600 group-hover:text-blue-800 transition-colors">
                        <span className="material-symbols-outlined mr-2 text-lg sm:text-xl">
                          person
                        </span>
                        <span className="font-medium text-sm sm:text-base">
                          Saját adatok megtekintése
                        </span>
                      </div>
                      <span className="material-symbols-outlined text-gray-500 text-lg sm:text-xl group-open:rotate-180 transition-transform">
                        expand_more
                      </span>
                    </summary>
                    <div className="mt-3 p-3 sm:p-4 bg-blue-50 rounded-lg space-y-2 animate-[fadeIn_0.2s_ease-in-out]">
                      <DetailItem
                        label="Felhasználónév"
                        value={user.username}
                        small
                      />
                      <DetailItem label="Email" value={user.email} small />
                      <DetailItem
                        label="Regisztráció dátuma"
                        value={user.dateCreated}
                        small
                      />
                    </div>
                  </details>
                </div>
              </div>

              <div className="bg-gray-50 px-4 sm:px-6 py-3 sm:py-4 flex flex-wrap justify-end gap-2 sm:gap-3 sticky bottom-0">
                <button
                  onClick={() => setSelectedBooking(null)}
                  className="flex items-center justify-center gap-1 sm:gap-2 px-3 sm:px-5 py-1.5 sm:py-2.5 bg-gray-200 hover:bg-gray-300 text-gray-800 rounded-lg font-medium transition-colors text-sm sm:text-base"
                >
                  <span className="material-symbols-outlined text-lg sm:text-xl">close</span>
                  Bezárás
                </button>
                <button
                  onClick={() => handleDeleteBooking(selectedBooking.BookingId)}
                  className="flex items-center justify-center gap-1 sm:gap-2 px-3 sm:px-5 py-1.5 sm:py-2.5 bg-red-100 hover:bg-red-200 text-red-700 rounded-lg font-medium transition-colors text-sm sm:text-base"
                >
                  <span className="material-symbols-outlined text-lg sm:text-xl">delete</span>
                  Törlés
                </button>
                <button
                  onClick={() => setShowCommentModal(true)}
                  className="flex items-center justify-center gap-1 sm:gap-2 px-3 sm:px-5 py-1.5 sm:py-2.5 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium transition-colors text-sm sm:text-base"
                >
                  <span className="material-symbols-outlined text-lg sm:text-xl">rate_review</span>
                  Értékelés
                </button>
                <button
                  onClick={() => {
                    if (selectedBooking.RoomId) {
                      navigate(`/Foglalas/${selectedBooking.RoomId}`, {
                        state: {
                          room: {
                            roomId: selectedBooking.RoomId,
                            roomNumber: selectedBooking.RoomNumber,
                            roomType: selectedBooking.RoomType,
                            floorNumber: selectedBooking.FloorNumber,
                            pricePerNight: selectedBooking.TotalPrice,
                            capacity: selectedBooking.NumberOfGuests,
                          },
                          checkInDate: selectedBooking.CheckInDate,
                          checkOutDate: selectedBooking.CheckOutDate,
                        },
                      });
                    } else {
                      navigate("/Foglalas/id", {
                        state: {
                          searchParams: {
                            roomNumber: selectedBooking.RoomNumber,
                            floor: selectedBooking.FloorNumber,
                            type: selectedBooking.RoomType,
                          },
                        },
                      });
                    }
                  }}
                  className="flex items-center justify-center gap-1 sm:gap-2 px-3 sm:px-5 py-1.5 sm:py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition-colors text-sm sm:text-base"
                >
                  <span className="material-symbols-outlined text-lg sm:text-xl">visibility</span>
                  Részletek
                </button>
              </div>
            </div>
          </div>
        )}
        {user && (
          <>
            <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md mb-6 sm:mb-8 hover:shadow-xl transition-shadow duration-300 border border-blue-100">
              <div className="flex items-center mb-4">
                <span className="material-symbols-outlined text-3xl sm:text-4xl mr-3 sm:mr-4 bg-indigo-100 p-2 sm:p-3 rounded-full text-indigo-600">
                  lock
                </span>
                <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">
                  Jelszó kezelése
                </h2>
              </div>
              <details className="mb-4 sm:mb-6 group">
                <summary className="list-none flex items-center justify-between cursor-pointer p-3 sm:p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors">
                  <span className="font-medium text-blue-800">
                    Jelszó megváltoztatása
                  </span>
                  <span className="material-symbols-outlined transition-transform group-open:rotate-180 text-blue-600">
                    expand_more
                  </span>
                </summary>
                <div className="p-3 sm:p-4 border-t border-blue-100 animate-[fadeIn_0.2s_ease-in-out]">
                  <form className="space-y-4" onSubmit={handleChangePassword}>
                    <div>
                      {passwordErrors.currentPassword && (
                        <p className="text-red-600 text-xs mb-1">
                          {passwordErrors.currentPassword}
                        </p>
                      )}
                      <label
                        className="block text-sm font-medium mb-1 text-blue-800"
                        htmlFor="current-password"
                      >
                        Jelenlegi jelszó
                      </label>
                      <div className="relative">
                        <input
                          type={showCurrentPassword ? "text" : "password"}
                          id="current-password"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="Jelenlegi jelszó"
                          value={passwordData.currentPassword}
                          onChange={(e) =>
                            setPasswordData({
                              ...passwordData,
                              currentPassword: e.target.value,
                            })
                          }
                          required
                        />
                        <button
                          type="button"
                          onClick={() =>
                            setShowCurrentPassword(!showCurrentPassword)
                          }
                          className="absolute right-3 top-1/2 transform -translate-y-1/2 text-blue-600"
                        >
                          {showCurrentPassword ? "Elrejtés" : "Megjelenítés"}
                        </button>
                      </div>
                    </div>
                    <div>
                      {passwordErrors.newPassword && (
                        <p className="text-red-600 text-xs mb-1">
                          {passwordErrors.newPassword}
                        </p>
                      )}
                      <label
                        className="block text-sm font-medium mb-1 text-blue-800"
                        htmlFor="new-password"
                      >
                        Új jelszó
                      </label>
                      <div className="relative">
                        <input
                          type={showNewPassword ? "text" : "password"}
                          id="new-password"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="Új jelszó"
                          value={passwordData.newPassword}
                          onChange={(e) =>
                            setPasswordData({
                              ...passwordData,
                              newPassword: e.target.value,
                            })
                          }
                          required
                        />
                        <button
                          type="button"
                          onClick={() => setShowNewPassword(!showNewPassword)}
                          className="absolute right-3 top-1/2 transform -translate-y-1/2 text-blue-600"
                        >
                          {showNewPassword ? "Elrejtés" : "Megjelenítés"}
                        </button>
                      </div>
                    </div>
                    <div>
                      {passwordErrors.confirmPassword && (
                        <p className="text-red-/initialPassword600 text-xs mb-1">
                          {passwordErrors.confirmPassword}
                        </p>
                      )}
                      <label
                        className="block text-sm font-medium mb-1 text-blue-800"
                        htmlFor="confirm-password"
                      >
                        Új jelszó megerősítése
                      </label>
                      <div className="relative">
                        <input
                          type={showConfirmPassword ? "text" : "password"}
                          id="confirm-password"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="Új jelszó megerősítése"
                          value={passwordData.confirmPassword}
                          onChange={(e) =>
                            setPasswordData({
                              ...passwordData,
                              confirmPassword: e.target.value,
                            })
                          }
                          required
                        />
                        <button
                          type="button"
                          onClick={() =>
                            setShowConfirmPassword(!showConfirmPassword)
                          }
                          className="absolute right-3 top-1/2 transform -translate-y-1/2 text-blue-600"
                        >
                          {showConfirmPassword ? "Elrejtés" : "Megjelenítés"}
                        </button>
                      </div>
                    </div>
                    {message && (
                      <div
                        className={`p-3 rounded-lg ${message.type === "success"
                          ? "bg-green-100 text-green-800"
                          : "bg-red-100 text-red-800"
                          }`}
                      >
                        {message.text}
                      </div>
                    )}
                    <button
                      type="submit"
                      className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                    >
                      Jelszó megváltoztatása
                    </button>
                  </form>
                </div>
              </details>
              <details className="group">
                <summary className="list-none flex items-center justify-between cursor-pointer p-3 sm:p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors">
                  <span className="font-medium text-blue-800">
                    Elfelejtett jelszó
                  </span>
                  <span className="material-symbols-outlined transition-transform group-open:rotate-180 text-blue-600">
                    expand_more
                  </span>
                </summary>
                <div className="p-3 sm:p-4 border-t border-blue-100 animate-[fadeIn_0.2s_ease-in-out]">
                  <p className="mb-4 text-xs sm:text-sm text-blue-700">
                    {forgotStep === 1 &&
                      "Adja meg az email címét a jelszó visszaállításához."}
                    {forgotStep === 2 &&
                      "Adja meg az emailben kapott 6 számjegyű kódot."}
                    {forgotStep === 3 && "Adja meg az új jelszót."}
                  </p>
                  {forgotStep === 1 && (
                    <form className="space-y-4" onSubmit={handleForgotPasswordStep1}>
                      <div>
                        <label
                          className="block text-sm font-medium mb-1 text-blue-800"
                          htmlFor="reset-email"
                        >
                          Email cím
                        </label>
                        <input
                          type="email"
                          id="reset-email"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="Email cím"
                          value={forgotEmail}
                          onChange={(e) => setForgotEmail(e.target.value)}
                          required
                        />
                      </div>
                      <button
                        type="submit"
                        className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                      >
                        Kód küldése
                      </button>
                    </form>
                  )}
                  {forgotStep === 2 && (
                    <form className="space-y-4" onSubmit={handleForgotPasswordStep2}>
                      <div>
                        <label
                          className="block text-sm font-medium mb-1 text-blue-800"
                          htmlFor="verification-code"
                        >
                          Ellenőrző kód
                        </label>
                        <input
                          type="text"
                          id="verification-code"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="6 számjegyű kód"
                          value={verificationCode}
                          onChange={(e) => {
                            const value = e.target.value.replace(/\D/g, "");
                            setVerificationCode(value.slice(0, 6));
                          }}
                          required
                        />
                      </div>
                      {message && (
                        <div
                          className={`p-3 rounded-lg ${message.type === "success"
                            ? "bg-green-100 text-green-800"
                            : "bg-red-100 text-red-800"
                            }`}
                        >
                          {message.text}
                        </div>
                      )}
                      <button
                        type="submit"
                        className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                      >
                        Kód ellenőrzése
                      </button>
                    </form>
                  )}
                  {forgotStep === 3 && (
                    <form className="space-y-4" onSubmit={handleForgotPasswordStep3}>
                      <div>
                        {passwordErrors.newPassword && (
                          <p className="text-red-600 text-xs mb-1">
                            {passwordErrors.newPassword}
                          </p>
                        )}
                        <label
                          className="block text-sm font-medium mb-1 text-blue-800"
                          htmlFor="new-password-forgot"
                        >
                          Új jelszó
                        </label>
                        <div className="relative">
                          <input
                            type={showNewPasswordForgot ? "text" : "password"}
                            id="new-password-forgot"
                            className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                            placeholder="Új jelszó"
                            value={newPasswordData.newPassword}
                            onChange={(e) =>
                              setNewPasswordData({
                                ...newPasswordData,
                                newPassword: e.target.value,
                              })
                            }
                            required
                          />
                          <button
                            type="button"
                            onClick={() =>
                              setShowNewPasswordForgot(!showNewPasswordForgot)
                            }
                            className="absolute right-3 top-1/2 transform -translate-y-1/2 text-blue-600"
                          >
                            {showNewPasswordForgot ? "Elrejtés" : "Megjelenítés"}
                          </button>
                        </div>
                      </div>
                      <div>
                        {passwordErrors.confirmPassword && (
                          <p className="text-red-600 text-xs mb-1">
                            {passwordErrors.confirmPassword}
                          </p>
                        )}
                        <label
                          className="block text-sm font-medium mb-1 text-blue-800"
                          htmlFor="confirm-password-forgot"
                        >
                          Új jelszó megerősítése
                        </label>
                        <div className="relative">
                          <input
                            type={
                              showConfirmPasswordForgot ? "text" : "password"
                            }
                            id="confirm-password-forgot"
                            className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                            placeholder="Új jelszó megerősítése"
                            value={newPasswordData.confirmPassword}
                            onChange={(e) =>
                              setNewPasswordData({
                                ...newPasswordData,
                                confirmPassword: e.target.value,
                              })
                            }
                            required
                          />
                          <button
                            type="button"
                            onClick={() =>
                              setShowConfirmPasswordForgot(
                                !showConfirmPasswordForgot
                              )
                            }
                            className="absolute right-3 top-1/2 transform -translate-y-1/2 text-blue-600"
                          >
                            {showConfirmPasswordForgot
                              ? "Elrejtés"
                              : "Megjelenítés"}
                          </button>
                        </div>
                      </div>
                      <button
                        type="submit"
                        className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                      >
                        Jelszó beállítása
                      </button>
                    </form>
                  )}
                </div>
              </details>
            </div>
            <div className="border-t border-blue-200 pt-6 sm:pt-8 mt-6 sm:mt-8">
              <div className="bg-red-50 p-4 sm:p-6 rounded-lg border border-red-200">
                <h2 className="text-lg sm:text-xl font-semibold text-red-600 mb-2 flex items-center">
                  <span className="material-symbols-outlined mr-2">warning</span>
                  Fiók törlése
                </h2>
                <p className="text-xs sm:text-sm text-blue-700 mb-4">
                  A fiók törlése végleges művelet, és nem vonható vissza. Az összes
                  adat, beleértve a mentett vendég adatokat is, véglegesen
                  törlődni fog.
                </p>
                <details className="group">
                  <summary className="list-none cursor-pointer flex items-center">
                    <span className="material-symbols-outlined mr-2">
                      delete_forever
                    </span>
                    <span>Fiók törlése</span>
                  </summary>
                  <div className="mt-4 p-3 sm:p-4 bg-white border border-red-200 rounded-lg animate-[fadeIn_0.2s_ease-in-out]">
                    <p className="font-medium text-red-600 mb-4 text-sm sm:text-base">
                      Biztosan törölni szeretné a fiókját? Ez a művelet nem
                      visszavonható.
                    </p>
                    <div className="mb-4">
                      <label
                        className="block text-sm font-medium mb-1 text-blue-800"
                        htmlFor="confirm-delete"
                      >
                        Írja be a jelszavát a megerősítéshez:
                      </label>
                      <input
                        type="password"
                        id="confirm-delete"
                        className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-red-500 transition-all"
                        placeholder="Jelszó"
                        value={deletePassword}
                        onChange={(e) => setDeletePassword(e.target.value)}
                      />
                    </div>
                    <button
                      type="button"
                      className="w-full md:w-auto bg-red-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-red-700 transform hover:scale-105 transition-all duration-200"
                      onClick={handleDeleteAccount}
                    >
                      Végleges törlés
                    </button>
                  </div>
                </details>
              </div>
            </div>
          </>
        )}
        {showGuestModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white p-4 sm:p-6 rounded-lg shadow-lg w-full max-w-lg sm:max-w-2xl">
              <h3 className="text-xl font-semibold text-blue-800 mb-4">
                {editGuestId ? "Vendég szerkesztése" : "Új vendég hozzáadása"}
              </h3>
              <form
                onSubmit={editGuestId ? handleUpdateGuest : handleAddGuest}
                className="grid grid-cols-1 sm:grid-cols-2 gap-4"
              >
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Vezetéknév
                    </label>
                    <input
                      placeholder="Petőfi"
                      type="text"
                      value={guestData.lastName}
                      onChange={(e) =>
                        setGuestData({ ...guestData, lastName: e.target.value })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Keresztnév
                    </label>
                    <input
                      placeholder="Sándor"
                      type="text"
                      value={guestData.firstName}
                      onChange={(e) =>
                        setGuestData({ ...guestData, firstName: e.target.value })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Email
                    </label>
                    <input
                      placeholder="example@gmail.com"
                      type="email"
                      value={guestData.email}
                      onChange={(e) =>
                        setGuestData({ ...guestData, email: e.target.value })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Telefonszám
                    </label>
                    <input
                      placeholder="36 70 123 456"
                      type="text"
                      value={guestData.phoneNumber}
                      onChange={(e) =>
                        setGuestData({
                          ...guestData,
                          phoneNumber: e.target.value,
                        })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Születési dátum
                    </label>
                    <input
                      type="date"
                      value={guestData.dateOfBirth}
                      onChange={(e) =>
                        setGuestData({
                          ...guestData,
                          dateOfBirth: e.target.value,
                        })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                      required
                    />
                  </div>
                </div>
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Cím
                    </label>
                    <input
                      placeholder="Palóczy László u. 3"
                      type="text"
                      value={guestData.address}
                      onChange={(e) =>
                        setGuestData({ ...guestData, address: e.target.value })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Település
                    </label>
                    <input
                      placeholder="Budapest"
                      type="text"
                      value={guestData.city}
                      onChange={(e) =>
                        setGuestData({ ...guestData, city: e.target.value })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Ország
                    </label>
                    <input
                      placeholder="Magyarország"
                      type="text"
                      value={guestData.country}
                      onChange={(e) =>
                        setGuestData({ ...guestData, country: e.target.value })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">
                      Nem
                    </label>
                    <select
                      value={guestData.gender}
                      onChange={(e) =>
                        setGuestData({ ...guestData, gender: e.target.value })
                      }
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    >
                      <option value="">Válasszon</option>
                      <option value="Férfi">Férfi</option>
                      <option value="Nő">Nő</option>
                      <option value="Egyéb">Kalács</option>
                    </select>
                  </div>
                </div>
                <div className="sm:col-span-2 flex gap-2 justify-end mt-4">
                  <button
                    type="submit"
                    className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700"
                  >
                    {editGuestId ? "Mentés" : "Hozzáadás"}
                  </button>
                  <button
                    type="button"
                    onClick={() => {
                      setShowGuestModal(false);
                      setEditGuestId(null);
                      setGuestData({
                        firstName: "",
                        lastName: "",
                        email: "",
                        phoneNumber: "",
                        address: "",
                        city: "",
                        country: "",
                        dateOfBirth: "",
                        gender: "",
                      });
                    }}
                    className="bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700"
                  >
                    Mégse
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
        {showCommentModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white p-4 sm:p-6 rounded-lg shadow-lg w-full max-w-md">
              <h3 className="text-xl font-semibold text-blue-800 mb-4">
                Értékelés írása
              </h3>
              <form onSubmit={handleSubmitComment} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-blue-800 mb-1">
                    Értékelés (1-5)
                  </label>
                  <input
                    type="number"
                    min="1"
                    max="5"
                    value={commentData.rating}
                    onChange={(e) =>
                      setCommentData({
                        ...commentData,
                        rating: parseInt(e.target.value),
                      })
                    }
                    className="w-full p-2 border border-blue-200 rounded-lg"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800 mb-1">
                    Megjegyzés
                  </label>
                  <textarea
                    value={commentData.comment}
                    onChange={(e) =>
                      setCommentData({ ...commentData, comment: e.target.value })
                    }
                    className="w-full p-2 border border-blue-200 rounded-lg"
                    rows="4"
                    placeholder="Írja meg a véleményét..."
                    required
                  />
                </div>
                <div className="flex gap-2 justify-end">
                  <button
                    type="submit"
                    className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700"
                  >
                    Küldés
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowCommentModal(false)}
                    className="bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700"
                  >
                    Mégse
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ProfilePage;