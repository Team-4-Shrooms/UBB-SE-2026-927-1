using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Logic.Models
{
    public class ActiveSale
    {
        public int Id { get; set; }

        public Movie Movie { get; set; }

        public decimal DiscountPercentage { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsExpired() => DateTime.Now > EndTime;
    }
}
