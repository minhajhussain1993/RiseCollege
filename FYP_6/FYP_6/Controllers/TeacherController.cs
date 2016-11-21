using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FYP_6.Models.Models_Logic;
using FYP_6.Models.Report_Models;
using FYP_6.Models;
using FYP_6.Models.ViewModels;
using FYP_6.SessionExpireChecker;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpireTeacher]
    public class TeacherController : Controller
    {
        #region Teacher Personal
        RCIS3Entities r = RCIS3Entities.getinstance();
        TeacherModel teacherModel = new TeacherModel();
        // GET: Teacher
        //[OutputCache(Duration = 60)]
        public ActionResult Index()//Teacher Profile Shown Here
        {
            try
            {
                string id = Session["ID"].ToString();
                Teacher l = r.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
                return View(l);
            }
            catch (Exception)
            {
                return View(new Teacher());
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
                string id = Session["ID"].ToString();
                string newpass1 = HttpUtility.HtmlEncode(newpass);
                string oldpass1 = HttpUtility.HtmlEncode(oldpass);

                ViewBag.Message = teacherModel.ChangePassword_TeacherModelFunction(oldpass1, newpass1, id);
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "Unable to Change Password!";
                return View("ChangePassword");
            }
             
        }
        //string oldpass,string newpass,string id
        #endregion

        #region Marks Management
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult ManageMarks(string month, string batch, string section, string degree,
            int? page, string search, string year, string searchbtnformarks
            , string generatepdf, string subjectMarksID,string part)
        {
            try
            {
                string t_id = Session["ID"].ToString();
                var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);

                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                ViewBag.RelatedSubjects = tsubj;

                if (generatepdf != null)
                {

                    if (teacherModel.CheckerIfAllFieldsAreSelectedOrNot(batch, section, degree, subjectMarksID,month,year))
                    {
                        Guid secID = Guid.Parse(section);
                        Guid degID = Guid.Parse(degree);

                        List<MarksBYTeacherReportClass> stdMarks = teacherModel.GetStudentMarksListForReport(month, batch
                        , secID, degID,t_id,  search, year, subjectMarksID,part);

                        if (stdMarks != null)
                        {
                            if (stdMarks.Count > 0)
                            {
                                try
                                {
                                    ReportDocument rd = new ReportDocument();
                                    rd.Load(Server.MapPath("~/Reports/MarksRPT.rpt"));

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
                                    return View("ManageMarks", null);
                                }

                            }
                            else
                            {
                                ViewBag.Message = "No Record Found For PDF Generation";
                                return View("ManageMarks", null);
                            }

                        }
                        else
                        {
                            ViewBag.Message = "No Record Found For PDF Generation";
                            return View("ManageMarks", null);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Month, Year, Batch, Degree, Section and Subject Fields are Required for Report Generation!";
                        return View("ManageMarks", null);
                    }

                }
                else if (searchbtnformarks != null || Request.QueryString["search"] != null || Request.QueryString["Month"] != null
                   || Request.QueryString["batch"] != null || Request.QueryString["section"] != null || Request.QueryString["year"] != null
                   || Request.QueryString["degree"] != null
                    || Request.QueryString["subjectMarksID"] != null)
                {

                    //var sectionsInDegree = r.Sections.Select(s => s);
                    if (degree == null || degree == "Please select"
                        || section == null || section == "Please select" ||
                        batch == null || batch == "Please select")
                    {

                        ViewBag.Message = "Batch, Degree and Section needs to be Selected!";

                        //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                        //ViewBag.ListofSections = sectionsInDegree;
                        return View("ManageMarks", null);
                    }

                    else
                    {
                        Guid secID = Guid.Parse(section);
                        Guid degID = Guid.Parse(degree);

                        string sec = r.Teachers_Batches.Where(s => s.Batch.SectionID == secID).Select(s => s.Batch.Section.SectionName).FirstOrDefault();
                        string DegreeP = r.Teachers_Batches.Where(s => s.Batch.DegreeProgram_ID == degID).Select(s => s.Batch.Degree_Program.Degree_ProgramName).FirstOrDefault();

                        IEnumerable<Student_Marks> EndResultListOfMarks = teacherModel.showResults_TeacherModelFunction(month, batch, secID, degID, search, t_id, year,subjectMarksID,part);

                        if (EndResultListOfMarks != null)
                        {
                            if (EndResultListOfMarks.Count() > 0)
                            {
                                //ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                                //    + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                                //ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                                return View("ManageMarks", EndResultListOfMarks.ToPagedList(page ?? 1, 20));
                            }
                            else
                            {
                                //ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                                //    + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                                ViewBag.Message = "No Records Found!";
                              ///  ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                                return View("ManageMarks", null);
                            }

                        }
                        else
                        {
                            //ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                            //    + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                            ViewBag.Message = "No Records Found!";
                            //ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                            return View("ManageMarks", null);
                        }
                    }
                }

                else
                {
                    IEnumerable<Student_Marks> EndResultListOfMarks = teacherModel.getResultRecords(t_id);



                    if (EndResultListOfMarks != null)
                    {
                        //ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                        //ViewBag.ListofPartsInDegree = r.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s.);
                        //ViewBag.ListofSections = r.Sections.Select(s => s);
                        return View("ManageMarks", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 20));
                    }
                    else
                    {
                        //ViewBag.ListofDegreePrograms = ListofDegreePrograms;;
                        //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                        //ViewBag.ListofSections = r.Sections.Select(s => s);
                        ViewBag.Message = "No Records Founds";
                        return View("ManageMarks", null);
                    }
                }
            
            }
            catch (Exception)
            {
                string t_id = Session["ID"].ToString();
                var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);

                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                ViewBag.RelatedSubjects = tsubj;
                ViewBag.Message = "Unable to load Records Plz try again";
                return View("ManageMarks", null);
            }
             
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ManageMarks(string month, string batch, string section, string degree,
        //    int? page, string search, string year)
        //{
        //    string t_id = Session["ID"].ToString();
        //    var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    //var sectionsInDegree = r.Sections.Select(s => s);
        //    if (degree == null || degree == "Please select"
        //        || section == null || section == "Please select" ||
        //        batch == null || batch == "Please select")
        //    {

        //        ViewBag.Message = "Plz Select All The Fields";
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;
        //        //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
        //        //ViewBag.ListofSections = sectionsInDegree;
        //        return View("ManageMarks", null);
        //    }

        //    else
        //    {
        //        Guid secID = Guid.Parse(section);
        //        Guid degID = Guid.Parse(degree);

        //        string sec = r.Teachers_Batches.Where(s => s.Batch.SectionID == secID).Select(s => s.Batch.Section.SectionName).FirstOrDefault();
        //        string DegreeP = r.Teachers_Batches.Where(s => s.Batch.DegreeProgram_ID == degID).Select(s => s.Batch.Degree_Program.Degree_ProgramName).FirstOrDefault();

        //        IEnumerable<Student_Marks> EndResultListOfMarks = teacherModel.showResults_TeacherModelFunction(month, batch, secID, degID, search, t_id, year);

        //        if (EndResultListOfMarks.Count() > 0)
        //        {
        //            ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
        //                + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;

        //            return View("ManageMarks", EndResultListOfMarks.ToPagedList(page ?? 1, 20));
        //        }
        //        else
        //        {
        //            ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
        //                + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
        //            ViewBag.Message = "No Records Found!";
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;

        //            return View("ManageMarks", null);
        //        }
        //    }
        //}

        public ActionResult EditMarks(string id)
        {
            try
            {
                Guid resID;
                if (Guid.TryParse(id, out resID))
                {
                    var getRecordOfSpecificResult = r.Student_Marks.Where(s => s.ResultID == resID).Select(s => s).FirstOrDefault();
                    return View(getRecordOfSpecificResult);
                }
                else
                {
                    return RedirectToAction("ManageMarks");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageMarks");
            }
             
        }
        [HttpGet]
        [ActionName("DetailMarks")]
        public ActionResult DetailMarks(string id)
        {
            try
            {
                Guid resID;
                if (Guid.TryParse(id, out resID))
                {
                    var getRecordOfSpecificResult = r.Student_Marks.Where(s => s.ResultID == resID).Select(s => s).FirstOrDefault();
                    return View(getRecordOfSpecificResult);
                }
                else
                {
                    return RedirectToAction("ManageMarks");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageMarks");
            }
             
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMarks(Student_Marks resultRecord, string rollno, string SubjectID, string Month)
        {
            //int roll = int.Parse(rollno);
            try
            {
                Guid subjid = Guid.Parse(SubjectID);
                var getResultRecord = r.Student_Marks.Where(s
                    => s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjid &&
                    s.Month == Month && s.Assign_Subject.Rollno == rollno).Select(s => s).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    if (resultRecord.Obtained_Marks > resultRecord.Total_Marks)
                    {
                        ViewBag.Message = "Please Make Sure that Obtained_Marks are always less than Or equal to Total_Marks";
                        return View(getResultRecord);
                    }
                    else
                    {
                        if (resultRecord.Total_Marks != getResultRecord.Total_Marks)
                        {
                            ViewBag.Message = "Plz Update Only Obtained Marks!";
                            return View(getResultRecord);
                        }
                        getResultRecord.Total_Marks = resultRecord.Total_Marks;
                        getResultRecord.Obtained_Marks = Math.Round( resultRecord.Obtained_Marks??0,2);
                        getResultRecord.Marks_Percentage = resultRecord.Marks_Percentage;
                        UpdateModel(getResultRecord, new string[] { "Total_Marks", "Obtained_Marks", "Marks_Percentage" });
                        r.SaveChanges();
                        ViewBag.Message = "Successfully Updated Record";
                        return View(getResultRecord);
                    }
                }
                else
                {
                    ViewBag.Message = "Unable to Update Record";
                    return View(getResultRecord);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageMarks");
            } 
        }

        //ShowOptionsAttendance
         
        [HttpGet]
         
        public ActionResult ShowOptionsMarks()
        {
            return View();
        }
         
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult AddMarksIndi()
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                ViewBag.RelatedSubjects = tsubj;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ManageMarks");
            }
             
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddMarksIndi(Student_Marks stdMarks,DateTime? date1234,
            string rollnohereIndi, string partInIndiMarks, string subjectMarksIDIndi)
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                ViewBag.RelatedSubjects = tsubj;

                 
                string result = teacherModel.AddIndiMarksRecord(stdMarks, date1234, rollnohereIndi, partInIndiMarks, subjectMarksIDIndi,t_id);
                if (result=="OK")
                {
                    ViewBag.Message = "Successfully Marks Uploaded";    

                }
                else
                {
                    ViewBag.Message = result;  
                }
                return View();
            }
            catch (Exception)
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                ViewBag.RelatedSubjects = tsubj;
                ViewBag.Message = "Unable To Upload Marks! Plz Try Again!";
                return View();
            }
             
        }

        [HttpGet]
        public ActionResult AddAllMarks()
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                List<Batch> batchesInTeacher = teacherModel.getRelatedStudentBatches(t_id);
                ViewBag.RelatedSubjects = tsubj;
                ViewBag.Batches = batchesInTeacher;

                ViewBag.Message = "";
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ManageMarks");
            }
             
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAllMarks(string searchButton, string batch,
            string part, IEnumerable<ViewModelMarks> VM1, string subjectID,
            string totalMarks, Nullable<DateTime> date)
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                List<Batch> batchesInTeacher = teacherModel.getRelatedStudentBatches(t_id);

                ViewBag.Batches = batchesInTeacher;
                ViewBag.RelatedSubjects = tsubj;

                if (searchButton != null)
                {
                    if ((part != null && part != "")
                        && (batch != null && batch != "Please select")
                        && (subjectID != null && subjectID != "Please select"))
                    {
                        Guid subj = Guid.Parse(subjectID);
                        TempData["Batch1"] = batch;
                        TempData["Part1"] = part;
                        TempData["SubjectID1"] = subj;
                        Session["Part1"] = part;
                        Session["SubjectID1"] = subj;

                        TempData["TeacherUploadMarks"] = "Batch: " + batch +
                            ", Degree Program: " + r.Batches.Where(s => s.BatchName == batch)
                            .Select(s => s.Degree_Program.Degree_ProgramName).FirstOrDefault()
                            + ", Section: " + r.Batches.Where(s => s.BatchName == batch)
                            .Select(s => s.Section.SectionName).FirstOrDefault() + ", Part: " + part +
                            ", Subject: " +
                            r.Subjects.Where(s => s.SubjectID == subj)
                            .Select(s => s.SubjectName).FirstOrDefault();

                        string result = teacherModel.checkerIfIamTeachingthisSubjectTothisBatch(batch, subjectID, t_id);

                        if (result != "OK")
                        {
                            ViewBag.Message = result;
                            return View();
                        }

                        IEnumerable<ViewModelMarks> VM2 = teacherModel.getListofStudentsAccordingToBatch(batch, part, subjectID);

                        if (VM2 == null)
                        {
                            ViewBag.Message = "No Records Found";
                            return View(VM2);
                        }
                        else if (VM2.Count() == 0)
                        {
                            ViewBag.Message = "No Records Found";
                            return View(VM2);
                        }
                        else
                        {
                            ViewBag.RelatedSubjects = tsubj;
                            ViewBag.Message = null;
                            return View(VM2);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Plz Select All the Fields";
                        return View();
                    }
                }
                else
                {
                    if (VM1 != null)
                    {
                        Guid subj = new Guid();
                        int partID = 0;
                        int total = 0;

                        if (Guid.TryParse(Session["SubjectID1"].ToString(), out subj) &&
                            int.TryParse(Session["Part1"].ToString(), out partID)
                            && int.TryParse(totalMarks, out total))
                        {
                            string result = teacherModel.AddAllMarksRecord(TempData["Batch1"].ToString(), date, partID, subj, VM1, total);

                            if (result == "OK")
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
                        else
                        {
                            ViewBag.Message = "Unable To Upload Marks!";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "No Student Was found To Upload Marks!";
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Unable To Upload Marks! Plz Try Again!";
                return View();
            }
             
        }

        #endregion

        #region Student Attendance Management

        [HttpGet]
        public ActionResult ShowOptionsAttendance()
        {
            return View();
        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult AddAttIndi()
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                ViewBag.RelatedSubjects = tsubj;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ManageAttendance");
            }

        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttIndi(Students_Attendance stdAtt, DateTime? date12345,
            string rollnohereIndi2, string partInIndiMarks2, string subjectMarksIDIndi2)
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                ViewBag.RelatedSubjects = tsubj;


                string result = teacherModel.AddIndiAttRecord(stdAtt, date12345, rollnohereIndi2, partInIndiMarks2, subjectMarksIDIndi2, t_id);
                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Attendance Uploaded";

                }
                else
                {
                    ViewBag.Message = result;
                }
                return View();
            }
            catch (Exception)
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                ViewBag.RelatedSubjects = tsubj;
                ViewBag.Message = "Unable To Upload Attendance! Plz Try Again!";
                return View();
            }

        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult ManageAttendance(string month, string batch, string section, string degree, int? page, string search
            , string year, string searchbuttonforAtt, string generatepdf, string subjectMarksID,string part)
        {
            try
            {
                string t_id = Session["ID"].ToString();
                var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);

                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                ViewBag.RelatedSubjects = tsubj;
                if (generatepdf != null)
                {
                     

                    if (teacherModel.CheckerIfAllFieldsAreSelectedOrNot(batch, section, degree, subjectMarksID,month,year))
                    {
                         
                        List<AttendanceBYTeacherReportClass> attendanceStudent = teacherModel.GetStudentAttendanceListForReport(month, batch
                        , section, degree,t_id
                        , search, year, subjectMarksID,part);

                        if (attendanceStudent != null)
                        {
                            if (attendanceStudent.Count > 0)
                            {
                                try
                                {
                                    ReportDocument rd = new ReportDocument();
                                    rd.Load(Server.MapPath("~/Reports/Student_Attendance_Reportrpt1.rpt"));

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
                                    return View("ManageAttendance", null);
                                }

                            }
                            else
                            {
                                ViewBag.Message = "No Record Found For PDF Generation";
                                return View("ManageAttendance", null);
                            }

                        }
                        else
                        {
                            ViewBag.Message = "No Record Found For PDF Generation";
                            return View("ManageAttendance", null);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Month, Year, Batch, Degree, Section and Subject Fields are Required for Report Generation!";
                        return View("ManageAttendance", null);
                    }

                }
                else if (searchbuttonforAtt != null || Request.QueryString["search"] != null || Request.QueryString["Month"] != null
                    || Request.QueryString["batch"] != null || Request.QueryString["section"] != null || Request.QueryString["year"] != null
                    || Request.QueryString["degree"] != null
                    || Request.QueryString["subjectMarksID"] != null)
                {
                     
                    if (degree == null || degree == "Please select" ||
                       section == null || section == "Please select"
                       || batch == null || batch == "Please select")
                    {
                        ViewBag.Message = "Batch, Degree and Section needs to be Selected!";
                      

                        return View("ManageAttendance", null);
                    }

                    else
                    {
                        Guid secID = Guid.Parse(section);
                        Guid degID = Guid.Parse(degree);

                        IEnumerable<Students_Attendance> EndResultListOfMarks = teacherModel.showResultsAttendance_TeacherModelFunction(month, batch, secID, degID, search, t_id, year, subjectMarksID,part);
                        string sec = r.Teachers_Batches.Where(s => s.Batch.SectionID == secID).Select(s => s.Batch.Section.SectionName).FirstOrDefault();
                        string DegreeP = r.Teachers_Batches.Where(s => s.Batch.DegreeProgram_ID == degID).Select(s => s.Batch.Degree_Program.Degree_ProgramName).FirstOrDefault();

                        if (EndResultListOfMarks != null)
                        {

                            //ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                            //    + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                       

                            return View("ManageAttendance", EndResultListOfMarks.ToPagedList(page ?? 1, 20));
                        }
                        else
                        {

                            //ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                            //    + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                            ViewBag.Message = "No Records Found!";
                             

                            return View("ManageAttendance", null);
                        }
                    }
                }
                else
                {
                    IEnumerable<Students_Attendance> EndResultListOfMarks = teacherModel.getResultRecordsAttendance(t_id);
                     


                    if (EndResultListOfMarks != null)
                    {
                       

                        return View("ManageAttendance", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 20));
                    }
                    else
                    {
                      
                        ViewBag.Message = "No Records Founds";
                        return View("ManageAttendance", null);
                    }
                }
            
            }
            catch (Exception)
            {
                ViewBag.Message = "Unable to Load Records! Plz Try Again!";
                return View("ManageAttendance",null);
            }
             
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ManageAttendance(string month, string batch, string section, string degree, int? page, string search
        //    , string year)
        //{
        //    var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    //var listOfSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
        //    string t_id = Session["ID"].ToString();
        //    if (degree == null || degree == "Please select" ||
        //        section == null || section == "Please select"
        //        || batch == null || batch == "Please select")
        //    {
        //        ViewBag.Message = "Plz Select All the Fields!";
        //        ViewBag.ListofDegreePrograms = ListofDegreePrograms;

        //        return View("ManageAttendance", null);
        //    }

        //    else
        //    {
        //        Guid secID = Guid.Parse(section);
        //        Guid degID = Guid.Parse(degree);

        //        IEnumerable<Students_Attendance> EndResultListOfMarks = teacherModel.showResultsAttendance_TeacherModelFunction(month, batch, secID, degID, search, t_id, year);
        //        string sec = r.Teachers_Batches.Where(s => s.Batch.SectionID == secID).Select(s => s.Batch.Section.SectionName).FirstOrDefault();
        //        string DegreeP = r.Teachers_Batches.Where(s => s.Batch.DegreeProgram_ID == degID).Select(s => s.Batch.Degree_Program.Degree_ProgramName).FirstOrDefault();

        //        if (EndResultListOfMarks.Count() > 0)
        //        {

        //            ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
        //                + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;

        //            return View("ManageAttendance", EndResultListOfMarks.ToPagedList(page ?? 1, 20));
        //        }
        //        else
        //        {

        //            ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
        //                + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
        //            ViewBag.Message = "No Records Found!";
        //            ViewBag.ListofDegreePrograms = ListofDegreePrograms;

        //            return View("ManageAttendance", null);
        //        }
        //    }
        //}


        public ActionResult EditAttendance(string id)
        {
            try
            {
                Guid attID;
                if (Guid.TryParse(id, out attID))
                {
                    var getRecordOfSpecificResult = r.Students_Attendance.Where(s => s.AttendanceID == attID).Select(s => s).FirstOrDefault();
                    return View(getRecordOfSpecificResult);
                }
                else
                {
                    return RedirectToAction("ManageAttendance");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageAttendance");
            }
             
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAttendance(Students_Attendance resultRecord, string rollno, string SubjectID, string Month, string resid2)
        {
            try
            {
                Guid resid = Guid.Parse(resid2);
                var getRecordOfSpecificResult = r.Students_Attendance.Where(s => s.AttendanceID == resid).Select(s => s).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    //int roll = int.Parse(rollno);
                    Guid subjid = Guid.Parse(SubjectID);
                    if (resultRecord.Attended_Lectures > resultRecord.Total_lectures)
                    {
                        ViewBag.Message = "Please Make Sure that Attended_Lectures are always less than Or equal to Total Lectures";
                        return View(getRecordOfSpecificResult);
                    }
                    else
                    {
                        var getResultRecord = r.Students_Attendance.Where(s => s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjid && s.Month == Month && s.Assign_Subject.Rollno == rollno).Select(s => s).FirstOrDefault();
                        try
                        {

                            string valueOfPercentageAtt = new string(resultRecord.Attendance_Percentage.Take(5).ToArray());
                            if (resultRecord.Total_lectures != getResultRecord.Total_lectures)
                            {
                                ViewBag.Message = "Plz Update Only Attended Lectures!";
                                return View(getRecordOfSpecificResult);
                            }
                            getResultRecord.Total_lectures = resultRecord.Total_lectures;
                            getResultRecord.Attended_Lectures = resultRecord.Attended_Lectures;
                            getResultRecord.Attendance_Percentage = valueOfPercentageAtt;
                            //UpdateModel(getResultRecord, new string[] { "Total_lectures", "Attended_Lectures", "Attendance_Percentage" });
                            r.SaveChanges();
                            ViewBag.Message = "Successfully Updated Record";
                            return View(getRecordOfSpecificResult);
                        }
                        catch (Exception e)
                        {
                            //JsonResult j = new JsonResult();
                            //foreach (var eve in e.EntityValidationErrors)
                            //{
                            //    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            //    //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            //    j.Data += eve.Entry.Entity.GetType().Name;
                            //    j.Data = eve.Entry.State;
                            //    foreach (var ve in eve.ValidationErrors)
                            //    {
                            //        //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            //        //    ve.PropertyName, ve.ErrorMessage);
                            //        j.Data += ve.PropertyName;
                            //        j.Data += ve.ErrorMessage;
                            //    }
                            //}
                            //var getResultRecord = r.Students_Attendance.Where(s => s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjid && s.Month == Month && s.Assign_Subject.Rollno == rollno).Select(s => s).FirstOrDefault();
                            ViewBag.Message = "Unable to Update Record";
                            return View(getRecordOfSpecificResult);
                        }

                    }
                }
                else
                {
                    ViewBag.Message = "Unable to Update Record";
                    return View(getRecordOfSpecificResult);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageAttendance");
            }
             
        }

        [HttpGet]

        public ActionResult DetailAttendance(string id)
        {
            try
            {
                Guid attID;
                if (Guid.TryParse(id, out attID))
                {
                    var getRecordOfSpecificResult = r.Students_Attendance.Where(s => s.AttendanceID == attID).Select(s => s).FirstOrDefault();
                    return View(getRecordOfSpecificResult);
                }
                else
                {
                    return RedirectToAction("ManageAttendance");
                }
            }
            catch (Exception)
            {
                 return RedirectToAction("ManageAttendance");
            }
             
        }

        public ActionResult AddAllAttendance()
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                List<Batch> batchesInTeacher = teacherModel.getRelatedStudentBatches(t_id);
                ViewBag.RelatedSubjects = tsubj;
                ViewBag.Batches = batchesInTeacher;

                ViewBag.Message = "";
                return View();
            }
            catch (Exception)
            {
                 return RedirectToAction("ManageAttendance");
            }
             
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAllAttendance(string searchButton, string batch,
            string part, IEnumerable<ViewModelAttendance> VM1, string subjectID,
            string totalMarks, Nullable<DateTime> date)
        {
            try
            {
                string t_id = Session["ID"].ToString();
                List<Subject> tsubj = teacherModel.getRelatedSubjects(t_id);
                List<Batch> batchesInTeacher = teacherModel.getRelatedStudentBatches(t_id);
                ViewBag.Batches = batchesInTeacher;
                ViewBag.RelatedSubjects = tsubj;

                if (searchButton != null)
                {
                    if ((part != null && part != "")
                        && (batch != null && batch != "Please select")
                        && (subjectID != null && subjectID != "Please select"))
                    {
                        Guid subj = Guid.Parse(subjectID);
                        TempData["Batch2"] = batch;
                        TempData["Part2"] = part;
                        TempData["SubjectID2"] = subj;
                        Session["Part2"] = part;
                        Session["SubjectID2"] = subj;

                        TempData["TeacherUploadAttendance"] = "Batch: " + batch +
                            ", Degree Program: " + r.Batches.Where(s => s.BatchName == batch)
                            .Select(s => s.Degree_Program.Degree_ProgramName).FirstOrDefault()
                            + ", Section: " + r.Batches.Where(s => s.BatchName == batch)
                            .Select(s => s.Section.SectionName).FirstOrDefault() + ", Part: " + part +
                            ", Subject: " +
                            r.Subjects.Where(s => s.SubjectID == subj)
                            .Select(s => s.SubjectName).FirstOrDefault();

                        string result= teacherModel.checkerIfIamTeachingthisSubjectTothisBatch(batch,subjectID,t_id);

                        if ( result!="OK")
                        {
                             ViewBag.Message = result;
                            return View();
                        }

                        IEnumerable<ViewModelAttendance> VM2 = teacherModel.getListofStudentsAttendanceAccordingToBatch(batch, part, subjectID);

                        if (VM2 == null)
                        {
                            ViewBag.Message = "No Records Found";
                            return View(VM2);
                        }
                        else if (VM2.Count()==0)
                        {
                            ViewBag.Message = "No Records Found";
                            return View(VM2);
                        }
                        else
                        {
                            //ViewBag.RelatedSubjects = tsubj;
                            ViewBag.Message = null;
                            return View(VM2);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Plz Select All the Fields";
                        return View();
                    }
                }
                else
                {
                    if (VM1 != null)
                    {
                        Guid subj = new Guid();
                        int partID = 0;
                        int total = 0;
                        if (Guid.TryParse(Session["SubjectID2"].ToString(), out subj)
                            && int.TryParse(Session["Part2"].ToString(), out partID)
                            && int.TryParse(totalMarks, out total))
                        {
                            string result = teacherModel.AddAllAttendanceRecord(TempData["Batch2"].ToString(), date, partID, subj, VM1, total);

                            if (result == "OK")
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
                        else
                        {
                            ViewBag.Message = "Unable To Upload Attendance!";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "No Student Was found To Upload Attendance!";
                        return View();
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("AddAllAttendance");
            }
             
        }

        #endregion

        #region Teacher View its Attendance
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult ViewAttendance(int? page, string tAttsearch,string Month,string year)
        {
            try
            {
                string t_id = Session["ID"].ToString();

                if (tAttsearch != null || Request.QueryString["Month"] != null || Request.QueryString["year"] != null)
                {
                    IEnumerable<Teacher_Attendance> EndResultListOfMarks = teacherModel.showResultsTeacherAttendance_TeacherModelFunction(t_id, Month, year);

                    if (EndResultListOfMarks != null)
                    {
                        ViewBag.Message = "Month: " + Month + " Year:" + year + " Total Lectures: " + EndResultListOfMarks.Count() + " Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count();

                        return View("ViewAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).ToPagedList(page ?? 1, 20));
                    }
                    else
                    {
                        ViewBag.Message = "No Records Founds";

                        return View("ViewAttendance", null);
                    }
                }
                else
                {
                    IEnumerable<Teacher_Attendance> EndResultListOfMarks = teacherModel.getResultRecordsForTeacherAttendance(t_id);

                    if (EndResultListOfMarks != null)
                    {
                        return View("ViewAttendance", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 20));
                    }
                    else
                    {
                        ViewBag.Message = "No Records Founds";
                        return View("ViewAttendance", null);
                    }
                }
            
            }
            catch (Exception)
            {
                return RedirectToAction("WelcomeTeacher","Home",null);     
            } 
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("ViewAttendance")]
        //public ActionResult ViewAttendance2(int? page, string Month, string year)
        //{
        //    string t_id = Session["ID"].ToString();

        //    IEnumerable<Teacher_Attendance> EndResultListOfMarks = teacherModel.showResultsTeacherAttendance_TeacherModelFunction(t_id, Month, year);

        //    if (EndResultListOfMarks.Count() > 0)
        //    {
        //        ViewBag.Message = "Month: " + Month + " Year:" + year + " Total Lectures: " + EndResultListOfMarks.Count() + " Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count();

        //        return View("ViewAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).ToPagedList(page ?? 1, 7));
        //    }
        //    else
        //    {
        //        ViewBag.Message = "No Records Founds";

        //        return View("ViewAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).ToPagedList(page ?? 1, 7));
        //    }
        //}
        #endregion

        #region View Students of Teacher
        [ValidateInput(false)]
        public ActionResult ViewStudents(int? page, string searchstdTeacherbtn,string search,string generatepdf)
        {
            try
            {
                string t_id = Session["ID"].ToString();
                if (generatepdf != null)
                {
                    List<ViewStudentsBYTeacher> vsbt = teacherModel.GetStudentsOfTeacherForReport(search, t_id);
                    if (vsbt != null)
                    {
                        if (vsbt.Count > 0)
                        {
                            ReportDocument rd = new ReportDocument();
                            rd.Load(Server.MapPath("~/Reports/StudentListForTeacher.rpt"));

                            rd.SetDataSource(vsbt.ToList());
                            Response.Buffer = false;
                            Response.ClearContent();
                            Response.ClearHeaders();
                            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            stream.Seek(0, SeekOrigin.Begin);
                            return File(stream, "application/Pdf", "StudentList.pdf");
                        }
                        else
                        {
                            ViewBag.Message = "No Record Found For Generating PDF!";
                            return View("ViewStudents", null);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "No Record Found For Generating PDF!";
                        return View("ViewStudents", null);
                    }

                }
                else if (searchstdTeacherbtn != null || Request.QueryString["search"] != null)
                {
                    List<Registeration> StudentRollnos = teacherModel.ViewStudents_Searched(t_id, search);
                    if (StudentRollnos == null)
                    {
                        return View("ViewStudents", null);
                    }
                    else
                    {
                        return View("ViewStudents", StudentRollnos.ToPagedList(page ?? 1, 20));
                    }
                }
                else
                {
                    //List<string[]> DataBasedOnRollnos = teacherModel.GetDataBasedOnRollnosForViewing(t_id);
                    List<Registeration> DataBasedOnRollnos = teacherModel.ViewStudents(t_id);
                    //ViewBag.Data = DataBasedOnRollnos;
                    if (DataBasedOnRollnos == null)
                    {
                        return View("ViewStudents", null);
                    }
                    else
                    {
                        return View("ViewStudents", DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 20));
                    }

                }
            
            }
            catch (Exception)
            {
                return View("ViewStudents",null);     
            }
             
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ViewStudents(string search,int? page)
        //{

        //    string t_id = Session["ID"].ToString();
        //    bool checker = false;
        //    List<Registeration> StudentRollnos = teacherModel.ViewStudents_Searched(t_id,search);

        //    //foreach (var item in StudentRollnos)
        //    //{
        //    //    if (search.StartsWith(item))
        //    //    {
        //    //        checker = true;
        //    //        break;
        //    //    }
        //    //}
        //    //if (checker)
        //    //{
        //    //    ViewBag.SearchQuery = "True";
        //    //    List<string> DataBasedOnRollnos = teacherModel.getSpecificSearchRecord(search);
        //    //    ViewBag.Data = DataBasedOnRollnos;
        //    //    return View();
        //    //}
        //    //else
        //    //{
        //    //    ViewBag.SearchQuery = "Roll no Doesnot Exists";
        //    //    return View();
        //    //}
        //    return View("ViewStudents", StudentRollnos.ToPagedList(page ?? 1, 10));

        //}
        #endregion

        #region View Teacher its Batches and Subjects
        //[ValidateInput(false)]
        public ActionResult ViewTeacherBatchesoFTeacher(int? page, string ifButtonPressed, string teacherID,
            string BatchesType, string degforTeacher)
        {
            try
            {
                ViewBag.DegreePrograms = r.Degree_Program.Select(s => s);
                string id = Session["ID"].ToString();
                
                if (ifButtonPressed != null || Request.QueryString["BatchesType"] != null || Request.QueryString["degforTeacher"] != null)
                {
                    if (degforTeacher!="Please select")
                    {
                        IEnumerable<Teachers_Batches> EndResultListOfMarks = teacherModel.GetAllTeacher_batchesRecordsForTeacher(id, 2, BatchesType, degforTeacher);
                        if (EndResultListOfMarks != null)
                        {
                            return View("ViewTeacherBatchesoFTeacher", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                        }
                        else
                        {
                            return View("ViewTeacherBatchesoFTeacher",null);
                        }
                    }
                    else
                    {
                        IEnumerable<Teachers_Batches> EndResultListOfMarks = teacherModel.GetAllTeacher_batchesRecordsForTeacher(id, 1, BatchesType, degforTeacher);
                        if (EndResultListOfMarks != null)
                        {
                            return View("ViewTeacherBatchesoFTeacher", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                        }
                        else
                        {
                            return View("ViewTeacherBatchesoFTeacher", null);
                        }
                    }
                }
                else
                {

                    IEnumerable<Teachers_Batches> EndResultListOfMarks = teacherModel.GetAllTeacher_batchesRecordsForTeacher(id, 3, BatchesType, degforTeacher);
                    if (EndResultListOfMarks != null)
                    {
                        return View("ViewTeacherBatchesoFTeacher", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 10));
                    }
                    else
                    {
                        return View("ViewTeacherBatchesoFTeacher", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 10));
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.DegreePrograms = r.Degree_Program.Select(s => s);
                return View("ViewTeacherBatchesoFTeacher", null);
            }
             
        }

        public ActionResult ViewSubjectsForSelectedBatchofTeacher(string ID)
        {
            try
            {
                Guid gd = new Guid();
                if (Guid.TryParse(ID, out gd))
                {
                    var getSubjectsForRelatedBatch = r.Teacher_Subject.Where(s => s.Teacher_BatchID == gd).Select(s => s);
                    ViewBag.BatchTeacher = r.Teacher_Subject.Where(s => s.Teacher_BatchID == gd).Select(s => s.Teachers_Batches.Batch).FirstOrDefault();
                    return View(getSubjectsForRelatedBatch);
                }
                else
                {
                    return RedirectToAction("ViewTeacherBatchesoFTeacher");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ViewTeacherBatchesoFTeacher");
            }
             
        }

        #endregion

        #region Json Methods

        public JsonResult GetTeacherSubjectsNamesDistinct(string id)
        {
            try
            {
                List<string> l = new List<string>();
                string TID = Session["ID"].ToString();
                var getSubjects = r.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == TID).Select(s => s.SubjectID);
                var getDistinctSubjects = r.Subjects.Where(s => getSubjects.Contains(s.SubjectID)).Distinct();
                foreach (var item in getDistinctSubjects)
                {
                    l.Add(item.SubjectName);
                }
                return Json(l);
            }
            catch(Exception)
            {
                return Json(null);
            } 
        }

        //ValidateObtMarksGetPercentage 
        public JsonResult GetBatches(string batch)
        {
            var getBatch = r.Batches.Where(s => s.BatchName == batch &&s.Status==1).Select(s => s).FirstOrDefault();

            List<string> l = new List<string>();
            l.Add(getBatch.Degree_Program.Degree_ProgramName);
            l.Add(getBatch.Section.SectionName);
            l.Add(getBatch.Registerations.Where(s =>
                s.Student_Profile.Status==1
                &&s.Status==1).Select(s => s.Part.ToString()).FirstOrDefault()??"1");
            if (l != null)
            {
                return Json(l);
            }
            else
            {
                return null;
            }
        }
        public JsonResult ValidateObtMarksGetPercentage(string obtMarks, string totalMarks)
        {
            Match m = Regex.Match(obtMarks, @"^(?:[1-9]\d*|0)?(?:\.\d+)?$");
            Match m2 = Regex.Match(totalMarks, "^[0-9]*$");
            if (m.Success && m2.Success)
            {

                int v1 = int.Parse(totalMarks);
                double v2 = double.Parse(obtMarks);
                if (v2 > v1)
                {
                    return Json(0);
                }
                else
                {
                    double percentage = (v2 * 100) / v1;
                    return Json(Math.Round(percentage, 2));
                }
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult ValidateAttendanceGetPercentage(string attLec, string totalLec)
        {
             
            Match m = Regex.Match(attLec, "^[0-9]*$");
            Match m2 = Regex.Match(totalLec, "^[0-9]*$");

            if (m.Success && m2.Success)
            {
                int v1 = int.Parse(totalLec);
                double v2 = double.Parse(attLec);

                if (v2 > v1)
                {
                    return Json(0);
                }
                else
                {
                    double percentage = (v2 * 100) / v1;
                    return Json(Math.Round(percentage,2));
                }
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult GetBatches2(string degree)
        {
            Guid programID;
            if (Guid.TryParse(degree, out programID))
            {
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

        #endregion

    }
}