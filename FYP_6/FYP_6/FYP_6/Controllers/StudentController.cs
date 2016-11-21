using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYP_6.Models;
using FYP_6.Models.Models_Logic;
using PagedList;
using PagedList.Mvc;
using FYP_6.SessionExpireChecker;
using FYP_6.Models.ViewModels;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpireStudent]
    public class StudentController : Controller
    {
        RCIS2Entities1 r = RCIS2Entities1.getinstance();

        //Previous Code
        //public ActionResult Students_Subject_Details(string id) //Subject and Course Details
        //{
        //    List<string> studentsDetails = StudentModel.StudentDegreeDetail_StudentModel(id);
        //    List<string> subjectNames = StudentModel.getAllSubjectNames(id);

        //    ViewBag.degreeName = studentsDetails.Last();
        //    studentsDetails.RemoveAt(2);
        //    ViewBag.subjects = subjectNames;
        //    ViewBag.student_Details = studentsDetails;
        //    return View();
        //}

        #region Student Personal
        // GET: Student
        //[OutputCache(Duration = 60)]
        public ActionResult Index()
        {
            string id = Session["rollno"].ToString();

            Student_Profile l = r.Registerations
                .Where(s => s.Rollno == id).Select(s => s.Student_Profile).FirstOrDefault();
            Registeration reg = r.Registerations
                .Where(s => s.Rollno == id && s.Status.Value == 1).Select(s => s).FirstOrDefault();

            IEnumerable<Assign_Subject> assignSubjList = r.Assign_Subject
                .Where(s => s.Rollno == id && s.Registeration.Status.Value == 1).Select(s => s).ToList();
            ViewModelStudentProReg vmProReg = new ViewModelStudentProReg
            {
                stdProfile = l,
                stdRegisteration = reg
                ,
                AssignedSubjects = assignSubjList
            };

            return View(vmProReg);
        } //Profile
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldpass, string newpass)
        {
            string id = Session["rollno"].ToString();
            string newpass1 = HttpUtility.HtmlEncode(newpass);
            string oldpass1 = HttpUtility.HtmlEncode(oldpass);

            string getPasswordChangeResult = StudentModel.ChangePassword_StudentModelFunction(oldpass1, newpass1, id);

            ViewBag.Message = getPasswordChangeResult;
            return View();
        }
        #endregion

        #region Student Results
        [HttpGet]
        public ActionResult SHowMarks( int? page)    //Subject Marks
        {
            string id = Session["rollno"].ToString();
            
                List<int?> y = StudentModel.getStudentYears(id);
                List<Student_Marks> listOfMarks = StudentModel.showMarks_StudentModelFunction(id);

                ViewBag.Year = y;
                ViewBag.Message = null;
                return View(listOfMarks.ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SHowMarks")]
        public ActionResult SHowMarks(string Month, string Part, int? page, int selectedYear) //Search Subject Marks By Month
        {
            string id = Session["rollno"].ToString();
            
                List<int?> y = StudentModel.getStudentYears(id);
                List<Student_Marks> listOfresults = new List<Student_Marks>();
                listOfresults = StudentModel.showMarks_StudentModelFunction(Month, Part, id, selectedYear);

                if (listOfresults.Count != 0)
                {
                    ViewBag.Monthname = Month;
                    ViewBag.PartName = Part;
                    ViewBag.SelectedYear = selectedYear;
                    ViewBag.Year = y;
                    ViewBag.Message = null;
                    return View("SHowMarks", listOfresults.ToPagedList(page ?? 1, 5));
                }
                else
                {
                    ViewBag.Monthname = Month;
                    ViewBag.PartName = Part;
                    ViewBag.SelectedYear = selectedYear;
                    ViewBag.Year = y;
                    ViewBag.Message = "No Record Found";
                    return View();
                }
        }
        [HttpGet]
        public ActionResult SHowAttendance(int? page)
        {
                string id = Session["rollno"].ToString();
                List<int?> y = StudentModel.getStudentYears(id);
                List<Students_Attendance> listOfAtt = StudentModel.showAttendance_StudentModelFunction(id);
                ViewBag.Year = y;
                ViewBag.Message = null;
                return View(listOfAtt.ToPagedList(page ?? 1, 10));
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SHowAttendance(string Month2, string Part, int? page, int selectedYear)//Search Subject Attendance By Month
        {
                string id = Session["rollno"].ToString();
            
                List<Students_Attendance> listOfresults = new List<Students_Attendance>();
                listOfresults = StudentModel.showAttendance_StudentModelFunction(Month2, Part, id, selectedYear);
                List<int?> y = StudentModel.getStudentYears(id);
                if (listOfresults.Count != 0)
                {
                    ViewBag.Monthname = Month2;
                    ViewBag.PartName = Part;
                    ViewBag.Year = y;
                    ViewBag.Message = null;
                    ViewBag.SelectedYear = selectedYear;
                    return View("SHowAttendance", listOfresults.ToPagedList(page ?? 1, 10));
                }
                else
                {
                    ViewBag.Monthname = Month2;
                    ViewBag.PartName = Part;
                    ViewBag.SelectedYear = selectedYear;
                    ViewBag.Year = y;
                    ViewBag.Message = "No Record Found";
                    return View();
                }
        }
        #endregion

        #region Student Fee
        public ActionResult SHowFee(int? page)    //Student Fee
        {
            string id = Session["rollno"].ToString();
            
                List<Fee> FeeList = new List<Fee>();
                List<int?> y = StudentModel.getStudentYears(id);
                ViewBag.Year = y;
                //ViewBag.SelectedYear = selectedYear;
                FeeList = StudentModel.ShowFeeRecords_StudentModelFunction(id, 1, 1);
                return View(FeeList.ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SHowFee( int? page, int selectedYear)//Search Subject Attendance By Month
        {
                string id = Session["rollno"].ToString();
            
                List<Fee> FeeList = new List<Fee>();
                FeeList = StudentModel.ShowFeeRecords_StudentModelFunction(id, 2, selectedYear);

                List<int?> y = StudentModel.getStudentYears(id);

                if (FeeList.Count != 0)
                {
                    ViewBag.Year = y;
                    ViewBag.SelectedYear = selectedYear;
                    return View("SHowFee", FeeList.ToPagedList(page ?? 1, 10));
                }
                else
                {
                    ViewBag.Year = y;
                    ViewBag.SelectedYear = selectedYear;
                    ViewBag.Message = "No Record Found";
                    return View();
                }
        }

        #endregion
    }
}