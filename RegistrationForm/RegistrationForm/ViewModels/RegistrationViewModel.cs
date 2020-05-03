using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegistrationForm.Models;

namespace RegistrationForm.ViewModels
{
    public class RegistrationViewModel
    {
        public User User { get; set; }
        public List<Skill> Skills { get; set; }
        public SelectList States { get; set; }
        public SelectList Countries { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFilePath { get; set; }
        public List<Country> conList { get; set; }
        public List<State> stateList { get; set; }
        public string DOBString { get; set; }
        public double noOfPages { get; set; }
    }
}