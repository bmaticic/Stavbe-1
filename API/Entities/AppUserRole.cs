using System;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUserRole : IdentityUserRole<int>
{
    public AppUser User { get; set; } = null!; // Required for EF Core to recognize the relationship
    public AppRole Role { get; set; } = null!; // Required for EF Core to recognize the relationship

}
