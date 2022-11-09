using System;
using System.Collections.Generic;

namespace Prm231_Project.Models
{
    public partial class Account
    {
        public Account()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public int AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public int? Role { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
