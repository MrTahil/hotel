using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Models;

public partial class HmzRtContext : DbContext
{
    public HmzRtContext()
    {
    }

    public HmzRtContext(DbContextOptions<HmzRtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Eventbooking> Eventbookings { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Loyaltyprogram> Loyaltyprograms { get; set; }

    public virtual DbSet<Marketing> Marketings { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Roominventory> Roominventories { get; set; }

    public virtual DbSet<Roommaintenance> Roommaintenances { get; set; }

    public virtual DbSet<Roomtype> Roomtypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Taxrate> Taxrates { get; set; }

    public virtual DbSet<Useraccount> Useraccounts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PRIMARY");

            entity.ToTable("amenities");

            entity.HasIndex(e => e.RoomId, "Amenities_fk5");

            entity.Property(e => e.AmenityId)
                .HasColumnType("int(11)")
                .HasColumnName("amenity_id");
            entity.Property(e => e.AmenityName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("amenity_name");
            entity.Property(e => e.Availability)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("availability");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("category");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Icon)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("icon");
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("priority");
            entity.Property(e => e.RoomId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");

            entity.HasOne(d => d.Room).WithMany(p => p.AmenitiesNavigation)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Amenities_fk5");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PRIMARY");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.RoomId, "Bookings_fk0");

            entity.HasIndex(e => e.GuestId, "Bookings_fk2");

            entity.Property(e => e.BookingId)
                .HasColumnType("int(11)")
                .HasColumnName("booking_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("booking_date");
            entity.Property(e => e.CheckInDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("check_in_date");
            entity.Property(e => e.CheckOutDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("check_out_date");
            entity.Property(e => e.GuestId)
                .HasColumnType("int(11)")
                .HasColumnName("guest_id");
            entity.Property(e => e.NumberOfGuests)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("number_of_guests");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("payment_status");
            entity.Property(e => e.RoomId)
                .HasColumnType("int(11)")
                .HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Guest).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.GuestId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Bookings_fk2");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Bookings_fk0");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PRIMARY");

            entity.ToTable("events");

            entity.Property(e => e.EventId)
                .HasColumnType("int(11)")
                .HasColumnName("event_id");
            entity.Property(e => e.Capacity)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("capacity");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("contact_info");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EventDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("event_date");
            entity.Property(e => e.EventName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("event_name");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("location");
            entity.Property(e => e.OrganizerName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("organizer_name");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Eventbooking>(entity =>
        {
            entity.HasKey(e => e.EventBookingId).HasName("PRIMARY");

            entity.ToTable("eventbookings");

            entity.HasIndex(e => e.EventId, "EventBookings_fk1");

            entity.HasIndex(e => e.GuestId, "EventBookings_fk2");

            entity.Property(e => e.EventBookingId)
                .HasColumnType("int(11)")
                .HasColumnName("event_booking_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("booking_date");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.EventId)
                .HasColumnType("int(11)")
                .HasColumnName("event_id");
            entity.Property(e => e.GuestId)
                .HasColumnType("int(11)")
                .HasColumnName("guest_id");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.NumberOfTickets)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("number_of_tickets");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("payment_status");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Event).WithMany(p => p.Eventbookings)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("EventBookings_fk1");

            entity.HasOne(d => d.Guest).WithMany(p => p.Eventbookings)
                .HasForeignKey(d => d.GuestId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("EventBookings_fk2");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PRIMARY");

            entity.ToTable("feedback");

            entity.HasIndex(e => e.GuestId, "Feedback_fk9");

            entity.Property(e => e.FeedbackId)
                .HasColumnType("int(11)")
                .HasColumnName("feedback_id");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("category");
            entity.Property(e => e.Comments)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("comments");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.FeedbackDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("feedback_date");
            entity.Property(e => e.GuestId)
                .HasColumnType("int(11)")
                .HasColumnName("guest_id");
            entity.Property(e => e.Rating)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("rating");
            entity.Property(e => e.Response)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("response");
            entity.Property(e => e.ResponseDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("response_date");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");

            entity.HasOne(d => d.Guest).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.GuestId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Feedback_fk9");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.GuestId).HasName("PRIMARY");

            entity.ToTable("guests");

            entity.Property(e => e.GuestId)
                .HasColumnType("int(11)")
                .HasColumnName("guest_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("country");
            entity.Property(e => e.DateOfBirth)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PRIMARY");

            entity.ToTable("invoices");

            entity.HasIndex(e => e.BookingId, "Invoices_fk2");

            entity.Property(e => e.InvoiceId)
                .HasColumnType("int(11)")
                .HasColumnName("invoice_id");
            entity.Property(e => e.BookingId)
                .HasColumnType("int(11)")
                .HasColumnName("booking_id");
            entity.Property(e => e.Currency)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("currency");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.DueDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("due_date");
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("invoice_date");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("payment_status");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Booking).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Invoices_fk2");
        });

        modelBuilder.Entity<Loyaltyprogram>(entity =>
        {
            entity.HasKey(e => e.LoyaltyProgramId).HasName("PRIMARY");

            entity.ToTable("loyaltyprograms");

            entity.Property(e => e.LoyaltyProgramId)
                .HasColumnType("int(11)")
                .HasColumnName("loyalty_program_id");
            entity.Property(e => e.Benefits)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("benefits");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("category");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ExpirationPeriod)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("expiration_period");
            entity.Property(e => e.PointsRequired)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("points_required");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("program_name");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TermsConditions)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("terms_conditions");
        });

        modelBuilder.Entity<Marketing>(entity =>
        {
            entity.HasKey(e => e.MarketingId).HasName("PRIMARY");

            entity.ToTable("marketing");

            entity.Property(e => e.MarketingId)
                .HasColumnType("int(11)")
                .HasColumnName("marketing_id");
            entity.Property(e => e.Budget)
                .HasPrecision(10)
                .HasDefaultValueSql("'10'")
                .HasColumnName("budget");
            entity.Property(e => e.CampaignName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("campaign_name");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("end_date");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TargetAudience)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("target_audience");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.UserId, "Notifications_fk8");

            entity.Property(e => e.NotificationId)
                .HasColumnType("int(11)")
                .HasColumnName("notification_id");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("category");
            entity.Property(e => e.DateRead)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_read");
            entity.Property(e => e.DateSent)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_sent");
            entity.Property(e => e.Message)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("type");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Notifications_fk8");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.BookingId, "Payments_fk1");

            entity.Property(e => e.PaymentId)
                .HasColumnType("int(11)")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId)
                .HasColumnType("int(11)")
                .HasColumnName("booking_id");
            entity.Property(e => e.Currency)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("currency");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentNotes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("payment_notes");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("transaction_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Payments_fk1");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PRIMARY");

            entity.ToTable("promotions");

            entity.HasIndex(e => e.RoomId, "Promotions_fk7");

            entity.Property(e => e.PromotionId)
                .HasColumnType("int(11)")
                .HasColumnName("promotion_id");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.DiscountPercentage)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("discount_percentage");
            entity.Property(e => e.EndDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("end_date");
            entity.Property(e => e.PromotionName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("promotion_name");
            entity.Property(e => e.RoomId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("room_id");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TermsConditions)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("terms_conditions");

            entity.HasOne(d => d.Room).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Promotions_fk7");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.GuestId, "Reviews_fk2");

            entity.HasIndex(e => e.RoomId, "Reviews_fk3");

            entity.Property(e => e.ReviewId)
                .HasColumnType("int(11)")
                .HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.GuestId)
                .HasColumnType("int(11)")
                .HasColumnName("guest_id");
            entity.Property(e => e.Rating)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("rating");
            entity.Property(e => e.Response)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("response");
            entity.Property(e => e.ResponseDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("response_date");
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("review_date");
            entity.Property(e => e.RoomId)
                .HasColumnType("int(11)")
                .HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");

            entity.HasOne(d => d.Guest).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GuestId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Reviews_fk2");

            entity.HasOne(d => d.Room).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Reviews_fk3");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PRIMARY");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomId)
                .HasColumnType("int(11)")
                .HasColumnName("room_id");
            entity.Property(e => e.Amenities)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("amenities");
            entity.Property(e => e.Images)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("images");
            entity.Property(e => e.Capacity)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("capacity");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.FloorNumber)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("floor_number");
            entity.Property(e => e.PricePerNight)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("room_number");
            entity.Property(e => e.RoomType)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("room_type");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Roominventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PRIMARY");

            entity.ToTable("roominventory");

            entity.HasIndex(e => e.RoomId, "RoomInventory_fk8");

            entity.Property(e => e.InventoryId)
                .HasColumnType("int(11)")
                .HasColumnName("inventory_id");
            entity.Property(e => e.CostPerItem)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("cost_per_item");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("item_name");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("last_updated");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.RoomId)
                .HasColumnType("int(11)")
                .HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.Supplier)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("supplier");

            entity.HasOne(d => d.Room).WithMany(p => p.Roominventories)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("RoomInventory_fk8");
        });

        modelBuilder.Entity<Roommaintenance>(entity =>
        {
            entity.HasKey(e => e.MaintenanceId).HasName("PRIMARY");

            entity.ToTable("roommaintenance");

            entity.HasIndex(e => e.RoomId, "RoomMaintenance_fk1");

            entity.HasIndex(e => e.StaffId, "RoomMaintenance_fk5");

            entity.Property(e => e.MaintenanceId)
                .HasColumnType("int(11)")
                .HasColumnName("maintenance_id");
            entity.Property(e => e.Cost)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("cost");
            entity.Property(e => e.DateReported)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_reported");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.MaintenanceDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("maintenance_date");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.ResolutionDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("resolution_date");
            entity.Property(e => e.RoomId)
                .HasColumnType("int(11)")
                .HasColumnName("room_id");
            entity.Property(e => e.StaffId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");

            entity.HasOne(d => d.Room).WithMany(p => p.Roommaintenances)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("RoomMaintenance_fk1");

            entity.HasOne(d => d.Staff).WithMany(p => p.Roommaintenances)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("RoomMaintenance_fk5");
        });

        modelBuilder.Entity<Roomtype>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PRIMARY");

            entity.ToTable("roomtypes");

            entity.Property(e => e.RoomTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("room_type_id");
            entity.Property(e => e.Amenities)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("amenities");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("base_price");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("image_url");
            entity.Property(e => e.MaxCapacity)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("max_capacity");
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PRIMARY");

            entity.ToTable("services");

            entity.Property(e => e.ServiceId)
                .HasColumnType("int(11)")
                .HasColumnName("service_id");
            entity.Property(e => e.Availability)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("availability");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Duration)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("duration");
            entity.Property(e => e.Price)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("price");
            entity.Property(e => e.Rating)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("rating");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("service_name");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("service_type");
            entity.Property(e => e.StaffId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)")
                .HasColumnName("staff_id");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PRIMARY");

            entity.ToTable("staff");

            entity.Property(e => e.StaffId)
                .HasColumnType("int(11)")
                .HasColumnName("staff_id");
            entity.Property(e => e.DateHired)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_hired");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("department");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("phone_number");
            entity.Property(e => e.Position)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("position");
            entity.Property(e => e.Salary)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("salary");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Taxrate>(entity =>
        {
            entity.HasKey(e => e.TaxRateId).HasName("PRIMARY");

            entity.ToTable("taxrates");

            entity.Property(e => e.TaxRateId)
                .HasColumnType("int(11)")
                .HasColumnName("tax_rate_id");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("country");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_added");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EffectiveDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("effective_date");
            entity.Property(e => e.Rate)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("rate");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("state");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.TaxName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("tax_name");
        });

        modelBuilder.Entity<Useraccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("useraccounts");

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("date_updated");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("email");
            entity.Property(e => e.LastLogin)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("last_login");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("role");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("refreshtoken");
            entity.Property(e => e.RefreshTokenExpiryTime)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("refreshtokenexpirytime");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
