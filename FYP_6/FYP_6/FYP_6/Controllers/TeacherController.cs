using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FYP_6.Models.Models_Logic;
using FYP_6.Models;
using FYP_6.Models.ViewModels;
using FYP_6.SessionExpireChecker;
using System.Text.RegularExpressions;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpireTeacher]
    public class TeacherController : Controller
    {
        #region Teacher Personal
        RCIS2Entities1 r = RCIS2Entities1.getinstance();
        // GET: Teacher
        //[OutputCache(Duration = 60)]
        public ActionResult Index()//Teacher Profile Shown Here
        {
            string id = Session["ID"].ToString();
            Teacher l = r.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
            return View(l);
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
            string id = Session["ID"].ToString();
            string newpass1 = HttpUtility.HtmlEncode(newpass);
            string oldpass1 = HttpUtility.HtmlEncode(oldpass);

            ViewBag.Message = TeacherModel.ChangePassword_TeacherModelFunction(oldpass1, newpass1, id);
            return View();
        }
        //string oldpass,string newpass,string id
        #endregion

        #region Marks Management
        [HttpGet]
        public ActionResult ManageMarks(int? page)
        {
            string t_id = Session["ID"].ToString();
            IEnumerable<Student_Marks> EndResultListOfMarks = TeacherModel.getResultRecords(t_id);
            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);

            if (EndResultListOfMarks != null)
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s.);
                //ViewBag.ListofSections = r.Sections.Select(s => s);
                return View("ManageMarks", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 20));
            }
            else
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                //ViewBag.ListofSections = r.Sections.Select(s => s);
                ViewBag.Message = "No Records Founds";
                return View("ManageMarks", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 20));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageMarks(string month, string batch, string section, string degree,
            int? page, string search, string year)
        {
            string t_id = Session["ID"].ToString();
            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var sectionsInDegree = r.Sections.Select(s => s);
            if (degree == null || degree == "Please select"
                || section == null || section == "Please select" ||
                batch == null || batch == "Please select")
            {

                ViewBag.Message = "Plz Select All The Fields";
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
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

                IEnumerable<Student_Marks> EndResultListOfMarks = TeacherModel.showResults_TeacherModelFunction(month, batch, secID, degID, search, t_id, year);

                if (EndResultListOfMarks.Count() > 0)
                {
                    ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                        + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                    ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                    return View("ManageMarks", EndResultListOfMarks.ToPagedList(page ?? 1, 20));
                }
                else
                {
                    ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                        + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                    ViewBag.Message = "No Records Found!";
                    ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                    return View("ManageMarks", null);
                }
            }
        }

        public ActionResult EditMarks(string id)
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
        [HttpGet]
        [ActionName("DetailMarks")]
        public ActionResult DetailMarks(string id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMarks(Student_Marks resultRecord, string rollno, string SubjectID, string Month)
        {
            //int roll = int.Parse(rollno);
            Guid subjid = Guid.Parse(SubjectID);
            var getResultRecord = r.Student_Marks.Where(s => s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjid && s.Month == Month && s.Assign_Subject.Rollno == rollno).Select(s => s).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (resultRecord.Obtained_Marks > resultRecord.Total_Marks)
                {
                    ViewBag.Message = "Please Make Sure that Obtained_Marks are always less than Or equal to Total_Marks";
                    return View(getResultRecord);
                }
                else
                {
                    getResultRecord.Total_Marks = resultRecord.Total_Marks;
                    getResultRecord.Obtained_Marks = resultRecord.Obtained_Marks;
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
        [HttpGet]
        public ActionResult AddAllMarks()
        {
            string t_id = Session["ID"].ToString();
            List<Subject> tsubj = TeacherModel.getRelatedSubjects(t_id);
            List<Batch> batchesInTeacher = TeacherModel.getRelatedStudentBatches(t_id);
            ViewBag.RelatedSubjects = tsubj;
            ViewBag.Batches = batchesInTeacher;

            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAllMarks(string searchButton, string batch,
            string part, IEnumerable<ViewModelMarks> VM1, string subjectID,
            string totalMarks, Nullable<DateTime> date)
        {
            string t_id = Session["ID"].ToString();
            List<Subject> tsubj = TeacherModel.getRelatedSubjects(t_id);
            List<Batch> batchesInTeacher = TeacherModel.getRelatedStudentBatches(t_id);

            ViewBag.Batches = batchesInTeacher;
            ViewBag.RelatedSubjects = tsubj;

            if (searchButton != null)
            {
                if ((part != null && part != "")
                    && (batch != null && batch != "Please select")
                    && (subjectID != null && subjectID != "Please select"))
                {
                    Guid subj = Guid.Parse(subjectID);
                    TempData["Batch"] = batch;
                    TempData["Part"] = part;
                    TempData["SubjectID"] = subj;

                    TempData["TeacherUploadMarks"] = "Batch: " + batch +
                        ", Degree Program: " + r.Batches.Where(s => s.BatchName == batch)
                        .Select(s => s.Degree_Program.Degree_ProgramName).FirstOrDefault()
                        + ", Section: " + r.Batches.Where(s => s.BatchName == batch)
                        .Select(s => s.Section.SectionName).FirstOrDefault() + ", Part: " + part +
                        ", Subject: " +
                        r.Subjects.Where(s => s.SubjectID == subj)
                        .Select(s => s.SubjectName).FirstOrDefault();

                    IEnumerable<ViewModelMarks> VM2 = TeacherModel.getListofStudentsAccordingToBatch(batch, part, subjectID);

                    if (VM2 == null)
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
                    Guid subj = Guid.Parse(TempData["SubjectID"].ToString());
                    int partID = 0;
                    int total = 0;

                    if (int.TryParse(TempData["Part"].ToString(), out partID)
                        && int.TryParse(totalMarks, out total))
                    {
                        string result = TeacherModel.AddAllMarksRecord(TempData["Batch"].ToString(), date, partID, subj, VM1, total);

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

        #endregion

        #region Student Attendance Management
        [HttpGet]
        public ActionResult ManageAttendance(int? page)
        {
            string t_id = Session["ID"].ToString();
            IEnumerable<Students_Attendance> EndResultListOfMarks = TeacherModel.getResultRecordsAttendance(t_id);
            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);


            if (EndResultListOfMarks != null)
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                return View("ManageAttendance", EndResultListOfMarks.Take(50).ToPagedList(page ?? 1, 20));
            }
            else
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                ViewBag.Message = "No Records Founds";
                return View("ManageAttendance", null);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageAttendance(string month, string batch, string section, string degree, int? page, string search
            , string year)
        {
            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var listOfSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
            string t_id = Session["ID"].ToString();
            if (degree == null || degree == "Please select" ||
                section == null || section == "Please select"
                || batch == null || batch == "Please select")
            {
                ViewBag.Message = "Plz Select All the Fields!";
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                return View("ManageAttendance", null);
            }

            else
            {
                Guid secID = Guid.Parse(section);
                Guid degID = Guid.Parse(degree);

                IEnumerable<Students_Attendance> EndResultListOfMarks = TeacherModel.showResultsAttendance_TeacherModelFunction(month, batch, secID, degID, search, t_id, year);
                string sec = r.Teachers_Batches.Where(s => s.Batch.SectionID == secID).Select(s => s.Batch.Section.SectionName).FirstOrDefault();
                string DegreeP = r.Teachers_Batches.Where(s => s.Batch.DegreeProgram_ID == degID).Select(s => s.Batch.Degree_Program.Degree_ProgramName).FirstOrDefault();

                if (EndResultListOfMarks.Count() > 0)
                {

                    ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                        + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                    ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                    return View("ManageAttendance", EndResultListOfMarks.ToPagedList(page ?? 1, 20));
                }
                else
                {

                    ViewBag.CompleteMessage = "Batch: " + batch + " ,Degree: " + DegreeP
                        + " ,Section: " + sec + " ,Month: " + month + " ,Year: " + year;
                    ViewBag.Message = "No Records Found!";
                    ViewBag.ListofDegreePrograms = ListofDegreePrograms;

                    return View("ManageAttendance", null);
                }
            }
        }


        public ActionResult EditAttendance(string id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAttendance(Students_Attendance resultRecord, string rollno, string SubjectID, string Month, string resid2)
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

                    getResultRecord.Total_lectures = resultRecord.Total_lectures;
                    getResultRecord.Attended_Lectures = resultRecord.Attended_Lectures;
                    getResultRecord.Attendance_Percentage = resultRecord.Attendance_Percentage;
                    UpdateModel(getResultRecord, new string[] { "Total_lectures", "Attended_Lectures", "Attendance_Percentage" });
                    r.SaveChanges();
                    ViewBag.Message = "Successfully Updated Record";
                    return View(getRecordOfSpecificResult);
                }
            }
            else
            {
                ViewBag.Message = "Unable to Update Record";
                return View(getRecordOfSpecificResult);
            }
        }

        [HttpGet]

        public ActionResult DetailAttendance(string id)
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

        public ActionResult AddAllAttendance()
        {
            string t_id = Session["ID"].ToString();
            List<Subject> tsubj = TeacherModel.getRelatedSubjects(t_id);
            List<Batch> batchesInTeacher = TeacherModel.getRelatedStudentBatches(t_id);
            ViewBag.RelatedSubjects = tsubj;
            ViewBag.Batches = batchesInTeacher;

            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAllAttendance(string searchButton, string batch,
            string part, IEnumerable<ViewModelAttendance> VM1, string subjectID,
            string totalMarks, Nullable<DateTime> date)
        {
            string t_id = Session["ID"].ToString();
            List<Subject> tsubj = TeacherModel.getRelatedSubjects(t_id);
            List<Batch> batchesInTeacher = TeacherModel.getRelatedStudentBatches(t_id);
            ViewBag.Batches = batchesInTeacher;
            ViewBag.RelatedSubjects = tsubj;

            if (searchButton != null)
            {
                if ((part != null && part != "")
                    && (batch != null && batch != "Please select")
                    && (subjectID != null && subjectID != "Please select"))
                {
                    Guid subj = Guid.Parse(subjectID);
                    TempData["Batch"] = batch;
                    TempData["Part"] = part;
                    TempData["SubjectID"] = subj;

                    TempData["TeacherUploadAttendance"] = "Batch: " + batch +
                        ", Degree Program: " + r.Batches.Where(s => s.BatchName == batch)
                        .Select(s => s.Degree_Program.Degree_ProgramName).FirstOrDefault()
                        + ", Section: " + r.Batches.Where(s => s.BatchName == batch)
                        .Select(s => s.Section.SectionName).FirstOrDefault() + ", Part: " + part +
                        ", Subject: " +
                        r.Subjects.Where(s => s.SubjectID == subj)
                        .Select(s => s.SubjectName).FirstOrDefault();

                    IEnumerable<ViewModelAttendance> VM2 = TeacherModel.getListofStudentsAttendanceAccordingToBatch(batch, part, subjectID);
                    if (VM2 == null)
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
                    if (Guid.TryParse(TempData["SubjectID"].ToString(), out subj)
                        && int.TryParse(TempData["Part"].ToString(), out partID)
                        && int.TryParse(totalMarks, out total))
                    {
                        string result = TeacherModel.AddAllAttendanceRecord(TempData["Batch"].ToString(), date, partID, subj, VM1, total);

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

        #endregion

        #region Teacher View its Attendance
        [HttpGet]
        public ActionResult ViewAttendance(int? page)
        {
            string t_id = Session["ID"].ToString();
            IEnumerable<Teacher_Attendance> EndResultListOfMarks = TeacherModel.getResultRecordsForTeacherAttendance(t_id);

            if (EndResultListOfMarks.Count() != 0)
            {
                return View("ViewAttendance", EndResultListOfMarks.ToPagedList(page ?? 1, 7));
            }
            else
            {
                ViewBag.Message = "No Records Founds";
                return View("ViewAttendance", EndResultListOfMarks.ToPagedList(page ?? 1, 7));
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ViewAttendance")]
        public ActionResult ViewAttendance2(int? page, string Month, string year)
        {
            string t_id = Session["ID"].ToString();

            IEnumerable<Teacher_Attendance> EndResultListOfMarks = TeacherModel.showResultsTeacherAttendance_TeacherModelFunction(t_id, Month, year);

            if (EndResultListOfMarks.Count() > 0)
            {
                ViewBag.Message = "Month: " + Month + " Year:" + year + " Total Lectures: " + EndResultListOfMarks.Count() + " Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count();

                return View("ViewAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).ToPagedList(page ?? 1, 7));
            }
            else
            {
                ViewBag.Message = "No Records Founds";

                return View("ViewAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).ToPagedList(page ?? 1, 7));
            }
        }
        #endregion

        #region View Students of Teacher
        public ActionResult ViewStudents(int? page)
        {
            string t_id = Session["ID"].ToString();
            //List<string[]> DataBasedOnRollnos = TeacherModel.GetDataBasedOnRollnosForViewing(t_id);
            List<Registeration> DataBasedOnRollnos = TeacherModel.ViewStudents(t_id);
            //ViewBag.Data = DataBasedOnRollnos;
            return View("ViewStudents", DataBasedOnRollnos.Take(100).ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewStudents(string search,int? page)
        {

            string t_id = Session["ID"].ToString();
            bool checker = false;
            List<Registeration> StudentRollnos = TeacherModel.ViewStudents_Searched(t_id,search);

            //foreach (var item in StudentRollnos)
            //{
            //    if (search.StartsWith(item))
            //    {
            //        checker = true;
            //        break;
            //    }
            //}
            //if (checker)
            //{
            //    ViewBag.SearchQuery = "True";
            //    List<string> DataBasedOnRollnos = TeacherModel.getSpecificSearchRecord(search);
            //    ViewBag.Data = DataBasedOnRollnos;
            //    return View();
            //}
            //else
            //{
            //    ViewBag.SearchQuery = "Roll no Doesnot Exists";
            //    return View();
            //}
            return View("ViewStudents", StudentRollnos.ToPagedList(page ?? 1, 10));

        }
        #endregion

        //ValidateObtMarksGetPercentage
        public JsonResult GetBatches(string batch)
        {
            var getBatch = r.Batches.Where(s => s.BatchName == batch &&s.Status==1).Select(s => s).FirstOrDefault();

            List<string> l = new List<string>();
            l.Add(getBatch.Degree_Program.Degree_ProgramName);
            l.Add(getBatch.Section.SectionName);
            l.Add(getBatch.Registerations.Select(s => s.Part.ToString()).FirstOrDefault());
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
            Match m = Regex.Match(obtMarks, @"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$");
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
                    return Json(percentage);
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
                    return Json(percentage);
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
    }
}