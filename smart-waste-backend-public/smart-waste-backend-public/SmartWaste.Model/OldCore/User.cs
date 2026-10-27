using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class User
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? Role { get; set; }

    public int? ParentId { get; set; }

    /// <summary>
    /// -1 - Inactive (Locked out)\\n0 - Registered - not logged in yet\\n1 - Active (After first login - forced to change password and changes to this)\\n\nIf we reset their password, we will reset this to 0 - forcing them to change their password the next they log in
    /// </summary>
    public int? Status { get; set; }

    public string? Locale { get; set; }

    public string? Name { get; set; }

    public string? Addr1 { get; set; }

    public string? Addr2 { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? Postcode { get; set; }

    public int? Country { get; set; }

    public string? Mobile { get; set; }

    public string? HomeTel { get; set; }

    public string? WorkTel { get; set; }

    public int? BinLevelAlert { get; set; }

    public DateTime? LastLoggedIn { get; set; }

    /// <summary>
    /// updated when any of their bins have activity recorded in the database
    /// </summary>
    public DateTime? LastActivity { get; set; }

    public int? InsertBy { get; set; }

    public DateTime? InsertDate { get; set; }

    public int? ModifiedBy { get; set; }

    /// <summary>
    /// This is only available to Municipal Type users - it is the level when the bin reachs that an email/sms alert is to be sent
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    public string? GcmToken { get; set; }

    public DateTime? InstalationDate { get; set; }

    public DateTime? SubRenewalDate { get; set; }

    public ulong? Suspended { get; set; }

    public virtual RefCountry? CountryNavigation { get; set; }

    public virtual ICollection<User> InverseParent { get; set; } = new List<User>();

    public virtual User? Parent { get; set; }

    public virtual RefRole? RoleNavigation { get; set; }

    public virtual ICollection<RouteName> RouteNames { get; set; } = new List<RouteName>();

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
