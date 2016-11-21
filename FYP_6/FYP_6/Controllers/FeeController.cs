using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using FYP_6.Models.Models_Logic;
using FYP_6.Models;
using System.IO;
using FYP_6.SessionExpireChecker;
using CrystalDecisions.CrystalReports.Engine;
using FYP_6.Models.Report_Models;
using FYP_6.Models.ViewModels;
using System.Data.Entity;

namespace FYP_6.Controllers
{
    [Authorize]
    [SessionExpire]
    public class FeeController : Controller
    {
        static RCIS3Entities r = RCIS3Entities.getinstance();
        // GET: Fee
        EmployeesModel empModel = new EmployeesModel();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FeeSummary(string id, int? page, string res, string ifButtonPressed, string search2
            , int? StudentType, string generatepdf)
        {
             
            if (res != null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
            }
            if (generatepdf != null)
            {
                IEnumerable<RptFeeSummaryClass> rcfslbe = empModel.GetReportModelForFeeSummary(search2, StudentType);

                if (rcfslbe != null)
                {
                    try
                    {
                        ReportDocument rd = new ReportDocument();
                        rd.Load(Server.MapPath("~/Reports/FeeSummaryReport.rpt"));

                        rd.SetDataSource(rcfslbe.ToList());
                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        return File(stream, "application/Pdf", "FeeSummaryReport.pdf");
                    }
                    catch (Exception)
                    {
                        ViewBag.Message = "Unable to Generate PDF! Plz Try Again!";
                        return View("FeeSummary", null);
                    }
                }
                else
                {
                    ViewBag.Message = "No Record Found For Generating PDF";
                    return View("FeeSummary", null);
                }

            }
            else if (ifButtonPressed != null || Request.QueryString["search2"] != null || Request.QueryString["StudentType"] != null)
            {
                IEnumerable<Overall_Fees> SearchedData = empModel.getSpecificSearchRecordOverAllFees(search2, StudentType);
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
            else
            {
                IEnumerable<Overall_Fees> DataBasedOnRollnos = empModel.getFeeRecordsOverall();
                if (DataBasedOnRollnos == null)
                {
                    ViewBag.SearchQuery = "Nothing";
                    return View("FeeSummary", null);
                }
                ViewBag.SearchQuery = "True";
                return View(DataBasedOnRollnos.Take(50).ToPagedList(page ?? 1, 10));
            }
            
        }
        [HttpGet]
        public ActionResult FeeRecords(string month2, int? page, string search2, int? StudentType, string res, string searchfee,string year,
            string generatepdf)
        {
            try
            {
                SessionClearOnReload();
                if (res != null)
                {
                    ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(res);
                }
                //if (generatepdf!=null)
                //{
                //    IEnumerable<FeeReportCLass> EndResultListOfMarks = empModel.GetReportDataForStudentFeesRecords(search2, month2, StudentType, year);

                //    if (EndResultListOfMarks != null)
                //    {
                //    //    TempData["CompleteMessage"] = "Month: " + month2 + ", Year:" + year + ", Rollno: " + search2;
                //        try
                //        {
                //            if (EndResultListOfMarks.Count()>0)
                //            {
                //                ReportDocument rd = new ReportDocument();
                //                rd.Load(Server.MapPath("~/Reports/FeeReportOfStudent.rpt"));

                //                rd.SetDataSource(EndResultListOfMarks.ToList());
                //                Response.Buffer = false;
                //                Response.ClearContent();
                //                Response.ClearHeaders();
                //                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //                stream.Seek(0, SeekOrigin.Begin);
                //                return File(stream, "application/Pdf", "StudentFeesList.pdf");    
                //            }
                //            else
                //            {
                //                ViewBag.Message = "No Records Found To Generate PDF!";
                //                return View("FeeRecords", null);
                //            }

                //        }
                //        catch (Exception)
                //        {
                //            ViewBag.Message = "Unable to Generate PDF! Plz Try Again!";
                //            return View("FeeRecords", null);
                //        }
                //    }
                //    else
                //    {
                //        ViewBag.Message = "No Records Found To Generate PDF!";
                //        return View("FeeRecords", null);
                //    }    
                //}
                if (searchfee != null || Request.QueryString["search2"] != null || Request.QueryString["Month2"] != null || Request.QueryString["StudentType"] != null
                  || Request.QueryString["year"] != null)
                {
                    IEnumerable<Fee> EndResultListOfMarks = empModel.showFee_EmployeeModelFunction(search2, month2, StudentType, year);

                    if (EndResultListOfMarks != null)
                    {
                        TempData["CompleteMessage"] = "Month: " + month2 + ", Year:" + year + ", Rollno: " + search2;
                        //ViewBag.Month = month2;
                        //ViewBag.RollNo = search2;
                        //ViewBag.Year = year;
                        return View("FeeRecords", EndResultListOfMarks.ToPagedList(page ?? 1, 10));
                    }
                    else
                    {
                        //ViewBag.Month = month2;
                        //ViewBag.RollNo = search2;
                        //ViewBag.Year = year;
                        TempData["CompleteMessage"] = "Month: " + month2 + ", Year:" + ", Rollno: " + search2;
                        ViewBag.Message = "No Records Found";
                        return View("FeeRecords", null);
                    }
                }
                else
                {
                    var getAllFeeRecords = r.Fees.Where(s =>  s.Overall_Fees.Registeration.Student_Profile.Status==1).OrderBy(s => s.Overall_Fees.RollNo).Select(s => s);
                    if (getAllFeeRecords == null)
                    {
                        return View("FeeRecords", null);
                    }
                    return View("FeeRecords", getAllFeeRecords.Take(100).ToPagedList(page ?? 1, 10));
                }
            }
            catch (Exception)
            {
                TempData["CompleteMessage"] ="Unable to Load Records! Plz Try Again!";
                return View("FeeRecords", null);
            }
             
        }
         
        [HttpGet]
        public ActionResult AddFeeRecords()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddFeeRecords(ViewModel_FeeManagement feeRec,Nullable<System.DateTime> date1
            , string totaldegfee, string totalSubmitfee, string totalremfee, string totalinstall, string paidInst)
        { 
                if (date1==null)
                {
                    ViewBag.Message = "Error! Unable to Add Record! Date is Required!";
                    return View();
                }
                feeRec.yearlyFee.Bill_No = HttpUtility.HtmlEncode(feeRec.yearlyFee.Bill_No).ToString();

                string result = empModel.AddFeeRec(feeRec,date1,totaldegfee,totalSubmitfee,totalremfee,totalinstall,paidInst);
                if (result == "OK")
                {
                    ViewBag.Message = "Successfully Record Added";
                    if (date1.HasValue)
                    {
                        TempData["DateSaved"] = date1.Value.ToShortDateString();
                    }
                      
                     return View("AddFeeRecords");
                }
                else
                {
                    ViewBag.Message = result;
                    if (date1.HasValue)
                    {
                        TempData["DateSaved"] = date1.Value.ToShortDateString();
                    }
                  
                    return View();
                }

 

        }
        public ActionResult EditFee(string id)
        {
            try
            {
                var getFeeRecordToUpdate = r.Fees.Where(s => s.Bill_No == id).Select(s => s).FirstOrDefault();
                ViewModel_FeeManagement vmfee = new ViewModel_FeeManagement();

                if (getFeeRecordToUpdate == null)
                {
                    return RedirectToAction("FeeRecords");
                }
                 
                //vmfee.feeSummary. = decimal.Truncate(getFeeRecordToUpdate.Overall_Fees.Total_Degree_Fee ?? 0);
                vmfee.feeSummary = getFeeRecordToUpdate.Overall_Fees;
                vmfee.yearlyFee = getFeeRecordToUpdate;
                vmfee.yearlyFee.Admission_Fee = decimal.Truncate(getFeeRecordToUpdate.Admission_Fee ?? 0);
                vmfee.yearlyFee.Tution_Fee = decimal.Truncate(getFeeRecordToUpdate.Tution_Fee ?? 0);
                vmfee.yearlyFee.Total_Fee = decimal.Truncate(getFeeRecordToUpdate.Total_Fee ?? 0);
                vmfee.yearlyFee.Registeration_Fee = decimal.Truncate(getFeeRecordToUpdate.Registeration_Fee ?? 0);
                vmfee.yearlyFee.Exam_Fee = decimal.Truncate(getFeeRecordToUpdate.Exam_Fee ?? 0);
                vmfee.yearlyFee.Fine = decimal.Truncate(getFeeRecordToUpdate.Fine ?? 0);
                ViewBag.Message = "";
                return View(vmfee);
            }
            catch (Exception)
            {
                return RedirectToAction("FeeRecords");
            }     
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditFee(ViewModel_FeeManagement feeRec, string bill, string rollno,Nullable<System.DateTime> date1)
        {
            ViewModel_FeeManagement vmfee = new ViewModel_FeeManagement();
            //string result = EmployeesModel.CheckFeeEditied(feeRec, bill);
            try
            {
                
                 
                string result = empModel.UpdateFeeRec(feeRec, rollno, bill,date1);

                if (result == "OK")
                {
                    var getFeeRecordToUpdate = r.Fees.Where(s => s.Bill_No == bill).Select(s => s).FirstOrDefault();

                    ViewBag.Message = "Successfully Record Updated";
                    vmfee.feeSummary = getFeeRecordToUpdate.Overall_Fees;
                    vmfee.yearlyFee = getFeeRecordToUpdate;
                    vmfee.yearlyFee.Admission_Fee = decimal.Truncate(getFeeRecordToUpdate.Admission_Fee ?? 0);
                    vmfee.yearlyFee.Tution_Fee = decimal.Truncate(getFeeRecordToUpdate.Tution_Fee ?? 0);
                    vmfee.yearlyFee.Total_Fee = decimal.Truncate(getFeeRecordToUpdate.Total_Fee ?? 0);
                    vmfee.yearlyFee.Registeration_Fee = decimal.Truncate(getFeeRecordToUpdate.Registeration_Fee ?? 0);
                    vmfee.yearlyFee.Exam_Fee = decimal.Truncate(getFeeRecordToUpdate.Exam_Fee ?? 0);
                    vmfee.yearlyFee.Fine = decimal.Truncate(getFeeRecordToUpdate.Fine ?? 0);
                    return View(vmfee);
                }
                else
                {
                    var getFeeRecordToUpdate = r.Fees.Where(s => s.Bill_No == bill).Select(s => s).FirstOrDefault();
                    ViewBag.Message = result;
                    vmfee.feeSummary = getFeeRecordToUpdate.Overall_Fees;
                    vmfee.yearlyFee = getFeeRecordToUpdate;
                    vmfee.yearlyFee.Admission_Fee = decimal.Truncate(getFeeRecordToUpdate.Admission_Fee ?? 0);
                    vmfee.yearlyFee.Tution_Fee = decimal.Truncate(getFeeRecordToUpdate.Tution_Fee ?? 0);
                    vmfee.yearlyFee.Total_Fee = decimal.Truncate(getFeeRecordToUpdate.Total_Fee ?? 0);
                    vmfee.yearlyFee.Registeration_Fee = decimal.Truncate(getFeeRecordToUpdate.Registeration_Fee ?? 0);
                    vmfee.yearlyFee.Exam_Fee = decimal.Truncate(getFeeRecordToUpdate.Exam_Fee ?? 0);
                    vmfee.yearlyFee.Fine = decimal.Truncate(getFeeRecordToUpdate.Fine ?? 0);
                    return View(vmfee);
                }
            }
            catch (Exception)
            {
                var getFeeRecordToUpdate = r.Fees.Where(s => s.Bill_No == bill).Select(s => s).FirstOrDefault();

                ViewBag.Message = "Unable to Update Record!";
                vmfee.feeSummary = getFeeRecordToUpdate.Overall_Fees;
                vmfee.yearlyFee = getFeeRecordToUpdate;
                vmfee.yearlyFee.Admission_Fee = decimal.Truncate(getFeeRecordToUpdate.Admission_Fee ?? 0);
                vmfee.yearlyFee.Tution_Fee = decimal.Truncate(getFeeRecordToUpdate.Tution_Fee ?? 0);
                vmfee.yearlyFee.Total_Fee = decimal.Truncate(getFeeRecordToUpdate.Total_Fee ?? 0);
                vmfee.yearlyFee.Registeration_Fee = decimal.Truncate(getFeeRecordToUpdate.Registeration_Fee ?? 0);
                vmfee.yearlyFee.Exam_Fee = decimal.Truncate(getFeeRecordToUpdate.Exam_Fee ?? 0);
                vmfee.yearlyFee.Fine = decimal.Truncate(getFeeRecordToUpdate.Fine ?? 0);
                return View(vmfee);
            }
            
        }
        public ActionResult DetailFee(string id)
        {
            var getFeeRecordToUpdate = r.Fees.Where(s => s.Bill_No == id).Select(s => s).FirstOrDefault();
            return View(getFeeRecordToUpdate);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFeeRecords(IEnumerable<string> deletefee, string delFee, string hiddenInput)
        {
            if (deletefee != null && hiddenInput != "")
            {
                if (delFee != null)
                {
                    string result = empModel.DeleteFeeRecordsOfStudents(deletefee);

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FeeSummary(IEnumerable<string> deleteRoll,string hiddenInput)
        {
            if (deleteRoll != null && hiddenInput != "")
            { 
                    string result = empModel.DeleteFeeSummaryRecordsOfStudents(deleteRoll);

                    if (result == "OK")
                    {
                        return RedirectToAction("FeeSummary", "Fee", new { res = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!") });
                    }
                    else
                    {
                        return RedirectToAction("FeeSummary", "Fee", new { res = SherlockHolmesEncryptDecrypt.Encrypt(result) });
                    }
                 
            }
            else
            {
                return RedirectToAction("FeeSummary", "Fee", new { res = SherlockHolmesEncryptDecrypt.Encrypt("Plz Select Records To Delete!") });
            }
        }
        public void SessionClearOnReload()
        {
            Session["Success"] = null;
        }
        public JsonResult FeeAddGetSummary(string roll)
        {
            try
            {
                var getFeeSummary = r.Overall_Fees.Where(s => s.RollNo == roll).Select(s => s).FirstOrDefault();

                var getReg = r.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();
                if (getReg.Student_Profile.Status==0)
                {
                    return Json(new string[] { "", "", "", "", "","", "rs2" });
                }

                if (getFeeSummary==null && getReg!=null)
                {
                    return Json(new string[] { "", "0", "0", "", "",
                        getReg.Student_Profile.FirstName+" "+getReg.Student_Profile.LastName, "rs" });
                }
                else if (getFeeSummary==null && getReg==null)
                {
                    return Json(new string[] { "", "", "", "", "","", "rs2" });
                }
                else
                {
                    if (getFeeSummary.Total_Installments==null && getFeeSummary.Paid_Installments==null)
                    {
                        return Json(new string[]{(decimal.Truncate(getFeeSummary.Total_Degree_Fee??0)).ToString(),
                        ((decimal.Truncate(getFeeSummary.SubmittedFee??0)).ToString()),
            (decimal.Truncate((getFeeSummary.RemainingFee??0)).ToString()),
            "",
            ""
            ,getReg.Student_Profile.FirstName+" "+getReg.Student_Profile.LastName,"OK"});   
                    }
                    else
                    {
                        return Json(new string[]{(decimal.Truncate(getFeeSummary.Total_Degree_Fee??0)).ToString(),
                        (decimal.Truncate(getFeeSummary.SubmittedFee??0)).ToString(),
            (decimal.Truncate(getFeeSummary.RemainingFee??0)).ToString(),
             (getFeeSummary.Total_Installments??0) .ToString(),
             (getFeeSummary.Paid_Installments??0).ToString()
            ,getReg.Student_Profile.FirstName+" "+getReg.Student_Profile.LastName,"OK"});   
                    }
                     
                }
            }
            catch (Exception)
            {
                return Json(new string[] { "", "", "", "","","","rs" });
            }
             
        }
    }
}