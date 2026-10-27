using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Model.OldCore;

public partial class MediatoriumSmartWasteContext : DbContext
{
    public MediatoriumSmartWasteContext()
    {
    }

    public MediatoriumSmartWasteContext(DbContextOptions<MediatoriumSmartWasteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<AlertDefn> AlertDefns { get; set; }

    public virtual DbSet<AlertEmail> AlertEmails { get; set; }

    public virtual DbSet<AlertEmailDefn> AlertEmailDefns { get; set; }

    public virtual DbSet<AlertPush> AlertPushes { get; set; }

    public virtual DbSet<AlertPushDefn> AlertPushDefns { get; set; }

    public virtual DbSet<AlertResult> AlertResults { get; set; }

    public virtual DbSet<AlertSm> AlertSms { get; set; }

    public virtual DbSet<AlertSmsDefn> AlertSmsDefns { get; set; }

    public virtual DbSet<AlertSmsResponse> AlertSmsResponses { get; set; }

    public virtual DbSet<Damage> Damages { get; set; }

    public virtual DbSet<DamageHistory> DamageHistories { get; set; }

    public virtual DbSet<Debug> Debugs { get; set; }

    public virtual DbSet<GuestDup> GuestDups { get; set; }

    public virtual DbSet<LatestReadingTime> LatestReadingTimes { get; set; }

    public virtual DbSet<LatestReadingsAll> LatestReadingsAlls { get; set; }

    public virtual DbSet<OsloReading> OsloReadings { get; set; }

    public virtual DbSet<OsloUnit> OsloUnits { get; set; }

    public virtual DbSet<PrometheusScrapeIndex> PrometheusScrapeIndexes { get; set; }

    public virtual DbSet<RawReading> RawReadings { get; set; }

    public virtual DbSet<RefAlertType> RefAlertTypes { get; set; }

    public virtual DbSet<RefAlertTypeLocale> RefAlertTypeLocales { get; set; }

    public virtual DbSet<RefBinLevel> RefBinLevels { get; set; }

    public virtual DbSet<RefBinLevelLocale> RefBinLevelLocales { get; set; }

    public virtual DbSet<RefBinType> RefBinTypes { get; set; }

    public virtual DbSet<RefBinTypeLocale> RefBinTypeLocales { get; set; }

    public virtual DbSet<RefContentType> RefContentTypes { get; set; }

    public virtual DbSet<RefContentTypeLocale> RefContentTypeLocales { get; set; }

    public virtual DbSet<RefCountry> RefCountries { get; set; }

    public virtual DbSet<RefCountryLocale> RefCountryLocales { get; set; }

    public virtual DbSet<RefDamageStatus> RefDamageStatuses { get; set; }

    public virtual DbSet<RefDamageStatusLocale> RefDamageStatusLocales { get; set; }

    public virtual DbSet<RefDamageType> RefDamageTypes { get; set; }

    public virtual DbSet<RefDamageTypeLocale> RefDamageTypeLocales { get; set; }

    public virtual DbSet<RefDeviceType> RefDeviceTypes { get; set; }

    public virtual DbSet<RefDeviceTypeLocale> RefDeviceTypeLocales { get; set; }

    public virtual DbSet<RefHardwareRevision> RefHardwareRevisions { get; set; }

    public virtual DbSet<RefLocale> RefLocales { get; set; }

    public virtual DbSet<RefLocaleLocale> RefLocaleLocales { get; set; }

    public virtual DbSet<RefRole> RefRoles { get; set; }

    public virtual DbSet<RefRolesLocale> RefRolesLocales { get; set; }

    public virtual DbSet<RefStatus> RefStatuses { get; set; }

    public virtual DbSet<RefStatusLocale> RefStatusLocales { get; set; }

    public virtual DbSet<RefUnitStatus> RefUnitStatuses { get; set; }

    public virtual DbSet<RefUnitStatusLocale> RefUnitStatusLocales { get; set; }

    public virtual DbSet<RouteName> RouteNames { get; set; }

    public virtual DbSet<RouteUnit> RouteUnits { get; set; }

    public virtual DbSet<SysConfig> SysConfigs { get; set; }

    public virtual DbSet<SysForgotPassword> SysForgotPasswords { get; set; }

    public virtual DbSet<TekelekMessage> TekelekMessages { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<UnitAction> UnitActions { get; set; }

    public virtual DbSet<UnitMessage> UnitMessages { get; set; }

    public virtual DbSet<UnitReading> UnitReadings { get; set; }

    public virtual DbSet<UnitReadingsFirmware> UnitReadingsFirmwares { get; set; }

    public virtual DbSet<UnitReadingsTemp> UnitReadingsTemps { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alerts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertDateTime)
                .HasComment("0 = new; 1=Being-processed; 2= processed")
                .HasColumnType("datetime")
                .HasColumnName("alertDateTime");
            entity.Property(e => e.AlertType).HasColumnName("alertType");
            entity.Property(e => e.Comments)
                .HasMaxLength(500)
                .HasComment("This is used for damage reports only")
                .HasColumnName("comments");
            entity.Property(e => e.DamageId).HasColumnName("damageId");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasColumnName("status");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
            entity.Property(e => e.UnitReadingId).HasColumnName("unitReadingId");
        });

        modelBuilder.Entity<AlertDefn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alert_defn");

            entity.HasIndex(e => new { e.CustomerId, e.UserId }, "idx_unique_alert_defn").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatteryOverTempLo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryOverTempLO");
            entity.Property(e => e.BatteryUvlo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryUVLO");
            entity.Property(e => e.BinEmptiedLastPeriod)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binEmptiedLastPeriod");
            entity.Property(e => e.BinFull)
                .HasColumnType("bit(1)")
                .HasColumnName("binFull");
            entity.Property(e => e.BinLocked)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binLocked");
            entity.Property(e => e.BinTilted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binTilted");
            entity.Property(e => e.CustomerId)
                .HasComment("This is the customer id of the bins been selected for alerting")
                .HasColumnName("customerId");
            entity.Property(e => e.Damage)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("damage");
            entity.Property(e => e.Email)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("email");
            entity.Property(e => e.FlapStuckOpen)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("flapStuckOpen");
            entity.Property(e => e.InsertBy).HasColumnName("insertBy");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.NotReporting)
                .HasColumnType("bit(1)")
                .HasColumnName("notReporting");
            entity.Property(e => e.Push)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("push");
            entity.Property(e => e.ServiceDoorOpen)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("serviceDoorOpen");
            entity.Property(e => e.Sms)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("sms");
            entity.Property(e => e.UserId)
                .HasComment("This is the user to be alerted when an alert is raised")
                .HasColumnName("userId");
            entity.Property(e => e.Whatsapp)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("whatsapp");
        });

        modelBuilder.Entity<AlertEmail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alert_email");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertId).HasColumnName("alertId");
            entity.Property(e => e.Body)
                .HasMaxLength(5000)
                .HasColumnName("body");
            entity.Property(e => e.Comment)
                .HasMaxLength(5000)
                .HasColumnName("comment");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.HtmlBody).HasColumnName("htmlBody");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.SentDateTime)
                .HasColumnType("datetime")
                .HasColumnName("sentDateTime");
            entity.Property(e => e.Status)
                .HasComment("0 = created, 1= sent, -1 = failed (reason will be in the comments field)")
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");
        });

        modelBuilder.Entity<AlertEmailDefn>(entity =>
        {
            entity.HasKey(e => new { e.AlertType, e.Locale }).HasName("PRIMARY");

            entity.ToTable("alert_email_defn");

            entity.Property(e => e.AlertType).HasColumnName("alertType");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Body)
                .HasMaxLength(5000)
                .HasColumnName("body");
            entity.Property(e => e.HtmlBody)
                .HasDefaultValueSql("'0'")
                .HasColumnName("htmlBody");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");
        });

        modelBuilder.Entity<AlertPush>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alert_push");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertId).HasColumnName("alertId");
            entity.Property(e => e.Body)
                .HasMaxLength(160)
                .HasColumnName("body");
            entity.Property(e => e.Comment)
                .HasMaxLength(5000)
                .HasColumnName("comment");
            entity.Property(e => e.GcmToken)
                .HasMaxLength(200)
                .HasColumnName("gcmToken");
            entity.Property(e => e.HttpResponseBody)
                .HasMaxLength(5000)
                .HasColumnName("httpResponseBody");
            entity.Property(e => e.HttpResponseCode).HasColumnName("httpResponseCode");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.ResponseDateTime)
                .HasColumnType("datetime")
                .HasColumnName("responseDateTime");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
        });

        modelBuilder.Entity<AlertPushDefn>(entity =>
        {
            entity.HasKey(e => new { e.AlertType, e.Locale }).HasName("PRIMARY");

            entity.ToTable("alert_push_defn");

            entity.Property(e => e.AlertType).HasColumnName("alertType");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Body)
                .HasMaxLength(160)
                .HasColumnName("body");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
        });

        modelBuilder.Entity<AlertResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alert_results");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertId).HasColumnName("alert_id");
            entity.Property(e => e.ContactDetail)
                .HasMaxLength(200)
                .HasComment("email, phone number etc..")
                .HasColumnName("contact_detail");
            entity.Property(e => e.ContactType)
                .HasComment("1 = email, 2 = sms, 3 = whatsapp, 4 = push")
                .HasColumnName("contact_type");
            entity.Property(e => e.FailReason)
                .HasMaxLength(500)
                .HasColumnName("fail_reason");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insert_date");
            entity.Property(e => e.Status)
                .HasComment("0 = success; -1 = failed")
                .HasColumnName("status");
        });

        modelBuilder.Entity<AlertSm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alert_sms");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertId).HasColumnName("alertId");
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .HasColumnName("comment");
            entity.Property(e => e.HttpResponseCode).HasColumnName("httpResponseCode");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.Message)
                .HasMaxLength(160)
                .HasColumnName("message");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(20)
                .HasColumnName("phoneNo");
            entity.Property(e => e.ResponseDateTime)
                .HasColumnType("datetime")
                .HasColumnName("responseDateTime");
            entity.Property(e => e.ResponseResultCode).HasColumnName("responseResultCode");
            entity.Property(e => e.ResponseResultDesc)
                .HasMaxLength(150)
                .HasColumnName("responseResultDesc");
            entity.Property(e => e.SmsCount).HasColumnName("smsCount");
            entity.Property(e => e.Status)
                .HasComment("0 = created, 1 = sent, 2 = DLR received")
                .HasColumnName("status");
        });

        modelBuilder.Entity<AlertSmsDefn>(entity =>
        {
            entity.HasKey(e => new { e.AlertType, e.Locale }).HasName("PRIMARY");

            entity.ToTable("alert_sms_defn");

            entity.Property(e => e.AlertType).HasColumnName("alertType");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Message)
                .HasMaxLength(300)
                .HasColumnName("message");
        });

        modelBuilder.Entity<AlertSmsResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alert_sms_response");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlertSmsId).HasColumnName("alertSmsId");
            entity.Property(e => e.Channel)
                .HasMaxLength(45)
                .HasColumnName("channel");
            entity.Property(e => e.Destination)
                .HasMaxLength(45)
                .HasColumnName("destination");
            entity.Property(e => e.DlrResponseDateTime)
                .HasColumnType("datetime")
                .HasColumnName("dlrResponseDateTime");
            entity.Property(e => e.DlrStatusCode).HasColumnName("dlrStatusCode");
            entity.Property(e => e.DlrStatusDetails)
                .HasMaxLength(45)
                .HasColumnName("dlrStatusDetails");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.MessageId)
                .HasMaxLength(50)
                .HasColumnName("messageId");
            entity.Property(e => e.Status)
                .HasComment("0 = inserted (after making API call and getting response), 1 = after receiving DLR report")
                .HasColumnName("status");
            entity.Property(e => e.StatusCode).HasColumnName("statusCode");
            entity.Property(e => e.StatusDesc)
                .HasMaxLength(45)
                .HasColumnName("statusDesc");
            entity.Property(e => e.Time)
                .HasMaxLength(45)
                .HasColumnName("time");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(45)
                .HasColumnName("transactionId");
        });

        modelBuilder.Entity<Damage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("damage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DamageTypeId).HasColumnName("damageTypeId");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<DamageHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("damage_history");

            entity.HasIndex(e => e.DamageId, "fk_damage_history_damage_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime")
                .HasColumnName("actionDate");
            entity.Property(e => e.ActionUserId).HasColumnName("actionUserId");
            entity.Property(e => e.AssignedTo).HasColumnName("assignedTo");
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .HasColumnName("comment");
            entity.Property(e => e.DamageId).HasColumnName("damageId");
            entity.Property(e => e.Image)
                .HasColumnType("mediumblob")
                .HasColumnName("image");
            entity.Property(e => e.Status)
                .HasComment("status references ref_damage_status = opened, assigned, closed")
                .HasColumnName("status");

            entity.HasOne(d => d.Damage).WithMany(p => p.DamageHistories)
                .HasForeignKey(d => d.DamageId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_damage_history_damage");
        });

        modelBuilder.Entity<Debug>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("debug");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DebugOutput)
                .HasMaxLength(1000)
                .HasColumnName("debug_output");
        });

        modelBuilder.Entity<GuestDup>(entity =>
        {
            entity.HasKey(e => e.GuestUnitId).HasName("PRIMARY");

            entity.ToTable("guest_dups");

            entity.Property(e => e.GuestUnitId).HasColumnName("guestUnitId");
            entity.Property(e => e.DupUnitId).HasColumnName("dupUnitId");
            entity.Property(e => e.GuestId).HasColumnName("guestId");
        });

        modelBuilder.Entity<LatestReadingTime>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("latest_reading_time");
        });

        modelBuilder.Entity<LatestReadingsAll>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("latest_readings_all");

            entity.Property(e => e.BatteryOverTempLo).HasColumnName("batteryOverTempLO");
            entity.Property(e => e.BatteryUvlo).HasColumnName("batteryUVLO");
            entity.Property(e => e.BatteryVoltage).HasColumnName("batteryVoltage");
            entity.Property(e => e.Ber).HasColumnName("ber");
            entity.Property(e => e.BinEmptiedLastPeriod).HasColumnName("binEmptiedLastPeriod");
            entity.Property(e => e.BinFull).HasColumnName("binFull");
            entity.Property(e => e.BinJustOn).HasColumnName("binJustOn");
            entity.Property(e => e.BinLevel).HasColumnName("binLevel");
            entity.Property(e => e.BinLevelBc).HasColumnName("binLevelBC");
            entity.Property(e => e.BinLocked).HasColumnName("binLocked");
            entity.Property(e => e.BinTilted).HasColumnName("binTilted");
            entity.Property(e => e.Firmware).HasColumnName("firmware");
            entity.Property(e => e.FlapStuckOpen).HasColumnName("flapStuckOpen");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InsertDateTime).HasColumnName("insertDateTime");
            entity.Property(e => e.MsgType).HasColumnName("msgType");
            entity.Property(e => e.NbIoTissue).HasColumnName("nbIoTIssue");
            entity.Property(e => e.NbIotSignalStrength).HasColumnName("nbIotSignalStrength");
            entity.Property(e => e.NoCompactions).HasColumnName("noCompactions");
            entity.Property(e => e.NoFlapOpenings).HasColumnName("noFlapOpenings");
            entity.Property(e => e.RawDataId).HasColumnName("rawDataId");
            entity.Property(e => e.ReadingDateTime).HasColumnName("readingDateTime");
            entity.Property(e => e.RegularPeriodicReporting).HasColumnName("regularPeriodicReporting");
            entity.Property(e => e.Rsrp).HasColumnName("rsrp");
            entity.Property(e => e.Rsrq).HasColumnName("rsrq");
            entity.Property(e => e.Rssi).HasColumnName("rssi");
            entity.Property(e => e.SerialNo).HasColumnName("serialNo");
            entity.Property(e => e.ServiceDoorClosed).HasColumnName("serviceDoorClosed");
            entity.Property(e => e.ServiceDoorOpen).HasColumnName("serviceDoorOpen");
            entity.Property(e => e.Snr).HasColumnName("snr");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.Src).HasColumnName("src");
            entity.Property(e => e.Temperature).HasColumnName("temperature");
            entity.Property(e => e.TimeDiff).HasColumnName("timeDiff");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<OsloReading>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("oslo_readings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlarmStatus)
                .HasDefaultValueSql("'0'")
                .HasColumnName("alarmStatus");
            entity.Property(e => e.BatteryOverTempLo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryOverTempLO");
            entity.Property(e => e.BatteryUvlo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryUVLO");
            entity.Property(e => e.BatteryVoltage)
                .HasDefaultValueSql("'0'")
                .HasColumnName("batteryVoltage");
            entity.Property(e => e.Ber)
                .HasComment("ber - SmartWaste units send this")
                .HasColumnName("ber");
            entity.Property(e => e.BinEmptiedLastPeriod)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binEmptiedLastPeriod");
            entity.Property(e => e.BinFull)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binFull");
            entity.Property(e => e.BinJustOn)
                .HasColumnType("bit(1)")
                .HasColumnName("binJustOn");
            entity.Property(e => e.BinLevel)
                .HasDefaultValueSql("'0'")
                .HasColumnName("binLevel");
            entity.Property(e => e.BinLevelBc)
                .HasDefaultValueSql("'0'")
                .HasColumnName("binLevelBC");
            entity.Property(e => e.BinLocked)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binLocked");
            entity.Property(e => e.BinTilted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binTilted");
            entity.Property(e => e.ContactReason)
                .HasDefaultValueSql("'0'")
                .HasColumnName("contactReason");
            entity.Property(e => e.Firmware)
                .HasMaxLength(20)
                .HasColumnName("firmware");
            entity.Property(e => e.FlapStuckOpen)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("flapStuckOpen");
            entity.Property(e => e.InsertDateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertDateTime");
            entity.Property(e => e.MsgType)
                .HasDefaultValueSql("'0'")
                .HasColumnName("msgType");
            entity.Property(e => e.NbIoTissue)
                .HasColumnType("bit(1)")
                .HasColumnName("nbIoTIssue");
            entity.Property(e => e.NbIotSignalStrength)
                .HasDefaultValueSql("'0'")
                .HasColumnName("nbIotSignalStrength");
            entity.Property(e => e.NoCompactions)
                .HasDefaultValueSql("'0'")
                .HasColumnName("noCompactions");
            entity.Property(e => e.NoFlapOpenings)
                .HasDefaultValueSql("'0'")
                .HasColumnName("noFlapOpenings");
            entity.Property(e => e.RawDataId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("rawDataId");
            entity.Property(e => e.ReadingDateTime)
                .HasColumnType("timestamp")
                .HasColumnName("readingDateTime");
            entity.Property(e => e.RegularPeriodicReporting)
                .HasColumnType("bit(1)")
                .HasColumnName("regularPeriodicReporting");
            entity.Property(e => e.Rsrp).HasColumnName("rsrp");
            entity.Property(e => e.Rsrq)
                .HasColumnType("double(18,6)")
                .HasColumnName("rsrq");
            entity.Property(e => e.Rssi)
                .HasComment("Received Signal Strength Indicator")
                .HasColumnType("double(18,6)")
                .HasColumnName("rssi");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(20)
                .HasColumnName("serialNo");
            entity.Property(e => e.ServiceDoorClosed)
                .HasDefaultValueSql("b'0'")
                .HasComment("No longer used but left here so it would not break old code")
                .HasColumnType("bit(1)")
                .HasColumnName("serviceDoorClosed");
            entity.Property(e => e.ServiceDoorOpen)
                .HasDefaultValueSql("b'0'")
                .HasComment("No longer used but left here so it would not break old code")
                .HasColumnType("bit(1)")
                .HasColumnName("serviceDoorOpen");
            entity.Property(e => e.Snr)
                .HasComment("Signal to Noise Ratio (Sigfox sends this)")
                .HasColumnType("double(18,6)")
                .HasColumnName("snr");
            entity.Property(e => e.Source)
                .HasMaxLength(45)
                .HasColumnName("source");
            entity.Property(e => e.Src)
                .HasComment("Sonic Result Code")
                .HasColumnName("src");
            entity.Property(e => e.Temperature)
                .HasDefaultValueSql("'0'")
                .HasColumnName("temperature");
            entity.Property(e => e.TimeDiff).HasColumnName("timeDiff");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<OsloUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("oslo_units");

            entity.HasIndex(e => e.Status, "idx_unit_status");

            entity.HasIndex(e => e.BinType, "idx_units_bintype");

            entity.HasIndex(e => e.ContentType, "idx_units_contenttype");

            entity.HasIndex(e => e.DeviceType, "idx_units_devicetype");

            entity.HasIndex(e => e.OwnerId, "idx_units_owner_id");

            entity.HasIndex(e => e.SerialNo, "serialNo_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BinJustOn)
                .HasColumnType("bit(1)")
                .HasColumnName("binJustOn");
            entity.Property(e => e.BinType).HasColumnName("binType");
            entity.Property(e => e.ContentType)
                .HasDefaultValueSql("'1'")
                .HasColumnName("contentType");
            entity.Property(e => e.DeviceType)
                .HasComment("1= PelBin Sensor; 2 = Tekelek Sensor")
                .HasColumnName("deviceType");
            entity.Property(e => e.EmptyLevel)
                .HasComment("Once bin level is over this, it should be flagged for emptying\n")
                .HasColumnName("emptyLevel");
            entity.Property(e => e.Firmware)
                .HasMaxLength(20)
                .HasColumnName("firmware");
            entity.Property(e => e.FullLevel).HasColumnName("fullLevel");
            entity.Property(e => e.HardwareRevision).HasColumnName("hardwareRevision");
            entity.Property(e => e.InsertBy).HasColumnName("insertBy");
            entity.Property(e => e.InsertDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.LastActivity)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("lastActivity");
            entity.Property(e => e.Latitude)
                .HasColumnType("double(12,6)")
                .HasColumnName("latitude");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.Longitude)
                .HasColumnType("double(12,6)")
                .HasColumnName("longitude");
            entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.NbIoTissue)
                .HasColumnType("bit(1)")
                .HasColumnName("nbIoTIssue");
            entity.Property(e => e.OwnerId).HasColumnName("ownerId");
            entity.Property(e => e.RegularPeriodicReporting)
                .HasColumnType("bit(1)")
                .HasColumnName("regularPeriodicReporting");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(20)
                .HasColumnName("serialNo");
            entity.Property(e => e.SigfoxAltId)
                .HasMaxLength(20)
                .HasComment("Some times the serial number is too large for sigfox so this is the code used to communicate over sigfox")
                .HasColumnName("sigfoxAltId");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasComment("0 = registered, 1 = installed, -1 = deactivate")
                .HasColumnName("status");
            entity.Property(e => e.TimeDiff).HasColumnName("timeDiff");
            entity.Property(e => e.UseBinTypeLevel)
                .HasDefaultValueSql("'1'")
                .HasComment(" = 0 - use emptyLevel of unit (this table)\\n = 1 - use emptyLevel of binType\\n")
                .HasColumnName("useBinTypeLevel");
        });

        modelBuilder.Entity<PrometheusScrapeIndex>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("prometheus_scrape_indexes");

            entity.Property(e => e.TableName).HasMaxLength(20);
        });

        modelBuilder.Entity<RawReading>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("raw_readings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InsertAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertAt");
            entity.Property(e => e.RawData)
                .HasColumnType("blob")
                .HasColumnName("rawData");
            entity.Property(e => e.Source)
                .HasMaxLength(45)
                .HasColumnName("source");
        });

        modelBuilder.Entity<RefAlertType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_alert_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefAlertTypeLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_alert_type_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefBinLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_bin_level");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefBinLevelLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_bin_level_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefBinType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_bin_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.EmptyLevel).HasColumnName("emptyLevel");
            entity.Property(e => e.FullLevel).HasColumnName("fullLevel");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefBinTypeLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_bin_type_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefContentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_content_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefContentTypeLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_content_type_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefCountry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_country");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Abbr)
                .HasMaxLength(3)
                .HasColumnName("abbr");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefCountryLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_country_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefDamageStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_damage_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefDamageStatusLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_damage_status_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefDamageType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_damage_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefDamageTypeLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_damage_type_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefDeviceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_device_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefDeviceTypeLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_device_type_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefHardwareRevision>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_hardware_revision");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefLocale>(entity =>
        {
            entity.HasKey(e => e.Abbr).HasName("PRIMARY");

            entity.ToTable("ref_locale");

            entity.Property(e => e.Abbr)
                .HasMaxLength(5)
                .HasColumnName("abbr");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefLocaleLocale>(entity =>
        {
            entity.HasKey(e => new { e.Abbr, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_locale_locale");

            entity.Property(e => e.Abbr)
                .HasMaxLength(5)
                .HasColumnName("abbr");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefRolesLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_roles_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_status");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefStatusLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_status_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefUnitStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ref_unit_status");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefUnitStatusLocale>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Locale }).HasName("PRIMARY");

            entity.ToTable("ref_unit_status_locale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasColumnName("locale");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RouteName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("route_names");

            entity.HasIndex(e => e.OwnerId, "fk_route_user_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InsertBy).HasColumnName("insertBy");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("ownerId");

            entity.HasOne(d => d.Owner).WithMany(p => p.RouteNames)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_route_user");
        });

        modelBuilder.Entity<RouteUnit>(entity =>
        {
            entity.HasKey(e => new { e.RouteId, e.RouteIndex }).HasName("PRIMARY");

            entity.ToTable("route_units");

            entity.HasIndex(e => e.UnitId, "fk_route_units_units_idx");

            entity.Property(e => e.RouteId).HasColumnName("routeId");
            entity.Property(e => e.RouteIndex).HasColumnName("routeIndex");
            entity.Property(e => e.UnitId).HasColumnName("unitId");

            entity.HasOne(d => d.Unit).WithMany(p => p.RouteUnits)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_route_units_units");
        });

        modelBuilder.Entity<SysConfig>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PRIMARY");

            entity.ToTable("sys_config");

            entity.HasIndex(e => e.Name, "name_UNIQUE").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.Value)
                .HasMaxLength(200)
                .HasColumnName("value");
        });

        modelBuilder.Entity<SysForgotPassword>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sys_forgot_password");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.RequestDate)
                .HasColumnType("datetime")
                .HasColumnName("requestDate");
            entity.Property(e => e.Uuid)
                .HasMaxLength(100)
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<TekelekMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tekelek_messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InsertBy).HasColumnName("insertBy");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.Message)
                .HasMaxLength(100)
                .HasColumnName("message");
            entity.Property(e => e.SentDate)
                .HasColumnType("datetime")
                .HasColumnName("sentDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("units", tb => tb.HasComment("These are the units on the bins - this table stores unit and bin specific data"));

            entity.HasIndex(e => e.Status, "idx_unit_status");

            entity.HasIndex(e => e.BinType, "idx_units_bintype");

            entity.HasIndex(e => e.ContentType, "idx_units_contenttype");

            entity.HasIndex(e => e.DeviceType, "idx_units_devicetype");

            entity.HasIndex(e => e.OwnerId, "idx_units_owner_id");

            entity.HasIndex(e => e.SerialNo, "serialNo_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BinJustOn)
                .HasColumnType("bit(1)")
                .HasColumnName("binJustOn");
            entity.Property(e => e.BinType).HasColumnName("binType");
            entity.Property(e => e.ContentType)
                .HasDefaultValueSql("'1'")
                .HasColumnName("contentType");
            entity.Property(e => e.DeviceType)
                .HasComment("1= PelBin Sensor; 2 = Tekelek Sensor")
                .HasColumnName("deviceType");
            entity.Property(e => e.EmptyLevel)
                .HasComment("Once bin level is over this, it should be flagged for emptying\n")
                .HasColumnName("emptyLevel");
            entity.Property(e => e.Firmware)
                .HasMaxLength(20)
                .HasColumnName("firmware");
            entity.Property(e => e.FullLevel).HasColumnName("fullLevel");
            entity.Property(e => e.HardwareRevision).HasColumnName("hardwareRevision");
            entity.Property(e => e.InsertBy).HasColumnName("insertBy");
            entity.Property(e => e.InsertDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.InstallationDate)
                .HasColumnType("datetime")
                .HasColumnName("installationDate");
            entity.Property(e => e.LastActivity)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("lastActivity");
            entity.Property(e => e.Latitude)
                .HasColumnType("double(12,6)")
                .HasColumnName("latitude");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.Longitude)
                .HasColumnType("double(12,6)")
                .HasColumnName("longitude");
            entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.NbIoTissue)
                .HasColumnType("bit(1)")
                .HasColumnName("nbIoTIssue");
            entity.Property(e => e.OwnerId).HasColumnName("ownerId");
            entity.Property(e => e.RegularPeriodicReporting)
                .HasColumnType("bit(1)")
                .HasColumnName("regularPeriodicReporting");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(20)
                .HasColumnName("serialNo");
            entity.Property(e => e.SigfoxAltId)
                .HasMaxLength(20)
                .HasDefaultValueSql("''")
                .HasColumnName("sigfoxAltId");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasComment("0 = registered, 1 = installed, -1 = deactivate")
                .HasColumnName("status");
            entity.Property(e => e.SubscriptionRenewalDate)
                .HasColumnType("datetime")
                .HasColumnName("subscriptionRenewalDate");
            entity.Property(e => e.TimeDiff).HasColumnName("timeDiff");
            entity.Property(e => e.UseBinTypeLevel)
                .HasDefaultValueSql("'1'")
                .HasComment(" = 0 - use emptyLevel of unit (this table)\\n = 1 - use emptyLevel of binType\\n")
                .HasColumnName("useBinTypeLevel");

            entity.HasOne(d => d.BinTypeNavigation).WithMany(p => p.Units)
                .HasForeignKey(d => d.BinType)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_units_bintype");

            entity.HasOne(d => d.ContentTypeNavigation).WithMany(p => p.Units)
                .HasForeignKey(d => d.ContentType)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_units_contenttype");

            entity.HasOne(d => d.DeviceTypeNavigation).WithMany(p => p.Units)
                .HasForeignKey(d => d.DeviceType)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_units_devicetype");

            entity.HasOne(d => d.Owner).WithMany(p => p.Units)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_units_owner");
        });

        modelBuilder.Entity<UnitAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("unit_actions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(45)
                .HasColumnName("action");
            entity.Property(e => e.ActionDateTime)
                .HasColumnType("timestamp")
                .HasColumnName("actionDateTime");
            entity.Property(e => e.ActionProcessed)
                .HasColumnType("timestamp")
                .HasColumnName("actionProcessed");
            entity.Property(e => e.ActionUserId).HasColumnName("actionUserId");
            entity.Property(e => e.InsertDateTime)
                .HasColumnType("timestamp")
                .HasColumnName("insertDateTime");
            entity.Property(e => e.Status)
                .HasComment("0 - Requested\\n1 - Completed")
                .HasColumnName("status");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<UnitMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("unit_messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InsertBy).HasColumnName("insertBy");
            entity.Property(e => e.InsertDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.Message)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("message");
            entity.Property(e => e.SentDate)
                .HasColumnType("datetime")
                .HasColumnName("sentDate");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasComment("0 = created\\n1 = sent")
                .HasColumnName("status");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<UnitReading>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("unit_readings");

            entity.HasIndex(e => e.SerialNo, "idx_unit_reading_serial_no");

            entity.HasIndex(e => new { e.UnitId, e.ReadingDateTime }, "idx_unit_reading_unit_id");

            entity.HasIndex(e => e.RawDataId, "idx_unit_readings_rawdata_id");

            entity.HasIndex(e => e.ReadingDateTime, "idx_unit_readings_readingDateTime");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlarmStatus)
                .HasDefaultValueSql("'0'")
                .HasColumnName("alarmStatus");
            entity.Property(e => e.BatteryOverTempLo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryOverTempLO");
            entity.Property(e => e.BatteryUvlo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryUVLO");
            entity.Property(e => e.BatteryVoltage)
                .HasDefaultValueSql("'0'")
                .HasColumnName("batteryVoltage");
            entity.Property(e => e.Ber)
                .HasComment("ber - SmartWaste units send this")
                .HasColumnName("ber");
            entity.Property(e => e.BinEmptiedLastPeriod)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binEmptiedLastPeriod");
            entity.Property(e => e.BinFull)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binFull");
            entity.Property(e => e.BinJustOn)
                .HasColumnType("bit(1)")
                .HasColumnName("binJustOn");
            entity.Property(e => e.BinLevel)
                .HasDefaultValueSql("'0'")
                .HasColumnName("binLevel");
            entity.Property(e => e.BinLevelBc)
                .HasDefaultValueSql("'0'")
                .HasColumnName("binLevelBC");
            entity.Property(e => e.BinLocked)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binLocked");
            entity.Property(e => e.BinTilted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binTilted");
            entity.Property(e => e.ContactReason)
                .HasDefaultValueSql("'0'")
                .HasColumnName("contactReason");
            entity.Property(e => e.Firmware)
                .HasMaxLength(20)
                .HasColumnName("firmware");
            entity.Property(e => e.FlapStuckOpen)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("flapStuckOpen");
            entity.Property(e => e.InsertDateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertDateTime");
            entity.Property(e => e.MsgType)
                .HasDefaultValueSql("'0'")
                .HasColumnName("msgType");
            entity.Property(e => e.NbIoTissue)
                .HasColumnType("bit(1)")
                .HasColumnName("nbIoTIssue");
            entity.Property(e => e.NbIotSignalStrength)
                .HasDefaultValueSql("'0'")
                .HasColumnName("nbIotSignalStrength");
            entity.Property(e => e.NoCompactions)
                .HasDefaultValueSql("'0'")
                .HasColumnName("noCompactions");
            entity.Property(e => e.NoFlapOpenings)
                .HasDefaultValueSql("'0'")
                .HasColumnName("noFlapOpenings");
            entity.Property(e => e.RawDataId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("rawDataId");
            entity.Property(e => e.ReadingDateTime)
                .HasColumnType("timestamp")
                .HasColumnName("readingDateTime");
            entity.Property(e => e.RegularPeriodicReporting)
                .HasColumnType("bit(1)")
                .HasColumnName("regularPeriodicReporting");
            entity.Property(e => e.Rsrp).HasColumnName("rsrp");
            entity.Property(e => e.Rsrq)
                .HasColumnType("double(18,6)")
                .HasColumnName("rsrq");
            entity.Property(e => e.Rssi)
                .HasComment("Received Signal Strength Indicator")
                .HasColumnType("double(18,6)")
                .HasColumnName("rssi");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(20)
                .HasColumnName("serialNo");
            entity.Property(e => e.ServiceDoorClosed)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("serviceDoorClosed");
            entity.Property(e => e.ServiceDoorOpen)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("serviceDoorOpen");
            entity.Property(e => e.Snr)
                .HasComment("Signal to Noise Ratio (Sigfox sends this)")
                .HasColumnType("double(18,6)")
                .HasColumnName("snr");
            entity.Property(e => e.Source)
                .HasMaxLength(45)
                .HasColumnName("source");
            entity.Property(e => e.Src)
                .HasComment("Sonic Result Code")
                .HasColumnName("src");
            entity.Property(e => e.Temperature)
                .HasDefaultValueSql("'0'")
                .HasColumnName("temperature");
            entity.Property(e => e.TimeDiff).HasColumnName("timeDiff");
            entity.Property(e => e.UnitId).HasColumnName("unitId");

            entity.HasOne(d => d.RawData).WithMany(p => p.UnitReadings)
                .HasForeignKey(d => d.RawDataId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_unitreadings_rawreadings");
        });

        modelBuilder.Entity<UnitReadingsFirmware>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("unit_readings_firmware");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BinJustOn)
                .HasColumnType("bit(1)")
                .HasColumnName("binJustOn");
            entity.Property(e => e.Firmware)
                .HasMaxLength(20)
                .HasColumnName("firmware");
            entity.Property(e => e.HardwareRevision).HasColumnName("hardwareRevision");
            entity.Property(e => e.InsertDate)
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.NbiotIssue)
                .HasColumnType("bit(1)")
                .HasColumnName("nbiotIssue");
            entity.Property(e => e.RawDataId).HasColumnName("rawDataId");
            entity.Property(e => e.RegularPeriodicReporting)
                .HasColumnType("bit(1)")
                .HasColumnName("regularPeriodicReporting");
            entity.Property(e => e.Source)
                .HasMaxLength(45)
                .HasColumnName("source");
            entity.Property(e => e.TimeDiff).HasColumnName("timeDiff");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<UnitReadingsTemp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("unit_readings_temp");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlarmStatus)
                .HasDefaultValueSql("'0'")
                .HasColumnName("alarmStatus");
            entity.Property(e => e.BatteryOverTempLo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryOverTempLO");
            entity.Property(e => e.BatteryUvlo)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("batteryUVLO");
            entity.Property(e => e.BatteryVoltage)
                .HasDefaultValueSql("'0'")
                .HasColumnName("batteryVoltage");
            entity.Property(e => e.Ber)
                .HasComment("ber - SmartWaste units send this")
                .HasColumnName("ber");
            entity.Property(e => e.BinEmptiedLastPeriod)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binEmptiedLastPeriod");
            entity.Property(e => e.BinFull)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binFull");
            entity.Property(e => e.BinJustOn)
                .HasColumnType("bit(1)")
                .HasColumnName("binJustOn");
            entity.Property(e => e.BinLevel)
                .HasDefaultValueSql("'0'")
                .HasColumnName("binLevel");
            entity.Property(e => e.BinLevelBc)
                .HasDefaultValueSql("'0'")
                .HasColumnName("binLevelBC");
            entity.Property(e => e.BinLocked)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binLocked");
            entity.Property(e => e.BinTilted)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("binTilted");
            entity.Property(e => e.ContactReason)
                .HasDefaultValueSql("'0'")
                .HasColumnName("contactReason");
            entity.Property(e => e.Firmware)
                .HasMaxLength(20)
                .HasColumnName("firmware");
            entity.Property(e => e.FlapStuckOpen)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("flapStuckOpen");
            entity.Property(e => e.InsertDateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertDateTime");
            entity.Property(e => e.MsgType)
                .HasDefaultValueSql("'0'")
                .HasColumnName("msgType");
            entity.Property(e => e.NbIoTissue)
                .HasColumnType("bit(1)")
                .HasColumnName("nbIoTIssue");
            entity.Property(e => e.NbIotSignalStrength)
                .HasDefaultValueSql("'0'")
                .HasColumnName("nbIotSignalStrength");
            entity.Property(e => e.NoCompactions)
                .HasDefaultValueSql("'0'")
                .HasColumnName("noCompactions");
            entity.Property(e => e.NoFlapOpenings)
                .HasDefaultValueSql("'0'")
                .HasColumnName("noFlapOpenings");
            entity.Property(e => e.RawDataId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("rawDataId");
            entity.Property(e => e.ReadingDateTime)
                .HasColumnType("timestamp")
                .HasColumnName("readingDateTime");
            entity.Property(e => e.RegularPeriodicReporting)
                .HasColumnType("bit(1)")
                .HasColumnName("regularPeriodicReporting");
            entity.Property(e => e.Rsrp).HasColumnName("rsrp");
            entity.Property(e => e.Rsrq)
                .HasColumnType("double(18,6)")
                .HasColumnName("rsrq");
            entity.Property(e => e.Rssi)
                .HasComment("Received Signal Strength Indicator")
                .HasColumnType("double(18,6)")
                .HasColumnName("rssi");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(20)
                .HasColumnName("serialNo");
            entity.Property(e => e.ServiceDoorClosed)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("serviceDoorClosed");
            entity.Property(e => e.ServiceDoorOpen)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("serviceDoorOpen");
            entity.Property(e => e.Snr)
                .HasComment("Signal to Noise Ratio (Sigfox sends this)")
                .HasColumnType("double(18,6)")
                .HasColumnName("snr");
            entity.Property(e => e.Source)
                .HasMaxLength(45)
                .HasColumnName("source");
            entity.Property(e => e.Src)
                .HasComment("Sonic Result Code")
                .HasColumnName("src");
            entity.Property(e => e.Temperature)
                .HasDefaultValueSql("'0'")
                .HasColumnName("temperature");
            entity.Property(e => e.TimeDiff).HasColumnName("timeDiff");
            entity.Property(e => e.UnitId).HasColumnName("unitId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Country, "fk_users_country_idx");

            entity.HasIndex(e => e.Role, "fk_users_roles_idx");

            entity.HasIndex(e => e.ParentId, "fk_users_users_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Addr1)
                .HasMaxLength(45)
                .HasColumnName("addr1");
            entity.Property(e => e.Addr2)
                .HasMaxLength(45)
                .HasColumnName("addr2");
            entity.Property(e => e.BinLevelAlert).HasColumnName("binLevelAlert");
            entity.Property(e => e.City)
                .HasMaxLength(45)
                .HasColumnName("city");
            entity.Property(e => e.Country).HasColumnName("country");
            entity.Property(e => e.County)
                .HasMaxLength(45)
                .HasColumnName("county");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.GcmToken)
                .HasMaxLength(200)
                .HasColumnName("gcmToken");
            entity.Property(e => e.HomeTel)
                .HasMaxLength(45)
                .HasColumnName("homeTel");
            entity.Property(e => e.InsertBy).HasColumnName("insertBy");
            entity.Property(e => e.InsertDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("insertDate");
            entity.Property(e => e.InstalationDate)
                .HasColumnType("datetime")
                .HasColumnName("instalationDate");
            entity.Property(e => e.LastActivity)
                .HasComment("updated when any of their bins have activity recorded in the database")
                .HasColumnType("datetime")
                .HasColumnName("lastActivity");
            entity.Property(e => e.LastLoggedIn)
                .HasColumnType("datetime")
                .HasColumnName("lastLoggedIn");
            entity.Property(e => e.Locale)
                .HasMaxLength(5)
                .HasDefaultValueSql("'en-IE'")
                .HasColumnName("locale");
            entity.Property(e => e.Mobile)
                .HasMaxLength(45)
                .HasColumnName("mobile");
            entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("This is only available to Municipal Type users - it is the level when the bin reachs that an email/sms alert is to be sent")
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.ParentId)
                .HasDefaultValueSql("'1'")
                .HasColumnName("parentId");
            entity.Property(e => e.Password)
                .HasMaxLength(45)
                .HasColumnName("password");
            entity.Property(e => e.Postcode)
                .HasMaxLength(45)
                .HasColumnName("postcode");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'4'")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasComment("-1 - Inactive (Locked out)\\\\n0 - Registered - not logged in yet\\\\n1 - Active (After first login - forced to change password and changes to this)\\\\n\\nIf we reset their password, we will reset this to 0 - forcing them to change their password the next they log in")
                .HasColumnName("status");
            entity.Property(e => e.SubRenewalDate)
                .HasColumnType("datetime")
                .HasColumnName("subRenewalDate");
            entity.Property(e => e.Suspended)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("suspended");
            entity.Property(e => e.WorkTel)
                .HasMaxLength(45)
                .HasColumnName("workTel");

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Country)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_country");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_users");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
