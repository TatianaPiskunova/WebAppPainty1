using Microsoft.AspNetCore.Identity;
using WebAppPainty1.Models;

namespace WebAppPainty1
{
    public class User:IdentityUser
    {
        public List<ImageFile>? Images { get; set; } = new();

    }
}
