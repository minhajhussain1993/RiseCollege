using FYP_6.Models;
using FYP_6.Models.Models_Logic;
using FYP_6.SessionExpireChecker;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FYP_6.Controllers
{
    public class HomeController : Controller
    {
        RCIS2Entities1 rc = RCIS2Entities1.getinstance();
        //string[] MonthsNames ={"","January","February","March","April","May","June","July",
        //                                 "August","September","October","November","December"};
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        #region Login Screen And Entry
        public ActionResult login()
        {
            if (Session["ID"] != null)
            {
                return RedirectToAction("WelcomeTeacher");
            }
            else if (Session["rollno"] != null)
            {
                return RedirectToAction("WelcomeStudent");
            }
            else if (Session["EmpID"] != null)
            {
                return RedirectToAction("WelcomeEmployee");
            }
            else if (Session["AdminID"] != null)
            {
                return RedirectToAction("WelcomeAdmin");
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        //Login Controller for Admin, Employee, Students and Teachers
        public ActionResult login(FormCollection dropdownid, string username, string password)
        {
            try
            {
                string id = dropdownid["Actors"].ToString();

                if (ModelState.IsValid)
                {
                    if (username == "")
                    {
                        ViewBag.Message2 = "UserName is Required";
                        return View();
                    }
                    else if (password == "")
                    {
                        ViewBag.Message3 = "Password is Required";
                        return View();
                    }
                    else
                    {
                        //If Student
                        if (id == "1")
                        {
                            string encodedUserName = HttpUtility.HtmlEncode(username);//Preventing Cross Site Scripting
                            string encodedpassword = HttpUtility.HtmlEncode(password);//Preventing Cross Site Scripting

                            //int studentId = Convert.ToInt32(encodedUserName);
                            var loginStudentQuery = rc.Registerations
                                .Where(s => s.Rollno == encodedUserName && s.Password == encodedpassword
                                && s.Student_Profile.Status == 1 && s.Status == 1
                                ).FirstOrDefault();

                            if (loginStudentQuery != null)
                            {
                                FormsAuthentication.SetAuthCookie(username, true);
                                Session["rollno"] = encodedUserName;
                                Session["UserName"] = loginStudentQuery.Student_Profile.FirstName.ToString() + " " + loginStudentQuery.Student_Profile.LastName.ToString();
                                //Session["Password"] = SherlockHolmesEncryptDecrypt.Encrypt(loginStudentQuery.Password.ToString());
                                ViewBag.Message = "";
                                return RedirectToAction("WelcomeStudent");
                            }
                            else
                            {
                                ViewBag.Message = "Username/Password is invalid";
                                return View();
                            }


                        }//else If Teacher
                        else if (id == "2")
                        {
                            string encodedUserName = HttpUtility.HtmlEncode(username);//Preventing Cross Site Scripting
                            string encodedpassword = HttpUtility.HtmlEncode(password);//Preventing Cross Site Scripting

                            var loginStudentQuery = rc.Teachers.
                                Where(s => s.TeacherID == encodedUserName && s.Password == encodedpassword).FirstOrDefault();

                            if (loginStudentQuery != null)
                            {
                                FormsAuthentication.SetAuthCookie(username, true);
                                Session["ID"] = encodedUserName;
                                Session["UserName"] = loginStudentQuery.Name.ToString();
                                //Session["Password"] = SherlockHolmesEncryptDecrypt.Encrypt(loginStudentQuery.Password.ToString());
                                ViewBag.Message = "";
                                return RedirectToAction("WelcomeTeacher");
                            }
                            else
                            {
                                ViewBag.Message = "Username/Password is invalid";
                                return View();
                            }
                        }//else If Employee
                        else if (id == "3")
                        {
                            string encodedUserName = HttpUtility.HtmlEncode(username);//Preventing Cross Site Scripting
                            string encodedpassword = HttpUtility.HtmlEncode(password);//Preventing Cross Site Scripting

                            var loginAdminQuery = rc.Employees.Where
                                (s => s.UserName == username && s.Password == encodedpassword).FirstOrDefault();
                            if (loginAdminQuery != null)
                            {
                                FormsAuthentication.SetAuthCookie(username, true);
                                Session["EmpID"] = loginAdminQuery.EmpID.ToString();
                                Session["UserName"] = loginAdminQuery.Name.ToString();
                                //Session["Password"] = SherlockHolmesEncryptDecrypt.Encrypt(loginAdminQuery.Password.ToString());
                                ViewBag.Message = "";
                                return RedirectToAction("WelcomeEmployee", "Home");
                            }
                            else
                            {
                                ViewBag.Message = "Username/Password is invalid";
                                return View();
                            }
                        }//else If Admin
                        else if (id == "4")
                        {
                            string encodedUserName = HttpUtility.HtmlEncode(username);//Preventing Cross Site Scripting
                            string encodedpassword = HttpUtility.HtmlEncode(password);//Preventing Cross Site Scripting

                            var loginAdminQuery = rc.Admins.
                                Where(s => s.UserName == username && s.Password == encodedpassword).FirstOrDefault();
                            if (loginAdminQuery != null)
                            {
                                FormsAuthentication.SetAuthCookie(username, true);
                                Session["AdminID"] = loginAdminQuery.ID.ToString();
                                Session["UserName"] = loginAdminQuery.UserName.ToString();
                                //Session["Password"] = SherlockHolmesEncryptDecrypt.Encrypt(loginAdminQuery.Password.ToString());
                                ViewBag.Message = "";
                                return RedirectToAction("WelcomeAdmin", "Home");
                            }
                            else
                            {
                                ViewBag.Message = "Username/Password is invalid";
                                return View();
                            }
                        }

                    }
                }
                ViewBag.Message = "Username/Password is invalid";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Username/Password is invalid";
                return View();
            }

        }
        #endregion

        #region Welcome Screens

        [Authorize]
        //[OutputCache(Duration = 60)]
        [SessionExpireEmp]
        public ActionResult WelcomeEmployee()
        {
            string[] mainScreenItems = EmployeesModel.WelcomeEmployeesScreenResults();

            if (mainScreenItems.Length > 0)
            {
                ViewBag.DegreePrograms = mainScreenItems[0];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Subjects = mainScreenItems[1];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Students = mainScreenItems[2];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Teachers = mainScreenItems[3];
                //mainScreenItems.RemoveAt(0);
            }
            Guid EmpID = Guid.Parse(Session["EmpID"].ToString());
            var getLoggedInEmpPicture = rc.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();

            Session["Picture"] = getLoggedInEmpPicture.Picture;


            ViewBag.Data = StatisticsForAdmins();
            return View();
        }

        [Authorize]
        //[OutputCache(Duration = 60)]
        [SessionExpireStudent]
        public ActionResult WelcomeStudent()
        {
            string roll = Session["rollno"].ToString();

            string[] mainScreenItems = StudentModel.WelcomeStudentScreenResults(roll);
            if (mainScreenItems.Length > 0)
            {
                ViewBag.Degree = mainScreenItems[0];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Section = mainScreenItems[1];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Subjects = mainScreenItems[2];
                //mainScreenItems.RemoveAt(0);
                ViewBag.DegreeStatus = mainScreenItems[3];
                //mainScreenItems.RemoveAt(0);
            }
            var getLoggedInStudentPicture = rc.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
            Session["Picture"] = getLoggedInStudentPicture.Student_Profile.Picture;
            //ViewBag.Data = StatisticsForStudents(roll);
            return View();
        }


        [Authorize]
        //[OutputCache(Duration = 60)]
        [SessionExpireTeacher]
        public ActionResult WelcomeTeacher()
        {
            string teacherID = Session["ID"].ToString();

            string[] mainScreenItems = TeacherModel.WelcomeTeacherScreenResults(teacherID);

            if (mainScreenItems.Length > 0)
            {
                ViewBag.Graduation = mainScreenItems[0];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Major = mainScreenItems[1];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Status = mainScreenItems[2];
                //mainScreenItems.RemoveAt(0);
                if (mainScreenItems[3] != null)
                {
                    int subjs = int.Parse(mainScreenItems[3]);
                    if (subjs > 0)
                    {
                        ViewBag.Subjects = mainScreenItems[3];
                    }
                    else
                    {
                        ViewBag.Subjects = 0;
                    }
                }
                else
                {
                    ViewBag.Subjects = 0;
                }
                //mainScreenItems.RemoveAt(0);
            }

            var getLoggedInTeacherPicture = rc.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();

            Session["Picture"] = getLoggedInTeacherPicture.Picture;
            return View();
        }

        [Authorize]
        //[OutputCache(Duration = 60)]
        [SessionExpireAdmin]
        public ActionResult WelcomeAdmin()
        {
            string[] mainScreenItems = EmployeesModel.WelcomeEmployeesScreenResults();

            if (mainScreenItems.Length > 0)
            {
                ViewBag.DegreePrograms = mainScreenItems[0];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Subjects = mainScreenItems[1];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Students = mainScreenItems[2];
                //mainScreenItems.RemoveAt(0);
                ViewBag.Teachers = mainScreenItems[3];
                //mainScreenItems.RemoveAt(0);
            }
            Guid AdminID = Guid.Parse(Session["AdminID"].ToString());
            var getLoggedInAdminPicture = rc.Admins.Where(s => s.ID == AdminID).Select(s => s).FirstOrDefault();

            Session["Picture"] = getLoggedInAdminPicture.Picture;


            ViewBag.Data = StatisticsForAdmins();
            return View();
        }


        #endregion

        #region Statistics For Employees And Admins Shown on Welcome Screens
        public HtmlString StatisticsForAdmins()
        {
            ArrayList arrMain = new ArrayList { "Years", "Batches", "Teachers", "Students" };

            ArrayList arr = new ArrayList();
            ArrayList arr2 = new ArrayList();

            ArrayList arrFinal = new ArrayList();
            arrFinal.Add(arrMain);

            int TeacherCount = rc.Teachers.Select(s => s).Count();

            var getSortedYear = rc.Years.OrderBy(s => s.FromYear).Select(s => s);

            foreach (var item in getSortedYear)
            {
                int BatchesInYear = rc.Batches.Where(s => s.YearID == item.YearID).Select(s => s).Count();
                int getRegs = rc.Registerations.Where(s => s.Batch.YearID == item.YearID).Select(s => s).Count();

                
                    arrFinal.Add(new ArrayList 
                {
                    item.FromYear.ToString() + "-" + item.ToYear.ToString(),
                    BatchesInYear,TeacherCount,getRegs
                });
            }
            
            //arrFinal.Sort();
            string data = JsonConvert.SerializeObject(arrFinal, Formatting.None);
            HtmlString arreh = new HtmlString(data);
            return arreh;
        }
        #endregion

        #region CodeForMarksStatistics
        //public HtmlString StatisticsForStudents(string rollno)
        //{
        //    DateTime dt = DateTime.Now;

        //    string MyMonth = MonthsNames[dt.Month];
        //    int? Currentyear = dt.Year;
        //    ArrayList arrMain = new ArrayList { "Subjects", "Marks Percentage" };

        //    ArrayList arr = new ArrayList();
        //    ArrayList arr2 = new ArrayList();

        //    var getPartNoRelatedToStudent = rc.Registerations.Where(s => s.Rollno == rollno).Select(s => s.Part).FirstOrDefault();

        //    var getSubjectQueryDekhoKyaMiltaHePhir = rc.Assign_Subject
        //        .Where(s => s.Rollno == rollno
        //            && s.Batch_Subjects_Parts.Part == getPartNoRelatedToStudent
        //        ).Select(s => s).ToList();

        //    ArrayList arrFinal = new ArrayList();
        //    arrFinal.Add(arrMain);

        //    var joiningRelatedMarksSubjects = from subjs in getSubjectQueryDekhoKyaMiltaHePhir
        //                                      join marks in rc.Student_Marks
        //                                      on subjs.AssignID equals marks.SubjectAssignID
        //                                      select marks;

        //    if (joiningRelatedMarksSubjects.Count()>1)
        //    {
        //        foreach (var item2 in joiningRelatedMarksSubjects)
        //        {
        //            if (item2.Month == MyMonth && item2.Year == Currentyear)
        //            {
        //                arrFinal.Add(new ArrayList 
        //                {
        //                    item2.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
        //                    item2.Marks_Percentage.ToString()
        //                });
        //            }
        //        }   
        //    }
        //    else
        //    {
        //        foreach (var item2 in getSubjectQueryDekhoKyaMiltaHePhir)
        //        {
        //                arrFinal.Add(new ArrayList 
        //                {
        //                    item2.Batch_Subjects_Parts.Subject.SubjectName,
        //                    0
        //                });
        //        }
        //    }
        //    //int TeacherCount = rc.Teachers.Select(s => s).Count();

        //    //foreach (var item in rc.Years)
        //    //{
        //    //    int BatchesInYear = rc.Batches.Where(s => s.YearID == item.YearID).Select(s => s).Count();
        //    //    int getRegs = rc.Registerations.Where(s => s.Batch.YearID == item.YearID).Select(s => s).Count();

        //    //    if (BatchesInYear == 0)
        //    //    {
        //    //        arrFinal.Add(new ArrayList 
        //    //    {
        //    //        item.FromYear.ToString() + "-" + item.ToYear.ToString(),
        //    //        BatchesInYear,TeacherCount,getRegs
        //    //    });
        //    //    }
        //    //}
        //    //foreach (var item in rc.Years)
        //    //{
        //    //    int BatchesInYear = rc.Batches.Where(s => s.YearID == item.YearID).Select(s => s).Count();
        //    //    int getRegs = rc.Registerations.Where(s => s.Batch.YearID == item.YearID).Select(s => s).Count();
        //    //    if (BatchesInYear == 0)
        //    //    {
        //    //    }
        //    //    else
        //    //    {
        //    //        arrFinal.Add(new ArrayList 
        //    //    {
        //    //        item.FromYear.ToString() + "-" + item.ToYear.ToString(),
        //    //        BatchesInYear,TeacherCount,getRegs
        //    //    });

        //    //    }

        //    //}
        //    string data = JsonConvert.SerializeObject(arrFinal, Formatting.None);
        //    HtmlString arreh = new HtmlString(data);
        //    return arreh;
        //}
        #endregion

        #region Remote Validation and others
        ////Remote Validation and other validations

        public JsonResult GetTeacherSubjectsRelatedToTBID(string id)
        {
            Guid gd = new Guid();
            if (Guid.TryParse(id, out gd))
            {
                List<string> st = new List<string>();
                var getSubjTB = rc.Teacher_Subject.Where(s => s.Teacher_BatchID == gd).Select(s => s.Subject.SubjectName).ToList();

                foreach (var item in getSubjTB)
                {
                    st.Add(item);
                }
                return Json(st);
            }
            else
            {
                return Json("");
            }

        }

        public JsonResult ValidateRollno(string Rollno)
        {
            return Json(!rc.Registerations.Any(s => s.Rollno == Rollno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateTeacherID(string teacherID)
        {
            return Json(!rc.Teachers.Any(s => s.TeacherID == teacherID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateTeacherIDForSubjAndBatches(string teacherID)
        {
            return Json(rc.Teachers.Any(s => s.TeacherID == teacherID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateBillNo(string Bill_No)
        {
            return Json(!rc.Fees.Any(s => s.Bill_No == Bill_No), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateRollnoForFee(string Rollno)
        {
            return Json(rc.Registerations.Any(s => s.Rollno == Rollno), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadImageEmployee(string id)
        {
            Guid getEmpID = Guid.Parse(Session["EmpID"].ToString());

            var getEmployee = rc.Employees.Where(s => s.EmpID == getEmpID).Select(s => s).FirstOrDefault();


            foreach (string file in Request.Files)
            {
                var fileContent = Request.Files[file];

                if (fileContent.ContentLength > 3048576)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Plz Select a file of length less than 3 MB");
                }
                else
                {
                    if (fileContent.ContentLength > 0 && fileContent != null && fileContent.ContentType.Contains("image"))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            fileContent.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            getEmployee.Picture = array;
                            Session["Picture"] = getEmployee.Picture;
                        }
                    }
                }
            }
            rc.SaveChanges();
            return Json("Successfully Updated Profile Picture!");
        }

        [HttpPost]
        public JsonResult UploadImageAdmin(string id)
        {
            Guid getAdminID = Guid.Parse(Session["AdminID"].ToString());

            var getAdmin = rc.Admins.Where(s => s.ID == getAdminID).Select(s => s).FirstOrDefault();


            foreach (string file in Request.Files)
            {
                var fileContent = Request.Files[file];

                if (fileContent.ContentLength > 3048576)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Plz Select a file of length less than 3 MB");
                }
                else
                {
                    if (fileContent.ContentLength > 0 && fileContent != null && fileContent.ContentType.Contains("image"))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            fileContent.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            getAdmin.Picture = array;
                            Session["Picture"] = getAdmin.Picture;
                        }
                    }
                }
            }
            rc.SaveChanges();
            return Json("Successfully Updated Profile Picture!");
        }
        
        public JsonResult GetTeacherIDIntellisense(string search)
        {
            List<string> teachersids = rc.Teachers.Where(s => s.TeacherID.StartsWith(search) &&s.Status=="Active").Select(s => s.TeacherID).ToList();
            return Json(teachersids);
        }
        #endregion

        #region Logout
        //Logout Here
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            //rc.Dispose();
            GC.Collect();
            return RedirectToAction("login");
        }
        #endregion
    }
}