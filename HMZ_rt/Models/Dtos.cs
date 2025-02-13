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
