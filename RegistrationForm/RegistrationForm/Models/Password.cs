using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RegistrationForm.Models
{
    public class Password
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PWord { get; set; }

    }
}