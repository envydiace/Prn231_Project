using System;
using System.Collections.Generic;

namespace Prm231_Project.Models
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string RefreshToken1 { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
