using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public int? ParentId { get; set; }

    public int RoleId { get; set; }

    public int? StatusId { get; set; }

    public int? LanguageId { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? PostCode { get; set; }

    public int? CountryId { get; set; }

    public string? PhoneNumber { get; set; }

    public int? BinLevelAlert { get; set; }

    public DateTime? LastLogin { get; set; }

    public int? CreateUserId { get; set; }

    public DateTime CreateDate { get; set; }

    public int? UpdateUserId { get; set; }

    public DateTime? UpdateDate { get; set; }

    public DateTime? SubscriptionRenewalDate { get; set; }

    public bool Suspended { get; set; }

    public bool Deleted { get; set; }

    public virtual Country? Country { get; set; }

    public virtual User? CreateUser { get; set; }

    public virtual ICollection<DamageHistory> DamageHistoryAssignedUsers { get; set; } = new List<DamageHistory>();

    public virtual ICollection<DamageHistory> DamageHistoryUsers { get; set; } = new List<DamageHistory>();

    public virtual ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();

    public virtual ICollection<User> InverseCreateUser { get; set; } = new List<User>();

    public virtual ICollection<User> InverseParent { get; set; } = new List<User>();

    public virtual ICollection<User> InverseUpdateUser { get; set; } = new List<User>();

    public virtual Language? Language { get; set; }

    public virtual ICollection<OneTimeToken> OneTimeTokens { get; set; } = new List<OneTimeToken>();

    public virtual User? Parent { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Route> RouteCreateUsers { get; set; } = new List<Route>();

    public virtual ICollection<Route> RouteUpdateUsers { get; set; } = new List<Route>();

    public virtual ICollection<Route> RouteUsers { get; set; } = new List<Route>();

    public virtual Status? Status { get; set; }

    public virtual ICollection<Unit> UnitCreateUsers { get; set; } = new List<Unit>();

    public virtual ICollection<Unit> UnitUpdateUsers { get; set; } = new List<Unit>();

    public virtual ICollection<Unit> UnitUsers { get; set; } = new List<Unit>();

    public virtual User? UpdateUser { get; set; }

    public virtual ICollection<UserAlert> UserAlertCustomerUsers { get; set; } = new List<UserAlert>();

    public virtual ICollection<UserAlert> UserAlertUsers { get; set; } = new List<UserAlert>();
}
