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
        RCIS2Entities1 r = RCIS2Entities1.getinstance();
        // GET: Admin

        #region AdminPersonal
        public ActionResult Index()
        {
            return View();
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
                Guid id = Guid.Parse(Session["AdminID"].ToString());
                string newpass1 = HttpUtility.HtmlEncode(newpass);
                string oldpass1 = HttpUtility.HtmlEncode(oldpass);

                ViewBag.Message = EmployeesModel.ChangePassword_AdminFunction(oldpass1, newpass1, id);
                return View();
        }
        #endregion

        #region Admin Functions
        public ActionResult ManageEmployees(int? page)
        {

            IEnumerable<Employee> DataBasedOnRollnos = EmployeesModel.GetAllEmployeesRecords();
            if (DataBasedOnRollnos == null)
            {
                ViewBag.Message = "No Record Found";
                return View();
            }
            return View(DataBasedOnRollnos.ToPagedList(page ?? 1, 10));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageEmployees(int? page, string search)
        {
            bool checker = false;

            IEnumerable<Employee> DataBasedOnRollnos = EmployeesModel.GetAllEmployeesRecords();
            foreach (var item in DataBasedOnRollnos)
            {
                if (item.Name.StartsWith(search))
                {
                    checker = true;
                    break;
                }
            }
            if (checker)
            {
                ViewBag.SearchQuery = "True";
                IEnumerable<Employee> SearchedData = EmployeesModel.getSpecificSearchRecordForEmployees(search);
                return View(SearchedData.ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.SearchQuery = "Name Doesnot Exists";
                return View();
            }
        }
        [HttpPost]
        public ActionResult DeleteEmployees(IEnumerable<Guid> deleteEmp)
        {
            if (deleteEmp != null)
            {
                List<Employee> listToDelete = r.Employees.Where(s => deleteEmp.Contains(s.EmpID)).ToList();
                foreach (var item in listToDelete)
                {
                    r.Employees.Remove(item);

                }
                r.SaveChanges();
                return RedirectToAction("ManageEmployees", "Admin");
            }
            else
            {
                return RedirectToAction("ManageEmployees", "Admin");
            }
        }
        public ActionResult AddNewEmp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewEmp(Employee tRec, HttpPostedFileBase file, Nullable<System.DateTime> date1)
        {

            try
            {
                if (EmployeesModel.NewEmployeeAddition(tRec, file, date1))
                {
                    ViewBag.Message = "Successfully Record Added";
                    return View();
                }
                else
                {
                    ViewBag.Message = "Unable to Add Records";
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                return View();
                throw;
            }
        }

        public ActionResult EditEmployees(Guid id)
        {
            var getEMP = r.Employees.Where(s => s.EmpID == id).Select(s => s).FirstOrDefault();
            return View(getEMP);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployees(Guid EmpID, Employee em, HttpPostedFileBase file
            , Nullable<System.DateTime> date1)
        {

            var getEMP = r.Employees.Where(s => s.EmpID == EmpID).Select(s => s).FirstOrDefault();

            if (em != null)
            {

                if (EmployeesModel.UpdateEmployeeRecord(EmpID, em, file, date1))
                {
                    ViewBag.Message = "Successfully Updated Record";
                    ViewBag.Message2 = "";
                    return View(getEMP);
                }
                else
                {
                    ViewBag.Message = "No Changes were Made to Update the Record";
                    return View(getEMP);
                }
            }
            else
            {
                ViewBag.Message = "No Changes were Made to Update the Record";
                return View(getEMP);
            }
        }

        #endregion

        #region Students
        public ActionResult StudentRecords(int? page)
        {
            IEnumerable<Registeration> DataBasedOnRollnos = EmployeesModel.GetAllStudentRecords();
            return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentRecords(string search, int StudentType, int? page)
        {
            bool checker = false;
            IEnumerable<Registeration> DataBasedOnRollnos = EmployeesModel.GetAllStudentRecords();
            foreach (var item in DataBasedOnRollnos)
            {
                if (item.Rollno.StartsWith(search))
                {
                    checker = true;
                    break;
                }
            }
            if (checker)
            {
                ViewBag.SearchQuery = "True";
                IEnumerable<Registeration> SearchedData = EmployeesModel.getSpecificSearchRecord(search, StudentType);
                return View(SearchedData.ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.SearchQuery = "Roll no Doesnot Exists";
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

        public ActionResult StudentMarksRecords(int? page)
        {
            IEnumerable<Student_Marks> DataBasedOnRollnos = EmployeesModel.GetAllStudentMarksRecords();
            return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentMarksRecords(string search, int StudentType, string ifButtonPressed
            , int? page,string year,string month)
        {
            IEnumerable<Student_Marks> SearchedData = EmployeesModel.getSpecificSearchStudentMarksRecord(search, StudentType,year,month);
            //IEnumerable<Student_Profile> SearchedData = EmployeesModel.getSpecificSearchRecord(search, StudentType);
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

        public ActionResult StudentAttendanceRecords(int? page)
        {
            IEnumerable<Students_Attendance> DataBasedOnRollnos = EmployeesModel.GetAllStudentAttendanceRecords();
            return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentAttendanceRecords(string search, IEnumerable<int> deleteroll, int StudentType, string ifButtonPressed
            , int? page,string year, string month)
        {

            IEnumerable<Students_Attendance> SearchedData = EmployeesModel.getSpecificSearchStudentAttendanceRecord(search, StudentType,year,month);
            //IEnumerable<Student_Profile> SearchedData = EmployeesModel.getSpecificSearchRecord(search, StudentType);
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


        public ActionResult Student_Subjects(int? page)
        {
            IEnumerable<Assign_Subject> DataBasedOnRollnos = EmployeesModel.GetAllStudentSubjectRecords();
            return View("Student_Subjects", DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 30));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Student_Subjects(string search, IEnumerable<int> deleteroll, int StudentType, string ifButtonPressed, int? page)
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

        #endregion

        #region Teachers
        public ActionResult TeacherRecords(int? page)
        {
            IEnumerable<Teacher> DataBasedOnRollnos = EmployeesModel.GetAllTeacherRecords();
            if (DataBasedOnRollnos == null)
            {
                ViewBag.Message = "ID no Doesnot Exists";
                return View();
            }
            return View(DataBasedOnRollnos.ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherRecords(string search, int? page, IEnumerable<string> deleteTeacher, string TeacherType)
        {

            bool checker = false;
            IEnumerable<Teacher> DataBasedOnRollnos = EmployeesModel.GetAllTeacherRecords();
            foreach (var item in DataBasedOnRollnos)
            {
                if (item.TeacherID.StartsWith(search))
                {
                    checker = true;
                    break;
                }
            }
            if (checker)
            {
                ViewBag.SearchQuery = "True";
                IEnumerable<Teacher> SearchedData = EmployeesModel.getSpecificSearchRecordForTeacher(search, TeacherType);
                return View(SearchedData.ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.SearchQuery = "ID no Doesnot Exists";
                return View();
            }
        }

        public ActionResult DetailTeacher(string id)
        {
            var getTeacher = r.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
            if (getTeacher==null)
            {
                return RedirectToAction("TeacherRecords");
            }
            return View(getTeacher);
        }
        [HttpGet]
        public ActionResult TeacherAttendance(int? page)
        {
            TempData["T_ID"] = null;

            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            var SectionsOfTeacher = r.Sections.Select(s => s).OrderBy(s => s.SectionID);
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
                ViewBag.Message = "No Records Founds";
                return View("TeacherAttendance", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherAttendance(int? page, string search,string month,string year)
        {
            if (search == null || search == "")
            {
                ViewBag.Message = "Plz Enter Teacher ID to Search Records";

                return View("TeacherAttendance", null);
            }
            else
            {

                IEnumerable<Teacher_Attendance> EndResultListOfMarks = EmployeesModel.showResultsTeacherAttendance_EmployeeModelFunction(search,month,year);

                if (EndResultListOfMarks!=null)
                {
                    TempData["T_ID"] = "Teacher ID: " + search + ", Total Lectures: "
                        + EndResultListOfMarks.Count() + ", Attended Lectures:" + EndResultListOfMarks.Where(s => s.Present.StartsWith("Y")).Select(s => s).Count() +
                    @", Month: " + month + ", Year: " + year;

                    return View("TeacherAttendance", EndResultListOfMarks.OrderBy(s => s.TeacherID).Take(50).ToPagedList(page ?? 1, 10));
                }
                else
                {
                    TempData["T_ID"] = search;

                    ViewBag.Message = "No Records Founds";
                    return View("TeacherAttendance", null);
                }
            }
        }
        //Teacher Subjects Add, Delete,Edit,Details Starts From Here
        public ActionResult Teacher_Subjects(int? page, string val)
        {
            TempData["TeacherID"] = null;
            TempData["Batch"] = null;
            TempData["Section"] = null;
            TempData["Degree"] = null;
            IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllTeacher_SubjectsRecords();

            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
            var ListofSections = r.Sections.Select(s => s);

            if (EndResultListOfMarks != null)
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s.);
                //ViewBag.ListofSections = ListofSections;
                ViewBag.TeacherIDs = TeachersIDS;
                ViewBag.Message = val;
                return View("Teacher_Subjects", EndResultListOfMarks.Take(100).ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                //ViewBag.ListofSections = r.Sections.Select(s => s);
                ViewBag.TeacherIDs = TeachersIDS;
                ViewBag.Message = val;
                ViewBag.Message = "No Records Founds";
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
            //var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
            //var ListofSections = r.Sections.Select(s => s);
            if (degree == null || degree == "Please select"
                || section == null || section == "Please select"
                || batch == null || batch == "Please select")
            {
                IEnumerable<Teacher_Subject> EndResultListOfMarks = EmployeesModel.GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(teacherID);
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                //ViewBag.ListofSections = r.Sections.Select(s => s);
                //ViewBag.TeacherIDs = TeachersIDS;
                ViewBag.Message = val;
                if (EndResultListOfMarks != null)
                {
                    return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                }
                else
                {
                    ViewBag.Message = "No Records Founds";
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
                    //ViewBag.ListofPartsInDegree = r.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s.);
                    //ViewBag.ListofSections = ListofSections;
                    //ViewBag.TeacherIDs = TeachersIDS;
                    ViewBag.Message = val;
                    return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                }
                else
                {
                    ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                    //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                    //ViewBag.ListofSections = r.Sections.Select(s => s);
                    //ViewBag.TeacherIDs = TeachersIDS;
                    ViewBag.Message = val;
                    ViewBag.Message = "No Records Founds";
                    return View("Teacher_Subjects", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                }
            }


        }
        public ActionResult Teacher_Batches(int? page, string val)
        {
            TempData["TeacherID"] = null;
            //TempData["Batch"] = null;
            //TempData["Section"] = null;
            //TempData["Degree"] = null;

            IEnumerable<Teachers_Batches> EndResultListOfMarks = EmployeesModel.GetAllTeacher_batchesRecords();

            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
            var ListofSections = r.Sections.Select(s => s);

            if (EndResultListOfMarks != null)
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s.);
                ViewBag.ListofSections = ListofSections;
                ViewBag.TeacherIDs = TeachersIDS;
                ViewBag.Message = val;
                return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.ListofDegreePrograms = ListofDegreePrograms;
                //ViewBag.ListofPartsInDegree = r.Teachers_DegreeProgram.Where(s => s.TeacherID == t_id).Select(s => s.Part);
                ViewBag.ListofSections = r.Sections.Select(s => s);
                ViewBag.TeacherIDs = TeachersIDS;
                ViewBag.Message = val;
                ViewBag.Message = "No Records Founds";
                return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Teacher_Batches(int? page, string val, string teacherID)
        {
            TempData["TeacherID"] = teacherID;
            var TeachersIDS = r.Teachers.OrderBy(s => s.TeacherID).Select(s => s.TeacherID);
            if (teacherID == null)
            {
                ViewBag.TeacherIDs = TeachersIDS;
                ViewBag.Message = val;
                ViewBag.Message = "No Records Founds";
                return View("Teacher_Batches", null);
            }
            else
            {
                IEnumerable<Teachers_Batches> EndResultListOfMarks = EmployeesModel.GetAllSearchSpecificTeacher_BatchesRecords(teacherID);
                if (EndResultListOfMarks != null)
                {
                    ViewBag.TeacherIDs = TeachersIDS;
                    ViewBag.Message = val;
                    return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                }
                else
                {
                    ViewBag.TeacherIDs = TeachersIDS;
                    ViewBag.Message = val;
                    ViewBag.Message = "No Records Founds";
                    return View("Teacher_Batches", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                }
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
                if (Guid.TryParse(degree,out deg))
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