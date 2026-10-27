using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Collections.Generic;

namespace SmartWaste.Model.Constants
{
    public static class DatabaseConstants
    {
        public static readonly RoleConstant Role = new();
        public static readonly StatusConstant Status = new();
        public static readonly AlertTypeCategoryConstant AlertTypeCategory = new();
        public static readonly UnitStatusConstant UnitStatus = new();
        public static readonly DamageStatusConstant DamageStatus = new();
        public static readonly UnitFullnessStatusConstant UnitFullnessStatus = new();
        public static readonly LanguageConstant Language = new();
        public static readonly AlertTypeConstant AlertType = new();
        public static readonly UnitCategoryConstant UnitCategory = new();
        public static readonly UnitTypeConstant UnitType = new();
    }

    public class RoleConstantConstants
    {
        public const string Admin = "Admin";
        public const int AdminRoleId = 1;
        public const int DistributorRoleId = 2;
        public const int CorporateCustomerRoleId = 3;
        public const int CustomerRoleId = 4;
        public const int DriverRoleId = 5;
        public const int TechnicianRoleId = 6;

    }

    public class RoleConstant
    {
        public DbShortData Admin { get; } = new DbShortData { Id = 1, Name = "Admin" };
        public DbShortData Distributor { get; } = new DbShortData { Id = 2, Name = "Distributor" };
        public DbShortData CorporateCustomer { get; } = new DbShortData { Id = 3, Name = "CorporateCustomer" };
        public DbShortData Customer { get; } = new DbShortData { Id = 4, Name = "Customer" };
        public DbShortData Driver { get; } = new DbShortData { Id = 5, Name = "Driver" };
        public DbShortData Technician { get; } = new DbShortData { Id = 6, Name = "Technician" };
    }

    public class AlertTypeConstant
    {
        public DbShortData BatteryUVLO { get; } = new DbShortData { Id = 1, Name = "Battery UVLO" };
        public DbShortData EmptiedLastPeriod { get; } = new DbShortData { Id = 2, Name = "Emptied Last Period" };
        public DbShortData BatteryOverTemperature { get; } = new DbShortData { Id = 3, Name = "Battery Over Temperature" };
        public DbShortData BinLocked { get; } = new DbShortData { Id = 4, Name = "Bin Locked" };
        public DbShortData BinTilted { get; } = new DbShortData { Id = 5, Name = "Bin Tilted" };
        public DbShortData ServiceDoorStuckOpen { get; } = new DbShortData { Id = 6, Name = "Service Door Stuck Open" };
        public DbShortData FlapStuckOpen { get; } = new DbShortData { Id = 7, Name = "Flap Stuck Open" };
        public DbShortData BinFull { get; } = new DbShortData { Id = 8, Name = "Bin Full" };
        public DbShortData DamageReport { get; } = new DbShortData { Id = 9, Name = "Damage Report" };
        public DbShortData DeviceNotReporting { get; } = new DbShortData { Id = 10, Name = "Device Not Reporting" };
        public DbShortData SMS { get; } = new DbShortData { Id = 11, Name = "SMS" };
        public DbShortData Email { get; } = new DbShortData { Id = 12, Name = "Email" };
        public DbShortData Push { get; } = new DbShortData { Id = 13, Name = "Push" };
    }
  
    public class StatusConstant
    {
        public DbShortData Registered { get; } = new DbShortData { Id = 1, Name = "Registered" };
        public DbShortData Active { get; } = new DbShortData { Id = 2, Name = "Active" };
        public DbShortData Inactive { get; } = new DbShortData { Id = 3, Name = "Inactive" };
    }
    
    public class UnitTypeConstant
    {
        public DbShortData NonCompacting { get; } = new DbShortData { Id = 1, Name = "Non-Compacting" };
        public DbShortData Compacting { get; } = new DbShortData { Id = 2, Name = "Compacting" };
    }

    public class UnitStatusConstant
    {
        public DbShortData Inactive { get; } = new DbShortData { Id = 9, Name = "Inactive" };
        public DbShortData Registered { get; } = new DbShortData { Id = 10, Name = "Registered" };
        public DbShortData Active { get; } = new DbShortData { Id = 11, Name = "Active" };
    }

    public class DamageStatusConstant
    {
        public DbShortData Open { get; } = new DbShortData { Id = 4, Name = "Open" };
        public DbShortData Assigned { get; } = new DbShortData { Id = 5, Name = "Assigned" };
        public DbShortData Closed { get; } = new DbShortData { Id = 6, Name = "Closed" };
    }
    public class UnitFullnessStatusConstant
    {
        public DbShortData Small { get; } = new DbShortData { Id = 12, Name = "Small" };
        public DbShortData Medium { get; } = new DbShortData { Id = 13, Name = "Medium" };
        public DbShortData Critical { get; } = new DbShortData { Id = 14, Name = "Critical" };
    }
    public class UnitCategoryConstant
    {
        public DbShortData Install { get; } = new DbShortData { Id = 1, Name = "Install" };
    }

    public class AlertTypeCategoryConstant
    {
        public DbShortData Warnings { get; } = new DbShortData { Id = 1, Name = "Warnings" };
        public DbShortData Send { get; } = new DbShortData { Id = 2, Name = "Send" };
    }

    public class LanguageConstant
    {
        public DbShortData English { get; } = new DbShortData { Id = 1, Name = "English" };
        public DbShortData German { get; } = new DbShortData { Id = 2, Name = "German" };
        public DbShortData French { get; } = new DbShortData { Id = 3, Name = "French" };
        public DbShortData Croatian { get; } = new DbShortData { Id = 4, Name = "Croatian" };
    }

    public class DbShortData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
