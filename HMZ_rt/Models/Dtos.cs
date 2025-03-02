using HMZ_rt.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
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
        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorCodeExpiry { get; set; }
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
    public record UpdateRoomDto(string? Status);
    public record CreateBookingDto(int RoomId, int GuestId, DateTime? CheckInDate, DateTime? CheckOutDate, int NumberOfGuests, decimal? TotalPrice, string? Status, string? PaymentStatus);
public class NewStaffDto
{
   public string? FirstName { get; set; }
   public string? LastName { get; set; }
    [EmailAddress(ErrorMessage = "Nem megfelelő Email forma.")]
    public     string? Email { get; set; }
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
    [Required(ErrorMessage = "A szoba karbantartás végrehajtási idejének megadása kötelező!")]
    public DateTime? ResolutionDate { get; set; }
    [Required(ErrorMessage = "A szoba karbantartás leírásának megadása kötelező!")]
    public string? Notes { get; set; }
    [Required(ErrorMessage = "A szobaszám megadása kötelező")]
    public int RoomId { get; set; }
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
    [MinLength(5, ErrorMessage = "Legalább 4 karakternek kell a családnévnek lennie!")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "A második név kötelező!")]
    [MinLength(5, ErrorMessage = "Legalább 4 karakternek kell lennie a kerewsztnévnek!")]
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