using System;

namespace VegaAsis.Core.DTOs
{
    public class UserDataDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string Role { get; set; }
    }
}
