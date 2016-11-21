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
    public class FeeController : Controller
    {
        static RCIS2Entities1 r = RCIS2Entities1.getinstance();
        // GET: Fee
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult FeeRecords(int? page, string res)
        {
            SessionClearOnReload();
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            var getAllFeeRecords = r.Fees.Where(s => s.Registeration.Status.Value == 1).OrderBy(s => s.Rollno).Select(s => s);
            return View("FeeRecords", getAllFeeRecords.ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FeeRecords(string month2, int? page, string search, int? StudentType)
        {
            IEnumerable<Fee> EndResultListOfMarks = EmployeesModel.showFee_EmployeeModelFunction(search, month2, StudentType);

            if (EndResultListOfMarks.Count() > 0)
            {
                ViewBag.Month = month2;
                ViewBag.RollNo = search;
                return View("FeeRecords", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.Message = "No Records Found";
                return View("FeeRecords", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
            }
        }
        [HttpGet]
        public ActionResult AddFeeRecords()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFeeRecords(Fee feeRec)
        {
            if (ModelState.IsValid)
            {
                int datedFee = feeRec.Dated.Value.Month;
                string result = EmployeesModel.AddFeeRec(datedFee, feeRec);
                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Record Added";
                    return View();
                }
                else
                {
                    ViewBag.Message = "Error! Unable to Add Record! " + result;
                    return View();
                }

            }
            else
            {
                ViewBag.Message = "Error! Unable to Add Record!";
                return View();
            }


        }
        public ActionResult EditFee(string id)
        {
            var getFeeRecordToUpdate = r.Fees.Where(s => s.Bill_No == id).Select(s => s).FirstOrDefault();
            if (getFeeRecordToUpdate == null)
            {
                return RedirectToAction("FeeRecords");
            }
            ViewBag.Message = "";
            return View(getFeeRecordToUpdate);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFee(Fee feeRec, string bill, string rollno)
        {
            
            //string result = EmployeesModel.CheckFeeEditied(feeRec, bill);
            try
            {
                int datedFee = feeRec.Dated.Value.Month;

                string result = EmployeesModel.UpdateFeeRec(datedFee, feeRec, rollno, bill);

                if (result == "OK")
                {
                    var getFeeRecordUpdatedCurrently = r.Fees.Where(s => s.Bill_No == bill).Select(s => s).FirstOrDefault();
                    ViewBag.Message = "Successfully Record Updated";
                    return View(getFeeRecordUpdatedCurrently);
                }
                else
                {
                    var getFeeRecordUpdatedCurrently = r.Fees.Where(s => s.Bill_No == bill).Select(s => s).FirstOrDefault();
                    ViewBag.Message = result;
                    return View(getFeeRecordUpdatedCurrently);
                }
            }
            catch (Exception)
            {
                var getFeeRecordUpdatedCurrently = r.Fees.Where(s => s.Bill_No == bill).Select(s => s).FirstOrDefault();
                ViewBag.Message = "Unable to Update Record!";
                return View(getFeeRecordUpdatedCurrently);
            }
            
        }
        public ActionResult DetailFee(string id)
        {
            var getFeeRecordToUpdate = r.Fees.Where(s => s.Bill_No == id).Select(s => s).FirstOrDefault();
            return View(getFeeRecordToUpdate);
        }
        [HttpPost]
        public ActionResult DeleteFeeRecords(IEnumerable<string> deletefee, string delFee, string hiddenInput)
        {
            if (deletefee != null && hiddenInput != "")
            {
                if (delFee != null)
                {
                    string result = EmployeesModel.DeleteFeeRecordsOfStudents(deletefee);

                    if (result == "OK")
                    {
                        return RedirectToAction("FeeRecords", "Fee", new { res = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!") });
                    }
                    else
                    {
                        return RedirectToAction("FeeRecords", "Fee", new { res = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                    }
                }
                else
                {
                    return RedirectToAction("FeeRecords", "Fee");
                }
            }
            else
            {
                return RedirectToAction("FeeRecords", "Fee", new { res = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!") });
            }
        }
        public void SessionClearOnReload()
        {
            Session["Success"] = null;
        }
    }
}