using Microsoft.AspNetCore.Identity;

namespace Sanatik.Models;

public class Like
{
    public int Id { get; set; }
    public int PhotoId { get; set; }
    public Photo Photo { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}
