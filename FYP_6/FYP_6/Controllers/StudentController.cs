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
        RCIS3Entities r = RCIS3Entities.getinstance();
        StudentModel studentModel = new StudentModel();

        #region Previous Code
        //Previous Code
        //public ActionResult Students_Subject_Details(string id) //Subject and Course Details
        //{
        //    List<string> studentsDetails = studentModel.StudentDegreeDetail_StudentModel(id);
        //    List<string> subjectNames = studentModel.getAllSubjectNames(id);

        //    ViewBag.degreeName = studentsDetails.Last();
        //    studentsDetails.RemoveAt(2);
        //    ViewBag.subjects = subjectNames;
        //    ViewBag.student_Details = studentsDetails;
        //    return View();
        //}
        #endregion

        #region Student Personal
        // GET: Student
        //[OutputCache(Duration = 60)]
        public ActionResult Index()
        {
            try
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
            }
            catch (Exception)
            {
                ViewModelStudentProReg vm = new ViewModelStudentProReg
                {
                    stdProfile = new Student_Profile(),
                    stdRegisteration = new Registeration(),
                    AssignedSubjects = new List<Assign_Subject>()
                };
                return View(vm);
            }
            
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

            string getPasswordChangeResult = studentModel.ChangePassword_StudentModelFunction(oldpass1, newpass1, id);

            ViewBag.Message = getPasswordChangeResult;
            return View();
        }
        #endregion

        #region Student Results
        [HttpGet]
        public ActionResult SHowMarks(int? page,string Month, string Part
            , string searchbtnformarksstd, int? selectedYear)    //Subject Marks
        {
            try
            {
                string id = Session["rollno"].ToString();

                if (searchbtnformarksstd != null || Request.QueryString["selectedYear"]!=null
                    || Request.QueryString["Month"]!=null|| Request.QueryString["Part"]!=null)
                {

                    List<int?> y = studentModel.getStudentYears(id);
                    List<Student_Marks> listOfresults = new List<Student_Marks>();
                    listOfresults = studentModel.showMarks_StudentModelFunction(Month, Part, id, selectedYear);

                    if (listOfresults!=null)
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
                        return View("SHowMarks", null);
                    }
                }
                else
                {
                    List<int?> y = studentModel.getStudentYears(id);
                    List<Student_Marks> listOfMarks = studentModel.showMarks_StudentModelFunction(id);

                    ViewBag.Year = y;
                    ViewBag.Message = null;
                    if (listOfMarks==null)
                    {
                        return View("SHowMarks",null);        
                    }
                    return View(listOfMarks.ToPagedList(page ?? 1, 10));
                }
                
            }
            catch (Exception)
            {
                return View("SHowMarks",null);
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SHowMarks")]
        public ActionResult SHowMarks(string Month, string Part, int? page, int selectedYear) //Search Subject Marks By Month
        {
            string id = Session["rollno"].ToString();
            
                List<int?> y = studentModel.getStudentYears(id);
                List<Student_Marks> listOfresults = new List<Student_Marks>();
                listOfresults = studentModel.showMarks_StudentModelFunction(Month, Part, id, selectedYear);

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
        public ActionResult SHowAttendance(int? page,string Month2, string Part
            , string searchbtnforattstd, int? selectedYear)
        {
            try
            {
                string id = Session["rollno"].ToString();

                if (searchbtnforattstd != null || Request.QueryString["selectedYear"] != null
                    || Request.QueryString["Month2"] != null || Request.QueryString["Part"] != null)
                {
                    
                List<Students_Attendance> listOfresults = new List<Students_Attendance>();
                listOfresults = studentModel.showAttendance_StudentModelFunction(Month2, Part, id, selectedYear);
                List<int?> y = studentModel.getStudentYears(id);

                if (listOfresults!=null)
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
                    return View("SHowAttendance", null);
                }
                }
                else
                {
                    List<int?> y = studentModel.getStudentYears(id);
                    List<Students_Attendance> listOfAtt = studentModel.showAttendance_StudentModelFunction(id);
                    ViewBag.Year = y;
                    ViewBag.Message = null;
                    if (listOfAtt==null)
                    {
                        return View("SHowAttendance", null);    
                    }
                    return View(listOfAtt.ToPagedList(page ?? 1, 10));
                }
                
            }
            catch (Exception)
            {
                return View("SHowAttendance",null);
            }
                
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SHowAttendance(string Month2, string Part, int? page, int selectedYear)//Search Subject Attendance By Month
        {
                string id = Session["rollno"].ToString();
            
                List<Students_Attendance> listOfresults = new List<Students_Attendance>();
                listOfresults = studentModel.showAttendance_StudentModelFunction(Month2, Part, id, selectedYear);
                List<int?> y = studentModel.getStudentYears(id);
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
        public ActionResult SHowFee()    //Student Fee
        {
            try
            {
                string id = Session["rollno"].ToString();
                //if (yearwiseSearchbtnfee != null || Request.QueryString["selectedYear"] != null)
                //{
                    
                //List<Fee> FeeList = new List<Fee>();
                //FeeList = studentModel.ShowFeeRecords_StudentModelFunction(id, 2, selectedYear);

                //List<int?> y = studentModel.getStudentYears(id);

                //if (FeeList.Count != 0)
                //{
                //    ViewBag.Year = y;
                //    ViewBag.SelectedYear = selectedYear;
                //    return View("SHowFee", FeeList.ToPagedList(page ?? 1, 10));
                //}
                //else
                //{
                //    ViewBag.Year = y;
                //    ViewBag.SelectedYear = selectedYear;
                //    ViewBag.Message = "No Record Found";
                //    return View("SHowFee", null);
                //}
                //}
                //else
                //{
                    //List<Fee> FeeList = new List<Fee>();
                    //List<int?> y = studentModel.getStudentYears(id);
                    //ViewBag.Year = y;
                    //ViewBag.SelectedYear = selectedYear;
                    Overall_Fees ov = studentModel.ShowFeeRecords_StudentModelFunction(id);
                    if (ov==null)
                    {
                        return View("SHowFee", null);
                    }
                    else
                    {
                        return View(ov);
                    }
                    
                //}
                
            }
            catch (Exception)
            {
                return View("SHowFee",null);
            }
            
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SHowFee( int? page, int? selectedYear)//Search Subject Attendance By Month
        //{
        //        string id = Session["rollno"].ToString();
            
        //        List<Fee> FeeList = new List<Fee>();
        //        FeeList = studentModel.ShowFeeRecords_StudentModelFunction(id, 2, selectedYear);

        //        List<int?> y = studentModel.getStudentYears(id);

        //        if (FeeList.Count != 0)
        //        {
        //            ViewBag.Year = y;
        //            ViewBag.SelectedYear = selectedYear;
        //            return View("SHowFee", FeeList.ToPagedList(page ?? 1, 10));
        //        }
        //        else
        //        {
        //            ViewBag.Year = y;
        //            ViewBag.SelectedYear = selectedYear;
        //            ViewBag.Message = "No Record Found";
        //            return View();
        //        }
        //}

        #endregion
    }
}