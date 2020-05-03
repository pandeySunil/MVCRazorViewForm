using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RegistrationForm.ViewModels
{
    public class Login
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int AuthLevel { get; set; }
    }
}