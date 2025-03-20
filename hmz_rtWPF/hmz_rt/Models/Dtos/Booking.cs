using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? GuestId { get; set; }
        public int? RoomId { get; set; }
        public decimal? TotalPrice { get; set; }
        //public DateTime? BookingDate { get; set; }
        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }

        public int? NumberOfGuests { get; set; }
        public string Status { get; set; }
        public List<Payment> Payments { get; set; }
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int? BookingId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public string PaymentNotes { get; set; }
        public DateTime? DateAdded { get; set; }
    }

    public class UpdatePaymentInfo
    {
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
    }




    public class CreateBookingDto
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int GuestId { get; set; }
        public int NumberOfGuests { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class UpdateBooking
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }

    public class UpdateBookingStatus
    {
        public string Status { get; set; }
    }

    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public int? GuestId { get; set; }
        public string GuestName { get; set; }
        public int? RoomId { get; set; }
        public string RoomNumber { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        //public DateTime? BookingDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public decimal? TotalPrice { get; set; }
        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }

        public string Status { get; set; }

        public bool CanChangeStatus
        {
            get
            {
                return Status != "Finished" && Status != "Lemondva";
            }
        }

        public bool HasFeedback { get; set; }
        public bool IsPaymentEnabled
        {
            get
            {
                // A fizetés gomb csak akkor aktív, ha a foglalás jóváhagyva van vagy check-in státuszban van
                // és a fizetési státusz nem "Fizetve"
                return (Status == "Jóváhagyva" || Status == "Checked In") &&
                        PaymentStatus?.Trim().ToLower() != "fizetve";
            }
        }



    }



    //Feedback osztályok
    public class CreateFeedback
    {
        public string Category { get; set; }
        public int? Rating { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public int GuestId { get; set; }
    }

    public class Feedback
    {
        public int FeedbackId { get; set; }
        public DateTime? FeedbackDate { get; set; }
        public string Comments { get; set; }
        public string Category { get; set; }
        public int? Rating { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? GuestId { get; set; }
    }
    public class UpdateFeedback
    {
        public string Comment { get; set; }
        public string Status { get; set; }
        public int? Rating { get; set; }
    }
}
