using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeapSession.Model
{
    public class UserProfile
    {
        public string name { get; set; }
        public string Description { get; set; }
        public DateTime CallbackTime { get; set; }
        public string Bug { get; set; }
        public string PhoneNumber { get; set; }
    }
}
