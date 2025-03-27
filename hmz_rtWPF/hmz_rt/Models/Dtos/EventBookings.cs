using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public class EventBookings : INotifyPropertyChanged
    {
        private int _eventBookingId;
        private int _eventId;
        private int _guestId;
        private DateTime _bookingDate;
        private int _numberOfTickets;
        private decimal? _totalPrice;
        private string _status;
        private string _paymentStatus;
        private DateTime _dateAdded;
        private string _notes;
        private string _guestName; 
        private string _eventName; 

        public int EventBookingId
        {
            get { return _eventBookingId; }
            set
            {
                if (_eventBookingId != value)
                {
                    _eventBookingId = value;
                    OnPropertyChanged(nameof(EventBookingId));
                }
            }
        }

        public int EventId
        {
            get { return _eventId; }
            set
            {
                if (_eventId != value)
                {
                    _eventId = value;
                    OnPropertyChanged(nameof(EventId));
                }
            }
        }

        public int GuestId
        {
            get { return _guestId; }
            set
            {
                if (_guestId != value)
                {
                    _guestId = value;
                    OnPropertyChanged(nameof(GuestId));
                }
            }
        }

        public DateTime BookingDate
        {
            get { return _bookingDate; }
            set
            {
                if (_bookingDate != value)
                {
                    _bookingDate = value;
                    OnPropertyChanged(nameof(BookingDate));
                }
            }
        }

        public int NumberOfTickets
        {
            get { return _numberOfTickets; }
            set
            {
                if (_numberOfTickets != value)
                {
                    _numberOfTickets = value;
                    OnPropertyChanged(nameof(NumberOfTickets));
                }
            }
        }

        public decimal? TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public string PaymentStatus
        {
            get { return _paymentStatus; }
            set
            {
                if (_paymentStatus != value)
                {
                    _paymentStatus = value;
                    OnPropertyChanged(nameof(PaymentStatus));
                }
            }
        }

        public DateTime DateAdded
        {
            get { return _dateAdded; }
            set
            {
                if (_dateAdded != value)
                {
                    _dateAdded = value;
                    OnPropertyChanged(nameof(DateAdded));
                }
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    OnPropertyChanged(nameof(Notes));
                }
            }
        }

        public string GuestName
        {
            get { return _guestName; }
            set
            {
                if (_guestName != value)
                {
                    _guestName = value;
                    OnPropertyChanged(nameof(GuestName));
                }
            }
        }

        public string EventName
        {
            get { return _eventName; }
            set
            {
                if (_eventName != value)
                {
                    _eventName = value;
                    OnPropertyChanged(nameof(EventName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CreateEventBooking
    {
        public int GuestId { get; set; }
        public int NumberOfTickets { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; }
    }

}
