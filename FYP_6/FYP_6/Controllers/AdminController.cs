using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FYP_6.Models.Models_Logic;
using FYP_6.Models;
using System.Collections;
using FYP_6.SessionExpireChecker;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpireAdmin]
    public class AdminController : Controller
    {
        RCIS3Entities r = RCIS3Entities.getinstance();
        EmployeesModel empModel = new EmployeesModel();
        // GET: Admin

        #region AdminPersonal
        public ActionResult Index()
        {
            try
            {
                Guid AdminID = Guid.Parse(Session["AdminID"].ToString());
                var getAdmin = r.Admins.Where(s => s.ID == AdminID).Select(s => s).FirstOrDefault();
                return View(getAdmin); 
            }
            catch (Exception)
            {
                return View(new Admin());
            }
             
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldpass, string newpass)
        {
            try
            {
                Guid id = Guid.Parse(Session["AdminID"].ToString());
                string newpass1 = HttpUtility.HtmlEncode(newpass);
                string oldpass1 = HttpUtility.HtmlEncode(oldpass);

                ViewBag.Message = empModel.ChangePassword_AdminFunction(oldpass1, newpass1, id);
                return View();
            }
            catch (Exception)
            {
                //ViewBag.Message = "Unable to Change Password!";
                ViewBag.Message = "Unable to Change Password!";
                return View("ChangePassword");
            }
             
        }
        #endregion

        #region Admin Functions
        public ActionResult ManageEmployees(int? page, string res, string searchEmployeebtn, string search2)
        {
            try
            {
                if (res != null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
                if (searchEmployeebtn != null || Request.QueryString["search2"] != null)
                {
                    bool checker = false;

                    IEnumerable<Employee> DataBasedOnRollnos = empModel.GetAllEmployeesRecords();

                    foreach (var item in DataBasedOnRollnos)
                    {
                        if (item.Name.ToLower().StartsWith(search2.ToLower()))
                        {
                            checker = true;
                            break;
                        }
                    }
                    if (checker)
                    {
                        ViewBag.SearchQuery = "True";
                        IEnumerable<Employee> SearchedData = empModel.getSpecificSearchRecordForEmployees(search2);
                        return View(SearchedData.ToPagedList(page ?? 1, 10));
                    }
                    else
                    {
                        ViewBag.SearchQuery = "Name Doesnot Exists";
                        return View();
                    }
                }
                else
                {
                    IEnumerable<Employee> DataBasedOnRollnos = empModel.GetAllEmployeesRecords();
                    if (DataBasedOnRollnos == null)
                    {
                        ViewBag.Message = "No Record Found";
                        return View();
                    }
                    return View(DataBasedOnRollnos.ToPagedList(page ?? 1, 10));
                }


            }
            catch (Exception)
            {
                return View("ManageEmployees",null);                 
            }
             
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("ManageEmployees")]
        //public ActionResult ManageEmployees2(int? page, string search)
        //{
        //    bool checker = false;

        //    IEnumerable<Employee> DataBasedOnRollnos = empModel.GetAllEmployeesRecords();
        //    foreach (var item in DataBasedOnRollnos)
        //    {
        //        if (item.Name.StartsWith(search))
        //        {
        //            checker = true;
        //            break;
        //        }
        //    }
        //    if (checker)
        //    {
        //        ViewBag.SearchQuery = "True";
        //        IEnumerable<Employee> SearchedData = empModel.getSpecificSearchRecordForEmployees(search);
        //        return View(SearchedData.ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        ViewBag.SearchQuery = "Name Doesnot Exists";
        //        return View();
        //    }
        //}
        [HttpPost]
        public ActionResult DeleteEmployees(IEnumerable<Guid> deleteEmp)
        {
            try
            {
                if (deleteEmp != null)
                {
                    List<Employee> listToDelete = r.Employees.Where(s => deleteEmp.Contains(s.EmpID)).ToList();
                    foreach (var item in listToDelete)
                    {
                        r.Employees.Remove(item);

                    }
                    r.SaveChanges();
                    return RedirectToAction("ManageEmployees", "Admin",
                                new { res = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!") });
                }
                else
                {
                    return RedirectToAction("ManageEmployees", "Admin",
                                new { res = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!") });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageEmployees", "Admin",
                               new { res = SherlockHolmesEncryptDecrypt.Encrypt("Unable To Delete Records Plz Try Again!") });
            }
             
        }
        public ActionResult AddNewEmp()
        {
            ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewEmp(Employee tRec, HttpPostedFileBase file, Nullable<System.DateTime> date1,
            string gender, string religion, string Marriedstatus)
        { 
             
            string result = empModel.NewEmployeeAddition(tRec, file, date1, gender, religion, Marriedstatus);
            try
            {
                ViewBag.Religions = r.Religions.OrderByDescending(s => s.ReligionName == religion).ThenBy(s => s);
                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Record Added";
                    if (date1.HasValue)
                    {
                        TempData["DateSaved"] = date1.Value.ToShortDateString();
                    }
                    return View();
                }
                else
                {
                    ViewBag.Message = result;
                    if (date1.HasValue)
                    {
                        TempData["DateSaved"] = date1.Value.ToShortDateString();
                    }
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                ViewBag.Message = "Unable to Add Record!";
                return View();
                //throw;
            }
        }

        public ActionResult EditEmployees(string id)
        {
            try
            {
                Guid gd = new Guid();
                if (Guid.TryParse(id, out gd))
                {
                    var getEMP = r.Employees.Where(s => s.EmpID == gd).Select(s => s).FirstOrDefault();
                    ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                    return View(getEMP);
                }
                else
                {
                    return RedirectToAction("ManageEmployees");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("ManageEmployees");
            }
             
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployees(Guid EmpID, Employee em, HttpPostedFileBase file
            , Nullable<System.DateTime> date1, string gender, string religion
            , string Marriedstatus)
        {
            try
            {
                ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);


                if (em != null)
                {
                    string result = empModel.UpdateEmployeeRecord(EmpID, em, file, date1, gender, religion, Marriedstatus);
                    if (result == "OK")
                    {
                        ViewBag.Message = "Successfully Updated Record";
                        ViewBag.Message2 = "";
                        var getEMP = r.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();
                        return View(getEMP);
                    }
                    else
                    {
                        ViewBag.Message = result;
                        var getEMP = r.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();
                        return View(getEMP);
                    }
                }
                else
                {
                    ViewBag.Message = "No Changes were Made to Update the Record";
                    var getEMP = r.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();
                    return View(getEMP);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageEmployees");
            }
             
        }

        public ActionResult DetailEmployees(string id)
        {
            try
            {
                Guid gd = new Guid();
                if (Guid.TryParse(id, out gd))
                {
                    var getEMP = r.Employees.Where(s => s.EmpID == gd).Select(s => s).FirstOrDefault();
                    ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                    return View(getEMP);
                }
                else
                {
                    return RedirectToAction("ManageEmployees");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("ManageEmployees"); 
            }
             
        }
        #endregion

        #region Students
        [ValidateInput(false)]
        public ActionResult StudentRecords(string id, int? page, string res, string ifButtonPressed, string search2
           , int? StudentType, string searchfname, string searchdeg
            , string searchsection, string searchpart)
        {
            try
            {
                if (res != null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
                if (ifButtonPressed != null || Request.QueryString["search2"] != null || Request.QueryString["StudentType"] != null)
                {
                    IEnumerable<Registeration> SearchedData = empModel.getSpecificSearchRecord(search2, StudentType, searchfname, searchdeg
                        , searchsection, searchpart);
                    if (SearchedData != null)
                    {
                        ViewBag.SearchQuery = "True";
                        return View(SearchedData.ToPagedList(page ?? 1, 20));
                    }
                    else
                    {
                        ViewBag.SearchQuery = "Nothing";
                        return View();
                    }
                }
                else
                {
                    IEnumerable<Registeration> DataBasedOnRollnos = empModel.GetAllStudentRecords();
                    if (DataBasedOnRollnos == null)
                    {
                        ViewBag.SearchQuery = "Nothing";
                        return View("StudentRecords", null);
                    }
                    return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 20));
                }

            }
            catch (Exception)
            {
                return View("StudentRecords", null);
            }
             
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult StudentRecords(string search, int StudentType, int? page)
        //{
        //    bool checker = false;
        //    IEnumerable<Registeration> DataBasedOnRollnos = empModel.GetAllStudentRecords();
        //    foreach (var item in DataBasedOnRollnos)
        //    {
        //        if (item.Rollno.StartsWith(search))
        //        {
        //            checker = true;
        //            break;
        //        }
        //    }
        //    if (checker)
        //    {
        //        ViewBag.SearchQuery = "True";
        //        IEnumerable<Registeration> SearchedData = empModel.getSpecificSearchRecord(search, StudentType);
        //        return View(SearchedData.ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        ViewBag.SearchQuery = "Roll no Doesnot Exists";
        //        return View();
        //    }
        //}
        public ActionResult DetailStudent(string id)
        {
            try
            {
                var getStudent = r.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();
                if (getStudent == null)
                {
                    return RedirectToAction("StudentRecords");
                }
                return View(getStudent);
            }
            catch (Exception)
            {
                return RedirectToAction("StudentRecords");
            }
             
        }

        public ActionResult StudentMarksRecords(string id, int? page, string res, string ifButtonPressed,
            string search2, int? StudentType, string month, string year)
        {
            //SessionClearOnReload();
            try
            {
                if (res != null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
                if (ifButtonPressed != null || Request.QueryString["search"] != null
                    || Request.QueryString["Month"] != null || Request.QueryString["year"] != null
                    || Request.QueryString["StudentType"] != null)
                {
                    IEnumerable<Student_Marks> SearchedData = empModel.getSpecificSearchStudentMarksRecord(search2, StudentType, year, month);
                    //IEnumerable<Student_Profile> SearchedData = empModel.getSpecificSearchRecord(search, StudentType);
                    if (SearchedData != null)
                    {
                        ViewBag.SearchQuery = "True";
                        return View(SearchedData.ToPagedList(page ?? 1, 10));
                    }
                    else
                    {
                        ViewBag.SearchQuery = "Nothing";
                        return View();
                    }
                }
                else
                {
                    IEnumerable<Student_Marks> DataBasedOnRollnos = empModel.GetAllStudentMarksRecords();
                    if (DataBasedOnRollnos == null)
                    {
                        ViewBag.SearchQuery = "Nothing";
                        return View("StudentMarksRecords", null);
                    }
                    //return View(DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 10));
                    ViewBag.SearchQuery = "True";
                    return View(DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 10));
                }

            }
            catch (Exception)
            {
                return View("StudentMarksRecords",null);
            }
             
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult StudentMarksRecords(string search, int StudentType, string ifButtonPressed
        //    , int? page,string year,string month)
        //{
        //    IEnumerable<Student_Marks> SearchedData = empModel.getSpecificSearchStudentMarksRecord(search, StudentType,year,month);
        //    //IEnumerable<Student_Profile> SearchedData = empModel.getSpecificSearchRecord(search, StudentType);
        //    if (SearchedData != null)
        //    {
        //        ViewBag.SearchQuery = "True";
        //        return View(SearchedData.ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        ViewBag.SearchQuery = "Nothing";
        //        return View();
        //    }
        //}

        public ActionResult StudentAttendanceRecords(string id, int? page, string res,
            string ifButtonPressed, string search2, int? StudentType, string month, string year)
        {
            try
            {
                if (res != null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
                if (ifButtonPressed != null || Request.QueryString["search2"] != null
                    || Request.QueryString["Month"] != null || Request.QueryString["year"] != null
                    || Request.QueryString["StudentType"] != null)
                {
                    IEnumerable<Students_Attendance> SearchedData = empModel.getSpecificSearchStudentAttendanceRecord(search2, StudentType, year, month);
                    //IEnumerable<Student_Profile> SearchedData = empModel.getSpecificSearchRecord(search, StudentType);
                    if (SearchedData != null)
                    {
                        ViewBag.SearchQuery = "True";
                        return View(SearchedData.ToPagedList(page ?? 1, 10));
                    }
                    else
                    {
                        ViewBag.SearchQuery = "Nothing";
                        return View();
                    }
                }
                else
                {
                    IEnumerable<Students_Attendance> DataBasedOnRollnos = empModel.GetAllStudentAttendanceRecords();
                    if (DataBasedOnRollnos == null)
                    {
                        ViewBag.SearchQuery = "Nothing";
                        return View("StudentAttendanceRecords", null);
                    }
                    ViewBag.SearchQuery = "True";
                    return View(DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 10));
                }


            }
            catch (Exception)
            {
                return View("StudentAttendanceRecords",null);
            }
             
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult StudentAttendanceRecords(string search, IEnumerable<int> deleteroll, int StudentType, string ifButtonPressed
        //    , int? page,string year, string month)
        //{

        //    IEnumerable<Students_Attendance> SearchedData = empModel.getSpecificSearchStudentAttendanceRecord(search, StudentType,year,month);
        //    //IEnumerable<Student_Profile> SearchedData = empModel.getSpecificSearchRecord(search, StudentType);
        //    if (SearchedData != null)
        //    {
        //        ViewBag.SearchQuery = "True";
        //        return View(SearchedData.ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        ViewBag.SearchQuery = "Nothing";
        //        return View();
        //    }
        //}


        public ActionResult Student_Subjects(int? page, string res, string ifButtonPressed, string search2, int? StudentType)
        {
            //SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            if (ifButtonPressed != null || Request.QueryString["search2"] != null || Request.QueryString["StudentType"] != null)
            {
                IEnumerable<Assign_Subject> SearchedData = empModel.getSpecificSearchStudentSubjRecord(search2, StudentType);

                if (SearchedData != null)
                {
                    ViewBag.SearchQuery = "True";
                    return View("Student_Subjects", SearchedData.Take(50).ToPagedList(page ?? 1, 30));
                }
                else
                {
                    ViewBag.SearchQuery = "Nothing";
                    return View();
                }
            }
            else
            {
                IEnumerable<Assign_Subject> DataBasedOnRollnos = empModel.GetAllStudentSubjectRecords();
                if (DataBasedOnRollnos == null)
                {
                    ViewBag.SearchQuery = "Nothing";
                    return View("StudentRecords", null);
                }
                return View("Student_Subjects", DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 30));
            }

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Student_Subjects(string search, IEnumerable<int> deleteroll, int StudentType, string ifButtonPressed, int? page)
        //{
        //    IEnumerable<Assign_Subject> SearchedData = empModel.getSpecificSearchStudentSubjRecord(search, StudentType);
        //    if (SearchedData != null)
        //    {
        //        ViewBag.SearchQuery = "True";
        //        return View("Student_Subjects", SearchedData.Take(50).ToPagedList(page ?? 1, 30));
        //    }
        //    else
        //    {
        //        ViewBag.SearchQuery = "Nothing";
        //        return View();
        //    }
        //}

        #endregion

        #region Teachers
        [HttpGet]
        public ActionResult TeacherRecords(int? page, string deleteResult, string search, string TeacherType, string submitButtonPressed)
        {
            //SessionClearOnReload();
            if (deleteResult != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
            }
            if (submitButtonPressed != null || Request.QueryString["search"] != null || Request.QueryString["TeacherType"] != null)
            {
                IEnumerable<Teacher> SearchedData = empModel.getSpecificSearchRecordForTeacher(search, TeacherType);
                if (SearchedData != null)
                {
                    ViewBag.SearchQuery = "True";
                    return View(SearchedData.ToPagedList(page ?? 1, 10));
                }
                else
                {
                    ViewBag.SearchQuery = "Nothing";
                    return View();
                }
            }
            else
            {
                IEnumerable<Teacher> DataBasedOnRollnos = empModel.GetAllTeacherRecords();
                if (DataBasedOnRollnos == null)
                {
                    ViewBag.SearchQuery = "Nothing";
                    return View("TeacherRecords", null);
                }
                return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));
            }
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult TeacherRecords(string search, int? page, IEnumerable<string> deleteTeacher, string TeacherType)
        //{

        //    bool checker = false;
        //    IEnumerable<Teacher> DataBasedOnRollnos = empModel.GetAllTeacherRecords();
        //    foreach (var item in DataBasedOnRollnos)
        //    {
        //        if (item.TeacherID.StartsWith(search))
        //        {
        //            checker = true;
        //            break;
        //        }
        //    }
        //    if (checker)
        //    {
        //        ViewBag.SearchQuery = "True";
        //        IEnumerable<Teacher> SearchedData = empModel.getSpecificSearchRecordForTeacher(search, TeacherType);
        //        return View(SearchedData.ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        ViewBag.SearchQuery = "ID no Doesnot Exists";
        //        return View();
        //    }
        //}

        public ActionResult DetailTeacher(string id)
        {
            var getTeacher = r.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
            if (getTeacher == null)
            {
                return RedirectToAction("TeacherRecords");
            }
            return View(getTeacher);
        }

        [HttpGet]
        public ActionResult TeacherAttendance(string search, string month, string year, int? page, string deleteResult, string tattsearchsubmit)
        {
            TempData["T_ID"] = null;

            //var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var SectionsOfTeacher = r.Sections.Select(s => s).OrderBy(s => s.SectionID);

            if (deleteResult != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
            }
            if (tattsearchsubmit != null || Request.QueryString["search"] != null || Request.QueryString["Month"] != null || Request.QueryString["year"] != null)
            {
                IEnumerable<Teacher_Attendance> EndResultListOfMarks = empModel.showResultsTeacherAttendance_EmployeeModelFunction(search, month, year);

                if (EndResultListOfMarks != null)
                {
                    TempData["T_ID"] = "Teacher ID: " + search + ", Total Lectures: "
                        + EndResultListOfMarks.Count() + ", Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count() +
                    @", Month: " + month + ", Year: " + year;

                    return View("TeacherAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).ToPagedList(page ?? 1, 20));
                }
                else
                {
                    TempData["T_ID"] = "Teacher ID: " + search + ", Total Lectures: "
                        + 0 + ", Attended Lectures:" + 0 +
                    @", Month: " + month + ", Year: " + year;

                    //ViewBag.Message = "No Records Founds";
                    return View("TeacherAttendance", null);
                }

            }

            else
            {
                IEnumerable<Teacher_Attendance> EndResultListOfMarks = empModel.getResultRecordsForTeacherAttendance();
                if (EndResultListOfMarks == null)
                {
                    //ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                    //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part.ToString());
                    //ViewBag.ListofSections = SectionsOfTeacher;
                    return View("TeacherAttendance", null);

                }
                else
                {
                    //ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                    //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                    //ViewBag.ListofSections = SectionsOfTeacher;
                    //ViewBag.Message = "No Records Founds";
                    return View("TeacherAttendance", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 20));

                }

            }

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult TeacherAttendance(int? page, string search,string month,string year)
        //{
        //    if (search == null || search == "")
        //    {
        //        ViewBag.Message = "Plz Enter Teacher ID to Search Records";

        //        return View("TeacherAttendance", null);
        //    }
        //    else
        //    {

        //        IEnumerable<Teacher_Attendance> EndResultListOfMarks = empModel.showResultsTeacherAttendance_EmployeeModelFunction(search,month,year);

        //        if (EndResultListOfMarks!=null)
        //        {
        //            TempData["T_ID"] = "Teacher ID: " + search + ", Total Lectures: "
        //                + EndResultListOfMarks.Count() + ", Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count() +
        //            @", Month: " + month + ", Year: " + year;

        //            return View("TeacherAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).Take(50).ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            TempData["T_ID"] = search;

        //            ViewBag.Message = "No Records Founds";
        //            return View("TeacherAttendance", null);
        //        }
        //    }
        //}
        //Teacher Subjects Add, Delete,Edit,Details Starts From Here
        //public ActionResult Teacher_Subjects(int? page, string val)
        //{
        //    TempData["TeacherID"] = null;
        //    TempData["Batch"] = null;
        //    TempData["Section"] = null;
        //    TempData["Degree"] = null;
        //    IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllTeacher_SubjectsRecords();

        //    var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
        //    var ListofSections = r.Sections.Select(s => s);

        //    if (EndResultListOfMarks != null)
        //    {
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //        //ViewBag.ListofPartsInDegree = r.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s.);
        //        //ViewBag.ListofSections = ListofSections;
        //        ViewBag.TeacherIDs = TeachersIDS;
        //        ViewBag.Message = val;
        //        return View("Teacher_Subjects", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //        //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
        //        //ViewBag.ListofSections = r.Sections.Select(s => s);
        //        ViewBag.TeacherIDs = TeachersIDS;
        //        ViewBag.Message = val;
        //        ViewBag.Message = "No Records Founds";
        //        return View("Teacher_Subjects", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
        //    }

        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Teacher_Subjects(int? page, string val, string degree, string section, string batch, string teacherID)
        //{
        //    TempData["TeacherID"] = teacherID;
        //    TempData["Batch"] = batch;
        //    TempData["Section"] = r.Sections.Where(s => s.SectionID.ToString() == section).Select(s => s.SectionName).FirstOrDefault();
        //    TempData["Degree"] = r.Degree_Program.Where(s => s.ProgramID.ToString() == degree).Select(s => s.Degree_ProgramName).FirstOrDefault();

        //    var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
        //    //var ListofSections = r.Sections.Select(s => s);
        //    if (degree == null || degree == "Please select"
        //        || section == null || section == "Please select"
        //        || batch == null || batch == "Please select")
        //    {
        //        IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(teacherID);
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //        //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
        //        //ViewBag.ListofSections = r.Sections.Select(s => s);
        //        //ViewBag.TeacherIDs = TeachersIDS;
        //        ViewBag.Message = val;
        //        if (EndResultListOfMarks != null)
        //        {
        //            return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            ViewBag.Message = "No Records Founds";
        //            return View("Teacher_Subjects", null);
        //        }
        //    }
        //    else
        //    {
        //        Guid degID = Guid.Parse(degree);
        //        Guid sectionID = Guid.Parse(section);

        //        IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_SubjectsRecords(degID, sectionID, batch, teacherID);

        //        if (EndResultListOfMarks != null)
        //        {
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //            //ViewBag.ListofPartsInDegree = r.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s.);
        //            //ViewBag.ListofSections = ListofSections;
        //            //ViewBag.TeacherIDs = TeachersIDS;
        //            ViewBag.Message = val;
        //            return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //            //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
        //            //ViewBag.ListofSections = r.Sections.Select(s => s);
        //            //ViewBag.TeacherIDs = TeachersIDS;
        //            ViewBag.Message = val;
        //            ViewBag.Message = "No Records Founds";
        //            return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //    }


        //}
        public ActionResult Teacher_Batches(int? page, string deleteResult, string searchbutton, string teacherID)
        {
            //   SessionClearOnReload();

            TempData["TeacherID"] = null;
            if (searchbutton != null || Request.QueryString["teacherID"] != null)
            {
                TempData["TeacherID"] = teacherID;
                //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);

                if (teacherID == null || teacherID == "")
                {
                    //ViewBag.TeacherIDs = TeachersIDS;
                    //ViewBag.Message = "No Records Founds";
                    TempData["TeacherID"] = null;
                    return View("Teacher_Batches", null);
                }
                else
                {
                    IEnumerable<Teachers_Batches> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_BatchesRecords(teacherID);
                    if (EndResultListOfMarks != null)
                    {
                        return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                    }
                    else
                    {
                        //ViewBag.Message = "No Records Founds";
                        return View("Teacher_Batches", null);
                    }
                }
            }
            else
            {
                IEnumerable<Teachers_Batches> EndResultListOfMarks = empModel.GetAllTeacher_batchesRecords();

                if (deleteResult != null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
                }
                if (EndResultListOfMarks != null)
                {
                    return View("Teacher_Batches", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 10));
                }
                else
                {
                    return View("Teacher_Batches", null);
                }
            }

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Teacher_Batches(int? page, string val, string teacherID)
        //{
        //    TempData["TeacherID"] = teacherID;
        //    var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
        //    if (teacherID == null)
        //    {
        //        ViewBag.TeacherIDs = TeachersIDS;
        //        ViewBag.Message = val;
        //        ViewBag.Message = "No Records Founds";
        //        return View("Teacher_Batches", null);
        //    }
        //    else
        //    {
        //        IEnumerable<Teachers_Batches> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_BatchesRecords(teacherID);
        //        if (EndResultListOfMarks != null)
        //        {
        //            ViewBag.TeacherIDs = TeachersIDS;
        //            ViewBag.Message = val;
        //            return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            ViewBag.TeacherIDs = TeachersIDS;
        //            ViewBag.Message = val;
        //            ViewBag.Message = "No Records Founds";
        //            return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //    }

        //}
        public ActionResult DetailBatchSubjectsTeacher(string id)
        {
            Guid idGuid = new Guid();

            if (Guid.TryParse(id, out idGuid))
            {
                var getTeacherBatch = r.Teachers_Batches.Where(s => s.ID == idGuid).Select(s => s).FirstOrDefault();
                var getTeacherSubjects = getTeacherBatch.Teacher_Subject.ToList();



                ViewBag.BatchTeacher = getTeacherBatch.Batch;
                ViewBag.TeacherName = getTeacherBatch.TeacherID;
                ViewBag.TeacherbatchesID = idGuid;

                return View(getTeacherSubjects);
            }
            else
            {
                return RedirectToAction("Teacher_Batches");
            }
        }
        #endregion

        #region Statistics
        //Graphs 
        public ActionResult Statistics()
        {
            ArrayList arrMain = new ArrayList { "Years", "Number of Students" };
            ArrayList arr = new ArrayList();
            ArrayList arr2 = new ArrayList();

            ArrayList arrFinal = new ArrayList();
            arrFinal.Add(arrMain);
            foreach (var item in r.Years)
            {
                int getRegs = r.Registerations.Where(s => s.Batch.YearID == item.YearID).Select(s => s).Count();
                if (getRegs == 0)
                {
                    arrFinal.Add(new ArrayList 
                {
                    item.FromYear.ToString() + "-" + item.ToYear.ToString(),
                    getRegs
                });
                }
            }
            foreach (var item in r.Years)
            {
                int getRegs = r.Registerations.Where(s => s.Batch.YearID == item.YearID).Select(s => s).Count();
                if (getRegs == 0)
                {
                }
                else
                {
                    arrFinal.Add(new ArrayList 
                {
                    item.FromYear.ToString() + "-" + item.ToYear.ToString(),
                    getRegs
                });

                }

            }
            string data = JsonConvert.SerializeObject(arrFinal, Formatting.None);
            ViewBag.Data = new HtmlString(data);

            return View();
        }
        public ActionResult Statistics2()
        {
            ArrayList arrMain = new ArrayList { "Years", "Number of Teachers" };
            ArrayList arr = new ArrayList();
            ArrayList arr2 = new ArrayList();

            ArrayList arrFinal = new ArrayList();
            arrFinal.Add(arrMain);
            foreach (var item in r.Years)
            {
                int getRegs = r.Teachers_Batches.Where(s => s.Batch.YearID == item.YearID).Select(s => s).Count();
                if (getRegs == 0)
                {
                    arrFinal.Add(new ArrayList 
                {
                    item.FromYear.ToString() + "-" + item.ToYear.ToString(),
                    getRegs
                });
                }
            }
            foreach (var item in r.Years)
            {
                int getRegs = r.Teachers_Batches.Where(s => s.Batch.YearID == item.YearID).Select(s => s).Count();
                if (getRegs == 0)
                {
                }
                else
                {
                    arrFinal.Add(new ArrayList 
                {
                    item.FromYear.ToString() + "-" + item.ToYear.ToString(),
                    getRegs
                });

                }

            }
            string data = JsonConvert.SerializeObject(arrFinal, Formatting.None);
            ViewBag.Data = new HtmlString(data);

            return View();
        }
        public ActionResult SalaryTeacher()
        {

            decimal?[] salaries = new decimal?[6];
            decimal? TenKaySalary = 10000;
            decimal? TwentyKaySalary = 20000;
            decimal? ThirtyKaySalary = 30000;
            decimal? FourtyKaySalary = 40000;
            decimal? FiftyKaySalary = 50000;

            //salaries[0] = TenKaySalary;
            //salaries[1] = TwentyKaySalary;
            //salaries[2] = ThirtyKaySalary;
            //salaries[3] = FourtyKaySalary;
            //salaries[4] = FiftyKaySalary;

            int[] getRegs = new int[6];
            ArrayList arrMain = new ArrayList { "Salary", "Number of Teachers" };
            ArrayList arr = new ArrayList();
            ArrayList arr2 = new ArrayList();
            ArrayList arrFinal = new ArrayList();


            arrFinal.Add(arrMain);
            getRegs[0] = r.Teachers.Where(s => s.Salary >= 0 && s.Salary <= TenKaySalary).Select(s => s).Count();
            getRegs[1] = r.Teachers.Where(s => s.Salary >= TenKaySalary && s.Salary < TwentyKaySalary).Select(s => s).Count();
            getRegs[2] = r.Teachers.Where(s => s.Salary >= TwentyKaySalary && s.Salary < ThirtyKaySalary).Select(s => s).Count();
            getRegs[3] = r.Teachers.Where(s => s.Salary >= ThirtyKaySalary && s.Salary < FourtyKaySalary).Select(s => s).Count();
            getRegs[4] = r.Teachers.Where(s => s.Salary >= FourtyKaySalary && s.Salary < FiftyKaySalary).Select(s => s).Count();
            getRegs[5] = r.Teachers.Where(s => s.Salary >= FiftyKaySalary).Select(s => s).Count();

            //Array.Sort(getRegs);
            arrFinal.Add(new ArrayList 
                {
                    "0 - "+" "+TenKaySalary,
                     getRegs[0]
                });

            arrFinal.Add(new ArrayList 
                {
                    TenKaySalary+ "- "+TwentyKaySalary,
                    getRegs[1]
                });
            arrFinal.Add(new ArrayList 
                {
                    TwentyKaySalary+ "- "+ThirtyKaySalary,
                    getRegs[2]
                    
                });
            arrFinal.Add(new ArrayList 
                {
                    ThirtyKaySalary+ "- "+FourtyKaySalary,
                    getRegs[3]
                });
            arrFinal.Add(new ArrayList 
                {
                    FourtyKaySalary+"- "+ FiftyKaySalary,
                    getRegs[4]
                });
            arrFinal.Add(new ArrayList 
                {
                    FiftyKaySalary+" +Salary",
                    getRegs[5]
                });
            string data = JsonConvert.SerializeObject(arrFinal, Formatting.None);
            ViewBag.Data = new HtmlString(data);

            return View();
        }
        #endregion

        #region others

        public ActionResult ManageCourses(int? page)
        {
            var getAllDegreePrograms = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
            if (getAllDegreePrograms == null)
            {
                ViewBag.Message = "No Records Found";
                return View(getAllDegreePrograms.ToPagedList(page ?? 1, 10));
            }
            return View(getAllDegreePrograms.ToPagedList(page ?? 1, 10));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageCourses(int? page, string del)
        {
            var getAllDegreePrograms = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
            if (getAllDegreePrograms == null)
            {
                ViewBag.Message = "No Records Found";
                return View(getAllDegreePrograms.ToPagedList(page ?? 1, 10));
            }
            return View(getAllDegreePrograms.ToPagedList(page ?? 1, 10));
        }

        public JsonResult GetBatches(string degree)
        {
            if (degree != null)
            {
                Guid deg;
                if (Guid.TryParse(degree, out deg))
                {
                    List<string> getAllBatches = r.Batches.
                        Where(s => s.DegreeProgram_ID == deg
                        && s.Status == 1).Select(s => s.BatchName).ToList();
                    return Json(getAllBatches);
                }
                else
                {
                    return Json(null);
                }
            }
            else
            {
                return Json(null);
            }
        }
        public JsonResult GetSections(string batch)
        {
            if (batch != null)
            {
                //int batchID = int.Parse(batch);
                List<Section> getAllSections = new List<Section>();
                foreach (var item in r.Batches)
                {
                    if (item.BatchName == batch)
                    {
                        getAllSections.Add(new Section
                        {
                            SectionID = item.Section.SectionID,
                            SectionName = item.Section.SectionName
                        });
                    }
                }
                return Json(getAllSections);
            }
            else
            {
                return Json(null);
            }
        }
        #endregion
    }
}