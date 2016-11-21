﻿using System;
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
    public class YearsController : Controller
    {

        RCIS2Entities1 r = RCIS2Entities1.getinstance();
        // GET: Years
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageDegreeDurations(int? page, string delFunResult)
        {
            SessionClearOnReload();
            if (delFunResult!=null)
            {
                var getAllYears = r.Years.Where(s => s.Status == 1).Select(s => s).OrderBy(s => s.FromYear);
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(delFunResult);

                return View(getAllYears.ToPagedList(page ?? 1, 10));
            }
            else
            {
                var getAllYears = r.Years.Where(s => s.Status == 1).Select(s => s).OrderBy(s => s.FromYear);
                if (getAllYears == null)
                {
                    ViewBag.Message = "No Records Found";
                    return View(getAllYears.ToPagedList(page ?? 1, 10));
                }
                return View(getAllYears.ToPagedList(page ?? 1, 10));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageDegreeDurations(int? page, string programID, int category)
        {
                var gettingYears = YearsModel.getAllYears(category);
                if (gettingYears == null)
                {
                    ViewBag.Message = "No Records Found";
                    return View(gettingYears.ToPagedList(page ?? 1, 10));
                }
                return View(gettingYears.ToPagedList(page ?? 1, 10));
        }
        public ActionResult EditDuration(string id)
        {
            //int yearID = int.Parse(id);
            try
            {
                Guid S_id = Guid.Parse(id);
                var getYear = r.Years.Where(s => s.YearID == S_id).Select(s => s).FirstOrDefault();
                var AlltheBatches = r.Batches.Where(s => s.YearID == S_id).Select(s => s.BatchName);
                ViewBag.Message = "";
                ViewBag.Batches = AlltheBatches;
                return View(getYear);
            }
            catch (Exception)
            {
                return RedirectToAction("ManageDegreeDurations");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDuration(string YearID, Year year)
        {
            try
            {
                Guid id = Guid.Parse(YearID);
                //int yearid = int.Parse(YearID);
                var getUpdatedRecord = r.Years.Where(s => s.YearID == id).Select(s => s).FirstOrDefault();
                var AlltheBatches = r.Batches.Where(s => s.YearID == id).Select(s => s.BatchName);
                try
                {
                    getUpdatedRecord.FromYear = year.FromYear;
                    getUpdatedRecord.ToYear = year.ToYear;
                    r.SaveChanges();
                    ViewBag.Message = "Successfully Record Updated";
                    ViewBag.Batches = AlltheBatches;
                    //var getU = r.Years.Where(s => s.YearID == yearid).Select(s => s).FirstOrDefault();
                    return View(getUpdatedRecord);
                }
                catch (Exception)
                {
                    ViewBag.Message = "Unable to Update Record";
                    ViewBag.Batches = AlltheBatches;
                    //var getYear = r.Years.Where(s => s.YearID == yearid).Select(s => s).FirstOrDefault();
                    return View(getUpdatedRecord);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageDegreeDurations");
            }
        }
        [HttpPost]
        public ActionResult DeleteDurationRecords(IEnumerable<Guid> deleteyear, string btnBackUpYear,string hiddenInput)
        {
            //Check if User Wants To BackUp
            if (btnBackUpYear!=null)
            {
                if (deleteyear != null && hiddenInput != "")
                {
                    string result = YearsModel.BackUpYearRecords(deleteyear);
                    if (result == "OK")
                    {
                        string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Saved!");

                        return RedirectToAction("ManageDegreeDurations", "Years", new { delFunResult = outputMessage });
                    }
                    else
                    {
                        string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Unable To BackUp Data!");

                        return RedirectToAction("ManageDegreeDurations", "Years", new { delFunResult = outputMessage });
                    }
                }
                else
                {
                    string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To BackUp!");

                    return RedirectToAction("ManageDegreeDurations", "Years", new { delFunResult = outputMessage });
                }
            }
            //Check if user wants to delete 
            else
            {
                if (deleteyear!=null && hiddenInput!="")
                {
                    string result = YearsModel.DeleteYearRecords(deleteyear);

                    if (result == "OK")
                    {
                        string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!");

                        return RedirectToAction("ManageDegreeDurations", "Years", new { delFunResult = outputMessage });
                    }
                    else
                    {
                        string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Unable To Delete Records!");

                        return RedirectToAction("ManageDegreeDurations", "Years", new { delFunResult = outputMessage });
                    }   
                }
                else
                {
                    string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!");

                    return RedirectToAction("ManageDegreeDurations", "Years", new { delFunResult = outputMessage });
                }

            }            
        }
        public ActionResult AddYearRecords()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddYearRecords(Year year)
        {
            if (ModelState.IsValid)
            {
                if (YearsModel.AddYear(year))
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