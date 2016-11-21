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
    public class SectionController : Controller
    {
        RCIS2Entities1 r = RCIS2Entities1.getinstance();
        // GET: Section
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageSections(int? page,string res)
        {
                SessionClearOnReload();
                if (res!=null)
                {
                    ViewBag.Message=SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
                var getAllSections = r.Sections.Select(s => s).OrderBy(s => s.SectionName);
                if (getAllSections == null)
                {
                    ViewBag.Message = "No Records Found";
                    return View(getAllSections.ToPagedList(page ?? 1, 10));
                }
                return View(getAllSections.ToPagedList(page ?? 1, 10));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageSections(int? page, string del, string programID)
        {
                var getAllSections = r.Sections.Select(s => s).OrderBy(s => s.SectionName);
                if (getAllSections == null)
                {
                    ViewBag.Message = "No Records Found";
                    return View(getAllSections.ToPagedList(page ?? 1, 10));
                }
                return View(getAllSections.ToPagedList(page ?? 1, 10));

        }
        public ActionResult EditSection(string id)
        {
            try
            {
                //int secID = int.Parse(id);
                Guid GID = Guid.Parse(id);
                var getSection = r.Sections.Where(s => s.SectionID == GID).Select(s => s).FirstOrDefault();
                //var AlltheDegreePrograms = r.Degree_Program.Select(s => s).OrderBy(s => s.ProgramID);
                ViewBag.Message = "";
                //ViewBag.Degrees = AlltheDegreePrograms;
                return View(getSection);
            }
            catch (Exception)
            {
                return RedirectToAction("ManageSections");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSection(string SectionID, Section sec)
        {
            try
            {
                //int secID = int.Parse(SectionID);
                Guid GID = Guid.Parse(SectionID);
                var getUpdatedRecord = r.Sections.Where(s => s.SectionID == GID).Select(s => s).FirstOrDefault();
                try
                {
                    getUpdatedRecord.SectionName = sec.SectionName;
                    r.SaveChanges();
                    ViewBag.Message = "Successfully Record Updated";
                    var getSection = r.Sections.Where(s => s.SectionID == GID).Select(s => s).FirstOrDefault();
                    return View(getSection);
                }
                catch (Exception)
                {
                    ViewBag.Message = "Unable to Update Record";
                    var getSection = r.Sections.Where(s => s.SectionID == GID).Select(s => s).FirstOrDefault();
                    return View(getSection);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageSections");
            }
        }

        [HttpPost]
        public ActionResult DeleteSectionRecords(IEnumerable<Guid> deletesec,string hiddenInput)
        {
            if (deletesec != null &&hiddenInput!="")
            {
                string result = SectionModel.DeleteSection_SectionModelFunction(deletesec);
                if (result=="OK")
                {
                    return RedirectToAction("ManageSections", "Section"
                        , new {
                            res=SherlockHolmesEncryptDecrypt.Encrypt("S")
                        });
                }
                else
                {
                    return RedirectToAction("ManageSections", "Section"
                        , new
                        {
                            res = SherlockHolmesEncryptDecrypt.Encrypt(result)
                        });
                }
            }
            else
            {
                return RedirectToAction("ManageSections", "Section"
                    , new
                    {
                        res = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!")
                    });
            }
        }
        public ActionResult AddSectionRecords()
        {
                ViewBag.Message = "";
                return View();   
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSectionRecords(Section sec)
        {   
                if (ModelState.IsValid)
                {

                    if (SectionModel.AddSection(sec))
                    {
                        ViewBag.Message = "Successfully Record Added";
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "Error! Unable to Add Record!";
                        //var AlltheLevels = r.Levels.Select(s => s).OrderBy(s => s.LevelID);
                        //ViewBag.Levels = AlltheLevels;
                        return View();
                    }

                }
                else
                {
                    ViewBag.Message = "Error! Unable to Add Record!";
                    //var AlltheLevels = r.Levels.Select(s => s).OrderBy(s => s.LevelID);
                    //ViewBag.Levels = AlltheLevels;
                    return View();
                }
            
        }
        public void SessionClearOnReload()
        {
            Session["Success"] = null;
        }
    }
}