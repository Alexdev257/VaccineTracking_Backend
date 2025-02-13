using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vaccine> Vaccines { get; set; }

    public virtual DbSet<VaccinesCombo> VaccinesCombos { get; set; }

    public virtual DbSet<VaccinesTracking> VaccinesTrackings { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=LAPTOP-8UGAAJKM\\SQLEXPRESS01;Database=DB_SWP_Vaccine_Tracking;User Id=sa;Password=12345;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__address__3213E83FE9DAF757");

            entity.ToTable("address");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__booking__3213E83F519F9275");

            entity.ToTable("booking");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AdvisoryDetail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("advisory_detail");
            entity.Property(e => e.ArrivedAt).HasColumnName("arrived_at");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");

            entity.HasOne(d => d.Child).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking__child_i__5BE2A6F2");

            entity.HasOne(d => d.Parent).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking__parent___5AEE82B9");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__booking___3213E83FD9EA057B");

            entity.ToTable("booking_detail");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking_d__booki__6477ECF3");

            entity.HasOne(d => d.Combo).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking_d__combo__656C112C");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking_d__vacci__66603565");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__child__3213E83F3E6C6ED0");

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
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Parent).WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__child__parent_id__5812160E");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payment__3213E83FAD891056");

            entity.ToTable("payment");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payment__booking__6B24EA82");

            entity.HasOne(d => d.PaymentMethodNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethod)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payment__payment__6C190EBB");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payment___3213E83F2AE02970");

            entity.ToTable("payment_method");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83F3B19AFCE");

            entity.ToTable("user");

            entity.HasIndex(e => e.PhoneNumber, "UQ__user__A1936A6B580D4C20").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC572F0818FBC").IsUnique();

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
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vaccine__3213E83FD80BDDE6");

            entity.ToTable("vaccine");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.DoesTimes).HasColumnName("does_times");
            entity.Property(e => e.EntryDate)
                .HasColumnType("datetime")
                .HasColumnName("entry_date");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.MaxiumIntervalDate).HasColumnName("maxium_interval_date");
            entity.Property(e => e.MiniumIntervalDate).HasColumnName("minium_interval_date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SuggestAgeMax).HasColumnName("suggest_age_max");
            entity.Property(e => e.SuggestAgeMin).HasColumnName("suggest_age_min");
            entity.Property(e => e.TimeExpired)
                .HasColumnType("datetime")
                .HasColumnName("time_expired");

            entity.HasOne(d => d.Address).WithMany(p => p.Vaccines)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vaccine__address__4F7CD00D");
        });

        modelBuilder.Entity<VaccinesCombo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vaccines__3213E83F90127458");

            entity.ToTable("vaccines_combo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ComboName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("combo_name");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");

            entity.HasMany(d => d.Vaccines).WithMany(p => p.VaccineCombos)
                .UsingEntity<Dictionary<string, object>>(
                    "VaccinesComboVaccine",
                    r => r.HasOne<Vaccine>().WithMany()
                        .HasForeignKey("VaccineId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__vaccinesC__vacci__5535A963"),
                    l => l.HasOne<VaccinesCombo>().WithMany()
                        .HasForeignKey("VaccineComboId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__vaccinesC__vacci__5441852A"),
                    j =>
                    {
                        j.HasKey("VaccineComboId", "VaccineId").HasName("PK__vaccines__0ECC8CE4F58C105B");
                        j.ToTable("vaccinesCombo_vaccines");
                        j.IndexerProperty<int>("VaccineComboId").HasColumnName("vaccine_combo_id");
                        j.IndexerProperty<int>("VaccineId").HasColumnName("vaccine_id");
                    });
        });

        modelBuilder.Entity<VaccinesTracking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vaccines__3213E83F071CB1F6");

            entity.ToTable("vaccines_tracking");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AdministeredBy).HasColumnName("administered_by");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.MaxiumIntervalDate)
                .HasColumnType("datetime")
                .HasColumnName("maxium_interval_date");
            entity.Property(e => e.MiniumIntervalDate)
                .HasColumnType("datetime")
                .HasColumnName("minium_interval_date");
            entity.Property(e => e.Reaction)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reaction");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VaccinationDate)
                .HasColumnType("datetime")
                .HasColumnName("vaccination_date");
            entity.Property(e => e.VaccinesId).HasColumnName("vaccines_id");

            entity.HasOne(d => d.AdministeredByNavigation).WithMany(p => p.VaccinesTrackingAdministeredByNavigations)
                .HasForeignKey(d => d.AdministeredBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vaccines___admin__619B8048");

            entity.HasOne(d => d.Child).WithMany(p => p.VaccinesTrackings)
                .HasForeignKey(d => d.ChildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vaccines___child__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.VaccinesTrackingUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vaccines___user___5FB337D6");

            entity.HasOne(d => d.Vaccines).WithMany(p => p.VaccinesTrackings)
                .HasForeignKey(d => d.VaccinesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vaccines___vacci__5EBF139D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
