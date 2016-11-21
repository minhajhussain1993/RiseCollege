using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FYP_6.Models.Models_Logic;
using FYP_6.Models;
using System.IO;
using FYP_6.SessionExpireChecker;
using FYP_6.Models.ViewModels;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpireEmp]
    public class EmployeesController : Controller
    {
        static RCIS2Entities1 r = RCIS2Entities1.getinstance();

        #region EmployeeActions
        // GET: Employees
        //[OutputCache(Duration = 60)]
        public ActionResult Index()//Profile
        {
            //SessionClearOnReload();
            Guid EmpID = Guid.Parse(Session["EmpID"].ToString());
            var getEmployee = r.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();
            return View(getEmployee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase file)//Profile
        {
            //SessionClearOnReload();
            Guid EmpID = Guid.Parse(Session["EmpID"].ToString());

            var getEmployee = r.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();
            if (file != null)
            {
                long filesize = file.ContentLength;
                if (filesize > 3048576)
                {

                }
                else
                {
                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            getEmployee.Picture = array;
                        }
                        r.SaveChanges();
                    }
                    catch (Exception)
                    {
                        return View(getEmployee);
                    }
                }
                Session["Picture"] = getEmployee.Picture;
            }
            return View(getEmployee);
        }
        public ActionResult ChangePassword()
        {
            //SessionClearOnReload();
            return View();

        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldpass, string newpass)
        {
            Guid EmpID = Guid.Parse(Session["EmpID"].ToString());
            string newpass1 = HttpUtility.HtmlEncode(newpass);
            string oldpass1 = HttpUtility.HtmlEncode(oldpass);
            
            ViewBag.Message = EmployeesModel.ChangePassword_EmployeeModelFunction(oldpass1, newpass1, EmpID);

            return View();
        }
        #endregion

        #region Teacher Management
        //Teacher Management

        //Teacher Records Add,Delete,Edit,Details Starts From Here
        public ActionResult EnrollTeacher()
        {
            SessionClearOnReload();
            //string t_ID = EmployeesModel.getTeacherID();
            //ViewBag.Teacher_ID = t_ID;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnrollTeacher(Teacher tRec, HttpPostedFileBase file)
        {
            string result=EmployeesModel.EnrollTeacherForCollege(tRec, file);
            if (result=="OK")
            {
                ViewBag.Message = "Successfully Record Added";
                return View();
            }
            else
            {
                ViewBag.Message = result;
                return View();
            }

        }

        public ActionResult TeacherRecords(int? page, string deleteResult)
        {
            SessionClearOnReload();
            if (deleteResult != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
            }
            IEnumerable<Teacher> DataBasedOnRollnos = EmployeesModel.GetAllTeacherRecords();
            return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]//Also Includes Deleting Teacher
        public ActionResult TeacherRecords(string search, int? page, IEnumerable<string> deleteTeacher, string TeacherType,
            string submitButtonPressed, string hiddenInput)
        {
            if (submitButtonPressed != null)
            {
                IEnumerable<Teacher> SearchedData = EmployeesModel.getSpecificSearchRecordForTeacher(search, TeacherType);
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
                if (deleteTeacher != null && hiddenInput != "")
                {
                    string result = EmployeesModel.DeleteTeacherRecords_EMP_ModelFunction(deleteTeacher);

                    if (result == "OK")
                    {
                        return RedirectToAction("TeacherRecords", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted") });
                    }
                    else
                    {
                        return RedirectToAction("TeacherRecords", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                    }
                }
                else
                {
                    return RedirectToAction("TeacherRecords", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete Records") });
                }
            }
        }

        public ActionResult EditTeacher(string id)
        {
                var getTeacher = r.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
                if (getTeacher==null)
                {
                    return RedirectToAction("TeacherRecords");
                }
                return View(getTeacher);   
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeacher(Teacher std, string tab, string teacherID, string batch, string degree, string part, string section, HttpPostedFileBase file)
        {
            var getTeacher = r.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();

            if (std != null)
            {
                string result= EmployeesModel.UpdateTeacherRecord(teacherID, std, file);

                if (result=="OK")
                {
                    ViewBag.Message = "Successfully Updated Record";
                    ViewBag.Message2 = "";
                    return View(getTeacher);
                }
                else
                {
                    ViewBag.Message = result;
                    return View(getTeacher);
                }
            }
            else
            {
                ViewBag.Message = "No Changes were Made to Update the Record";
                return View(getTeacher);
            }


        }
        public ActionResult DetailTeacher(string id)
        {
            var getTeacher = r.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
            if (getTeacher == null)
            {
                return RedirectToAction("TeacherRecords");
            }
            return View(getTeacher);
        }


        //Teacher Attendance Module Starts From Here
        [HttpGet]
        public ActionResult TeacherAttendance(int? page, string deleteResult)
        {
            TempData["T_ID"] = null;

            //var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var SectionsOfTeacher = r.Sections.Select(s => s).OrderBy(s => s.SectionID);

            if (deleteResult != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
            }
            IEnumerable<Teacher_Attendance> EndResultListOfMarks = EmployeesModel.getResultRecordsForTeacherAttendance();
            if (EndResultListOfMarks.Count() != 0)
            {
                //ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part.ToString());
                //ViewBag.ListofSections = SectionsOfTeacher;
                return View("TeacherAttendance", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
            }
            else
            {
                //ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                //ViewBag.ListofSections = SectionsOfTeacher;
                //ViewBag.Message = "No Records Founds";
                return View("TeacherAttendance", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("TeacherAttendance")]
        public ActionResult TeacherAttendance2(int? page, string search, string month, string year)
        {
            SessionClearOnReload();

            if (search == null || search == "")
            {
                TempData["T_ID"] = "Plz Enter Teacher ID to Search Records";

                return View("TeacherAttendance", null);
            }

            else
            {
                IEnumerable<Teacher_Attendance> EndResultListOfMarks = EmployeesModel.showResultsTeacherAttendance_EmployeeModelFunction(search,month,year);

                if (EndResultListOfMarks!=null)
                {
                    TempData["T_ID"] = "Teacher ID: " + search + ", Total Lectures: " 
                        + EndResultListOfMarks.Count() + ", Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count()+
                    @", Month: "+month+", Year: "+year;

                    return View("TeacherAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).Take(50).ToPagedList(page ?? 1, 10));
                }
                else
                {
                    TempData["T_ID"] = "No Record was Found For Teacher ID: "+ search;

                    //ViewBag.Message = "No Records Founds";
                    return View("TeacherAttendance", null);
                }
            }



        }
        public ActionResult AddTeacher_Attendance_Records()
        {
            var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = EmployeesModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();

            return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeacher_Attendance_Records(Nullable<DateTime> date,
            IEnumerable<string> PresentStatus, IEnumerable<string> T_IDS)
        {
            #region Old Teacher Addition Code
            //var getAllTeachersIDs = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
            //var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
            //var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
            //if (TeacherIds != null || TeacherIds != "")
            //{
            //    string result = EmployeesModel.AddTeacherAtt(tAtt, TeacherIds, date);
            //    if (result == "OK")
            //    {
            //        ViewBag.Message = "Successfully Record Added";
            //        ViewBag.Degrees = getAllDegrees;
            //        ViewBag.Sections = getAllSections;
            //        ViewBag.TeachersIDs = getAllTeachersIDs;
            //        ViewBag.Subjects = getAllSubjects;
            //        return View();
            //    }
            //    else
            //    {
            //        ViewBag.Message = result;
            //        ViewBag.Degrees = getAllDegrees;
            //        ViewBag.Sections = getAllSections;
            //        ViewBag.TeachersIDs = getAllTeachersIDs;
            //        ViewBag.Subjects = getAllSubjects;
            //        return View();
            //    }
            //}
            //else
            //{
            //    ViewBag.Message = "Unable to Mark Attendance!";
            //    ViewBag.Degrees = getAllDegrees;
            //    ViewBag.Sections = getAllSections;
            //    ViewBag.TeachersIDs = getAllTeachersIDs;
            //    ViewBag.Subjects = getAllSubjects;
            //    return View();
            //}
            #endregion

            if (T_IDS != null)
            {
                string result = EmployeesModel.NewTeacherAttendancesAdditionCode(PresentStatus, date, T_IDS);

                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Attendance Uploaded";
                    var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = EmployeesModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();
                    return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
                }
                else
                {
                    ViewBag.Message = result;
                    var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = EmployeesModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();
                    return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
                }
            }
            else
            {
                ViewBag.Message = "No Record Found To Upload Attendance!";
                var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = EmployeesModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();
                return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
            }
        }
        public ActionResult EditTeacher_Attendance(string id, string Date)
        {
            DateTime dt = new DateTime();
            if (DateTime.TryParse(Date,out dt))
            {
                var getUpdatedRecord = r.Teacher_Attendance.Where(s => s.Date == dt
                && s.TeacherID == id).Select(s => s).FirstOrDefault();
                if (getUpdatedRecord==null)
                {
                    return RedirectToAction("TeacherAttendance");
                }
                return View(getUpdatedRecord);    
            }
            else
            {
                return RedirectToAction("TeacherAttendance");
            }

            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeacher_Attendance(string TeacherID, string OldDate, Teacher_Attendance tAtt, string PresentStatus)
        {

            DateTime dtOld = DateTime.Parse(OldDate);
            string result = EmployeesModel.EditTeacherAttWithDateAndStatus(tAtt, TeacherID, OldDate, PresentStatus);

            if (result == "OK")
            {
                ViewBag.Message = "Successfully Record Updated";

                var getUpdatedRecord = r.Teacher_Attendance.Where(s => s.Date == dtOld
                        && s.TeacherID == TeacherID).Select(s => s).FirstOrDefault();

                return View(getUpdatedRecord);
            }
            else
            {
                ViewBag.Message = result;

                var getUpdatedRecord = r.Teacher_Attendance.Where(s => s.Date == dtOld
                        && s.TeacherID == TeacherID).Select(s => s).FirstOrDefault();

                return View(getUpdatedRecord);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeacherAttendanceRecords(IEnumerable<Guid> deleteTatt, string hiddenInput)
        {
            if (deleteTatt != null && hiddenInput != "")
            {
                string result = EmployeesModel.DeleteTeacherAttendanceRecords_EMP_ModelFunction(deleteTatt);
                if (result == "OK")
                {
                    return RedirectToAction("TeacherAttendance", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted") });
                }
                else
                {
                    return RedirectToAction("TeacherAttendance", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                }
            }
            else
            {
                return RedirectToAction("TeacherAttendance", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records to Delete Records") });
            }

        }



        //Teacher Subjects Add, Delete,Edit,Details Starts From Here
        #region Previous Teacher Subjects Add, Delete,Edit,Details Starts From Here 
        //public ActionResult Teacher_Subjects(int? page, string deleteResult)
        //{
        //    TempData["TeacherID"] = null;
        //    TempData["Batch"] = null;
        //    TempData["Section"] = null;
        //    TempData["Degree"] = null;
        //    SessionClearOnReload();

        //    IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllTeacher_SubjectsRecords();

        //    var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    if (deleteResult != null)
        //    {
        //        ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
        //    }

        //    if (EndResultListOfMarks != null)
        //    {
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //        return View("Teacher_Subjects", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;
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
        //    if (degree == null || degree == "Please select"
        //        || section == null || section == "Please select"
        //        || batch == null || batch == "Please select")
        //    {
        //        TempData["TeacherID"] = teacherID;
        //        TempData["Batch"] = null;
        //        TempData["Section"] = null;
        //        TempData["Degree"] = null;

        //        IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(teacherID);
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //        if (EndResultListOfMarks != null)
        //        {
        //            return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            return View("Teacher_Subjects", null);
        //        }
        //    }
        //    else
        //    {
        //        Guid degID = Guid.Parse(degree);
        //        Guid sectionID = Guid.Parse(section);
        //        IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllSearchSpecificTeacher_SubjectsRecords(degID, sectionID, batch, teacherID);

        //        if (EndResultListOfMarks != null)
        //        {
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //            return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //            return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
        //        }
        //    }


        //}


        ////Add Teacher Subject New Version
        //public ActionResult AddTeacherSubjects()
        //{
        //    var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
        //    //var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
        //    //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);

        //    ViewBag.Degrees = ListofDegreePrograms;
        //    ViewBag.Sections = getAllSections;
        //    //ViewBag.Subjects = getAllSubjects;
        //    //ViewBag.TeacherIDs = TeachersIDS;

        //    return View();


        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddTeacherSubjects(string degree, string subjects, string section, string batch, string teacherID, string part)
        //{
        //    var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
        //    //var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
        //    //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);


        //    string result = EmployeesModel.AddTeacherSubject(teacherID, degree, section, batch, subjects, part);
        //    if (result == "OK")
        //    {
        //        ViewBag.Message = null;
        //        ViewBag.Degrees = getAllDegrees;
        //        ViewBag.Sections = getAllSections;
        //        //ViewBag.Subjects = getAllSubjects;
        //        //ViewBag.TeacherIDs = TeachersIDS;
        //        return RedirectToAction("AddTeacherSubjects2", new { id = batch, t_id = teacherID });
        //    }
        //    else
        //    {
        //        ViewBag.Message = result;
        //        ViewBag.Degrees = getAllDegrees;
        //        ViewBag.Sections = getAllSections;
        //        //ViewBag.Subjects = getAllSubjects;
        //        //ViewBag.TeacherIDs = TeachersIDS;
        //        return View();
        //    }

        //}
        //public ActionResult AddTeacherSubjects2(string id, string t_id)
        //{
        //    var getBatch = r.Batches.Where(s => s.BatchName == id).Select(s => s).FirstOrDefault();
        //    //var specificLevel = r.Levels.Where(s => s.LevelID == getDegreeProgram.LevelID).Select(s => s.Level_Name).FirstOrDefault();
        //    IEnumerable<Batch_Subjects_Parts> ds = EmployeesModel.getBPSForAssigningToTeachers(getBatch);
        //    //ViewBag.level = specificLevel;
        //    ViewBag.Batch = getBatch;
        //    ViewBag.teacherID = t_id;
        //    return View(ds);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddTeacherSubjects2(IEnumerable<Guid> subjects,
        //    string batch, string t_id)
        //{
        //    var getBatch = r.Batches.Where(s => s.BatchName == batch).Select(s => s).FirstOrDefault();

        //    if (subjects != null)
        //    {
        //        string result = EmployeesModel.SubjectAddToBPS(subjects, getBatch, t_id);

        //        if (result == "OK")
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.teacherID = t_id;
        //            ViewBag.Message = "Successfully Records Updated";
        //            IEnumerable<Batch_Subjects_Parts> ds = EmployeesModel.getBPSForAssigningToTeachers(getBatch);
        //            return View(ds);
        //        }
        //        else
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.teacherID = t_id;
        //            ViewBag.Message = result;
        //            IEnumerable<Batch_Subjects_Parts> ds = EmployeesModel.getBPSForAssigningToTeachers(getBatch);
        //            return View(ds);
        //        }
        //    }
        //    else
        //    {
        //        if (EmployeesModel.DeleteAllSubjectsToBPS(getBatch, t_id))
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.teacherID = t_id;
        //            ViewBag.Message = "Successfully Records Updated";
        //            IEnumerable<Batch_Subjects_Parts> ds = EmployeesModel.getBPSForAssigningToTeachers(getBatch);
        //            return View(ds);
        //        }
        //        else
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.teacherID = t_id;
        //            ViewBag.Message = "Unable to Update Subjects";
        //            IEnumerable<Degree_Subject> ds = BatchModel.getSubjectsForEditing(getBatch);
        //            return View(ds);
        //        }
        //    }
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeacherSubjectRecords(IEnumerable<Guid> deleteTSub, string hiddenInput)
        {
            if (deleteTSub != null && hiddenInput != "")
            {
                string result = EmployeesModel.DeleteTeacherSubjectRecords_EMP_ModelFunction(deleteTSub);
                if (result == "OK")
                {
                    return RedirectToAction("Teacher_Subjects", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted") });
                }
                else
                {
                    return RedirectToAction("Teacher_Subjects", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                }
            }
            else
            {
                return RedirectToAction("Teacher_Subjects", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!") });
            }
        }

        #endregion

        public ActionResult Teacher_Subjects(int? page, string deleteResult)
        {
            TempData["TeacherID"] = null;
            TempData["Batch"] = null;
            TempData["Section"] = null;
            TempData["Degree"] = null;
            SessionClearOnReload();

            IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllTeacher_SubjectsRecords();

            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            if (deleteResult != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
            }

            if (EndResultListOfMarks != null)
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                return View("Teacher_Subjects", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                return View("Teacher_Subjects", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Teacher_Subjects(int? page, string val, string degree, string section, string batch, string teacherID)
        {
            TempData["TeacherID"] = teacherID;
            TempData["Batch"] = batch;
            TempData["Section"] = r.Sections.Where(s => s.SectionID.ToString() == section).Select(s => s.SectionName).FirstOrDefault();
            TempData["Degree"] = r.Degree_Program.Where(s => s.ProgramID.ToString() == degree).Select(s => s.Degree_ProgramName).FirstOrDefault();

            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            if (degree == null || degree == "Please select"
                || section == null || section == "Please select"
                || batch == null || batch == "Please select")
            {
                TempData["TeacherID"] = teacherID;
                TempData["Batch"] = null;
                TempData["Section"] = null;
                TempData["Degree"] = null;

                IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(teacherID);
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                if (EndResultListOfMarks != null)
                {
                    return View("Teacher_Subjects", EndResultListOfMarks.OrderBy(s=>s.Teachers_Batches.BatchName).ToPagedList(page ?? 1, 10));
                }
                else
                {
                    return View("Teacher_Subjects", null);
                }
            }
            else
            {
                Guid degID = Guid.Parse(degree);
                Guid sectionID = Guid.Parse(section);
                IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllSearchSpecificTeacher_SubjectsRecords(degID, sectionID, batch, teacherID);

                if (EndResultListOfMarks != null)
                {
                    ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                    return View("Teacher_Subjects", EndResultListOfMarks.OrderBy(s => s.Teachers_Batches.BatchName).ToPagedList(page ?? 1, 10));
                }
                else
                {
                    ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                    return View("Teacher_Subjects", EndResultListOfMarks.OrderBy(s => s.Teachers_Batches.BatchName).ToPagedList(page ?? 1, 10));
                }
            }


        }

        //Add Teacher Subject New Version
        public ActionResult AddTeacherSubjects()
        {
            var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectName).Select(s => s);            
            return View(getAllSubjects);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddTeacherSubjects(string degree, string subjects, string section, string batch, string teacherID, string part)
        //{
        //    var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
        //    //var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
        //    //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);


        //    string result = EmployeesModel.AddTeacherSubject(teacherID, degree, section, batch, subjects, part);
        //    if (result == "OK")
        //    {
        //        ViewBag.Message = null;
        //        ViewBag.Degrees = getAllDegrees;
        //        ViewBag.Sections = getAllSections;
        //        //ViewBag.Subjects = getAllSubjects;
        //        //ViewBag.TeacherIDs = TeachersIDS;
        //        return RedirectToAction("AddTeacherSubjects2", new { id = batch, t_id = teacherID });
        //    }
        //    else
        //    {
        //        ViewBag.Message = result;
        //        ViewBag.Degrees = getAllDegrees;
        //        ViewBag.Sections = getAllSections;
        //        //ViewBag.Subjects = getAllSubjects;
        //        //ViewBag.TeacherIDs = TeachersIDS;
        //        return View();
        //    }

        //}
        //public ActionResult AddTeacherSubjects2(string id, string t_id)
        //{
        //    var getBatch = r.Batches.Where(s => s.BatchName == id).Select(s => s).FirstOrDefault();
        //    //var specificLevel = r.Levels.Where(s => s.LevelID == getDegreeProgram.LevelID).Select(s => s.Level_Name).FirstOrDefault();
        //    IEnumerable<Batch_Subjects_Parts> ds = EmployeesModel.getBPSForAssigningToTeachers(getBatch);
        //    //ViewBag.level = specificLevel;
        //    ViewBag.Batch = getBatch;
        //    ViewBag.teacherID = t_id;
        //    return View(ds);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddTeacherSubjects(IEnumerable<Guid> subjSelect, IEnumerable<string> batchNames
        //    , string teacherID)
        //{
        //    var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectName).Select(s => s);

        //    string result=EmployeesModel.AssignTeacherSubjects_NewFunction(subjSelect,batchNames,teacherID);

        //    if (result=="OK")
        //    {
        //        ViewBag.Message = "Successfully Subject Assigned";
        //        return View(getAllSubjects);
        //    }
        //    else
        //    {
        //        ViewBag.Message = result;
        //        return View(getAllSubjects);
        //    }
        //    //return View(getAllSubjects);
        //}


        //Teacher Batches Add, Delete,Edit,Details Starts From Here
        public ActionResult Teacher_Batches(int? page, string deleteResult)
        {
            SessionClearOnReload();

            TempData["TeacherID"] = null;
            IEnumerable<Teachers_Batches> EndResultListOfMarks = EmployeesModel.GetAllTeacher_batchesRecords();

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
                return View("Teacher_Batches", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 10));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Teacher_Batches")]
        public ActionResult Teacher_Batches1(int? page, string teacherID)
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
                IEnumerable<Teachers_Batches> EndResultListOfMarks = EmployeesModel.GetAllSearchSpecificTeacher_BatchesRecords(teacherID);
                if (EndResultListOfMarks != null)
                {
                    return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                }
                else
                {
                    //ViewBag.Message = "No Records Founds";
                    return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeacherBatchesRecords(IEnumerable<Guid> deleteTbat, string hiddenInput)
        {
            if (deleteTbat != null && hiddenInput != "")
            {
                List<Teachers_Batches> listToDelete = r.Teachers_Batches.Where(s => deleteTbat.Contains(s.ID)).ToList();

                if (EmployeesModel.DeleteBatchPlusBatchRelatedSubjectsOfTeacher(listToDelete))
                {
                    return RedirectToAction("Teacher_Batches", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted") });
                }
                else
                {
                    return RedirectToAction("Teacher_Batches", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Unable to Delete Records") });
                }
            }
            else
            {
                return RedirectToAction("Teacher_Batches", "Employees", new { deleteResult = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records to Delete Records") });
            }
        }
        public ActionResult AddTeacherBatches()
        {
            var getBatches = r.Batches.Where(s=>s.Status==1).OrderBy(s => s.BatchName).Select(s=>s);

            //var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var SectionsOfTeacher = r.Sections.Select(s => s).OrderBy(s => s.SectionID);
            //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
            //ViewBag.Degrees = ListofDegreePrograms;
            //ViewBag.Sections = SectionsOfTeacher;
            //ViewBag.TeacherIDs = TeachersIDS;

            return View(getBatches);
        }
        #region Previous Batch Assignment Code
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddTeacherBatches(string degree, string subjects, string section, string batch, string teacherID)
        //{
        //    var getBatches = r.Batches.OrderBy(s => s.BatchName).Select(s => s);
        //    var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
        //    //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);

        //    string result = EmployeesModel.AddTeacherBat(teacherID, degree, section, batch);
        //    if (result == "OK")
        //    {
        //        ViewBag.Message = "Successfully Record Added";
        //        ViewBag.Degrees = getAllDegrees;
        //        ViewBag.Sections = getAllSections;
        //        //ViewBag.TeacherIDs = TeachersIDS;
        //        return View();
        //    }
        //    else
        //    {
        //        ViewBag.Message = result;
        //        ViewBag.Degrees = getAllDegrees;
        //        ViewBag.Sections = getAllSections;
        //        //ViewBag.TeacherIDs = TeachersIDS;
        //        return View();
        //    }

        //}

        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeacherBatches(IEnumerable<string> TeacherBatchesAssign, string teacherID)
        {
            var getTeacherRec = r.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();
            var getBatches = r.Batches.Where(s => s.Status == 1).OrderBy(s => s.BatchName).Select(s => s);
            
            if (getTeacherRec == null)
            {
                ViewBag.Message = "Teacher ID is incorrect!";
                return View(getBatches);
            }
            else
            {
                if (TeacherBatchesAssign==null)
                {
                    if(EmployeesModel.DeleteAllTeacher_Batches(getTeacherRec))
                    {
                        ViewBag.Message = "Successfully Record Updated";
                        return View(getBatches);
                    }
                    else
                    {
                        ViewBag.Message = "Unable to Update Teacher Batches!";
                        return View(getBatches);
                    }
                }
                else
                {
                    string result = EmployeesModel.TeacherBatchAssign(TeacherBatchesAssign, getTeacherRec);
                    if (result=="OK")
                    {
                        ViewBag.Message = "Successfully Record Updated";
                        return View(getBatches);
                    }
                    else
                    {
                        ViewBag.Message = "Unable to Update Teacher Batches!";
                        return View(getBatches);
                    }
                }
            }
        }
        public ActionResult EditBatchSubjectsTeacher(string id)
        {
            Guid idGuid=new Guid();

            if (Guid.TryParse(id,out idGuid))
            {
                var getTeacherBatch = r.Teachers_Batches.Where(s => s.ID == idGuid).Select(s => s).FirstOrDefault();
                var getTeacherSubjects = getTeacherBatch.Teacher_Subject.ToList();
                var getSubjectsRelatedToTeacherBatch = r.Batch_Subjects_Parts.Where(s => s.BatchName == getTeacherBatch.BatchName).Select(s => s.SubjectID);
                var getDistinctSubjects = r.Subjects.Where(s => getSubjectsRelatedToTeacherBatch.Contains(s.SubjectID)).Select(s => s).Distinct().ToList();

                ViewModel_TeacherBatchSubjects vmT = new ViewModel_TeacherBatchSubjects();
                vmT.TeacherSubjBatch = getTeacherSubjects;
                vmT.SubjBatch = getDistinctSubjects;

                ViewBag.BatchTeacher = getTeacherBatch.Batch;
                ViewBag.TeacherName = getTeacherBatch.TeacherID;
                ViewBag.TeacherbatchesID = idGuid;

                return View(vmT);
            }
            else
            {
                return RedirectToAction("Teacher_Batches");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBatchSubjectsTeacher(string TeacherbatchesID, IEnumerable<Guid> subjects)
        {
            Guid idGuid = new Guid();

            if (Guid.TryParse(TeacherbatchesID, out idGuid))
            {
                var getTeacherBatch = r.Teachers_Batches.Where(s => s.ID == idGuid).Select(s => s).FirstOrDefault();

                string result = EmployeesModel.AssignTeacherSubjects_NewFunction(subjects, getTeacherBatch);
                if (result=="OK")
                {   
                    var getTeacherSubjects = getTeacherBatch.Teacher_Subject.ToList();
                    var getSubjectsRelatedToTeacherBatch = r.Batch_Subjects_Parts.Where(s => s.BatchName == getTeacherBatch.BatchName).Select(s => s.SubjectID);
                    var getDistinctSubjects = r.Subjects.Where(s => getSubjectsRelatedToTeacherBatch.Contains(s.SubjectID)).Select(s => s).Distinct().ToList();

                    ViewModel_TeacherBatchSubjects vmT = new ViewModel_TeacherBatchSubjects();
                    vmT.TeacherSubjBatch = getTeacherSubjects;
                    vmT.SubjBatch = getDistinctSubjects;

                    ViewBag.BatchTeacher = getTeacherBatch.Batch;
                    ViewBag.TeacherName = getTeacherBatch.TeacherID;
                    ViewBag.TeacherbatchesID = idGuid;
                    ViewBag.Message = "Successfully Records Updated";

                    return View(vmT);
                }
                else
                {
                    var getTeacherSubjects = getTeacherBatch.Teacher_Subject.ToList();
                    var getSubjectsRelatedToTeacherBatch = r.Batch_Subjects_Parts.Where(s => s.BatchName == getTeacherBatch.BatchName).Select(s => s.SubjectID);
                    var getDistinctSubjects = r.Subjects.Where(s => getSubjectsRelatedToTeacherBatch.Contains(s.SubjectID)).Select(s => s).Distinct().ToList();

                    ViewModel_TeacherBatchSubjects vmT = new ViewModel_TeacherBatchSubjects();
                    vmT.TeacherSubjBatch = getTeacherSubjects;
                    vmT.SubjBatch = getDistinctSubjects;

                    ViewBag.BatchTeacher = getTeacherBatch.Batch;
                    ViewBag.TeacherName = getTeacherBatch.TeacherID;
                    ViewBag.TeacherbatchesID = idGuid;
                    ViewBag.Message = result;

                    return View(vmT);
                }
            }
            else
            {
                return RedirectToAction("Teacher_Batches");
            }
        }
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

        //Ended Teacher Module For Employee
        #endregion

        #region Student Management
        //Student Records
        public ActionResult StudentRecords(string id, int? page, string res)
        {
            SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            IEnumerable<Registeration> DataBasedOnRollnos = EmployeesModel.GetAllStudentRecords();
            return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentRecords(string search, string id,
            IEnumerable<string> deleteroll, int? StudentType, string ifButtonPressed
            , int? page, string checkerForDeleteRecords, string hiddenInput)
        {
            if (ifButtonPressed != null)
            {
                IEnumerable<Registeration> SearchedData = EmployeesModel.getSpecificSearchRecord(search, StudentType);
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
                if (deleteroll != null && hiddenInput != "")
                {
                    string result = EmployeesModel.DeleteStudentRecords(deleteroll);

                    if (result == "OK")
                    {
                        return RedirectToAction("StudentRecords", "Employees",
                            new { res = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!") });
                    }
                    else
                    {
                        return RedirectToAction("StudentRecords", "Employees",
                            new { res = SherlockHolmesEncryptDecrypt.Encrypt("Unable to Delete Records!") });
                    }
                }
                else
                {
                    return RedirectToAction("StudentRecords", "Employees",
                            new { res = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!") });
                }
            }
        }
        [ValidateInput(false)]
        public ActionResult EditStudent(string id)
        {
            var getStudent = r.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();

            if (getStudent==null)
            {
                return RedirectToAction("StudentRecords");
            }
            //var getSections = r.Sections.Select(s => s);
            var getParts = r.Parts.Select(s => s.PartID);
            var getDegrees = r.Degree_Program.Select(s => s);
            //var getBatches = r.Batches.Select(s => s.BatchName);
            //ViewBag.Sections = getSections;


            ViewBag.Parts = getParts;
            ViewBag.Degrees = getDegrees;
            //ViewBag.Batches = getBatches;
            return View(getStudent);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(Registeration std, string tab, string roll,
            string batch, string degree, string section, HttpPostedFileBase file
            , string part)
        {

            var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();

            //var getSections = r.Sections.Select(s => s);
            var getParts = r.Parts.Select(s => s.PartID);
            var getDegrees = r.Degree_Program.Select(s => s);
            //var getBatches = r.Batches.Select(s => s.BatchName);
            //ViewBag.Sections = getSections;
            ViewBag.Parts = getParts;
            ViewBag.Degrees = getDegrees;
            //ViewBag.Batches = getBatches;

            if (tab == "tab1")
            {
                if (std != null)
                {
                    string result= EmployeesModel.UpdateStudentRecord(roll, std, file);
                    if (result=="OK")
                    {
                        ViewBag.Message = "Successfully Updated Record";
                        ViewBag.Message2 = "";
                        return View(getStudent);
                    }
                    else
                    {
                        ViewBag.Message = result;
                        return View(getStudent);
                    }
                }
                else
                {
                    ViewBag.Message = "No Changes were Made to Update the Record";
                    return View(getStudent);
                }
            }

                //Tab 2 Edit Student Registeration
            else
            {
                if (std != null)
                {
                    if (std.Password != getStudent.Password)
                    {
                        getStudent.Password = std.Password;
                        r.SaveChanges();
                        TempData["S"] = "OKPass";
                        ViewBag.Message2 = "Student with Name " +
                                    getStudent.Student_Profile.FirstName + " " +
                                    getStudent.Student_Profile.LastName +
                                    @" has changed its Password to" +
                                    getStudent.Password;

                        return View(getStudent);
                    }
                    else
                    {
                        if ((degree != "Please select" && degree != null)
                        && (section != null && section != "Please select")
                        && (batch != null && batch != "Please select") && part != null)
                        {
                            Guid degID = Guid.Parse(degree);
                            Guid secID = Guid.Parse(section);

                            var getSectionName = r.Sections.Where(s => s.SectionID == secID).Select(s => s.SectionName).FirstOrDefault();
                            var getDegName = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s.Degree_ProgramName).FirstOrDefault();

                            string[] result = EmployeesModel.UpdateStudentRegRecord(roll, degree, batch, section, getDegName, getSectionName, part);

                            if (result[0] == "OK")
                            {
                                TempData["S"] = result[0];

                                string NewRoll = result[1];
                                var getUpdatedStudent = r.Registerations.Where(s => s.Rollno == NewRoll
                                        && s.Status == 1)
                                        .Select(s => s).FirstOrDefault();

                                ViewBag.Message2 = "Student with Name " +
                                    getUpdatedStudent.Student_Profile.FirstName + " " +
                                    getUpdatedStudent.Student_Profile.LastName +
                                    @" has changed its degree to" +
                                    getDegName + " ,Batch: " + batch +
                                    " And Section: " + getSectionName +
                                    " New Rollno is: " + result[1];

                                return View(getUpdatedStudent);
                            }
                            else
                            {
                                TempData["S"] = result[0];
                                ViewBag.Message2 = result[1];
                                return View(getStudent);
                            }
                        }
                        else
                        {
                            TempData["S"] = "NotOK";
                            ViewBag.Message2 = "Plz Fill All the Fields!";
                            return View(getStudent);
                        }
                    }

                }
                else
                {
                    TempData["S"] = "NotOK";
                    ViewBag.Message2 = "No Changes were Made to Update the Record";
                    return View(getStudent);
                }

            }


        }
        public ActionResult AssignSubjectToUpdatedRegisteration(string id)
        {

            if (id != null)
            {
                var getStudentThatIsUpdated = r.Registerations
                    .Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();

                ViewBag.Student = getStudentThatIsUpdated;


                var getSubjectsInBatch = r.Batch_Subjects_Parts
                    .Where(s => s.BatchName == getStudentThatIsUpdated.BatchID
                    && s.Part == getStudentThatIsUpdated.Part).Select(s => s);

                return View(getSubjectsInBatch);
            }
            else
            {
                ViewBag.Message = "Unable To Assign Subjects";
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignSubjectToUpdatedRegisteration
            (IEnumerable<int?> subjects, string rollno, string assign
            , string roll)
        {

            if (assign != null)
            {
                var getStudentThatIsUpdated = r.Registerations
                    .Where(s => s.Rollno == roll)
                    .Select(s => s).FirstOrDefault();
                var getSubjectsInBatch = r.Batch_Subjects_Parts
                    .Where(s => s.BatchName == getStudentThatIsUpdated.BatchID
                    && s.Part == getStudentThatIsUpdated.Part).Select(s => s);

                ViewBag.Student = getStudentThatIsUpdated;
                return View("AssignSubjectToUpdatedRegisteration", getSubjectsInBatch);

            }
            if (subjects != null)
            {
                //Get Student That Needs to be Subject Assigned
                var getStudentThatIsUpdated = r.Registerations
                    .Where(s => s.Rollno == rollno
                    && s.Status == 1)
                    .Select(s => s).FirstOrDefault();

                //Get The Subjects
                List<Batch_Subjects_Parts> getSubjectsInBatch = r.Batch_Subjects_Parts
                    .Where(s => s.BatchName == getStudentThatIsUpdated.BatchID
                    && s.Part == getStudentThatIsUpdated.Part).Select(s => s).ToList();

                if (getStudentThatIsUpdated != null)
                {
                    ViewBag.Student = getStudentThatIsUpdated;

                    //Assign Subject To student
                    if (EmployeesModel.AssignSubjectsOnUpdationOfBatchOrDegreeOfStudent(subjects, getStudentThatIsUpdated, getSubjectsInBatch))
                    {
                        ViewBag.Message = "Succesfully Subjects Added";
                        return View(getSubjectsInBatch);
                    }
                    else
                    {
                        ViewBag.Message = "UnAble to Register Subjects to Rollno " + getStudentThatIsUpdated.Rollno;
                        return View(getSubjectsInBatch);
                    }
                }
                else
                {
                    ViewBag.Message = "UnAble to Register Subjects!";
                    return View(getSubjectsInBatch);
                }
            }

            else
            {
                ViewBag.Message = "Plz Select A Subject First";
                return View();
            }

        }

        public ActionResult DetailStudent(string id)
        {
            var getStudent = r.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();
            if (getStudent==null)
            {
                return RedirectToAction("StudentRecords");
            }
            return View(getStudent);
        }

        //Step1
        //New Admission
        public ActionResult NewAdmission()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewAdmission(Student_Profile std, string pic, HttpPostedFileBase file, string newAdmission,
            Nullable<System.DateTime> date1)
        {
            if (newAdmission != null)
            {
                Session["Success"] = "";
                //Session["stdprofile"] = new Student_Profile();
                //Session["reg"] = new Registeration();
                ViewBag.Message = "";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (file != null && file.ContentType.Contains("image") && file.ContentLength > 0 && file.ContentLength <= 3048576)
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.InputStream.CopyTo(ms);
                                byte[] array = ms.GetBuffer();
                                std.Picture = array;
                            }
                        }
                        catch (Exception)
                        {
                            ViewBag.Message = "UnAble to Add Picture in the record " + std.FirstName + " " + std.LastName;
                            return View();
                        }

                    }
                    if (file != null)
                    {
                        if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                        {
                            ViewBag.Message = "Plz Select image File less than 3 MB";
                            return View();   
                        }
                    }
                    if (date1 == null)
                    {
                        ViewBag.Message = "Date Of Birth is Required";
                        return View();
                    }
                    //stdprofile.FirstName = std.FirstName;
                    //stdprofile.LastName = std.LastName;
                    //stdprofile.Address = std.Address;
                    //stdprofile.CNIC = std.CNIC;
                    std.Date_of_Birth = date1;
                    EmployeesModel.ClearOrNotToClearValues(1);
                    EmployeesModel.SetStudentProfileNewAdmission(std);
                    //stdprofile.FatherName_Guardian_Name = std.FatherName_Guardian_Name;
                    //stdprofile.Father_Guardian_Contact = std.Father_Guardian_Contact;
                    //stdprofile.Father_Guardian_Occupation = std.Father_Guardian_Occupation;
                    //stdprofile.Gender = std.Gender;
                    //stdprofile.Religion = std.Religion;
                    //stdprofile.Domicile = std.Domicile;
                    //stdprofile.Nationality = std.Nationality;
                    //stdprofile.ProfileID = EmployeesModel.GetNewStudentProfileID();

                    return View("NewAdmissionEducationDetails");
                }
                else
                {
                    ViewBag.Message = "Error Unable To Add Student_Profile";
                    return View();
                }
            }
        }
        //Step2
        public ActionResult NewAdmissionEducationDetails()
        {
            //ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewAdmissionEducationDetails(Student_Profile std, string newAdmission)
        {
            if (newAdmission != null)
            {
                Session["Success"] = "";
                //Session["stdprofile"] = new Student_Profile();
                //Session["reg"] = new Registeration();
                //ViewBag.Message = "";
                return View();
            }
            else
            {
                //Student_Profile stdprofile = GetStudentProfile();
                //stdprofile.Matric_Marks = std.Matric_Marks;
                //stdprofile.Intermediate_Marks = std.Intermediate_Marks;
                //stdprofile.MatricFrom = std.MatricFrom;
                //stdprofile.IntermediateFrom = std.IntermediateFrom;
                EmployeesModel.SetStudentProfileNewAdmissionEducationDetails(std);
                return RedirectToAction("RegisterationForNewAdmission");
            }
        }
        //Step3
        public ActionResult RegisterationForNewAdmission()
        {
            var getSections = r.Sections.Select(s => s);
            var getParts = r.Parts.Select(s => s.PartID);
            var getDegrees = r.Degree_Program.Select(s => s);

            TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
            ViewBag.Sections = getSections;
            ViewBag.Parts = getParts;
            ViewBag.Degrees = getDegrees;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterationForNewAdmission(Registeration regForSTD, string batch,
            string degree, string part, string section, string roll, string newAdmission)
        {
            if (newAdmission != null)
            {
                Session["Success"] = "";
                ViewBag.Message = "";
                return RedirectToAction("NewAdmission", "Employees");
            }
            else
            {
                if (batch != null && batch != "Please select" && section != null && section != "Please select" && part != null && degree != null && degree != "Please select")
                {
                    //Registeration std = GetRegisteration();
                    //Student_Profile stdprofile = GetStudentProfile();

                    //int studentID = EmployeesModel.GetNewStudentID();
                    string result = EmployeesModel.NewAdmissionRegister(batch, section, part, degree);


                    var getSections = r.Sections.Select(s => s);
                    var getParts = r.Parts.Select(s => s.PartID);
                    var getDegrees = r.Degree_Program.Select(s => s);

                    TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                    ViewBag.Sections = getSections;
                    ViewBag.Parts = getParts;
                    ViewBag.Degrees = getDegrees;


                    if (result == "OK")
                    {
                        Guid secID = Guid.Parse(section);
                        Guid degID = Guid.Parse(degree);
                        int partID = int.Parse(part);

                        //std.Password = regForSTD.Password;
                        //std.RegisterationNo = regForSTD.RegisterationNo;
                        regForSTD.Status = 1;
                        //std.Rollno = regForSTD.Rollno;
                        //std.StudentID = studentID;
                        //std.ProfileID = stdprofile.ProfileID;

                        regForSTD.BatchID = batch;
                        regForSTD.Part = partID;
                        regForSTD.Batch = r.Batches.Where(s => s.BatchName == batch).Select(s => s).FirstOrDefault();
                        regForSTD.Batch.Section = r.Sections.Where(s => s.SectionID == secID).Select(s => s).FirstOrDefault();
                        regForSTD.Batch.Degree_Program = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                        regForSTD.Part1 = r.Parts.Where(s => s.PartID == partID).Select(s => s).FirstOrDefault();
                        //stdprofile.RollNo = std.Rollno;
                        EmployeesModel.SetRegisterationNewAdmission(regForSTD);

                        var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == regForSTD.BatchID
                        && s.Part == regForSTD.Part).Select(s => s).OrderBy(s => s.ID);

                        return View("SubjectAssignView", getSubjects);
                    }
                    else if (result == "Roll no Already Exists")
                    {
                        ViewBag.Message = "Roll no Already Exists";
                        TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = result;
                        TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                        return View();
                    }

                }
                else
                {
                    var getSections = r.Sections.Select(s => s);
                    var getParts = r.Parts.Select(s => s.PartID);
                    var getDegrees = r.Degree_Program.Select(s => s);
                    TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                    ViewBag.Sections = getSections;
                    ViewBag.Parts = getParts;
                    ViewBag.Degrees = getDegrees;
                    ViewBag.Message = "Unable to Register Student! Plz Ensure All Fields are Filled";
                    return View(regForSTD);
                }

            }


        }


        //Step4
        public ActionResult SubjectAssignView(Batch_Subjects_Parts bps)
        {
            if (bps==null)
            {
                return RedirectToAction("RegisterationForNewAdmission");
            }
            else
            {
                return View(bps);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectAssignView(IEnumerable<Guid> subj, string newAdmission)
        {
            if (newAdmission != null)
            {
                Session["Success"] = "";
                ViewBag.Message = "";
                return RedirectToAction("NewAdmission", "Employees");
            }
            else
            {
                Registeration regSTD = EmployeesModel.vmReg.stdRegisteration;
                //Student_Profile stdprofile = GetStudentProfile();
                //Registeration std = GetRegisteration();
                var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == regSTD.BatchID
                        && s.Part == regSTD.Part).Select(s => s).OrderBy(s => s.ID);

                if (subj != null)
                {
                    string result = EmployeesModel.NewAdmissionStudentFinalRegister(subj);
                    if (result == "Successfully Record Added")
                    {
                        ViewBag.Message = "Successfully Record Added";
                        //Session["stdprofile"] = null;
                        //Session["reg"] = null;
                        //RemoveStudentProfile();
                        //RemoveRegisteration();
                        Session["Success"] = "Successfully Record Added";
                        EmployeesModel.ClearOrNotToClearValues(2);
                        return RedirectToAction("NewAdmission", "Employees");
                    }
                    else
                    {
                        ViewBag.Message = "Unable to Add Records";
                        //Registeration re = GetRegisteration();
                        Session["Success"] = "";
                        return View(getSubjects);
                    }

                }
                else
                {
                    ViewBag.Message = "Unable to Add Records! Plz Select At Least 1 Subject";

                    //Registeration re = GetRegisteration();
                    //var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == re.BatchID
                    //    && s.PartID == re.PartID).OrderBy(s => s.ID);
                    Session["Success"] = "";
                    return View(getSubjects);
                }
            }



        }



        //Particular Student Subject Assignment
        public ActionResult Student_Subjects(int? page, string res)
        {
            SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            IEnumerable<Assign_Subject> DataBasedOnRollnos = EmployeesModel.GetAllStudentSubjectRecords();

            return View("Student_Subjects", DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 30));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Student_Subjects(string search, string id, IEnumerable<Guid> deleteroll,
            int? StudentType, string ifButtonPressed, int? page, string hiddenInput)
        {
            if (ifButtonPressed != null)
            {
                IEnumerable<Assign_Subject> SearchedData = EmployeesModel.getSpecificSearchStudentSubjRecord(search, StudentType);

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
                if (deleteroll != null && hiddenInput != "")
                {
                    string result = EmployeesModel.DeleteStudentSubjects(deleteroll);
                    if (result == "OK")
                    {
                        return RedirectToAction("Student_Subjects", "Employees"
                            , new { res = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!") });
                    }
                    else
                    {
                        return RedirectToAction("Student_Subjects", "Employees"
                            , new { res = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                    }
                }
                else
                {
                    return RedirectToAction("Student_Subjects", "Employees"
                         , new { res = SherlockHolmesEncryptDecrypt.Encrypt("Please Select Records To Delete!") });
                }
            }
        }

        public ActionResult AddSubjectToParticularStudent()
        {
            var getSections = r.Sections.Select(s => s);
            //var getParts = r.Parts.Select(s => s.PartID);
            var getSubjects = r.Subjects.Select(s => s);
            var getDegrees = r.Degree_Program.Select(s => s);
            ViewBag.Sections = getSections;
            //ViewBag.Parts = getParts;
            ViewBag.Degrees = getDegrees;
            ViewBag.Subjects = getSubjects;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubjectToParticularStudent(Assign_Subject regForSTD, string batch, string degree, string part, string section, string subjectID)
        {
            if (batch != null && section != null && part != null && degree != null && regForSTD.Registeration.Rollno != null)
            {
                
                string result = EmployeesModel.AddStudentSubject_In_Student_SubjectsView(regForSTD, batch, section, part, degree, regForSTD.Registeration.Rollno, subjectID);

                var getSections = r.Sections.Select(s => s);
                var getParts = r.Parts.Select(s => s.PartID);
                var getDegrees = r.Degree_Program.Select(s => s);
                var getSubjects = r.Subjects.Select(s => s);

                ViewBag.Sections = getSections;
                ViewBag.Parts = getParts;
                ViewBag.Degrees = getDegrees;
                ViewBag.Subjects = getSubjects;

                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Subject Assigned!";
                    return View();
                }
                else
                {
                    ViewBag.Message = result;
                    return View();
                }

            }
            else
            {
                var getSections = r.Sections.Select(s => s);
                var getParts = r.Parts.Select(s => s.PartID);
                var getDegrees = r.Degree_Program.Select(s => s);
                var getSubjects = r.Subjects.Select(s => s);

                ViewBag.Sections = getSections;
                ViewBag.Parts = getParts;
                ViewBag.Degrees = getDegrees;
                ViewBag.Subjects = getSubjects;
                ViewBag.Message = "Please Make Sure All Fields Are Selected!";

                return View();
            }
        }

        public ActionResult StudentMarksRecords(string id, int? page, string res)
        {
            SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }

            IEnumerable<Student_Marks> DataBasedOnRollnos = EmployeesModel.GetAllStudentMarksRecords();

            return View(DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentMarksRecords(string search, string id, IEnumerable<Guid> deleteroll, int? StudentType, string ifButtonPressed
            , int? page, string hiddenInput,string month,string year)
        {
            //If Search Button is pressed!
            if (ifButtonPressed != null)
            {
                IEnumerable<Student_Marks> SearchedData = EmployeesModel.getSpecificSearchStudentMarksRecord(search, StudentType,year,month);
                //IEnumerable<Student_Profile> SearchedData = EmployeesModel.getSpecificSearchRecord(search, StudentType);
                if (SearchedData!=null)
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
                if (deleteroll != null && hiddenInput != "")
                {
                    string result = EmployeesModel.DeleteStudentMarksRecords(deleteroll);

                    if (result == "OK")
                    {
                        return RedirectToAction("StudentMarksRecords", "Employees",
                            new { res = SherlockHolmesEncryptDecrypt.Encrypt("S") });
                    }
                    else
                    {
                        return RedirectToAction("StudentMarksRecords", "Employees",
                            new { res = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                    }
                }
                else
                {
                    return RedirectToAction("StudentMarksRecords", "Employees"
                        , new
                        {
                            res = SherlockHolmesEncryptDecrypt
                            .Encrypt("Plz Select Records To Delete!")
                        });
                }
            }
        }

        public ActionResult StudentAttendanceRecords(string id, int? page, string res)
        {
            SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            IEnumerable<Students_Attendance> DataBasedOnRollnos = EmployeesModel.GetAllStudentAttendanceRecords();
            return View(DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 10));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentAttendanceRecords(string search, string id, IEnumerable<Guid> deleteroll, int? StudentType, string ifButtonPressed
            , int? page, string hiddenInput,string year,string month)
        {
            if (ifButtonPressed != null)
            {
                IEnumerable<Students_Attendance> SearchedData = EmployeesModel.getSpecificSearchStudentAttendanceRecord(search, StudentType,year,month);
                //IEnumerable<Student_Profile> SearchedData = EmployeesModel.getSpecificSearchRecord(search, StudentType);
                if (SearchedData !=null)
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
                if (deleteroll != null && hiddenInput != "")
                {
                    string result = EmployeesModel.DeleteStudentAttendanceRecords(deleteroll);

                    if (result == "OK")
                    {
                        return RedirectToAction("StudentAttendanceRecords", "Employees",
                            new { res = SherlockHolmesEncryptDecrypt.Encrypt("S") });
                    }
                    else
                    {
                        return RedirectToAction("StudentAttendanceRecords", "Employees",
                            new { res = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                    }
                }
                else
                {
                    return RedirectToAction("StudentAttendanceRecords", "Employees"
                        , new
                        {
                            res = SherlockHolmesEncryptDecrypt
                            .Encrypt("Plz Select Records To Delete!")
                        });
                }
            }

        }

        //Other Methods
        private Student_Profile GetStudentProfile()
        {
            if (Session["stdprofile"] == null)
            {
                Session["stdprofile"] = new Student_Profile();
            }
            return (Student_Profile)Session["stdprofile"];
        }
        private void RemoveStudentProfile()
        {
            Session.Remove("stdprofile");
        }
        private Registeration GetRegisteration()
        {
            if (Session["reg"] == null)
            {
                Session["reg"] = new Registeration();
            }
            return (Registeration)Session["reg"];
        }

        private void RemoveRegisteration()
        {
            Session.Remove("reg");
        }


        #endregion

        #region Other Methods
        
        //Final Teacher Subject And Batch Assigning
        public JsonResult FinalResultGet(string[] batchesNames, string[] subjIDs, string teachID)
        {
            
                var getTeacherRec = r.Teachers.Where(s => s.TeacherID == teachID &&s.Status=="Active").Select(s => s).FirstOrDefault();

                if (batchesNames == null || subjIDs == null)
                {
                    return Json("Plz Select At least One Batch And Subject To Continue!");
                }
                if (getTeacherRec == null)
                {
                    return Json("Teacher Record with ID: "+teachID+" is not Found!");
                }

                //Teacher Batch Assignment Code
                

                using (TransactionScope t=new TransactionScope())
                {
                    try
                    {
                        var getTeacherBatches = getTeacherRec.Teachers_Batches;
                        bool checker2 = true;

                        foreach (var item in batchesNames)
                        {
                            foreach (var item2 in getTeacherBatches)
                            {
                                if (item == item2.BatchName)
                                {
                                    checker2 = false;
                                    break;
                                }
                            }
                            if (checker2)
                            {
                                r.Teachers_Batches.Add(new Teachers_Batches
                                {
                                    ID = Guid.NewGuid(),
                                    BatchName = item,
                                    Batch = r.Batches.Where(s => s.BatchName == item).Select(s => s).FirstOrDefault(),
                                    TeacherID = getTeacherRec.TeacherID,
                                    Teacher = getTeacherRec
                                });
                            }
                            checker2 = true;
                        }
                        r.SaveChanges();
                        t.Complete();
                    }
                    catch (Exception)
                    {
                        return Json("Unable to Assign Batches To Teacher!");
                    }
                }
            
                
//Teacher Subject Assignment Code, After Batches

                using (TransactionScope t=new TransactionScope())
                {
                    try
                    {
                        var getAllTeacherSubjects = r.Teacher_Subject.Select(s => s);

                        Guid g = new Guid();

                        //bool checker = true;
                        foreach (var item in batchesNames)
                        {
                            foreach (var item2 in subjIDs)
                            {
                                if (!getAllTeacherSubjects.Any(s => s.SubjectID.ToString() == item2
                                    && s.Teachers_Batches.BatchName == item))
                                {
                                    var recBatchTeacher = r.Teachers_Batches.Where(s => s.TeacherID == getTeacherRec.TeacherID && s.BatchName == item).Select(s => s).FirstOrDefault();

                                    r.Teacher_Subject.Add(new Teacher_Subject
                                    {
                                        ID = Guid.NewGuid(),
                                        SubjectID = Guid.Parse(item2),
                                        Teacher_BatchID = recBatchTeacher.ID,
                                        Teachers_Batches = recBatchTeacher,
                                        Subject = r.Subjects.Where(s => s.SubjectID.ToString() == item2).Select(s => s).FirstOrDefault()
                                    });
                                    //break;
                                }
                            }
                        }
                        r.SaveChanges();
                        t.Complete();
                    }
                    catch (Exception)
                    {
                        return Json("Unable To Assign Subjects To The TeacherID: "+getTeacherRec.TeacherID);
                    }
                }
                    return Json("S");
        }


        //Check Teacher ID
        public JsonResult TeacherIDVal(string teachID)
        {
            var getRecTeacher = r.Teachers.Where(s => s.TeacherID == teachID).Select(s => s).FirstOrDefault();
            return Json(getRecTeacher.TeacherID);
        }

        //Get Subjects Related To Selected Batches in Teacher Batch Assignment
        public JsonResult GetSubjects(string[] batchesNames)
        {
            var getSubj = r.Batch_Subjects_Parts.Where(s => batchesNames.Contains(s.BatchName)).Select(s => s.SubjectID).ToList();

            var getSubjects = r.Subjects.Where(s => getSubj.Contains(s.SubjectID)).Select(s => s).Distinct();

            //var getSubjectObjs = r.Subjects.Where(s => getSubj.Contains(s.SubjectID)).Select(s => s);

            //var getTBatches = rc.Teachers_Batches.Where(s => s.TeacherID == teachID).Select(s => s.Batch).ToList();
            List<string[]> l = new List<string[]>();
            foreach (var item in getSubjects)
            {
                l.Add(new string[]
                {
                    item.SubjectID.ToString(),
                    item.SubjectName,
                });
            }
            return Json(l);
        }

        //GetTbatches
        public JsonResult GetTbatches(string search)
        {
            var getTBatches = r.Teachers_Batches.Where(s => s.TeacherID == search).Select(s => s.Batch).ToList();

            List<string> l = new List<string>();
            foreach (var item in getTBatches)
            {
                l.Add(item.BatchName);
            }
            return Json(l);
        }
        public JsonResult GetTeacherBatches(string teachID1)
        {
            var getTBatches = r.Teachers_Batches.Where(s => s.TeacherID == teachID1).Select(s => s.BatchName).ToList();
            return Json(getTBatches);
        }
        public JsonResult GetBatches(string degree)
        {
            if (degree != null)
            {
                Guid programID = Guid.Parse(degree);
                List<string> getAllBatches = r.Batches.
                    Where(s => s.DegreeProgram_ID == programID
                    && s.Status == 1).Select(s => s.BatchName).ToList();
                return Json(getAllBatches);
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
        public void SessionClearOnReload()
        {
            Session["Success"] = null;
            //Session["stdprofile"] = null;
            //Session["reg"] = null;
        }
        #endregion
        
    }
}