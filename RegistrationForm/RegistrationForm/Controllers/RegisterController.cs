using System;
using System.Collections.Generic;
using System.IO;
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
        public ActionResult LoadRegistrationForm(int? UsrId=19 )
        {
            
            var registrationViewModelObj = new RegistrationForm.ViewModels.RegistrationViewModel();
            if (UsrId != null && UsrId > 0)
            {
                var usr = _db.User.FirstOrDefault(u=>u.Id==UsrId);
                 registrationViewModelObj = new RegistrationForm.ViewModels.RegistrationViewModel();
                registrationViewModelObj.Countries = new SelectList(GetAllCountries(usr.CountryId), "Id", "CountryName");
                registrationViewModelObj.States = new SelectList(GetAllStates().Where(s=>s.Id==usr.StateId).ToList(), "Id", "StateName");
                registrationViewModelObj.Skills = _db.Skill.ToList();
                registrationViewModelObj.User = usr;
                registrationViewModelObj.User.DOB = DateTime.Now;
                var Img = _db.Image.FirstOrDefault(img => img.UserId == UsrId).ImageName;
                registrationViewModelObj.ImageFilePath = Img;
            }
            else {

                registrationViewModelObj.Countries = new SelectList(GetAllCountries(), "Id", "CountryName");
                registrationViewModelObj.States = new SelectList(GetAllStates(), "Id", "StateName");
                registrationViewModelObj.Skills = _db.Skill.ToList();
                DateTime? today = DateTime.Today;
                var U = new User();
                registrationViewModelObj.User = U;
                registrationViewModelObj.User.DOB = DateTime.Now;

            }
            return View("RegistrationForm", registrationViewModelObj);
        }
        public ActionResult SkillList(IEnumerable<RegistrationForm.Models.Skill> s) { 
        return View();
        }
        public ActionResult RegisterUser(RegistrationForm.ViewModels.RegistrationViewModel registrationViewModelObj )
        {
            User usr = new User();
           _db.Database.BeginTransaction();
            var trns = _db.Database.CurrentTransaction;
            try {
              
                if (registrationViewModelObj != null)
                {
                    usr.CountryId = registrationViewModelObj.User.CountryId;
                    usr.StateId = registrationViewModelObj.User.StateId;
                    usr.FullName = registrationViewModelObj.User.FullName;
                    usr.DOB = registrationViewModelObj.User.DOB;
                    usr.EmailAddress = registrationViewModelObj.User.EmailAddress;
                    usr.MobileNo = registrationViewModelObj.User.MobileNo;
                }
                _db.User.Add(usr);
                Image image = new Image();
                var imgNameGuid = Guid.NewGuid();
                if (registrationViewModelObj?.ImageFile != null) {
                    image = new Image { UserId = usr.Id, ImageName = registrationViewModelObj.ImageFile.FileName };
                }
                _db.Image.Add(image);
                
                var diskSaveFileName = registrationViewModelObj.ImageFile.FileName;
                SaveImageOnDisk(registrationViewModelObj.ImageFile, diskSaveFileName);
                _db.SaveChanges();
                trns.Commit();
                return LoadRegistrationForm(usr.Id);
            }

            catch (Exception Ex) {
                trns.Rollback();
            
            }



            return LoadRegistrationForm();

        }

        private void SaveImageOnDisk(HttpPostedFileBase ImageFile,string diskFileSaveName)
        {

            if (ImageFile!=null&&ImageFile.ContentLength > 0)
            {
                
                ImageFile.SaveAs(@"G:\RegistrationFormMVCProject\MVCRazorViewForm\RegistrationForm\RegistrationForm\Content\ProfileImages\" + diskFileSaveName);
               // ViewBag.Message = "File uploaded successfully.";
            }

        }
        private bool SaveUSer(User usr){



            return true;
        }
        private List<Country> GetAllCountries(int? Id=0 ) {
            List<Country> countries = new List<Country>();
            if (Id != 0) {
                if (_db != null)
                {
                    countries = _db.Country.Where(c=>c.Id==Id).ToList();
                }
            }
            else {
                if (_db != null)
                {

                    countries = _db.Country.ToList();
                }
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

        public ActionResult GetStates(int? Id)
        {
            var states = GetAllStates().Where(s => s.CountryId == Id).ToList();
            return Json(new { data = states }, JsonRequestBehavior.AllowGet);
        }
    }
}