using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClassLib.Models;

public partial class DbSwpVaccineTrackingFinalContext : DbContext
{
    public DbSwpVaccineTrackingFinalContext()
    {
    }

    public DbSwpVaccineTrackingFinalContext(DbContextOptions<DbSwpVaccineTrackingFinalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vaccine> Vaccines { get; set; }

    public virtual DbSet<VaccinesCombo> VaccinesCombos { get; set; }

    public virtual DbSet<VaccinesTracking> VaccinesTrackings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //=> optionsBuilder.UseSqlServer("data source=TieHung\\SQLEXPRESS;initial catalog=DB_SWP_Vaccine_Tracking;user id=sa;password=123456;TrustServerCertificate=True");
    ////=> optionsBuilder.UseSqlServer("Server=LAPTOP-8UGAAJKM\\SQLEXPRESS01;Database=DB_SWP_Vaccine_Tracking;User Id=sa;Password=12345;TrustServerCertificate=True;");
        //=> optionsBuilder.UseSqlServer("Server=LAPTOP-8UGAAJKM\\SQLEXPRESS01;Database=DB_SWP_Vaccine_Tracking_Final;User Id=sa;Password=12345;TrustServerCertificate=True;");
        => optionsBuilder.UseSqlServer("data source=TieHung\\SQLEXPRESS;initial catalog=DB_SWP_Vaccine_Tracking_Final;user id=sa;password=123456;TrustServerCertificate=True");
    //=> optionsBuilder.UseSqlServer("data source=DESKTOP-LIE3GLO\\SQLEXPRESS;initial catalog=DB_SWP_Vaccine_Tracking_Final;user id=sa;password=123456;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Address__3213E83FBC7F4543");

            entity.ToTable("Address");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3213E83F78EEE450");

            entity.ToTable("Booking");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdvisoryDetails)
                .HasMaxLength(255)
                .HasColumnName("advisory_details");
            entity.Property(e => e.ArrivedAt)
                .HasColumnType("datetime")
                .HasColumnName("arrived_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.Parent).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_parent_id_foreign");

            entity.HasMany(d => d.Children).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingChildId",
                    r => r.HasOne<Child>().WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("booking_childid_child_id_foreign"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("booking_childid_booking_id_foreign"),
                    j =>
                    {
                        j.HasKey("BookingId", "ChildId").HasName("booking_childid_pk");
                        j.ToTable("Booking_ChildID");
                        j.IndexerProperty<int>("BookingId").HasColumnName("booking_id");
                        j.IndexerProperty<int>("ChildId").HasColumnName("child_id");
                    });

            entity.HasMany(d => d.Combos).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingCombo",
                    r => r.HasOne<VaccinesCombo>().WithMany()
                        .HasForeignKey("ComboId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("booking_combo_combo_id_foreign"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("booking_combo_booking_id_foreign"),
                    j =>
                    {
                        j.HasKey("BookingId", "ComboId").HasName("booking_combo_pk");
                        j.ToTable("Booking_Combo");
                        j.IndexerProperty<int>("BookingId").HasColumnName("booking_id");
                        j.IndexerProperty<int>("ComboId").HasColumnName("combo_id");
                    });

            entity.HasMany(d => d.Vaccines).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingVaccine",
                    r => r.HasOne<Vaccine>().WithMany()
                        .HasForeignKey("VaccineId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("booking_vaccine_vaccine_id_foreign"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("booking_vaccine_booking_id_foreign"),
                    j =>
                    {
                        j.HasKey("BookingId", "VaccineId").HasName("booking_vaccine_pk");
                        j.ToTable("Booking_Vaccine");
                        j.IndexerProperty<int>("BookingId").HasColumnName("booking_id");
                        j.IndexerProperty<int>("VaccineId").HasColumnName("vaccine_id");
                    });
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Child__3213E83F5E9E2CBB");

            entity.ToTable("Child");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ParentId).HasColumnName("parent_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.Parent).WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("child_parent_id_foreign");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Feedback");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.RatingScore).HasColumnName("rating_score");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedback_user_id_foreign");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EAE761EBF6");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(255)
                .HasColumnName("payment_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Currency)
                .HasMaxLength(255)
                .HasColumnName("currency");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.PayerId)
                .HasMaxLength(255)
                .HasColumnName("payer_id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");

            entity.HasOne(d => d.PaymentMethodNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethod)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payment_payment_method_foreign");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment___3213E83FC0444889");

            entity.ToTable("Payment_Method");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Refresh___3213E83FEB750D50");

            entity.ToTable("Refresh_Token");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(1024)
                .HasColumnName("access_token");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("expired_at");
            entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
            entity.Property(e => e.IsUsed).HasColumnName("is_used");
            entity.Property(e => e.IssuedAt)
                .HasColumnType("datetime")
                .HasColumnName("issued_at");
            entity.Property(e => e.RefreshToken1)
                .HasMaxLength(255)
                .HasColumnName("refresh_token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refresh_token_user_id_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83FB7BE2B6C");

            entity.ToTable("User");

            entity.HasIndex(e => e.Gmail, "user_gmail_unique").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "user_phone_number_unique").IsUnique();

            entity.HasIndex(e => e.Username, "user_username_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Gmail)
                .HasMaxLength(255)
                .HasColumnName("gmail");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vaccines__3213E83F2797F412");

            entity.ToTable("Vaccines");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_ID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DoesTimes).HasColumnName("does_times");
            entity.Property(e => e.EntryDate)
                .HasColumnType("datetime")
                .HasColumnName("entry_date");
            entity.Property(e => e.FromCountry)
                .HasMaxLength(255)
                .HasColumnName("from_country");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MaximumIntervalDate).HasColumnName("maximum_interval_date");
            entity.Property(e => e.MinimumIntervalDate).HasColumnName("minimum_interval_date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.SuggestAgeMax).HasColumnName("suggest_age_max");
            entity.Property(e => e.SuggestAgeMin).HasColumnName("suggest_age_min");
            entity.Property(e => e.TimeExpired)
                .HasColumnType("datetime")
                .HasColumnName("time_expired");

            entity.HasOne(d => d.Address).WithMany(p => p.Vaccines)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vaccines_address_id_foreign");
        });

        modelBuilder.Entity<VaccinesCombo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vaccines__3213E83FCA5A42D7");

            entity.ToTable("Vaccines_Combo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComboName)
                .HasMaxLength(255)
                .HasColumnName("combo_name");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.FinalPrice)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("final_price");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("total_price");

            entity.HasMany(d => d.Vaccines).WithMany(p => p.VacineCombos)
                .UsingEntity<Dictionary<string, object>>(
                    "VaccinesComboVaccine",
                    r => r.HasOne<Vaccine>().WithMany()
                        .HasForeignKey("VaccineId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("vaccinescombo_vaccines_vaccine_id_foreign"),
                    l => l.HasOne<VaccinesCombo>().WithMany()
                        .HasForeignKey("VacineCombo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("vaccinescombo_vaccines_vacine_combo_foreign"),
                    j =>
                    {
                        j.HasKey("VacineCombo", "VaccineId").HasName("vaccinescombo_vaccines_pk");
                        j.ToTable("VaccinesCombo_Vaccines");
                        j.IndexerProperty<int>("VacineCombo").HasColumnName("vacine_combo");
                        j.IndexerProperty<int>("VaccineId").HasColumnName("vaccine_id");
                    });
        });

        modelBuilder.Entity<VaccinesTracking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vaccines__3213E83F77900388");

            entity.ToTable("Vaccines_Tracking");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdministeredBy).HasColumnName("administered_by");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MaximumIntervalDate)
                .HasColumnType("datetime")
                .HasColumnName("maximum_interval_date");
            entity.Property(e => e.MinimumIntervalDate)
                .HasColumnType("datetime")
                .HasColumnName("minimum_interval_date");
            entity.Property(e => e.PreviousVaccination).HasColumnName("previous_vaccination");
            entity.Property(e => e.Reaction)
                .HasMaxLength(255)
                .HasColumnName("reaction");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VaccinationDate)
                .HasColumnType("datetime")
                .HasColumnName("vaccination_date");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.VaccinesTrackings)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vaccines_tracking_booking_id_foreign");

            entity.HasOne(d => d.Child).WithMany(p => p.VaccinesTrackings)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("vaccines_tracking_child_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.VaccinesTrackings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("vaccines_tracking_user_id_foreign");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.VaccinesTrackings)
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vaccines_tracking_vaccine_id_foreign");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
