using Microsoft.AspNetCore.Identity;
using System;

namespace AutoROFL.Models
{
    public class User : IdentityUser
    {
        public string FName { get; set; }
        public string SName { get; set; }
        public string MName { get; set; }
        public DateTime Year { get; set; }
        public string Adress { get; set; }
    }
}
