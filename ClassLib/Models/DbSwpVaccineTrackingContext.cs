using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Models;

public partial class DbSwpVaccineTrackingContext : DbContext
{
    public DbSwpVaccineTrackingContext()
    {
    }

    public DbSwpVaccineTrackingContext(DbContextOptions<DbSwpVaccineTrackingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingChildId> BookingChildIds { get; set; }

    public virtual DbSet<BookingComboId> BookingComboIds { get; set; }

    public virtual DbSet<BookingIdVaccineId> BookingIdVaccineIds { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vaccine> Vaccines { get; set; }

    public virtual DbSet<VaccinesCombo> VaccinesCombos { get; set; }

    public virtual DbSet<VaccinesTracking> VaccinesTrackings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("Server=LAPTOP-8UGAAJKM\\SQLEXPRESS01;Database=DB_SWP_Vaccine_Tracking;User Id=sa;Password=12345;TrustServerCertificate=True;");
        => optionsBuilder.UseSqlServer( "data source=TieHung\\SQLEXPRESS;initial catalog=DB_SWP_Vaccine_Tracking;user id=sa;password=123456;TrustServerCertificate=True" );
        //=> optionsBuilder.UseSqlServer("data source=DESKTOP-LIE3GLO\\SQLEXPRESS;initial catalog=DB_SWP_Vaccine_Tracking2;user id=sa;password=123456;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("address_id_primary");

            entity.ToTable("address");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("booking_id_primary");

            entity.ToTable("booking");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AdvisoryDetails)
                .HasMaxLength(255)
                .HasColumnName("advisory_details");
            entity.Property(e => e.ArrivedAt)
                .HasColumnType("datetime")
                .HasColumnName("arrived-at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.Parent).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_parent_id_foreign");
        });

        modelBuilder.Entity<BookingChildId>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Booking_ChildID");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ChildId).HasColumnName("child_id");

            entity.HasOne(d => d.Booking).WithMany()
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_childid_booking_id_foreign");

            entity.HasOne(d => d.Child).WithMany()
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_childid_child_id_foreign");
        });

        modelBuilder.Entity<BookingComboId>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Booking_ComboId");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");

            entity.HasOne(d => d.Booking).WithMany()
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_comboid_booking_id_foreign");

            entity.HasOne(d => d.Combo).WithMany()
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_comboid_combo_id_foreign");
        });

        modelBuilder.Entity<BookingIdVaccineId>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BookingId_VaccineId");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Booking).WithMany()
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingid_vaccineid_booking_id_foreign");

            entity.HasOne(d => d.Vaccine).WithMany()
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingid_vaccineid_vaccine_id_foreign");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("child_id_primary");

            entity.ToTable("child");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.Gender).HasColumnName("gender");
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

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("payment_booking_id_primary");

            entity.ToTable("payment");

            entity.Property(e => e.BookingId)
                .ValueGeneratedNever()
                .HasColumnName("booking_id");
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

            entity.HasOne(d => d.Booking).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payment_booking_id_foreign");

            entity.HasOne(d => d.PaymentMethodNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethod)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payment_payment_method_foreign");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_method_id_primary");

            entity.ToTable("payment_method");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Decription)
                .HasMaxLength(255)
                .HasColumnName("decription");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refreshtoken_id_primary");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(255)
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
                .HasConstraintName("refreshtoken_user_id_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_id_primary");

            entity.ToTable("user");

            entity.HasIndex(e => e.PhoneNumber, "user_phone_number_unique").IsUnique();

            entity.HasIndex(e => e.Username, "user_username_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
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
            entity.HasKey(e => e.Id).HasName("vaccine_id_primary");

            entity.ToTable("vaccine");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
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
                .HasConstraintName("vaccine_address_id_foreign");
        });

        modelBuilder.Entity<VaccinesCombo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vaccines_combo_id_primary");

            entity.ToTable("vaccines_combo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ComboName)
                .HasMaxLength(255)
                .HasColumnName("combo_name");
            entity.Property(e => e.Disount).HasColumnName("disount");
            entity.Property(e => e.FinalPrice)
                .HasColumnType("decimal(16, 2)")
                .HasColumnName("final_price");
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
                        j.HasKey("VacineCombo", "VaccineId").HasName("vaccinescombo_vaccines_primary");
                        j.ToTable("vaccinesCombo_vaccines");
                        j.IndexerProperty<int>("VacineCombo").HasColumnName("vacine_combo");
                        j.IndexerProperty<int>("VaccineId").HasColumnName("vaccine_id");
                    });
        });

        modelBuilder.Entity<VaccinesTracking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vaccines_tracking_id_primary");

            entity.ToTable("vaccines_tracking");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AdministeredBy).HasColumnName("administered_by");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.MaximumIntervalDate)
                .HasColumnType("datetime")
                .HasColumnName("maximum_interval_date");
            entity.Property(e => e.MinimumIntervalDate)
                .HasColumnType("datetime")
                .HasColumnName("minimum_interval_date");
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

            entity.HasOne(d => d.AdministeredByNavigation).WithMany(p => p.VaccinesTrackingAdministeredByNavigations)
                .HasForeignKey(d => d.AdministeredBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vaccines_tracking_administered_by_foreign");

            entity.HasOne(d => d.Child).WithMany(p => p.VaccinesTrackings)
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vaccines_tracking_child_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.VaccinesTrackingUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
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
