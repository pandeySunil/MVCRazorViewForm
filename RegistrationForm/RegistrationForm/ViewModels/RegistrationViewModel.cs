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

    }
}