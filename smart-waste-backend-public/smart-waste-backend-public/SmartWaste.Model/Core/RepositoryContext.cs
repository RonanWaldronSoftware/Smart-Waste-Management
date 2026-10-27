using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Model.Core;

public partial class RepositoryContext : DbContext
{
    public RepositoryContext()
    {
    }

    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertType> AlertTypes { get; set; }

    public virtual DbSet<AlertTypeCategory> AlertTypeCategories { get; set; }

    public virtual DbSet<ContentType> ContentTypes { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Damage> Damages { get; set; }

    public virtual DbSet<DamageHistory> DamageHistories { get; set; }

    public virtual DbSet<DamageType> DamageTypes { get; set; }

    public virtual DbSet<DeviceInputType> DeviceInputTypes { get; set; }

    public virtual DbSet<DeviceToken> DeviceTokens { get; set; }

    public virtual DbSet<DeviceType> DeviceTypes { get; set; }

    public virtual DbSet<HardwareRevision> HardwareRevisions { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<LatestUnitReading> LatestUnitReadings { get; set; }

    public virtual DbSet<OneTimeToken> OneTimeTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<RouteUnit> RouteUnits { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<StatusCategory> StatusCategories { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<UnitCategory> UnitCategories { get; set; }

    public virtual DbSet<UnitImage> UnitImages { get; set; }

    public virtual DbSet<UnitModelType> UnitModelTypes { get; set; }

    public virtual DbSet<UnitReading> UnitReadings { get; set; }

    public virtual DbSet<UnitType> UnitTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAlert> UserAlerts { get; set; }

    public virtual DbSet<UserAlertSend> UserAlertSends { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AlertTyp__3214EC071BB23369");

            entity.ToTable("AlertType");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.AlertTypeCategory).WithMany(p => p.AlertTypes)
                .HasForeignKey(d => d.AlertTypeCategoryId)
                .HasConstraintName("FK_AlertType_AlertTypeCategory");
        });

        modelBuilder.Entity<AlertTypeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AlertTyp__3214EC077CE2292B");

            entity.ToTable("AlertTypeCategory");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ContentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentT__3214EC07A5EB25F9");

            entity.ToTable("ContentType");

            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Country__3214EC07AAE6549A");

            entity.ToTable("Country");

            entity.Property(e => e.Code)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(64);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Damage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Damage__3214EC075B78E291");

            entity.ToTable("Damage");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DamageType).WithMany(p => p.Damages)
                .HasForeignKey(d => d.DamageTypeId)
                .HasConstraintName("FK_Damage_DamageType");

            entity.HasOne(d => d.Status).WithMany(p => p.Damages)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Damage_Status");

            entity.HasOne(d => d.Unit).WithMany(p => p.Damages)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_Damage_Unit");
        });

        modelBuilder.Entity<DamageHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DamageHi__3214EC0744A44F93");

            entity.ToTable("DamageHistory");

            entity.Property(e => e.Comment).HasMaxLength(512);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.DamageHistoryAssignedUsers)
                .HasForeignKey(d => d.AssignedUserId)
                .HasConstraintName("FK_DamageHistory_AssignedUser");

            entity.HasOne(d => d.Damage).WithMany(p => p.DamageHistories)
                .HasForeignKey(d => d.DamageId)
                .HasConstraintName("FK_DamageHistory_Damage");

            entity.HasOne(d => d.Status).WithMany(p => p.DamageHistories)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_DamageHistory_Status");

            entity.HasOne(d => d.User).WithMany(p => p.DamageHistoryUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_DamageHistory_User");
        });

        modelBuilder.Entity<DamageType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DamageTy__3214EC0781304AA5");

            entity.ToTable("DamageType");

            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DeviceInputType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DeviceIn__3214EC07CF2831C0");

            entity.ToTable("DeviceInputType");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DeviceToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DeviceTo__3214EC0742383DBC");

            entity.ToTable("DeviceToken");

            entity.Property(e => e.DeviceToken1)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("DeviceToken");

            entity.HasOne(d => d.DeviceType).WithMany(p => p.DeviceTokens)
                .HasForeignKey(d => d.DeviceTypeId)
                .HasConstraintName("FK_DeviceToken_DeviceType");

            entity.HasOne(d => d.User).WithMany(p => p.DeviceTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_DeviceToken_User");
        });

        modelBuilder.Entity<DeviceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DeviceTy__3214EC073E76D8DE");

            entity.ToTable("DeviceType");

            entity.Property(e => e.Name)
                .HasMaxLength(8)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HardwareRevision>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hardware__3214EC075D39F829");

            entity.ToTable("HardwareRevision");

            entity.Property(e => e.Name).HasMaxLength(64);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Language__3214EC07F5A784DE");

            entity.ToTable("Language");

            entity.Property(e => e.Code).HasMaxLength(4);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(64);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LatestUnitReading>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LatestUnitRead__3214EC07A9B8AC71");

            entity.ToTable("LatestUnitReading");

            entity.Property(e => e.BatteryOverTempLo).HasColumnName("BatteryOverTempLO");
            entity.Property(e => e.BatteryUvlo).HasColumnName("BatteryUVLO");
            entity.Property(e => e.BinLevelBc).HasColumnName("BinLevelBC");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Firmware).HasMaxLength(32);
            entity.Property(e => e.NbloTissue).HasColumnName("NbloTIssue");
            entity.Property(e => e.ReadingDate).HasColumnType("datetime");
            entity.Property(e => e.Rssi).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.Source).HasMaxLength(64);

            entity.HasOne(d => d.FullnessStatus).WithMany(p => p.LatestUnitReadings)
                .HasForeignKey(d => d.FullnessStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LatestStatus_FullnessStatusId");

            entity.HasOne(d => d.Unit).WithMany(p => p.LatestUnitReadings)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_LatestUnitReadings_Unit");
        });

        modelBuilder.Entity<OneTimeToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OneTimeT__3214EC0756935B3A");

            entity.ToTable("OneTimeToken");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.OneTimeTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OneTimeToken_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07A56AE7C1");

            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(64);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Route__3214EC07330BC682");

            entity.ToTable("Route", tb => tb.HasTrigger("Route_UpdateDateTrigger"));

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.RouteCreateUsers)
                .HasForeignKey(d => d.CreateUserId)
                .HasConstraintName("FK_Route_CreateUser");

            entity.HasOne(d => d.UpdateUser).WithMany(p => p.RouteUpdateUsers)
                .HasForeignKey(d => d.UpdateUserId)
                .HasConstraintName("FK_Route_UpdateUser");

            entity.HasOne(d => d.User).WithMany(p => p.RouteUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Route_User");
        });

        modelBuilder.Entity<RouteUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RouteUni__3214EC07EF6B059B");

            entity.HasOne(d => d.Route).WithMany(p => p.RouteUnits)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("FK_RouteUnits_Route");

            entity.HasOne(d => d.Unit).WithMany(p => p.RouteUnits)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_RouteUnits_Unit");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3214EC076BA9C82E");

            entity.ToTable("Status");

            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.StatusCategory).WithMany(p => p.Statuses)
                .HasForeignKey(d => d.StatusCategoryId)
                .HasConstraintName("FK_Status_StatusCategory");
        });

        modelBuilder.Entity<StatusCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatusCa__3214EC07C1B7472E");

            entity.ToTable("StatusCategory");

            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Unit__3214EC073DE7F59A");

            entity.ToTable("Unit", tb => tb.HasTrigger("Unit_UpdateDateTrigger"));

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FirmWare).HasMaxLength(32);
            entity.Property(e => e.InstallationDate).HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasColumnType("decimal(12, 6)");
            entity.Property(e => e.Location).HasMaxLength(128);
            entity.Property(e => e.LockUntil).HasColumnType("datetime");
            entity.Property(e => e.Longitude).HasColumnType("decimal(12, 6)");
            entity.Property(e => e.NbloTissue).HasColumnName("NbloTIssue");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.SigFoxAltId).HasMaxLength(32);
            entity.Property(e => e.SubscriptionRenewalDate).HasColumnType("datetime");
            entity.Property(e => e.SuperfyGuid)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.ContentType).WithMany(p => p.Units)
                .HasForeignKey(d => d.ContentTypeId)
                .HasConstraintName("FK_Unit_ContentType");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.UnitCreateUsers)
                .HasForeignKey(d => d.CreateUserId)
                .HasConstraintName("FK_Unit_CreateUser");

            entity.HasOne(d => d.DeviceInputType).WithMany(p => p.Units)
                .HasForeignKey(d => d.DeviceInputTypeId)
                .HasConstraintName("FK_Unit_DeviceInputType");

            entity.HasOne(d => d.Status).WithMany(p => p.Units)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Unit_Status");

            entity.HasOne(d => d.UnitModelType).WithMany(p => p.Units)
                .HasForeignKey(d => d.UnitModelTypeId)
                .HasConstraintName("FK_Unit_UnitModelType");

            entity.HasOne(d => d.UnitType).WithMany(p => p.Units)
                .HasForeignKey(d => d.UnitTypeId)
                .HasConstraintName("FK_Unit_UnitType");

            entity.HasOne(d => d.UpdateUser).WithMany(p => p.UnitUpdateUsers)
                .HasForeignKey(d => d.UpdateUserId)
                .HasConstraintName("FK_Unit_UpdateUser");

            entity.HasOne(d => d.User).WithMany(p => p.UnitUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Unit_User");
        });

        modelBuilder.Entity<UnitCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UnitCate__3214EC07ADADD972");

            entity.ToTable("UnitCategory");

            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UnitImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UnitImag__3214EC07A00253F8");

            entity.ToTable("UnitImage");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.UnitCategory).WithMany(p => p.UnitImages)
                .HasForeignKey(d => d.UnitCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnitImage_UnitCategory");

            entity.HasOne(d => d.Unit).WithMany(p => p.UnitImages)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnitImage_Unit");
        });

        modelBuilder.Entity<UnitModelType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UnitMode__3214EC07A263B399");

            entity.ToTable("UnitModelType");

            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UnitReading>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UnitRead__3214EC07C728E01D");

            entity.HasIndex(e => new { e.UnitId, e.CreateDate }, "IX_UnitReadings_UnitId_CreatedAt");

            entity.Property(e => e.BatteryOverTempLo).HasColumnName("BatteryOverTempLO");
            entity.Property(e => e.BatteryUvlo).HasColumnName("BatteryUVLO");
            entity.Property(e => e.BinLevelBc).HasColumnName("BinLevelBC");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Firmware).HasMaxLength(32);
            entity.Property(e => e.NbloTissue).HasColumnName("NbloTIssue");
            entity.Property(e => e.ReadingDate).HasColumnType("datetime");
            entity.Property(e => e.Rssi).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.Source).HasMaxLength(64);

            entity.HasOne(d => d.FullnessStatus).WithMany(p => p.UnitReadings)
                .HasForeignKey(d => d.FullnessStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Status_FullnessStatusId");

            entity.HasOne(d => d.Unit).WithMany(p => p.UnitReadings)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_UnitReadings_Unit");
        });

        modelBuilder.Entity<UnitType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UnitType__3214EC07D3D3787C");

            entity.ToTable("UnitType");

            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.TranslationKey)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC073BB9B0BC");

            entity.ToTable("User", tb => tb.HasTrigger("User_UpdateDateTrigger"));

            entity.Property(e => e.Address1).HasMaxLength(128);
            entity.Property(e => e.Address2).HasMaxLength(128);
            entity.Property(e => e.City).HasMaxLength(64);
            entity.Property(e => e.County).HasMaxLength(64);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(64);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(64);
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PostCode)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.SubscriptionRenewalDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_User_Country");

            entity.HasOne(d => d.CreateUser).WithMany(p => p.InverseCreateUser)
                .HasForeignKey(d => d.CreateUserId)
                .HasConstraintName("FK_User_CreateUser");

            entity.HasOne(d => d.Language).WithMany(p => p.Users)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK_User_Language");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_User_UserParent");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_User_Status");

            entity.HasOne(d => d.UpdateUser).WithMany(p => p.InverseUpdateUser)
                .HasForeignKey(d => d.UpdateUserId)
                .HasConstraintName("FK_User_UpdateUser");
        });

        modelBuilder.Entity<UserAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAler__3214EC07924E570C");

            entity.ToTable("UserAlert");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AlertType).WithMany(p => p.UserAlerts)
                .HasForeignKey(d => d.AlertTypeId)
                .HasConstraintName("FK_UserAlerts_AlertType");

            entity.HasOne(d => d.CustomerUser).WithMany(p => p.UserAlertCustomerUsers)
                .HasForeignKey(d => d.CustomerUserId)
                .HasConstraintName("FK_UserAlerts_CustomerUser");

            entity.HasOne(d => d.User).WithMany(p => p.UserAlertUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserAlerts_User");
        });

        modelBuilder.Entity<UserAlertSend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAler__3214EC07E3D565FE");

            entity.ToTable("UserAlertSend");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Unit).WithMany(p => p.UserAlertSends)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK__UserAlert__UnitI__6E565CE8");

            entity.HasOne(d => d.UserAlert).WithMany(p => p.UserAlertSends)
                .HasForeignKey(d => d.UserAlertId)
                .HasConstraintName("FK__UserAlert__UserA__6D6238AF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
