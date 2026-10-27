using SmartWaste.Model.DataTransferObjects.WebConfiguration;

namespace SmartWaste.Model.DataTransferObjects.Account;

public class UserDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public int? ParentId { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? PostCode { get; set; }

    public int? CountryId { get; set; }

    public string? PhoneNumber { get; set; }

    public int? BinLevelAlert { get; set; }

    public bool Suspended { get; set; }

    public bool Deleted { get; set; }

    public virtual CountryDto? Country { get; set; }

    public virtual LanguageDto? Language { get; set; }

    public virtual RoleDto Role { get; set; } = null!;

    public virtual StatusDto? Status { get; set; }

    public virtual ShortUserDto? Parent { get; set; }
}

public class ShortUserDto
{
    public int Id { get; set; } 
    public string? Name { get; set; }
    public string? Email { get; set; }
}