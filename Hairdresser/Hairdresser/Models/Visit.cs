using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hairdresser.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string HairdresserName { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
    }
}
