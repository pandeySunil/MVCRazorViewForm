using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RegistrationForm.Models
{
    public class UserJsonModel
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public string DOB { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }

        public string state { get; set; }
      
        public string skills { get; set; }
        public string country { get; set; }
        public  string imagePath { get; set; }






    }
}