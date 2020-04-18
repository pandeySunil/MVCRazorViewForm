using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegistrationForm.Models;
using RegistrationForm.ViewModels;

namespace RegistrationForm.Controllers
{
    public class RegisterController : Controller
    {
        // GET: RegisterController
        private readonly ApplicationDbContext  _db;
        public RegisterController() {
            _db = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            var c = new List<TestCheckBox>{new TestCheckBox{ISCheck=true}};
            return View("SkillList", c);
        }
        public ActionResult LoadRegistrationForm()
        {
            var registrationViewModelObj = new RegistrationForm.ViewModels.RegistrationViewModel();
            registrationViewModelObj.Countries = new SelectList(GetAllCountries(), "Id", "CountryName");
            registrationViewModelObj.States = new SelectList(GetAllStates(), "Id", "StateName");
            registrationViewModelObj.Skills = _db.Skill.ToList();
            DateTime? today = DateTime.Today;
            var U = new User();
            registrationViewModelObj.User = U;
            registrationViewModelObj.User.DOB = DateTime.Now;
            return View("RegistrationForm", registrationViewModelObj);
        }
        public ActionResult SkillList(IEnumerable<RegistrationForm.Models.Skill> s) { 
        return View();
        }
        public ActionResult RegisterUser(RegistrationForm.ViewModels.RegistrationViewModel registrationViewModelObj )
        {
            User usr = new User();
            if(registrationViewModelObj!=null){
                usr.CountryId = registrationViewModelObj.User.CountryId;
                usr.StateId = registrationViewModelObj.User.StateId;
                usr.FullName = registrationViewModelObj.User.FullName;
                usr.DOB = registrationViewModelObj.User.DOB;
                usr.EmailAddress = registrationViewModelObj.User.EmailAddress;
                usr.MobileNo = registrationViewModelObj.User.MobileNo;
            }
            


            return Redirect("LoadRegistrationForm");
        }

        private bool SaveUSer(User usr){



            return true;
        }
        private List<Country> GetAllCountries() {
            List<Country> countries = new List<Country>();
            if (_db != null)
            {
                countries = _db.Country.ToList();
            }
            return countries;
        }
        private List<State> GetAllStates()
        {
            List<State> states = new List<State>();
            if(_db!=null){
                states = _db.State.ToList();
            }
            return states;
        }

        public ActionResult GetStates(int Id)
        {
            var states = GetAllStates().Where(s => s.CountryId == Id).ToList();
            return Json(new { data = states }, JsonRequestBehavior.AllowGet);
        }
    }
}