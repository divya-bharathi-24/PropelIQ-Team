using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Auth.Api.Data.Entities;

public sealed class Role
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(256)]
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
