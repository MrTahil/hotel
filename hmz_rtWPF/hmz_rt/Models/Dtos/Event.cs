using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int? Capacity { get; set; }
        public string Status { get; set; }
        public DateTime? EventDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string OrganizerName { get; set; }
        public string ContactInfo { get; set; }
        public DateTime? DateAdded { get; set; }
        public decimal? Price { get; set; }

        public string Images { get; set; }
        public List<EventBooking> Eventbookings { get; set; }
    }

    public class EventBooking
    {
        public int EventBookingId { get; set; }
        public int? EventId { get; set; }
        public int? GuestId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string Status { get; set; }
        public int? NumberOfTickets { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class CreateEvent
    {
        public string EventName { get; set; }
        public int? Capacity { get; set; }
        public string Status { get; set; }
        public DateTime? EventDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string OrganizerName { get; set; }
        public string ContactInfo { get; set; }
        public decimal? Price { get; set; }
    }

    public class UpdateEvent
    {
        public string EventName { get; set; }
        public int? Capacity { get; set; }
        public string Status { get; set; }
        public DateTime? EventDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string OrganizerName { get; set; }
        public string ContactInfo { get; set; }
        public decimal? Price { get; set; }
    }
}
