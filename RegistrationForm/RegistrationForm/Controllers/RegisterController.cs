using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegistrationForm.Models;
using RegistrationForm.ViewModels;
using RegistrationForm.Filters;

namespace RegistrationForm.Controllers
{
    public class RegisterController : Controller
    {
        // GET: RegisterController
        private readonly ApplicationDbContext  _db;
        private List<RegistrationViewModel> userList;
        private double numberOdUsers;
        private double noOfPages;
        private int skipRecords;
        public RegisterController() {
            _db = new ApplicationDbContext();
            ViewBag.shortOrder = ViewBag.shortOrder == ""|| ViewBag.shortOrder ==null ? "UP" : ViewBag.shortOrder;
            ViewBag.shortedColumByOrder = ViewBag.shortedColumByOrder == "" || ViewBag.shortedColumByOrder == null ? "FullName_UP" : ViewBag.shortedColumByOrder;
            ViewBag.skippedRecords = ViewBag.skippedRecords == null ? 0 : ViewBag.skippedRecords;
            ViewBag.noOfPages = ViewBag.noOfPages == null || ViewBag.noOfPages == 0 ? 1 : ViewBag.noOfPages;

        }

        [AuthFilter]
        [HttpGet]
        public ActionResult CheckIfAuthanticated() 
        {
            var loginUser = new Login();

            return View("Login", loginUser);
        }
      
        public ActionResult Login(Login loginUsr)
        {

            return View("Login", loginUsr);
        }

        public ActionResult Index()
        {
            var c = new List<TestCheckBox>{new TestCheckBox{ISCheck=true}};
            return View("LoadUserGridV2");
            
        }
        public ActionResult LoadRegistrationForm(int? UsrId=0 )
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
                DateTime.TryParse(usr.DOB.ToString(), out DateTime date);
                registrationViewModelObj.DOBString = date.ToString("yyyy-MM-dd");
               

                var Img = _db.Image.FirstOrDefault(img => img.UserId == UsrId).ImageName;
                registrationViewModelObj.ImageFilePath = Img;
                registrationViewModelObj.conList = GetAllCountries().Where(c => c.Id == usr.CountryId).ToList();
                registrationViewModelObj.stateList = GetAllStates().Where(s => s.CountryId == registrationViewModelObj.conList[0].Id).ToList();
            }
            else {

                registrationViewModelObj.Countries = new SelectList(GetAllCountries(), "Id", "CountryName");
                registrationViewModelObj.States = new SelectList(GetAllStates(), "Id", "StateName");
                registrationViewModelObj.Skills = _db.Skill.ToList();
                DateTime? today = DateTime.Today;
                var U = new User();
                registrationViewModelObj.User = U;
                registrationViewModelObj.User.DOB = DateTime.Now;
                registrationViewModelObj.conList = GetAllCountries();
                registrationViewModelObj.stateList = GetAllStates().Where(s => s.CountryId == registrationViewModelObj.conList[0].Id).ToList();

            }
            return View("RegistrationFormV3", registrationViewModelObj);
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

        public ActionResult JumpOnPageNo(int pageNo) {

            ViewBag.skippedRecords = (pageNo - 1) * 5;
            OrderBy("", ViewBag.shortedColumByOrder);
            return View("LoadUserGrid", ShortListBasedOnColumn(userList, ViewBag.shortedColumByOrder, ViewBag.shortOrder));

        }

        public ActionResult OrderBy(string ColumnName,string sortOrder) {
            ViewBag.shortedColumByOrder = sortOrder;

            if (sortOrder.Substring(sortOrder.IndexOf("_")+1) == "UP") {
                ViewBag.shortOrder = "DOWN";
            }
            else {
                ViewBag.shortOrder = "UP";
            }
           
            LoadUserGrid(ColumnName);
            int skip = Convert.ToInt32(ViewBag.skippedRecords);
            userList = userList?.Skip(skip).Take(5).ToList();

            return View("LoadUserGrid", ShortListBasedOnColumn(userList, sortOrder, ViewBag.shortOrder));
        }

        private List<RegistrationViewModel> ShortListBasedOnColumn(List<RegistrationViewModel> RegistrationViewModellist,string orderBy, string shortOrder) {
           
            switch (orderBy) {
                case "FullName_UP":
                    RegistrationViewModellist = RegistrationViewModellist.OrderBy(l => l.User.FullName).ToList();
                    break;
                case "FullName_DOWN":
                    RegistrationViewModellist = RegistrationViewModellist.OrderByDescending(l => l.User.FullName).ToList();
                    break;
                case "DateOfBirth_DOWN":
                    RegistrationViewModellist = RegistrationViewModellist.OrderByDescending(l => l.User.DOB).ToList();
                    break;
                case "DateOfBirth_UP":
                    RegistrationViewModellist = RegistrationViewModellist.OrderBy(l => l.User.DOB).ToList();
                    break;
                case "EmailAddress_DOWN":
                    RegistrationViewModellist = RegistrationViewModellist.OrderByDescending(l => l.User.EmailAddress).ToList();
                    break;
                case "EmailAddress_UP":
                    RegistrationViewModellist = RegistrationViewModellist.OrderBy(l => l.User.EmailAddress).ToList();
                    break;
                case "MobileNo_DOWN":
                    RegistrationViewModellist = RegistrationViewModellist.OrderByDescending(l => l.User.MobileNo).ToList();
                    break;
                case "MobileNo_UP":
                    RegistrationViewModellist = RegistrationViewModellist.OrderBy(l => l.User.MobileNo).ToList();
                    break;
                case "Country_DOWN":
                    RegistrationViewModellist = RegistrationViewModellist.OrderByDescending(l => l.User.Country.CountryName).ToList();
                    break;
                case "Country_UP":
                    RegistrationViewModellist = RegistrationViewModellist.OrderBy(l => l.User.Country.CountryName).ToList();
                    break;
                case "State_DOWN":
                    RegistrationViewModellist = RegistrationViewModellist.OrderByDescending(l => l.User.State.StateName).ToList();
                    break;
                case "State_UP":
                    RegistrationViewModellist = RegistrationViewModellist.OrderBy(l => l.User.State.StateName).ToList();
                    break;

            }
                

            return RegistrationViewModellist;
        }

        public ActionResult getNext()
        {
            OrderBy("", ViewBag.shortedColumByOrder);
            if (ViewBag.skippedRecords + 5 <= numberOdUsers) {

                ViewBag.skippedRecords = ViewBag.skippedRecords + 5;

            }
            OrderBy("", ViewBag.shortedColumByOrder);
            return View("LoadUserGrid", ShortListBasedOnColumn(userList, ViewBag.shortedColumByOrder, ViewBag.shortOrder));
        }
        public ActionResult getPrev()
        {
            if (ViewBag.skippedRecords - 5 >= 0)
            {

                ViewBag.skippedRecords = ViewBag.skippedRecords - 5;

            }
            OrderBy("", ViewBag.shortedColumByOrder);
            return View("LoadUserGrid", ShortListBasedOnColumn(userList, ViewBag.shortedColumByOrder, ViewBag.shortOrder));
        }
        public ActionResult LoadUserGrid(string ColumnName="none" )
        {
            var Users = _db.User.ToList();
            var registrationViewModelObjList = new List<RegistrationViewModel>();
            foreach (var usr in Users ) {
            var registrationViewModelObj = new RegistrationForm.ViewModels.RegistrationViewModel();
                registrationViewModelObj = new RegistrationForm.ViewModels.RegistrationViewModel();
                registrationViewModelObj.Countries = new SelectList(GetAllCountries(usr.CountryId), "Id", "CountryName");
                registrationViewModelObj.States = new SelectList(GetAllStates().Where(s => s.Id == usr.StateId).ToList(), "Id", "StateName");
                registrationViewModelObj.Skills = _db.Skill.ToList();
                registrationViewModelObj.User = usr;
                registrationViewModelObj.User.DOB = DateTime.Now;
                var Img = _db.Image.FirstOrDefault(img => img.UserId == usr.Id).ImageName;
                registrationViewModelObj.ImageFilePath = Img;
                registrationViewModelObjList.Add(registrationViewModelObj);
              
            }
            // return View(ShortListBasedOnColumn(registrationViewModelObjList, ColumnName));
            numberOdUsers = registrationViewModelObjList.Count();
            noOfPages = (double)Math.Round((decimal)registrationViewModelObjList.Count()/5);
            ViewBag.noOfPages = noOfPages + 1;
            registrationViewModelObjList[0].noOfPages = noOfPages + 1;
            userList = registrationViewModelObjList;
            int skip = Convert.ToInt32(ViewBag.skippedRecords);
            registrationViewModelObjList = registrationViewModelObjList?.Skip(skip).Take(5).ToList();
            return View(registrationViewModelObjList);

        }
        public ActionResult GetGridData(string ColumnName = "none")
        {
            var Users = _db.User.ToList();
            var usrJsonModelList = new List<UserJsonModel>();
            foreach (var usr in Users)
            {
                var usrJsonModel = new UserJsonModel();
                usrJsonModel.country = usr.Country.CountryName;
                usrJsonModel.state = usr.State.StateName;
                //usrJsonModel.skills = 
                DateTime.TryParse(usr.DOB.ToString(), out DateTime date);
                usrJsonModel.DOB = date.ToString("yyyy-MM-dd");
                usrJsonModel.imagePath = _db.Image.FirstOrDefault(img => img.UserId == usr.Id).ImageName;
                usrJsonModelList.Add(usrJsonModel);

            }
           
            return Json(new { data = usrJsonModelList }, JsonRequestBehavior.AllowGet);

        }

    }
}