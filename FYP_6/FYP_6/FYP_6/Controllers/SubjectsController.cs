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
    public class SubjectsController : Controller
    {
        RCIS2Entities1 r = RCIS2Entities1.getinstance();
        // GET: Subjects
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageSubjects(int? page,string res)
        {
                SessionClearOnReload();
                if (res!=null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
                var getAllSubjects = r.Subjects.Select(s => s).OrderBy(s => s.SubjectName);
                if (getAllSubjects == null)
                {
                    ViewBag.Message = "No Records Found";
                    return View(getAllSubjects.ToPagedList(page ?? 1, 10));
                }
                return View(getAllSubjects.ToPagedList(page ?? 1, 10));
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageSubjects(int? page, string del, string programID)
        {
            var getAllSubjects = r.Subjects.Select(s => s).OrderBy(s => s.SubjectName);
                if (getAllSubjects == null)
                {
                    ViewBag.Message = "No Records Found";
                    return View(getAllSubjects.ToPagedList(page ?? 1, 10));
                }
                return View(getAllSubjects.ToPagedList(page ?? 1, 10));

        }
        public ActionResult EditSubject(string id)
        {
            try
            {
                Guid subjID = Guid.Parse(id);
                var getSubject = r.Subjects.Where(s => s.SubjectID == subjID).Select(s => s).FirstOrDefault();
                //var AlltheDegreePrograms = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
                var getRelatedDegrees = r.Degree_Subject.Where(s => s.SubjectID == subjID).Select(s => s.Degree_Program.Degree_ProgramName).Distinct().Count();

                ViewBag.Message = "";
                //ViewBag.Degrees = AlltheDegreePrograms;
                ViewBag.RelatedDegrees = getRelatedDegrees;
                return View(getSubject);            
            }
            catch (Exception)
            {

                return RedirectToAction("ManageSubjects");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSubject(string SubjectID, Subject sub)
        {
            try
            {
                Guid subj_ID = Guid.Parse(SubjectID);
                var getUpdatedRecord = r.Subjects.Where(s => s.SubjectID == subj_ID).Select(s => s).FirstOrDefault();
                var getRelatedDegrees = r.Degree_Subject.Where(s => s.SubjectID == subj_ID).Select(s => s.Degree_Program.Degree_ProgramName).Distinct().Count();

                getUpdatedRecord.SubjectName = sub.SubjectName;
                r.SaveChanges();
                ViewBag.Message = "Successfully Record Updated";
                var getSubject = r.Subjects.Where(s => s.SubjectID == subj_ID).Select(s => s).FirstOrDefault();
                ViewBag.RelatedDegrees = getRelatedDegrees;
                return View(getSubject);

            }
            catch (Exception)
            {
                return RedirectToAction("ManageSubjects");
            }
        }
        [HttpPost]

        public ActionResult DeleteSubjectRecords(IEnumerable<Guid> deletesub,string hiddenInput)
        {
            if (deletesub != null &&hiddenInput!="")
            {
                string result = SubjectsModel.DeleteSubjects(deletesub);
                if (result=="OK")
                {
                    return RedirectToAction("ManageSubjects", "Subjects", new 
                    {
                        res=SherlockHolmesEncryptDecrypt.Encrypt("S")
                    });   
                }
                else
                {
                    return RedirectToAction("ManageSubjects", "Subjects", new
                    {
                        res = SherlockHolmesEncryptDecrypt.Encrypt(result)
                    });
                }
            }
            else
            {
                return RedirectToAction("ManageSubjects", "Subjects", new 
                    {
                        res=SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!")
                    });
            }

        }
        public ActionResult AddSubjectRecords()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubjectRecords(Subject sub, string degree)
        {
            if (ModelState.IsValid)
            {
                string result = SubjectsModel.AddSubject(sub);
                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Record Added";
                    return View();
                }
                else
                {
                    ViewBag.Message = "Error! " + result;
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "Error! Unable to Add Record!";
                return View();
            }
        }
        public void SessionClearOnReload()
        {
            Session["Success"] = null;
        }
    }
}