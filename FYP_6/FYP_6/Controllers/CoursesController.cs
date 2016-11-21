using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FYP_6.Models.Models_Logic;
using FYP_6.Models;
using FYP_6.SessionExpireChecker;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpire]
    public class CoursesController : Controller
    {
        RCIS3Entities r = RCIS3Entities.getinstance();

        CoursesModel coursesModel = new CoursesModel();
        // GET: Courses
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageCourses(int? page,string res)
        {
                SessionClearOnReload();
                if (res!=null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
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
        public ActionResult ManageCourses(int? page, string del, Guid programID)
        {            
                //int prog_ID = int.Parse(programID);
                if (programID != null)
                {
                    try
                    {
                        var getDegreeRecToDelete = r.Degree_Program.Where(s => s.ProgramID == programID).Select(s => s).FirstOrDefault();
                        r.Degree_Program.Remove(getDegreeRecToDelete);
                        r.SaveChanges();
                        return RedirectToAction("ManageCourses");
                    }
                    catch (Exception)
                    {
                        ViewBag.Message = "Unable to Remove Record, ERROR: " + " There are related records for this Degree Program";
                        var getAllDegreePrograms = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
                        return View(getAllDegreePrograms.ToPagedList(page ?? 1, 10));
                    }
                    //var getDegreeRecToDelete = r.Degree_Program.Where(s => s.ProgramID == prog_ID).Select(s => s).FirstOrDefault();
                    //r.Degree_Program.Remove(getDegreeRecToDelete);
                    //r.SaveChanges();
                    //return RedirectToAction("ManageCourses");
                }
                else
                {
                    var getAllDegreePrograms = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
                    if (getAllDegreePrograms == null)
                    {
                        ViewBag.Message = "No Records Found";
                        return View(getAllDegreePrograms.ToPagedList(page ?? 1, 10));
                    }
                    return View(getAllDegreePrograms.ToPagedList(page ?? 1, 10));
                }

            

        }
        public ActionResult EditCourse(string id)
        {
            Guid degID;
            if (Guid.TryParse(id,out degID))
            {
                var getDegreeProgram = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                var specificLevel = r.Levels.Where(s => s.LevelID == getDegreeProgram.LevelID).Select(s => s.Level_Name).FirstOrDefault();

                IEnumerable<Subject> ds = coursesModel.getSubjectsRelatedToDegreeForEditing();

                ViewBag.level = specificLevel;
                ViewBag.Degree = getDegreeProgram;

                return View(ds);
            }
            else
            {
                return RedirectToAction("ManageCourses");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse(IEnumerable<Guid> subjects, Guid degID,int part)
        {
            //int degreeID = int.Parse(degID);
            var getDegreeProgram = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();

            if (subjects != null)
            {
                string result=coursesModel.SubjectAddToDegree(subjects,degID,part);

                if (result=="OK")
                {
                    IEnumerable<Subject> ds = coursesModel.getSubjectsRelatedToDegreeForEditing();
                    ViewBag.Degree = getDegreeProgram;
                    ViewBag.level = getDegreeProgram.Level.Level_Name;
                    ViewBag.Message = "Successfully Records Updated";
                    return View(ds);
                }
                else
                {
                    IEnumerable<Subject> ds = coursesModel.getSubjectsRelatedToDegreeForEditing();
                    ViewBag.Degree = getDegreeProgram;
                    ViewBag.level = getDegreeProgram.Level.Level_Name;
                    ViewBag.Message = result;
                    return View(ds);
                }
            }
            else
            {
                IEnumerable<Subject> ds = coursesModel.getSubjectsRelatedToDegreeForEditing();
                if (coursesModel.DeleteAllSubjects(getDegreeProgram,part))
                {
                    ViewBag.Degree = getDegreeProgram;
                    ViewBag.level = getDegreeProgram.Level.Level_Name;
                    ViewBag.Message = "Successfully Records Updated";
                    return View(ds);   
                }
                else
                {
                    ViewBag.Degree = getDegreeProgram;
                    ViewBag.level = getDegreeProgram.Level.Level_Name;
                    ViewBag.Message = "Unable to Update Subjects";
                    return View(ds);   
                }
            }
        }
        public ActionResult DetailCourse(string id)
        {
            try
            {
                Guid degID = Guid.Parse(id);
                var getDegreeProgram = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                var specificLevel = r.Levels.Where(s => s.LevelID == getDegreeProgram.LevelID).Select(s => s.Level_Name).FirstOrDefault();
                IEnumerable<Degree_Subject> ds = coursesModel.getSubjectsRelatedToDegree(degID);

                ViewBag.level = specificLevel;
                ViewBag.Degree = getDegreeProgram.Degree_ProgramName;

                return View(ds);
            }
            catch (Exception)
            {
                return RedirectToAction("ManageCourses");
            }
        }
        
        [HttpPost]
        public ActionResult DeleteCourseRecords(IEnumerable<Guid> deleteCourses,string hiddenInput)
        {
                if (deleteCourses != null &&hiddenInput!="")
                {
                    string result = coursesModel.DeleteAllDegreePrograms(deleteCourses);
                    if (result=="OK")
                    {
                        return RedirectToAction("ManageCourses", "Courses"
                            , new {res=SherlockHolmesEncryptDecrypt.Encrypt("S") });   
                    }
                    else
                    {
                        return RedirectToAction("ManageCourses", "Courses"
                            , new { res = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                    }
                }
                else
                {
                    return RedirectToAction("ManageCourses", "Courses",
                        new {
                       res=SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!")
                        });
                }
        }

        public ActionResult AddDegreeRecords()
        {
                var AlltheLevels = r.Levels.Select(s => s).OrderBy(s => s.LevelID);
                ViewBag.Levels = AlltheLevels;
                ViewBag.Message = "";
                return View();
            
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddDegreeRecords(Degree_Program deg, string level)
        {
                if (level== "Select")
                {
                    ViewBag.Message = "Error! Unable to Add Record!" + " Plz Select a Valid Degree level";
                    var AlltheLevels = r.Levels.Select(s => s).OrderBy(s => s.LevelID);
                    ViewBag.Levels = AlltheLevels;
                    return View();
                }
                else
                {
                    Guid levelID = Guid.Parse(level);
                    deg.LevelID = levelID;
                    deg.Level = r.Levels.Where(s => s.LevelID == levelID).Select(s => s).FirstOrDefault();

                    if (coursesModel.AddCourse(levelID, deg))
                    {
                        ViewBag.Message = "Successfully Record Added";
                        var AlltheLevels = r.Levels.Select(s => s).OrderBy(s => s.LevelID);
                        ViewBag.Levels = AlltheLevels;
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "Error! Unable to Add Record! " + deg.Degree_ProgramName + " Already Exists";
                        var AlltheLevels = r.Levels.Select(s => s).OrderBy(s => s.LevelID);
                        ViewBag.Levels = AlltheLevels;
                        return View();
                    }

                    //}
                    //else
                    //{
                    //    foreach (var item in ModelState)
                    //    {
                    //        ViewBag.M2 = item.Key;
                    //        ViewBag.M1 = item.Value;
                    //    }
                    //    ViewBag.Message = "Error! Unable to Add Record!";
                    //    var AlltheLevels = r.Levels.Select(s => s).OrderBy(s => s.LevelID);
                    //    ViewBag.Levels = AlltheLevels;
                    //    return View();
                    //}
                }

         
        }
        public ActionResult AddSubjectsToDegreeProgram()
        {
         
                var Degrees = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
                ViewBag.Degrees = Degrees;

                ViewBag.Message = "";
                return View();
         
        }
        public ActionResult AddSubjectsForSelectedDegree(IEnumerable<Subject> subjects)
        {
                if (subjects != null)
                {
                    //int degID=int.Parse(degree);
                    //Degree_Program deg= r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                    //ViewBag.DegreeSelected = deg;
                    //var getSubjects = r.Subjects.OrderBy(s=>s.SubjectID).Select(s => s);
                    return View(subjects);
                }
                else
                {
                    ViewBag.Message = "No Subjects Found";
                    return View();
                }
         
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubjectsForSelectedDegree(IEnumerable<Guid> subjects, string selectedDegID, string degree)
        {
         
                if (degree != null)
                {
                    Guid degID = Guid.Parse(degree);
                    Degree_Program deg = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                    ViewBag.DegreeSelected = deg;
                    ViewBag.DegreeName = deg.Degree_ProgramName
;
                    var getSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
                    return View(getSubjects);
                }
                else
                {
                    if (subjects != null)
                    {
                        //int degID = int.Parse(selectedDegID);
                        Guid degID = Guid.Parse(degree);

                        Degree_Program getDegree = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();

                        List<Subject> listToDelete = r.Subjects.Where(s => subjects.Contains(s.SubjectID)).ToList();

                        if (coursesModel.CheckToSeeIfThereExistARecordWithSameDegreeNameAndSubject(getDegree, listToDelete))
                        {
                            foreach (var item in listToDelete)
                            {
                                r.Degree_Subject.Add(new Degree_Subject
                                {
                                    DegreeID = getDegree.ProgramID,
                                    SubjectID = item.SubjectID,
                                    ID=Guid.NewGuid()
                                });
                            }
                            r.SaveChanges();
                            ViewBag.DegreeSelected = getDegree.Degree_ProgramName;
                            ViewBag.Message = "Succesfully Subjects Assigned";
                            return View(listToDelete);
                        }
                        else
                        {
                            ViewBag.Message = "The Degree " + getDegree.Degree_ProgramName + " has already assigned some Selected Subjects";
                            return View(listToDelete);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Plz Select A Degree First";
                        return View();
                    }
                }
         
        }
        public ActionResult RemoveSubjectsToDegreeProgram()
        {
         
                var Degrees = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
                ViewBag.Degrees = Degrees;

                ViewBag.Message = "";
                return View();
        }
        public ActionResult RemoveSubjectsForSelectedDegree(IEnumerable<Degree_Subject> subjects)
        {
            
                if (subjects != null)
                {
                    //int degID=int.Parse(degree);
                    //Degree_Program deg= r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                    //ViewBag.DegreeSelected = deg;
                    //var getSubjects = r.Subjects.OrderBy(s=>s.SubjectID).Select(s => s);
                    return View(subjects);
                }
                else
                {
                    ViewBag.Message = "No Subjects Found";
                    return View();
                }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSubjectsForSelectedDegree(IEnumerable<Guid> subjects, string selectedDegID, string degree)
        {
            
                if (degree != null)
                {
                    //int degID = int.Parse(degree);
                    Guid degID = Guid.Parse(degree);

                    Degree_Program deg = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                    ViewBag.DegreeSelected = deg;
                    ViewBag.DegreeName = deg.Degree_ProgramName;
                    var getSubjects = r.Degree_Subject.OrderBy(s => s.ID).Where(s => s.DegreeID == degID).Select(s => s);
                    return View(getSubjects);
                }
                else
                {
                    if (subjects != null)
                    {
                        try
                        {
                            //int degID = int.Parse(selectedDegID);
                            Guid degID = Guid.Parse(degree);

                            Degree_Program getDegree = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                            List<Degree_Subject> listToDelete = r.Degree_Subject.Where(s => subjects.Contains(s.SubjectID.Value)
                                && s.DegreeID == getDegree.ProgramID).ToList();
                            if (coursesModel.DeleteRelatedBPSRecordsOnDeletionOfSubjects(listToDelete))
                            {
                                foreach (var item in listToDelete)
                                {
                                    r.Degree_Subject.Remove(item);
                                }
                                r.SaveChanges();
                                ViewBag.DegreeSelected = getDegree.Degree_ProgramName;
                                ViewBag.Message = "Succesfully Subjects Removed";
                                return View(listToDelete);
                            }
                            else
                            {
                                ViewBag.DegreeSelected = getDegree.Degree_ProgramName;
                                ViewBag.Message = "Unable To Delete Subjects";
                                return View(listToDelete);
                            }

                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Plz Select A Degree First";
                        return View();
                    }
                }
            
        }
        public void SessionClearOnReload()
        {
            Session["Success"] = null;
        }
    }
}