using HMZ_rt.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
#pragma warning disable
namespace HMZ_rt.Models
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Meg kell adnod egy felhasználónevet!")]
        [MinLength(5, ErrorMessage = "A felhasználónévnek legalább 5 karakter hosszúnak kell lennie!")]
        [MaxLength(20, ErrorMessage = "A felhasználónév nem lehet hosszabb 20 karakternél!")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Jelszó megadása kötelező!")]
        [MinLength(8, ErrorMessage = "A jelszónak legalább 8 karakter hosszúnak kell lennie!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "A jelszónak tartalmaznia kell legalább 1, kis és Nagy betűt illetve számot.")]
        public string? Password { get; set; }
        [EmailAddress(ErrorMessage = "Nem megfelelő Email forma.")]
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }

    public record CreateNotifiactionDto( string? Message, string? Status, string? Type, int? Priority, string? Notes, int UserId, string? Category);
    }

    public record CreateRoom(string? RoomType, string? RoomNumber, int? Capacity, decimal? PricePerNight, string? Status, string? Description, int? FLoorNumber, string? Images);
    public record UploadAmenitiesForNewRoomDto(string? AmenityName, string? Descript, string? Availability, string? Status, string? Icon, string? Categ, int? Priority, int? RoomId);
    public class LoginDto
{
    [Required(ErrorMessage = "Meg kell adnod egy felhasználónevet!")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Jelszó megadása kötelező!")]
    public string? Password { get; set; }
}
    public record UpdateRoomDto(string? Status, string? Description, string? RoomType, string? RoomNumber, int? Capacity,
        decimal? PricePerNight, int? FloorNumber);
    public class CreateBookingDto {
        public int GuestId { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string PaymentMethod { get; set; }
        
}   
public class UpdateBooking
{
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }

}
public class Subscribenewsletter
{
    [EmailAddress(ErrorMessage = "Nem email formátum")]
    public string Email { get; set; }
}




public class NewStaffDto
{
   public string? FirstName { get; set; }
   public string? LastName { get; set; }
    [EmailAddress(ErrorMessage = "Nem megfelelő Email forma.")]
    public     string? Email { get; set; }
    [Phone(ErrorMessage = "Nem megfelelő telefonszám formátum")]
  public      string? PhoneNumber { get; set; }
     public   string? Position { get; set; }
   public     decimal? Salary { get; set; }
     public   string? Status { get; set; }
     public   string? Departmen { get; set; }
}

public class UpdateStaffDto
{
    [Required(ErrorMessage = "Az Családnév megadása kötelező!")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "A Keresztnév megadása kötelező!")]
    public string? LastName { get; set; }
    [Required(ErrorMessage = "Az Email megadása kötelező!")]
    [EmailAddress(ErrorMessage ="Nem megfelelő Email forma.")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "A Telefonszám megadása kötelező!")]
    [Phone(ErrorMessage ="Nem megfelelő telefonszám formátum.")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "A Pozicíó megadása kötelező!")]
    public string? Position { get; set; }
    [Required(ErrorMessage = "A Fizetésnek meg kell adva lennie!")]
    public decimal? Salary { get; set; }
    [Required(ErrorMessage = "Kötelező megadnod egy Státuszt!")]
    public string? Status { get; set; }
    [Required(ErrorMessage = "Meg kell adnod kötelezően egy Beosztást!")]
    public string? Department { get; set; }
}
public class fa2
{
    [Required(ErrorMessage = "Add meg kötelezően!")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Add meg kötelezően ezt is!")]
    public string? Code { get; set; }
}

public class kacsa
{
    [Required(ErrorMessage = "A szoba karbantartás idejének megadása kötelező!")]
    public DateTime? MaintenanceDate { get; set; }
    [Required(ErrorMessage = "A karbantartás leírásának megadása kötelező!")]
    public string? Description{get;set;}
    [Required(ErrorMessage = "A szoba karbantartás leírásának megadása kötelező!")]
    public string? Notes { get; set; }
    [Required(ErrorMessage = "A szobaszám megadása kötelező")]
    public int RoomId { get; set; }
}

public class MaintanceUpdate {

    [Required(ErrorMessage = "A szoba karbantartás leírásának megadása kötelező!")]
    public string? Notes { get; set; }
    [Required(ErrorMessage = "A szoba karbantartás státuszának megadása kötelező!")]
    public string? Status { get; set; }
    [Required(ErrorMessage = "A szoba karbantartás hozzárendelt staffjának az idjának megadása kötelező!")]
    public int StaffId { get; set; }
    [Required(ErrorMessage = "A szoba karbantartás megoldási dátumának megadása kötelező!")]
    public DateTime? ResolutionDate { get; set; }
    [Required(ErrorMessage = "A szoba karbantartás költségének megadása kötelező!")]
    public decimal? Cost { get; set; }

}


public class promotionupdate
{
    [Required(ErrorMessage = "A név megadása kötelező!")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "A leírás megadása kötelező!")]
    public string? Description { get; set; }
    [Required(ErrorMessage ="A kezdeti időpont megadása kötelező!")]
    public DateTime? StartDate { get; set; }
    [Required(ErrorMessage = "A Terms and conditions megadása kötelező!")]
    public string? TermsConditions { get; set; }
    [Required(ErrorMessage = "A végzeti időpont megadása kötelező!")]
    public DateTime? EndDate { get; set; }
    [Required(ErrorMessage = "A a kedvezmény százalék megadása kötelező!")]
    public decimal? DiscountPercentage { get; set; }
    [Required(ErrorMessage = "A szobid kötelező!")]
    public int? RoomId { get; set; }
    [Required(ErrorMessage = "A státusz megadása kötelező!")]
    public string? Status { get; set; }

}


public class promotioncreate
{
    [Required(ErrorMessage = "A név megadása kötelező!")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "A leírás megadása kötelező!")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "A kezdeti időpont megadása kötelező!")]
    public DateTime? StartDate { get; set; }
    [Required(ErrorMessage = "A Terms and conditions megadása kötelező!")]
    public string? TermsConditions { get; set; }
    [Required(ErrorMessage = "A végzeti időpont megadása kötelező!")]
    public DateTime? EndDate { get; set; }
    [Required(ErrorMessage = "A a kedvezmény százalék megadása kötelező!")]
    public decimal? DiscountPercentage { get; set; }
    [Required(ErrorMessage = "A szobid kötelező!")]
    public int? RoomId { get; set; }
    [Required(ErrorMessage = "A státusz megadása kötelező!")]
    public string? Status { get; set; }

}

public class PromotionStatus
{
    [Required(ErrorMessage = "Nem lehet üres!")]
    public string? Status { get; set; }
}



public class CreateGuest
{
    [Required(ErrorMessage = "Az elsőnév kötelező!")]
    [MinLength(2, ErrorMessage = "Legalább 2 karakternek kell a családnévnek lennie!")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "A második név kötelező!")]
    [MinLength(2, ErrorMessage = "Legalább 2 karakternek kell lennie a kerewsztnévnek!")]
    public string? LastName { get; set; }
    [Required(ErrorMessage = "Az email megadása kötelező!")]
    [EmailAddress(ErrorMessage = "Nem megfelelő email forma!")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Telefonszám megadása kötelező!")]
    [Phone(ErrorMessage = "Nem megfelelő telefonszám formátum!")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "A cím megadása kötelező (utca,házszám)")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "A város megadása kötelező")]
    public string? City { get; set; }
    [Required(ErrorMessage = "Az ország megadása kötelező!")]
    public string? Country { get; set; }
    [Required(ErrorMessage = "A születési dátum megadása kötelező")]
    public DateTime? DateOfBirth { get; set; }
    [Required(ErrorMessage = "A gender megadása kötelező!")]
    public string? Gender { get; set; }
    public int? UserId { get; set; }
}




public class UpdateGuest
{
    [Required(ErrorMessage = "Az elsőnév kötelező!")]
    [MinLength(2, ErrorMessage = "Legalább 2 karakternek kell a családnévnek lennie!")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "A második név kötelező!")]
    [MinLength(2, ErrorMessage = "Legalább 2 karakternek kell lennie a kerewsztnévnek!")]
    public string? LastName { get; set; }
    [Required(ErrorMessage = "Az email megadása kötelező!")]
    [EmailAddress(ErrorMessage = "Nem megfelelő email forma!")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Telefonszám megadása kötelező!")]
    [Phone(ErrorMessage = "Nem megfelelő telefonszám formátum!")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "A cím megadása kötelező (utca,házszám)")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "A város megadása kötelező")]
    public string? City { get; set; }
    [Required(ErrorMessage = "Az ország megadása kötelező!")]
    public string? Country { get; set; }
    [Required(ErrorMessage = "A születési dátum megadása kötelező")]
    public DateTime? DateOfBirth { get; set; }
    [Required(ErrorMessage = "A gender megadása kötelező!")]
    public string? Gender { get; set; }
}


public class CreateEvent
{
    [Required(ErrorMessage = "Kapacitás megadása kötelező!")]
    public int? Capacity { get; set; }

    [Required(ErrorMessage = "A státusz megadása kötelező!")]
    public string? Status { get; set; }

    [Required(ErrorMessage = "Az esemény nem szerepelhet név nélkül")]
    public string? EventName { get; set; }

    [Required(ErrorMessage = "Az esemény időpontjának megadása kötelező!")]
    public DateTime? EventDate { get; set; }

    [Required(ErrorMessage = "A hely megadása kötelező!")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "A leírás megadása kötelező!")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Szervező nevének megadása kötelező!")]
    public string? OrganizerName { get; set; }

    [Required(ErrorMessage = "A kontaktinfó megadása kötelező")]
    public string? ContactInfo { get; set; }

    [Required(ErrorMessage = "Az ár megadása kötelező!")]
    public decimal? Price { get; set; }

}
public class UpdateEvent
{
    [Required(ErrorMessage = "Kapacitás megadása kötelező!")]
    public int? Capacity { get; set; }
    [Required(ErrorMessage = "A státusz megadása kötelező!")]
    public string? Status { get; set; }
    [Required(ErrorMessage = "Az esemény nem szerepelhet név nélkül")]
    public string? EventName { get; set; }
    [Required(ErrorMessage = "Az esemény időpontjának megadásakötelező!")]
    public DateTime? EventDate { get; set; }
    [Required(ErrorMessage = "A hely megadása kötelező!")]
    public string? Location { get; set; }
    [Required(ErrorMessage = "A leírás megadásakötelező!")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Szervező nevének megadása kötelező!")]
    public string? OrganizerName { get; set; }
    [Required(ErrorMessage = "A kontaktinfó megadása kötelező")]
    public string? ContactInfo { get; set; }
    [Required(ErrorMessage = "Az ár megadása kötelező!")]
    public decimal? Price { get; set; }
}



public class Getrooms
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int GuestNumber { get; set; }
}





public class CreateFeedback
{
    [Required(ErrorMessage = "Kategória megadása kötelező!")]
    public string? Category { get; set; }
    [Required(ErrorMessage = "A rating megadása kötelező!")]
    [Range(1, 10)]
    public decimal? Rating { get; set; }
    [Required(ErrorMessage = "Status megadása kötelező!")]
    public string? Status { get; set; }
    [Required(ErrorMessage = "A response nem lehet üres!")]
    public string? Response { get; set; }
    [Required(ErrorMessage = "A guest id megadása kötelező!")]
    public int GuestId { get;set; }
}

public record Forgotpass(string? Email, string? Code);

public class Forgotpass1
{
    [EmailAddress(ErrorMessage = "Nem megfelelő Email forma.")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Jelszó megadása kötelező!")]
    [MinLength(8, ErrorMessage = "A jelszónak legalább 8 karakter hosszúnak kell lennie!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "A jelszónak tartalmaznia kell legalább 1, kis és Nagy betűt illetve számot.")]
    public string? Password { get; set; }
}

public class UpdateBookingStatus
{
    [Required(ErrorMessage = "Nem lehet üres a status")]
    public string? Status { get; set; }
}



public class UpdateNotiStatus
{
    [Required(ErrorMessage ="Kötelező az új státuszt megadni!")]
    public string? Status { get; set; }
}



public class UpdateAmenity
{
    public string? AmenityName { get; set; }
    public string? Descript { get; set; }
    public string? Availability { get; set; }
    public string? Status { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public int? Priority { get; set; }
}


public class UpdateFeedback
{
    public string? Comment { get; set; }
    public string? Status { get; set; }

}

public class UpdatePaymentInfo
{
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }  
}

public class CreateEventBooking
{
    public int GuestId { get; set; }
    public int? NumberOfTickets { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public string? Notes {  get; set; }
}


public class SetNewPass
{
    [Required(ErrorMessage = "Jelszó megadása kötelező!")]
    [MinLength(8, ErrorMessage = "A jelszónak legalább 8 karakter hosszúnak kell lennie!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "A jelszónak tartalmaznia kell legalább 1, kis és Nagy betűt illetve számot.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "A régi jelszó megadása kötelező!")]
    public string? OldPassword { get; set; }
}



public class DeleteAccount
{
    [Required(ErrorMessage ="A jelszó megadása törléshez szükséges")]
    public string Password { get; set; }
}


public class NewRevi
{
    [Required(ErrorMessage = "Nem lehet üres a GuestId")]
    public int GuestId { get; set; }
    [Required(ErrorMessage = "Nem lehet üres az értékelés!")]
    [Range(1, 10, ErrorMessage = "1 és 10 között kell lennie az értékelésnek!")]
    public decimal? Rating { get; set; }
    public string? Comment {get;set;}
    [Required(ErrorMessage = "Nem lehet üres a foglalási szám")]
    public int BookingId { get; set; }
}


public class EmailBooking
{
    [Required(ErrorMessage ="A user id megadása kötelező!")]
    public int UserId { get; set; }

}
public class Newslettersign
{
    [Required(ErrorMessage = "Az email kötelező.")]
    [EmailAddress]
    public string Email { get; set; }
}

public class NewsletterDto
{
    [Required(ErrorMessage = "A tárgy megadása kötelező")]
    public string Subject { get; set; }
    [Required(ErrorMessage = "A body kötelező")]
    public string HtmlBody { get; set; }
}