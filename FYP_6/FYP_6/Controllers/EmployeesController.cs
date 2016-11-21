using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FYP_6.Models.Models_Logic;
using FYP_6.Models.Report_Models;
using FYP_6.Models;
using System.IO;
using FYP_6.SessionExpireChecker;
using FYP_6.Models.ViewModels;
using CrystalDecisions.CrystalReports.Engine;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpireEmp]
    public class EmployeesController : Controller
    {
        static RCIS3Entities r = RCIS3Entities.getinstance();
        EmployeesModel empModel = new EmployeesModel();

        #region EmployeeActions
        // GET: Employees
        //[OutputCache(Duration = 60)]
        public ActionResult Index()//Profile
        {
            //SessionClearOnReload();
            try
            {
                Guid EmpID = Guid.Parse(Session["EmpID"].ToString());
                var getEmployee = r.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();
                return View(getEmployee);
            }
            catch (Exception)
            {
                return View(new Employee());
            }
             
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
            
            ViewBag.Message = empModel.ChangePassword_EmployeeModelFunction(oldpass1, newpass1, EmpID);

            return View();
        }
        #endregion

        #region Teacher Management
        //Teacher Management
        #region Teacher Records Add,Delete,Edit,Details Starts From Here
        //Teacher Records Add,Delete,Edit,Details Starts From Here
        public ActionResult EnrollTeacher()
        {
            SessionClearOnReload();
            ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
            ViewBag.GradDegrees = r.GradPostDegrees.Where(s => s.Type == 1).Select(s => s);
            ViewBag.PostDegrees = r.GradPostDegrees.Where(s => s.Type == 2).Select(s => s);
            //string t_ID = empModel.getTeacherID();
            //ViewBag.Teacher_ID = t_ID;
            return View(new Teacher());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnrollTeacher(Teacher tRec, HttpPostedFileBase file, 
            string gender, Nullable<System.DateTime> date1, string Marriedstatus
            ,string PostDeg,string gradDeg,string religion)
        {
            ViewBag.GradDegrees = empModel.getGradPostDegreesType1(gradDeg);
            ViewBag.PostDegrees = empModel.getGradPostDegreesType2(PostDeg);
            ViewBag.Religions = r.Religions.OrderByDescending(s => s.ReligionName==religion).ThenBy(s => s);

            string result = empModel.EnrollTeacherForCollege(tRec, file, gender, date1, gradDeg, PostDeg, Marriedstatus);

            try
            {
                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Record Added";
                    if (date1.HasValue)
                    {
                        TempData["DateSaved"] = date1.Value.ToShortDateString();
                    }
                    return View(new Teacher());
                }
                else
                {
                    ViewBag.Message = result;
                    if (date1.HasValue)
                    {
                        TempData["DateSaved"] = date1.Value.ToShortDateString();
                    }
                    return View(new Teacher());
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Unable To Enroll Teacher! Plz Try Again!";
                return View(new Teacher());
            }
             

        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult TeacherRecords(int? page, string deleteResult, string search, string TeacherType, string submitButtonPressed)
        {
            SessionClearOnReload();
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
                return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//Also Includes Deleting Teacher
        public ActionResult TeacherRecords(string search, int? page, IEnumerable<string> deleteTeacher, string TeacherType,
            string submitButtonPressed, string hiddenInput)
        {
            //if (submitButtonPressed != null)
            //{
            //    IEnumerable<Teacher> SearchedData = empModel.getSpecificSearchRecordForTeacher(search, TeacherType);
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
            //else
            //{
                if (deleteTeacher != null && hiddenInput != "")
                {
                    string result = empModel.DeleteTeacherRecords_EMP_ModelFunction(deleteTeacher);

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
            //}
        }

        public ActionResult EditTeacher(string id)
        {
                var getTeacher = r.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
                if (getTeacher==null)
                {
                    return RedirectToAction("TeacherRecords");
                }
                ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                ViewBag.GradDegrees = r.GradPostDegrees.Where(s => s.Type == 1).Select(s => s);
                ViewBag.PostDegrees = r.GradPostDegrees.Where(s => s.Type == 2).Select(s => s);
                return View(getTeacher);   
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeacher(Teacher std, string teacherID, string batch, string degree, string part, string section, HttpPostedFileBase file,string gender
            , Nullable<System.DateTime> date1, string Marriedstatus
            , string PostDeg, string gradDeg, string religion)
        {
            var getTeacher = r.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();

            if (std != null)
            {
                string result = empModel.UpdateTeacherRecord(teacherID, std, file, gender, date1, Marriedstatus, PostDeg, gradDeg, religion);

                if (result=="OK")
                {
                    ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                    ViewBag.GradDegrees = r.GradPostDegrees.Where(s => s.Type == 1).Select(s => s);
                    ViewBag.PostDegrees = r.GradPostDegrees.Where(s => s.Type == 2).Select(s => s);
                    ViewBag.Message = "Successfully Updated Record";
                    ViewBag.Message2 = "";
                    return View(getTeacher);
                }
                else
                {
                    ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                    ViewBag.GradDegrees = r.GradPostDegrees.Where(s => s.Type == 1).Select(s => s);
                    ViewBag.PostDegrees = r.GradPostDegrees.Where(s => s.Type == 2).Select(s => s);
                    ViewBag.Message = result;
                    return View(getTeacher);
                }
            }
            else
            {
                ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                ViewBag.GradDegrees = r.GradPostDegrees.Where(s => s.Type == 1).Select(s => s);
                ViewBag.PostDegrees = r.GradPostDegrees.Where(s => s.Type == 2).Select(s => s);
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
#endregion

        #region Teacher Attendance Module Starts From Here
        //Teacher Attendance Module Starts From Here
        public ActionResult ShowOptions()
        {
            return View();
        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IndividualAttendanceTeacher()
        {
            return View(new Teacher_Attendance());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndividualAttendanceTeacher(string PresentStatusindi, Teacher_Attendance tatt,
            DateTime? date123)
        {
            try
            {
                string result = empModel.NewTeacherIndividualAttendancesAdditionCode(PresentStatusindi, date123, tatt.TeacherID);
                if (result=="OK")
                {
                    if (date123.HasValue)
                    {
                        TempData["datingNach"] = date123.Value.ToShortDateString();;
                    }
                    ViewBag.Message = "Successfully Attendance Uploaded";   
                }
                else
                {
                    if (date123.HasValue)
                    {
                        TempData["datingNach"] = date123.Value.ToShortDateString(); ;
                    }
                    ViewBag.Message = result;
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Unable to Upload Attendance! Plz Try Again!";
                return View();    
            }
             
        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult TeacherAttendance(string search, string month, string year, int? page, string deleteResult, string tattsearchsubmit
            ,string generatepdf)
        {
            TempData["T_ID"] = null;

            //var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var SectionsOfTeacher = r.Sections.Select(s => s).OrderBy(s => s.SectionID);

            if (deleteResult != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(deleteResult);
            }
            if (generatepdf!=null)
            {
                int yearInNumber = 0;
                if (!int.TryParse(year, out yearInNumber)
                    || (search == "" && search == null) || month == "None Selected")
                {
                    TempData["T_ID"] = "Plz Select All the Fields For Report Generation!";
                    return View("TeacherAttendance", null);
                }
                else
                {
                    IEnumerable<TeacherAttendanceReportClass> rpModelAtt = empModel.GetReportModelForTeacherAttendance(search, year, month);
                    if (rpModelAtt != null)
                    {
                        try
                        {
                            if (rpModelAtt.Count()==0)
                            {
                                TempData["T_ID"] = "No Record Found For Generating PDF";
                                return View("TeacherAttendance", null);
                            }
                            ReportDocument rd = new ReportDocument();
                            rd.Load(Server.MapPath("~/Reports/TeacherATTrpt.rpt"));

                            rd.SetDataSource(rpModelAtt.ToList());
                            Response.Buffer = false;
                            Response.ClearContent();
                            Response.ClearHeaders();
                            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            stream.Seek(0, SeekOrigin.Begin);
                            return File(stream, "application/Pdf", "TeacherAttendanceList.pdf");
                        }
                        catch (Exception)
                        {
                            TempData["T_ID"] = "Unable to Generate PDF! Plz Try Again!";
                            return View("TeacherAttendance", null);
                        }
                    }
                    else
                    {
                        TempData["T_ID"] = "No Record Found For Generating PDF";
                        return View("TeacherAttendance", null);
                    }

                }
                
            }
            else if (tattsearchsubmit != null || Request.QueryString["search"] != null || Request.QueryString["Month"] != null || Request.QueryString["year"] != null)
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
                            + 0 + ", Attended Lectures:" + 0+
                        @", Month: " + month + ", Year: " + year;

                        //ViewBag.Message = "No Records Founds";
                        return View("TeacherAttendance", null);
                    }
            }

            else
            {
                IEnumerable<Teacher_Attendance> EndResultListOfMarks = empModel.getResultRecordsForTeacherAttendance();
                if (EndResultListOfMarks!=null)
                {
                    //ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                    //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part.ToString());
                    //ViewBag.ListofSections = SectionsOfTeacher;
                    return View("TeacherAttendance", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 20));

                }
                else
                {
                    //ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                    //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                    //ViewBag.ListofSections = SectionsOfTeacher;
                    //ViewBag.Message = "No Records Founds";
                    return View("TeacherAttendance", null);

                }
            
            }
            
        }

        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("TeacherAttendance")]
        //public ActionResult TeacherAttendance2(int? page, string search, string month, string year)
        //{
        //    SessionClearOnReload();

        //    if (search == null || search == "")
        //    {
        //        TempData["T_ID"] = "Plz Enter Teacher ID to Search Records";

        //        return View("TeacherAttendance", null);
        //    }

        //    else
        //    {
        //        IEnumerable<Teacher_Attendance> EndResultListOfMarks = empModel.showResultsTeacherAttendance_EmployeeModelFunction(search,month,year);

        //        if (EndResultListOfMarks!=null)
        //        {
        //            TempData["T_ID"] = "Teacher ID: " + search + ", Total Lectures: " 
        //                + EndResultListOfMarks.Count() + ", Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count()+
        //            @", Month: "+month+", Year: "+year;

        //            return View("TeacherAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).Take(50).ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            TempData["T_ID"] = "No Record was Found For Teacher ID: "+ search;

        //            //ViewBag.Message = "No Records Founds";
        //            return View("TeacherAttendance", null);
        //        }
        //    }



        //}
        public ActionResult AddTeacher_Attendance_Records()
        {
            var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = empModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();

            return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeacher_Attendance_Records(Nullable<System.DateTime> date,
            IEnumerable<string> PresentStatus, IEnumerable<string> T_IDS)
        {
            #region Old Teacher Addition Code
            //var getAllTeachersIDs = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
            //var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
            //var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
            //if (TeacherIds != null || TeacherIds != "")
            //{
            //    string result = empModel.AddTeacherAtt(tAtt, TeacherIds, date);
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
                string result = empModel.NewTeacherAttendancesAdditionCode(PresentStatus, date, T_IDS);

                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Attendance Uploaded";
                    var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = empModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();
                    return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
                }
                else
                {
                    ViewBag.Message = result;
                    var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = empModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();
                    return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
                }
            }
            else
            {
                ViewBag.Message = "No Record Found To Upload Attendance!";
                var getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords = empModel.GetAllTeacherIDsNamesForUploadingTeacherAttendance();
                return View(getAllTeachersNamesAndIDsPlusEmptyAttendanceRecords);
            }
        }
        public ActionResult EditTeacher_Attendance(string id, DateTime? Date)
        {
             
             
                var getUpdatedRecord = r.Teacher_Attendance.Where(s => s.Date == Date
                && s.TeacherID == id).Select(s => s).FirstOrDefault();

                if (getUpdatedRecord==null)
                {
                    return RedirectToAction("TeacherAttendance");
                }
                return View(getUpdatedRecord);    
             
            
        }

        private string ConvertDateTimeToDate(string Date)
        {
            char[] date = new char[10];

            for (int i = 0; i < date.Length; i++)
            {
                if(Date[i] != ' ')
                {
                    date[i] = Date[i];
                }
                else
                {
                    break;
                }
            }

            string s  = new string(date);

            return s.Trim();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeacher_Attendance(string TeacherID, string OldDate, Teacher_Attendance tAtt, string PresentStatus)
        {
            try
            {
                DateTime dtOld = new DateTime();
                if (DateTime.TryParse(OldDate, out dtOld))
                {
                    //DateTime dtOld = DateTime.Parse(OldDate);
                    string result = empModel.EditTeacherAttWithDateAndStatus(tAtt, TeacherID, dtOld, PresentStatus);

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
                else
                {
                    return RedirectToAction("TeacherAttendance");
                }
            
            }
            catch (Exception)
            {
                return RedirectToAction("TeacherAttendance");
            }
              
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeacherAttendanceRecords(IEnumerable<Guid> deleteTatt, string hiddenInput)
        {
            if (deleteTatt != null && hiddenInput != "")
            {
                string result = empModel.DeleteTeacherAttendanceRecords_EMP_ModelFunction(deleteTatt);
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

        #endregion

        //Teacher Subjects Add, Delete,Edit,Details Starts From Here
        #region Previous Code of No Use Now For Teacher Subjects Add, Delete,Edit,Details Starts From Here 
        //public ActionResult Teacher_Subjects(int? page, string deleteResult)
        //{
        //    TempData["TeacherID"] = null;
        //    TempData["Batch"] = null;
        //    TempData["Section"] = null;
        //    TempData["Degree"] = null;
        //    SessionClearOnReload();

        //    IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllTeacher_SubjectsRecords();

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

        //        IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(teacherID);
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
        //        IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_SubjectsRecords(degID, sectionID, batch, teacherID);

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


        //    string result = empModel.AddTeacherSubject(teacherID, degree, section, batch, subjects, part);
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
        //    IEnumerable<Batch_Subjects_Parts> ds = empModel.getBPSForAssigningToTeachers(getBatch);
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
        //        string result = empModel.SubjectAddToBPS(subjects, getBatch, t_id);

        //        if (result == "OK")
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.teacherID = t_id;
        //            ViewBag.Message = "Successfully Records Updated";
        //            IEnumerable<Batch_Subjects_Parts> ds = empModel.getBPSForAssigningToTeachers(getBatch);
        //            return View(ds);
        //        }
        //        else
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.teacherID = t_id;
        //            ViewBag.Message = result;
        //            IEnumerable<Batch_Subjects_Parts> ds = empModel.getBPSForAssigningToTeachers(getBatch);
        //            return View(ds);
        //        }
        //    }
        //    else
        //    {
        //        if (empModel.DeleteAllSubjectsToBPS(getBatch, t_id))
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.teacherID = t_id;
        //            ViewBag.Message = "Successfully Records Updated";
        //            IEnumerable<Batch_Subjects_Parts> ds = empModel.getBPSForAssigningToTeachers(getBatch);
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
                string result = empModel.DeleteTeacherSubjectRecords_EMP_ModelFunction(deleteTSub);
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

        

        public ActionResult Teacher_Subjects(int? page, string deleteResult)
        {
            TempData["TeacherID"] = null;
            TempData["Batch"] = null;
            TempData["Section"] = null;
            TempData["Degree"] = null;
            SessionClearOnReload();

            IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllTeacher_SubjectsRecords();

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

                IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(teacherID);
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
                IEnumerable<Teacher_Subject> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_SubjectsRecords(degID, sectionID, batch, teacherID);

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


        //    string result = empModel.AddTeacherSubject(teacherID, degree, section, batch, subjects, part);
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
        //    IEnumerable<Batch_Subjects_Parts> ds = empModel.getBPSForAssigningToTeachers(getBatch);
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

        //    string result=empModel.AssignTeacherSubjects_NewFunction(subjSelect,batchNames,teacherID);

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

        #endregion

        //Teacher Batches Add, Delete,Edit,Details Starts From Here

        #region Teacher Batches Add, Delete,Edit,Details Starts From Here
        public ActionResult Teacher_Batches(int? page, string deleteResult, string searchbutton, string teacherID)
        {
            SessionClearOnReload();

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
                        return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
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
                    return View("Teacher_Batches", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 10));
                }
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
                IEnumerable<Teachers_Batches> EndResultListOfMarks = empModel.GetAllSearchSpecificTeacher_BatchesRecords(teacherID);
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

                if (empModel.DeleteBatchPlusBatchRelatedSubjectsOfTeacher(listToDelete))
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

        //    string result = empModel.AddTeacherBat(teacherID, degree, section, batch);
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
                    if(empModel.DeleteAllTeacher_Batches(getTeacherRec))
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
                    string result = empModel.TeacherBatchAssign(TeacherBatchesAssign, getTeacherRec);
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

                string result = empModel.AssignTeacherSubjects_NewFunction(subjects, getTeacherBatch);
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

#endregion

        //Ended Teacher Module For Employee

        #endregion

        #region Student Management

        #region Student Records Edit, Details, View,Delete
        //Student Records
        [ValidateInput(false)] 
        public ActionResult StudentRecords(string id, int? page, string res, string ifButtonPressed, string search2
            , int? StudentType, string generatepdf, string searchfname, string searchdeg
            , string searchsection, string searchpart)
        {
            SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }

            if (generatepdf!=null)
            {
                IEnumerable<ReportClassForStudentsListByEmployee> rcfslbe = empModel.
                    GetReportModelForStudentList(search2, StudentType, searchfname, searchdeg
                    , searchsection, searchpart);

                if (rcfslbe != null)
                {
                    if (rcfslbe.Count()==0)
                    {
                        ViewBag.Message = "No Record Found For Generating PDF";
                        return View("StudentRecords", null);
                    }
                    try
                    {
                        ReportDocument rd = new ReportDocument();
                        rd.Load(Server.MapPath("~/Reports/StudentsReportByEmployeeModel.rpt"));

                        rd.SetDataSource(rcfslbe.ToList());
                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        return File(stream, "application/Pdf", "StudentList.pdf");
                    }
                    catch (Exception)
                    {
                        ViewBag.Message = "Unable to Generate PDF! Plz Try Again!";
                        return View("StudentRecords", null);
                    }
                }
                else
                {
                    ViewBag.Message = "No Record Found For Generating PDF";
                    return View("StudentRecords", null);
                }

            }
            else if (ifButtonPressed != null || Request.QueryString["search2"] != null || Request.QueryString["StudentType"] != null
                || Request.QueryString["searchfname"] != null
                || Request.QueryString["searchdeg"] != null || Request.QueryString["searchsection"] != null
                || Request.QueryString["searchpart"] != null)
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
                if (DataBasedOnRollnos==null)
                {
                    ViewBag.SearchQuery = "Nothing";
                    return View("StudentRecords", null);
                }
                ViewBag.SearchQuery = "True";
                return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 20));
            }
            
        }
        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult StudentRecords(string search, string id,
            IEnumerable<string> deleteroll, int? StudentType, string ifButtonPressed
            , int? page, string checkerForDeleteRecords, string hiddenInput)
        {
                if (deleteroll != null && hiddenInput != "")
                {
                    string result = empModel.DeleteStudentRecords(deleteroll);

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
            ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
            ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
            //ViewBag.Batches = getBatches;
            return View(getStudent);


        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(Registeration std, string tab, string roll,
            string batch, string degree, string section, HttpPostedFileBase file
            , string part, string gender, Nullable<System.DateTime> date1, string province
            , string domicile, string dom, string religion, IEnumerable<Guid> deletesubjinEdit)
        {
            
             
            var getParts = r.Parts.Select(s => s.PartID);
            var getDegrees = r.Degree_Program.Select(s => s);
             
            ViewBag.Parts = getParts;
            ViewBag.Degrees = getDegrees;
             
            ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
            ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);

            if (tab == "tab1")
            {
                if (std != null)
                {
                    string result= empModel.UpdateStudentRecord(roll, std, file,gender,date1,province,domicile,dom,religion);
                    if (result=="OK")
                    {
                        ViewBag.Message = "Successfully Updated Record";
                        ViewBag.Message2 = "";
                        var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                        return View(getStudent);
                    }
                    else
                    {
                        ViewBag.Message = result;
                        var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                        return View(getStudent);
                    }
                }
                else
                {
                    ViewBag.Message = "No Changes were Made to Update the Record";
                    var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                    return View(getStudent);
                }
            }

                //Tab 2 Edit Student Registeration
            else if (tab == "tab2")
            {
                
                if (std != null)
                {
                    var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                    if (getStudent.Student_Profile.Status == 0)
                    {
                        TempData["S"] = "NotOK";
                        ViewBag.Message2 = "Past Students Password OR Registeration cannot be Changed!";
                        //var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                        return View(getStudent);
                    }
                    if (std.Password != getStudent.Password)
                    {
                        getStudent.Password = std.Password;
                        r.SaveChanges();
                        TempData["S"] = "OKPass";
                        ViewBag.Message2 = "Student with Name " +
                                    getStudent.Student_Profile.FirstName + " " +
                                    getStudent.Student_Profile.LastName +
                                    @" has changed its Password to " +
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

                            string[] result = empModel.UpdateStudentRegRecord(roll, degree, batch, section, getDegName, getSectionName, part);

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
                    var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                    return View(getStudent);
                }

            }
            else if (tab == "tab3") 
            {
                try
                {
                    var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                    if (deletesubjinEdit == null)
                    {
                        TempData["t3"] = "Plz Select Records To Delete!";
                         
                        return View(getStudent);
                    }
                    string result = empModel.DeleteStudentSubjects(deletesubjinEdit,getStudent.Part);
                    if (result == "OK")
                    {
                        TempData["t3"] = "Successfully Records Deleted";
                        //var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                        return View(getStudent);
                    }
                    else
                    {
                        TempData["t3"] = result;
                        //var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                        return View(getStudent);
                    }
                }
                catch (Exception)
                {
                    return RedirectToAction("StudentRecords");
                }
                 
            }
            else
            {
                var getStudent = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                return View(getStudent);
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
                    if (empModel.AssignSubjectsOnUpdationOfBatchOrDegreeOfStudent(subjects, getStudentThatIsUpdated, getSubjectsInBatch))
                    {
                        ViewBag.Message = "Successfully Subjects Added";
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

        #endregion

        #region New Admission
        //Step1
        //New Admission
        public ActionResult NewAdmission()
        {
            ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
            ViewBag.Provinces=r.Provinces.OrderBy(s=>s.ID).Select(s=>s);
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult NewAdmission(Student_Profile std, string pic, HttpPostedFileBase file, string newAdmission,
            Nullable<System.DateTime> date1,string gender,string province,string domicile
            ,string religion)
        {
            if (newAdmission != null)
            {
                Session["Success"] = "";
                ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
                ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                //Session["stdprofile"] = new Student_Profile();
                //Session["reg"] = new Registeration();
                ViewBag.Message = "";
                return View();
            }
            else
            {
                try
                {
                    int provinceID=0;
                    int domicileID=0;

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
                            ViewBag.Religions = r.Religions.OrderByDescending(s => s.ReligionName == religion).ThenBy(s => s);
                            ViewBag.Message = "UnAble to Add Picture in the record " + std.FirstName + " " + std.LastName;
                            ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
                            if (date1.HasValue)
                            {
                                TempData["DateSaved"] = date1.Value.ToShortDateString();    
                            }
                             
                            return View();
                        }

                    }
                    if (file != null)
                    {
                        if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                        {
                            ViewBag.Message = "Plz Select image File less than 3 MB";
                            ViewBag.Religions = r.Religions.OrderByDescending(s => s.ReligionName == religion).ThenBy(s => s);
                            ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
                            if (date1.HasValue)
                            {
                                TempData["DateSaved"] = date1.Value.ToShortDateString();
                            }
                            return View();   
                        }
                    }
                    if (date1 == null)
                    {
                        ViewBag.Message = "Date Of Birth is Required";
                        ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
                        ViewBag.Religions = r.Religions.OrderByDescending(s => s.ReligionName == religion).ThenBy(s => s);
                         
                        return View();
                    }

                    if(int.TryParse(province,out provinceID) && int.TryParse(domicile,out domicileID))
                    {
                        std.Province = r.Provinces.Where(s => s.ID == provinceID).Select(s => s.Province_Name).FirstOrDefault()??"";
                        std.Domicile = r.Districts.Where(s => s.DistrictID== domicileID).Select(s => s.DistrictName).FirstOrDefault()??"";
                    }
                    else
                    {
                        ViewBag.Message = "Plz Provide Valid Province and Domicile Details!";
                        ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
                        ViewBag.Religions = r.Religions.OrderByDescending(s => s.ReligionName == religion).ThenBy(s => s);
                        if (date1.HasValue)
                        {
                            TempData["DateSaved"] = date1.Value.ToShortDateString();
                        }
                        return View();
                    }
                    std.Date_of_Birth = date1;
                    empModel.ClearOrNotToClearValues(1);
                    empModel.SetStudentProfileNewAdmission(std,gender,religion);
                    

                    return RedirectToAction("NewAdmissionEducationDetails");
                }
                catch(Exception e)
                {
                    ViewBag.Message = "Error Unable To Add Student_Profile";
                    ViewBag.Provinces = r.Provinces.OrderBy(s => s.ID).Select(s => s);
                    ViewBag.Religions = r.Religions.OrderBy(s => s.ID).Select(s => s);
                    if (date1.HasValue)
                    {
                        TempData["DateSaved"] = date1.Value.ToShortDateString();
                    }
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
        [ValidateInput(false)]
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
                try
                {
                    if ((std.IntermediateFrom != ""
                        && std.IntermediateFrom != null)
                        && (std.Intermediate_Marks.ToString() != ""
                        && std.Intermediate_Marks != null)
                        && (std.Total_Inter_Marks.ToString() != "" &&
                        std.Total_Inter_Marks != null))
                    {
                        if (std.Matric_Marks> std.Total_Matric_Marks)
                        {
                            ViewBag.Message = "Obtained Matric Marks Must be Less than Total Matric Marks!";
                            return View("NewAdmissionEducationDetails");
                        }
                        if (std.Intermediate_Marks > std.Total_Inter_Marks)
                        {
                            ViewBag.Message = "Obtained Intermediate Marks Must be Less than Total Intermediate Marks!";
                            return View("NewAdmissionEducationDetails");
                        }
                        empModel.SetStudentProfileNewAdmissionEducationDetails(std);
                        return RedirectToAction("RegisterationForNewAdmission");    
                    }
                    else if ((std.IntermediateFrom == ""
                        || std.IntermediateFrom == null)
                        && (std.Intermediate_Marks.ToString() == ""
                        || std.Intermediate_Marks == null)
                        && (std.Total_Inter_Marks.ToString() == "" ||
                        std.Total_Inter_Marks == null))
                    {
                        if (std.Matric_Marks > std.Total_Matric_Marks)
                        {
                            ViewBag.Message = "Obtained Matric Marks Must be Less than Total Matric Marks!";
                            return View("NewAdmissionEducationDetails");
                        }
                        empModel.SetStudentProfileNewAdmissionEducationDetails(std);
                        return RedirectToAction("RegisterationForNewAdmission");    
                    }
                    else
                    {
                        ViewBag.Message = "Plz Enter All The Intermediate Details OR Leave All of Them Empty";
                        return View("NewAdmissionEducationDetails");
                    }
                      
                }
                catch (Exception)
                {
                    ViewBag.Message = "Error Occured During Registeration! Plz Try Again!";
                    return View("NewAdmissionEducationDetails");
                }
                
            }
        }
        //Step3
        public ActionResult RegisterationForNewAdmission()
        {
            //var getSections = r.Sections.Select(s => s);
            var getParts = r.Parts.Select(s => s.PartID);
            var getDegrees = r.Degree_Program.Select(s => s);

            TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
            //ViewBag.Sections = getSections;
            ViewBag.Parts = getParts;
            ViewBag.Degrees = getDegrees;

            return View();

        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterationForNewAdmission(Registeration regForSTD, string batch,
            string degree, string part, string section, string roll, string newAdmission)
        {
            try
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

                        //int studentID = empModel.GetNewStudentID();
                        string result = empModel.NewAdmissionRegister(batch, section, part, degree, regForSTD.Rollno);


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
                            empModel.SetRegisterationNewAdmission(regForSTD);

                            var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == regForSTD.BatchID
                            && s.Part == regForSTD.Part).Select(s => s).OrderBy(s => s.ID);

                            //TempData["BatchSAV"] = batch;
                            //TempData["SectionSAV"] = regForSTD.Batch.Section.SectionName;
                            //TempData["degreeProgramSAV"] = regForSTD.Batch.Degree_Program.Degree_ProgramName;
                            //TempData["roll"]=regForSTD.Rollno;
                            ViewModel_FeeManagement vmFee = new ViewModel_FeeManagement();
                            vmFee.feeSummary = new Overall_Fees();
                            vmFee.feeSummary.RollNo = regForSTD.Rollno;
                            vmFee.feeSummary.SubmittedFee = 0;
                            vmFee.feeSummary.RemainingFee = 0;
                            vmFee.Name= EmployeesModel.vmReg.stdProfile.FirstName +" "+EmployeesModel.vmReg.stdProfile.LastName;

                            return View("AddFeeRecords", vmFee);
                        }
                        else if (result == "Roll no Already Exists")
                        {
                            ViewBag.Message = "Roll no Already Exists";
                            TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                            return View(regForSTD);
                        }
                        else
                        {
                            ViewBag.Message = result;
                            TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                            return View(regForSTD);
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
            catch (Exception)
            {
                var getSections = r.Sections.Select(s => s);
                var getParts = r.Parts.Select(s => s.PartID);
                var getDegrees = r.Degree_Program.Select(s => s);
                TempData["StudentName"] = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                ViewBag.Sections = getSections;
                ViewBag.Parts = getParts;
                ViewBag.Degrees = getDegrees;
                ViewBag.Message = "Unable to Register Student! Plz Try Again";
                return View("RegisterationForNewAdmission");
            } 
        }


        //Step4
        [HttpGet]
        public ActionResult AddFeeRecords()
        {
            //ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddFeeRecords(ViewModel_FeeManagement feeRec, Nullable<System.DateTime> date1
            , string totaldegfee, string totalSubmitfee, string totalremfee, string totalinstall, string paidInst
            , string newAdmission)
        {
            if (newAdmission != null)
            {
                Session["Success"] = "";
                ViewBag.Message = "";
                return RedirectToAction("NewAdmission", "Employees");
            }
            else
            {

            }
            //ViewModel_FeeManagement vmfee = new ViewModel_FeeManagement();
            if (date1 == null)
            {
                ViewBag.Message = "Error! Unable to Add Record! Date is Required!";
                return View();
            }
            feeRec.yearlyFee.Bill_No = HttpUtility.HtmlEncode(feeRec.yearlyFee.Bill_No).ToString();
            string result = empModel.NewAdmissionStudentFinalRegister(feeRec, date1, paidInst);
            //string result2 = empModel.AddFeeRec(feeRec, date1, totaldegfee, totalSubmitfee, totalremfee, totalinstall, paidInst);
            if (result == "OK" )
            {
                //ViewBag.Message = "Successfully Record Added";
                if (date1.HasValue)
                {
                    TempData["DateSaved"] = date1.Value.ToShortDateString();
                }
                Session["Success"] = "Successfully Record Added";
                empModel.ClearOrNotToClearValues(2);
                return RedirectToAction("NewAdmission", "Employees");
            }
            else
            {
                ViewBag.Message = result;
                feeRec.Name = EmployeesModel.vmReg.stdProfile.FirstName + " " + EmployeesModel.vmReg.stdProfile.LastName;
                if (date1.HasValue)
                {
                    TempData["DateSaved"] = date1.Value.ToShortDateString();
                }

                return View(feeRec);
            } 
        }
        //public ActionResult SubjectAssignView(Batch_Subjects_Parts bps)
        //{ 
        //        return View(bps); 
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SubjectAssignView(IEnumerable<Guid> subj, string newAdmission)
        //{
        //    if (newAdmission != null)
        //    {
        //        Session["Success"] = "";
        //        ViewBag.Message = "";
        //        return RedirectToAction("NewAdmission", "Employees");
        //    }
        //    else
        //    {
        //        Registeration regSTD = EmployeesModel.vmReg.stdRegisteration;
        //        if (subj != null)
        //        {
                    
        //            string result = empModel.NewAdmissionStudentFinalRegister(subj);
        //            try
        //            { 
        //                var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == regSTD.BatchID
        //                        && s.Part == regSTD.Part).Select(s => s).OrderBy(s => s.ID);

                         
        //                if (result == "Successfully Record Added")
        //                {
        //                    ViewBag.Message = "Successfully Record Added";
        //                    //Session["stdprofile"] = null;
        //                    //Session["reg"] = null;
        //                    //RemoveStudentProfile();
        //                    //RemoveRegisteration();
        //                    Session["Success"] = "Successfully Record Added";
        //                    empModel.ClearOrNotToClearValues(2);
        //                    return RedirectToAction("NewAdmission", "Employees");
        //                }
        //                else
        //                {
        //                    TempData["BatchSAV"] = regSTD.Batch.BatchName;
        //                    TempData["SectionSAV"] = regSTD.Batch.Section.SectionName;
        //                    TempData["degreeProgramSAV"] = regSTD.Batch.Degree_Program.Degree_ProgramName;

        //                    ViewBag.Message = "Unable to Add Records";
        //                    //Registeration re = GetRegisteration();
        //                    Session["Success"] = "";
        //                    return View(getSubjects);
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                TempData["BatchSAV"] = regSTD.Batch.BatchName;
        //                TempData["SectionSAV"] = regSTD.Batch.Section.SectionName;
        //                TempData["degreeProgramSAV"] = regSTD.Batch.Degree_Program.Degree_ProgramName;
        //                return View();
        //            }
                     

        //        }
        //        else
        //        {
        //            ViewBag.Message = "Unable to Add Records! Plz Select At Least 1 Subject To Continue!";
        //            TempData["BatchSAV"] = regSTD.Batch.BatchName;
        //            TempData["SectionSAV"] = regSTD.Batch.Section.SectionName;
        //            TempData["degreeProgramSAV"] = regSTD.Batch.Degree_Program.Degree_ProgramName;
        //            //Registeration re = GetRegisteration();
        //            //var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == re.BatchID
        //            //    && s.PartID == re.PartID).OrderBy(s => s.ID);
        //            Session["Success"] = "";
        //            return View();
        //        }
        //    }



        //}

        #endregion

        #region Student Subjects CRUD
        //Particular Student Subject Assignment
        public ActionResult Student_Subjects(int? page, string res, string ifButtonPressed, string search2, int? StudentType)
        {
            SessionClearOnReload();
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

                return View("Student_Subjects", DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 30));
            }
            
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Student_Subjects(string search, string id, IEnumerable<Guid> deleteroll,
        //    int? StudentType, string ifButtonPressed, int? page, string hiddenInput)
        //{
        //        if (deleteroll != null && hiddenInput != "")
        //        {
        //            string result = empModel.DeleteStudentSubjects(deleteroll);
        //            if (result == "OK")
        //            {
        //                return RedirectToAction("Student_Subjects", "Employees"
        //                    , new { res = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!") });
        //            }
        //            else
        //            {
        //                return RedirectToAction("Student_Subjects", "Employees"
        //                    , new { res = SherlockHolmesEncryptDecrypt.Encrypt(result) });
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Student_Subjects", "Employees"
        //                 , new { res = SherlockHolmesEncryptDecrypt.Encrypt("Please Select Records To Delete!") });
        //        }
        //}

        public ActionResult AddSubjectToParticularStudent(string id)
        {
            try
            {
                var rec = r.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();
                if (rec != null)
                {
                    //var getSections = r.Sections.Select(s => s);
                    //var getParts = r.Parts.Select(s => s.PartID);
                    var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == rec.BatchID
                        && s.Part == rec.Part).Select(s => s);

                    //var getDegrees = r.Degree_Program.Select(s => s);
                    //ViewBag.Sections = getSections;
                    //ViewBag.Parts = getParts;
                    //ViewBag.Degrees = getDegrees;
                    ViewBag.Subjects = getSubjects;
                     
                    return View(new Assign_Subject { Rollno = id,Registeration=rec});
                }
                else
                {
                    return RedirectToAction("StudentRecords", "Employees");
                }
            
            }
            catch (Exception)
            {
                return RedirectToAction("StudentRecords", "Employees");
            }
              
            //return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubjectToParticularStudent(Assign_Subject regForSTD, string subjectID)
        {
            try
            {
                    var rec = r.Registerations.Where(s => s.Rollno == regForSTD.Rollno).Select(s => s).FirstOrDefault();
                     
                if ( rec!= null )
                {
                    var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == rec.BatchID
                        && s.Part == rec.Part).Select(s => s);

                    ViewBag.Subjects = getSubjects;

                    if (rec.Student_Profile.Status==0)
                    { 
                        ViewBag.Message = "Cannot Assign Subjects To Past Students!";
                        return View(new Assign_Subject { Rollno = regForSTD.Rollno, Registeration = rec });
                    }
                    string result = empModel.AddStudentSubject_In_Student_SubjectsView(regForSTD, regForSTD.Rollno, subjectID,rec); 

                    if (result == "OK")
                    {
                        ViewBag.Message = "Successfully Subject Assigned!";
                        //var rec = r.Registerations.Where(s => s.Rollno == regForSTD.Rollno).Select(s => s).FirstOrDefault();

                        return View(new Assign_Subject { Rollno = regForSTD.Rollno, Registeration = rec });
                    }
                    else
                    {
                        ViewBag.Message = result;
                        return View(new Assign_Subject { Rollno = regForSTD.Rollno, Registeration = rec });
                    }

                }
                else
                {
                     //var rec = r.Registerations.Where(s => s.Rollno == regForSTD.Rollno).Select(s => s).FirstOrDefault();
                    //var getSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == rec.BatchID
                    //    && s.Part == rec.Part).Select(s => s);

                    //var getSections = r.Sections.Select(s => s);
                    //var getParts = r.Parts.Select(s => s.PartID);
                    //var getDegrees = r.Degree_Program.Select(s => s);
                    //var getSubjects = r.Subjects.Select(s => s);

                    //ViewBag.Sections = getSections;
                    //ViewBag.Parts = getParts;
                    //ViewBag.Degrees = getDegrees;
                    //ViewBag.Subjects = getSubjects;
                    ViewBag.Message = "Please Make Sure Rollno is accurate!";

                    return View(new Assign_Subject { Rollno = regForSTD.Rollno, Registeration = rec });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("StudentRecords", "Employees");
            }
             
        }

        #endregion

        #region Student Results- Marks AND Attendance
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult StudentMarksRecords(string id, int? page, string res, string ifButtonPressed,
            string search2,int? StudentType,string month,string year,string generatepdf)
        {
            //SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            if (generatepdf != null)
            {
                if (month == "None Selected" || (year == null || year == ""))
                {
                    ViewBag.Message = "Month and Year Fields are required for Report Generation!";
                    return View("StudentMarksRecords", null);
                }

                List<MarksBYTeacherReportClass> stdMarks = empModel.GetStudentMarksListForReport(month, StudentType, search2, year);

                if (stdMarks != null)
                {
                    if (stdMarks.Count > 0)
                    {
                        try
                        {
                            ReportDocument rd = new ReportDocument();
                            rd.Load(Server.MapPath("~/Reports/MarksRPTEmp.rpt"));

                            rd.SetDataSource(stdMarks.ToList());
                            Response.Buffer = false;
                            Response.ClearContent();
                            Response.ClearHeaders();
                            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            stream.Seek(0, SeekOrigin.Begin);
                            return File(stream, "application/Pdf", "ListOfMarks.pdf");
                        }
                        catch (Exception)
                        {
                            ViewBag.Message = "Unable to Generate PDF! Plz Try Again!";
                            return View("StudentMarksRecords", null);
                        }

                    }
                    else
                    {
                        ViewBag.Message = "No Record Found For PDF Generation";
                        return View("StudentMarksRecords", null);
                    }

                }
                else
                {
                    ViewBag.Message = "No Record Found For PDF Generation";
                    return View("StudentMarksRecords", null);
                }
            }
                  
             else if (ifButtonPressed != null || Request.QueryString["search2"] != null
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentMarksRecords(string search, string id, IEnumerable<Guid> deleteroll, int? StudentType, string ifButtonPressed
            , int? page, string hiddenInput,string month,string year)
        {
            //If Search Button is pressed!
            
            
                if (deleteroll != null && hiddenInput != "")
                {
                    string result = empModel.DeleteStudentMarksRecords(deleteroll);

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

        public ActionResult StudentAttendanceRecords(string id, int? page, string res,
            string ifButtonPressed, string search2, int? StudentType, string month, string year,
            string generatepdf)
        {
            SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            if (generatepdf != null)
            {
                if (month == "None Selected" || (year == null || year == ""))
                {
                    ViewBag.Message = "Month and Year Fields are required for Report Generation!";
                    return View("StudentAttendanceRecords", null);
                }
                //var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
                //ViewBag.ListofDegreePrograms = ListofDegreePrograms;


                List<AttendanceBYTeacherReportClass> attendanceStudent = empModel.
                    GetStudentAttendanceListForReport (month,StudentType,search2,year);

                    if (attendanceStudent != null)
                    {
                        if (attendanceStudent.Count > 0)
                        {
                            try
                            {
                                ReportDocument rd = new ReportDocument();
                                rd.Load(Server.MapPath("~/Reports/AttendanceRPTEmp.rpt"));

                                rd.SetDataSource(attendanceStudent.ToList());
                                Response.Buffer = false;
                                Response.ClearContent();
                                Response.ClearHeaders();
                                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                stream.Seek(0, SeekOrigin.Begin);
                                return File(stream, "application/Pdf", "ListOfAttendance.pdf");
                            }
                            catch (Exception)
                            {
                                ViewBag.Message = "Unable to Generate PDF! Plz Try Again!";
                                return View("StudentAttendanceRecords", null);
                            }

                        }
                        else
                        {
                            ViewBag.Message = "No Record Found For PDF Generation";
                            return View("StudentAttendanceRecords", null);
                        }

                    }
                    else
                    {
                        ViewBag.Message = "No Record Found For PDF Generation";
                        return View("StudentAttendanceRecords", null);
                    } 
                 
            }
            else if (ifButtonPressed != null || Request.QueryString["search2"] != null
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
                if (DataBasedOnRollnos==null)
                {
                    ViewBag.SearchQuery = "Nothing";
                    return View("StudentAttendanceRecords",null);
                }
                ViewBag.SearchQuery = "True";
                return View(DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 10));
            }
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentAttendanceRecords(string search, string id, IEnumerable<Guid> deleteroll, int? StudentType, string ifButtonPressed
            , int? page, string hiddenInput,string year,string month)
        {
                if (deleteroll != null && hiddenInput != "")
                {
                    string result = empModel.DeleteStudentAttendanceRecords(deleteroll);

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
        #endregion

        #region Student Admission Helper Methods I used it in the past but now of No Use
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

        #endregion

        #region Json Methods


        public JsonResult GetNewRollNo(string batch)
        {
            var getLastRegisterationBatchRecord = r.Registerations.
                Where(s => s.BatchID == batch).
                OrderByDescending(s => s.Rollno).
                Select(s => s.Rollno).FirstOrDefault();

            if (getLastRegisterationBatchRecord==null)
            {
                string s3 = batch + "-" + "1";
                //newRollno = int.Parse(s3);
                return Json(s3);
            }
            //getLastRegisterationBatchRecord= 
            string[] s2 = getLastRegisterationBatchRecord.Split(new char[] { '-' });
            int newRollno = 0;
            //int newRollno = int.Parse(s2[1]);
            if (s2 == null)
            {
                string s3 = batch + "-" + "1";
                //newRollno = int.Parse(s3);
                return Json(s3);
            }
            else
            {
                newRollno = int.Parse(s2[1]);
                newRollno++;
                string s4 = s2[0] + "-" + newRollno.ToString();
                //newRollno = int.Parse(s4);
                return Json(s4);
            }
        }
        //Get Domiciles
        public JsonResult GetCitiesDomicile(string province)
        {
            try
            {
                int val = 0;
                List<District> getRelatedRec = new List<District>();
                if (int.TryParse(province, out val))
                {
                    foreach (var item in r.Districts)
                    {
                        if (item.ProvinceID==val)
                        {
                            getRelatedRec.Add(new District
                            {
                                DistrictID=item.DistrictID,
                                DistrictName=item.DistrictName
                            });
                        }
                    }
                    //List<District> getRelatedRec = r.Districts.Where(s => s.ProvinceID == val).OrderBy(s => s.DistrictID).Select(s => s).ToList();
                    return Json(getRelatedRec);
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception)
            {
                return Json(null);
            }
            
        }
        //Final Teacher Subject And Batch Assigning
        public JsonResult FinalResultGet(string[] batchesNames, string[] subjIDs, string teachID)
        {
            
            var getTeacherRec = r.Teachers.Where(s => s.TeacherID == teachID &&s.Status=="Active").Select(s => s).FirstOrDefault();

            using (TransactionScope t=new TransactionScope())
                {
            try
            {
                List<Guid> lGuid = new List<Guid>();

                if (batchesNames == null || subjIDs == null)
                {
                    return Json("Plz Select At least One Batch And Subject To Continue!");
                }
                if (getTeacherRec == null)
                {
                    return Json("Teacher Record with ID: " + teachID + " is not Found!");
                }

                foreach (var item in subjIDs)
                {
                    lGuid.Add(Guid.Parse(item));
                }
                foreach (var item in batchesNames)
                {
                    foreach (var item2 in lGuid)
                    {
                        //Guid gd = Guid.Parse(item2);
                        if (!r.Batch_Subjects_Parts.Any(s => s.BatchName == item
                            && s.SubjectID == item2))
                        {
                            return Json("The Selected Subject " + r.Subjects.Where(s => s.SubjectID == item2).Select(s => s.SubjectName).FirstOrDefault()
                                + " is not assigned to Batch " + item);
                        }
                        if (r.Teacher_Subject.Any(s => s.Teachers_Batches.BatchName == item
                            && s.SubjectID == item2))
                        {
                            return Json("The Selected Subject " + r.Subjects.Where(s => s.SubjectID == item2).Select(s => s.SubjectName).FirstOrDefault()
                                + " of Batch " + item + " is being Taught By Another teacher");
                        }
                    }
                }

                //foreach (var item in batchesNames)
                //{
                //    foreach (var item2 in lGuid)
                //    {
                //        if (!r.Batch_Subjects_Parts.Any(s=>s.BatchName==item
                //            &&s.SubjectID==item2))
                //        {
                //            return Json("The Selected Subject " + r.Subjects.Where(s => s.SubjectID == item2).Select(s => s.SubjectName).FirstOrDefault()
                //                + " is not assigned to Batch " + item );
                //        }
                //    }

                //}
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
                var getAllTeacherSubjects = r.Teacher_Subject.Select(s => s);

                //Guid g = new Guid();

                //bool checker = true;
                foreach (var item in batchesNames)
                {
                    foreach (var item2 in lGuid)
                    {
                        if (!getAllTeacherSubjects.Any(s => s.SubjectID == item2
                            && s.Teachers_Batches.BatchName == item))
                        {
                            var recBatchTeacher = r.Teachers_Batches.Where(s => s.TeacherID == getTeacherRec.TeacherID && s.BatchName == item).Select(s => s).FirstOrDefault();

                            r.Teacher_Subject.Add(new Teacher_Subject
                            {
                                ID = Guid.NewGuid(),
                                SubjectID = item2,
                                Teacher_BatchID = recBatchTeacher.ID,
                                Teachers_Batches = recBatchTeacher,
                                Subject = r.Subjects.Where(s => s.SubjectID == item2).Select(s => s).FirstOrDefault()
                            });
                            //break;
                        }
                    }
                }
                r.SaveChanges();
                 
                t.Complete();
                return Json("S");

            }
            catch (Exception)
            {
                t.Dispose();
                return Json("Unable To Assign Batches And Subjects To The TeacherID: " + getTeacherRec.TeacherID);
            }
                }
                 
                     
        }


        //Check Teacher ID
        public JsonResult TeacherIDVal(string teachID)
        {
            var getRecTeacher = r.Teachers.Where(s => s.TeacherID == teachID).Select(s => s).FirstOrDefault();
            if (getRecTeacher!=null)
            {
                return Json(getRecTeacher.TeacherID);
            }
            return Json(null);
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