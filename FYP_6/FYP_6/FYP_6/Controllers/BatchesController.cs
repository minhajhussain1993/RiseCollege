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
    [SessionExpire]
    public class BatchesController : Controller
    {
        RCIS2Entities1 r = RCIS2Entities1.getinstance();
        // GET: Batches
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewBatches(int? page, string delFunResult)
        {
            SessionClearOnReload();
            if (delFunResult!=null)
            {
                ViewBag.Message = SherlockHolmesEncryptDecrypt.Decrypt(delFunResult);
            }
            var getAllBatchRecords = r.Batches.Where(s => s.Status == 1).Select(s => s).OrderBy(s => s.BatchName);
            return View("ViewBatches", getAllBatchRecords.ToPagedList(page ?? 1, 10));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewBatches(int? page, int category)
        {
            SessionClearOnReload();
            var getAllBatchRecords = BatchModel.getAllBatches(category);
            return View("ViewBatches", getAllBatchRecords.ToPagedList(page ?? 1, 10));
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ViewBatches(int? page)
        //{
        //    if (Session["UserName"] != null)
        //    {
        //        var getAllBatchRecords = r.Batches.Select(s => s).OrderBy(s => s.BatchName);
        //        return View("ViewBatches", getAllBatchRecords.ToPagedList(page ?? 1, 10));
        //    }
        //    else
        //    {
        //        return RedirectToAction("login", "Home");
        //    }

        //}
        [HttpGet]
        public ActionResult AddBatchRecords()
        {

            if (Session["Success"] == "Successfully Record Added")
            {
                return View();
            }
            else
            {
                var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
                var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
                var getAllYears = r.Years.OrderBy(s => s.YearID).Select(s => s);
                ViewBag.Degrees = getAllDegrees;
                ViewBag.Sections = getAllSections;
                ViewBag.Years = getAllYears;
                ViewBag.Message = "";
                return View();
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBatchRecords(Batch BatchRec, string degree, string section, string newBatch, string year)
        {
            try
            {
                var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
                var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
                var getAllYears = r.Years.OrderBy(s => s.YearID).Select(s => s);

                string result = BatchModel.AddBatch(BatchRec, degree, section, year);
                if (result == "OK")
                {
                    ViewBag.Degrees = getAllDegrees;
                    ViewBag.Sections = getAllSections;
                    ViewBag.Years = getAllYears;
                    ViewBag.Message = "Successfully Record Added";
                    return View();
                }
                else
                {
                    ViewBag.Degrees = getAllDegrees;
                    ViewBag.Sections = getAllSections;
                    ViewBag.Years = getAllYears;
                    ViewBag.Message = "Error! " + result;
                    return View();
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ViewBatches");
            }
            

        }

        //public ActionResult Batch_Assign_Subjects_AddBatch()
        //{
        //    if (Session["UserName"] != null)
        //    {
        //        Batch bat = GetBatch();
        //        var getBatchSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == bat.BatchName).OrderBy(s => s.ID);
        //        return View(getBatchSubjects);
        //    }
        //    else
        //    {
        //        return RedirectToAction("login", "Home");
        //    }
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Batch_Assign_Subjects_AddBatch(IEnumerable<string> subj,string newBatch)
        //{
        //    if (Session["UserName"] != null)
        //    {
        //        if (newBatch != null)
        //        {
        //            Session["Success"] = "";
        //            return RedirectToAction("AddBatchRecords","Batches");
        //        }
        //        else
        //        {
        //            Batch bat = GetBatch();
        //            Batch_Subjects_Parts bsp = GetSubjectsInAddBatch();
        //            if (subj != null)
        //            {
        //                string result = BatchModel.NewBatchFinalRegister(bat, bsp, subj);
        //                if (result == "Successfully Record Added")
        //                {
        //                    ViewBag.Message = "Successfully Record Added";
        //                    Session["BatchAdd"] = null;
        //                    Session["subjBatch"] = null;
        //                    RemoveBatch();
        //                    RemoveSubjectsInAddBatch();
        //                    Session["Success"] = "Successfully Record Added";
        //                    return RedirectToAction("AddBatchRecords", "Batches");
        //                }
        //                else
        //                {
        //                    ViewBag.Message = "Unable to Add Records";
        //                    var getBatchSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == bat.BatchName).OrderBy(s => s.ID);
        //                    Session["Success"] = "";
        //                    return View(getBatchSubjects);
        //                }

        //            }
        //            else
        //            {
        //                ViewBag.Message = "Unable to Add Records! Plz Select At Least 1 Subject";
        //                var getBatchSubjects = r.Batch_Subjects_Parts.Where(s => s.BatchName == bat.BatchName).OrderBy(s => s.ID);
        //                Session["Success"] = "";
        //                return View(getBatchSubjects);
        //            }
        //        }
        //    }


        //    else
        //    {
        //        return RedirectToAction("login", "Home");
        //    }

        //}

        public ActionResult EditBatch(string id)
        {
            var getBatchRecordToUpdate = r.Batch_Subjects_Parts.Where(s => s.BatchName == id).Select(s => s);
            if (getBatchRecordToUpdate==null)
            {
                return RedirectToAction("ViewBatches");
            }
            return View(getBatchRecordToUpdate);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBatch(IEnumerable<Guid> BatchID)
        {

            //var getFeeRecordToUpdate = r.Batches.Where(s => s.BatchName == batchRec.BatchName).Select(s => s).FirstOrDefault();
            if (BatchID != null)
            {
                List<Batch_Subjects_Parts> listToDelete = r.Batch_Subjects_Parts.Where(s => BatchID.Contains(s.ID)).ToList();

                string getBatchName = listToDelete.OrderBy(s => s.BatchName).Select(s => s.BatchName).FirstOrDefault();
                var getRecordOfBatch = r.Batch_Subjects_Parts.Where(s => s.BatchName == getBatchName).Select(s => s);

                foreach (var item in listToDelete)
                {
                    r.Batch_Subjects_Parts.Remove(item);
                }
                r.SaveChanges();
                ViewBag.Message = "Successfully Records Deleted";
                return View(getRecordOfBatch);
            }
            else
            {
                //ViewBag.Message = "Plz Select Records To be Deleted";
                return RedirectToAction("ViewBatches", "Batches");
            }

        }
        public ActionResult DetailBatch(string id)
        {
            var getBatchRecordToView = r.Batches.Where(s => s.BatchName == id).Select(s => s).FirstOrDefault();
            if (getBatchRecordToView==null)
            {
                return RedirectToAction("ViewBatches");
            }
            return View(getBatchRecordToView);
        }
        [HttpPost]
        public ActionResult DeleteBatchesRecords(IEnumerable<Guid> deletebatch, string hiddenInput)
        {
            if (deletebatch!=null &&hiddenInput!="")
            {
                string result = BatchModel.DeleteBatchesRecords_BatchModelFunc(deletebatch);

                if (result == "OK")
                {
                    string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Successfully Records Deleted!");
                    return RedirectToAction("ViewBatches", "Batches", new { delFunResult = outputMessage });
                }
                else
                {
                    string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Unable To Delete Records!");
                    return RedirectToAction("ViewBatches", "Batches", new { delFunResult = outputMessage });
                }   
            }
            else
            {
                string outputMessage = SherlockHolmesEncryptDecrypt.Encrypt("Unable To Delete Records!");
                return RedirectToAction("ViewBatches", "Batches", new { delFunResult = outputMessage });
            }
        }

        #region Old Batch Subject Assign Code
        //public ActionResult AssignSubjects()
        //{

        //    var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    //var SectionsOfTeacher = r.Sections.Select(s => s).OrderBy(s => s.SectionID);
        //    var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
        //    ViewBag.Degrees = ListofDegreePrograms;
        //    //ViewBag.Sections = SectionsOfTeacher;
        //    ViewBag.Subjects = getAllSubjects;
        //    return View();


        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AssignSubjects(Guid degree, Guid subjects, Guid section, string batch, int part)
        //{
        //    var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
        //    //var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
        //    var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);
            
        //    if (part != null && (degree != null && degree.ToString() != "Please select")
        //        && subjects != null && (section != null && section.ToString() != "Please select")
        //        && (batch != null && batch != "Please select"))
        //    {
        //        List<Registeration> studentsSpecificToABatch = BatchModel.GetAllRelevantBatchStudents(batch);
        //        string result = BatchModel.AddBatch_Subj_PartRec(degree, batch, section, subjects, part, studentsSpecificToABatch);
        //        if (result == "OK")
        //        {
        //            ViewBag.Message = "Successfully Subject Assigned!";
        //            ViewBag.Degrees = getAllDegrees;
        //            //ViewBag.Sections = getAllSections;
        //            ViewBag.Subjects = getAllSubjects;
        //            return View();
        //        }
        //        else
        //        {
        //            ViewBag.Message = result;
        //            ViewBag.Degrees = getAllDegrees;
        //            //ViewBag.Sections = getAllSections;
        //            ViewBag.Subjects = getAllSubjects;
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Plz Fill All The Fields";
        //        ViewBag.Degrees = getAllDegrees;
        //        //ViewBag.Sections = getAllSections;
        //        ViewBag.Subjects = getAllSubjects;
        //        return View();
        //    }

        //}

        #endregion

        #region Old Subject Updation Code
        //public ActionResult EditBatchSubjects(string id)
        //{
        //    var getBatch = r.Batches.Where(s => s.BatchName == id).Select(s => s).FirstOrDefault();
        //    if (getBatch == null)
        //    {
        //        return RedirectToAction("ViewBatches");
        //    }
        //    var specificLevel = r.Levels.Where(s => s.LevelID == getDegreeProgram.LevelID).Select(s => s.Level_Name).FirstOrDefault();
        //    IEnumerable<Degree_Subject> ds = BatchModel.getSubjectsForEditing(getBatch);
        //    ViewBag.level = specificLevel;
        //    ViewBag.Batch = getBatch;

        //    return View(ds);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditBatchSubjects(IEnumerable<Guid> subjects,
        //    string batch, int part)
        //{
        //    var getBatch = r.Batches.Where(s => s.BatchName == batch).Select(s => s).FirstOrDefault();

        //    if (subjects != null)
        //    {
        //        string result = BatchModel.SubjectAddToBPS(subjects, getBatch, part);

        //        if (result == "OK")
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.Message = "Successfully Records Updated";
        //            IEnumerable<Degree_Subject> ds = BatchModel.getSubjectsForEditing(getBatch);
        //            return View(ds);
        //        }
        //        else
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.Message = result;
        //            IEnumerable<Degree_Subject> ds = BatchModel.getSubjectsForEditing(getBatch);
        //            return View(ds);
        //        }
        //    }
        //    else
        //    {
        //        if (BatchModel.DeleteAllSubjectsToBPS(getBatch, part))
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.Message = "Successfully Records Updated";
        //            IEnumerable<Degree_Subject> ds = BatchModel.getSubjectsForEditing(getBatch);
        //            return View(ds);
        //        }
        //        else
        //        {
        //            ViewBag.Batch = getBatch;
        //            ViewBag.Message = "Unable to Update Subjects";
        //            IEnumerable<Degree_Subject> ds = BatchModel.getSubjectsForEditing(getBatch);
        //            return View(ds);
        //        }
        //    }
        //}

        #endregion
        public ActionResult DetailSubjectsInBatch(string id)
        {
            var getBatchRecordToUpdate = r.Batch_Subjects_Parts.
                Where(s => s.BatchName == id).OrderBy(s=>s.Part).Select(s => s);
            if (getBatchRecordToUpdate==null)
            {
                return RedirectToAction("ViewBatches");
            }
            ViewBag.BatchName = id;
            ViewBag.DegreeProgram = r.Batches.Where(s=>s.BatchName==id).Select(s=>s.Degree_Program.Degree_ProgramName).FirstOrDefault();
            ViewBag.Section = r.Batches.Where(s => s.BatchName == id).Select(s => s.Section.SectionName).FirstOrDefault();
            return View(getBatchRecordToUpdate);
        }

        
        [HttpGet]
        public ActionResult EditBatchReg()
        {
            var ListofDegreePrograms = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var SectionsOfTeacher = r.Sections.Select(s => s).OrderBy(s => s.SectionID);
            var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);

            //var getBatchRecord = r.Batches.Where(s => s.BatchName == id).Select(s => s).FirstOrDefault();
            //if (getBatchRecord==null)
            //{
            //    return RedirectToAction("ViewBatches");
            //}

            //ViewBag.PartStd = r.Registerations.Where(s => s.BatchID == id).Select(s => s.Part).FirstOrDefault();

            ViewBag.Degrees = ListofDegreePrograms;
            //ViewBag.Sections = SectionsOfTeacher;
            ViewBag.Subjects = getAllSubjects;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBatchReg(string degree, string section, string batch, int part, string SubjectRegToPartChange)
        {
            var getAllDegrees = r.Degree_Program.OrderBy(s => s.ProgramID).Select(s => s);
            //var getAllSections = r.Sections.OrderBy(s => s.SectionID).Select(s => s);
            var getAllSubjects = r.Subjects.OrderBy(s => s.SubjectID).Select(s => s);

            if (part != null && (degree != null && degree.ToString() != "Please select")
                && (section != null && section.ToString() != "Please select")
                && (batch != null && batch != "Please select"))
            {
                Guid degID = Guid.Parse(degree);
                //Guid Part = Guid.Parse(part);
                Guid secID = Guid.Parse(section);

                string result = BatchModel.UpdateBatch_Subj_PartRec(degID, batch, secID, part);

                var getDegreeName = r.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s.Degree_ProgramName).FirstOrDefault();
                if (result == "OK")
                {
                    ViewBag.Message = "Batch " + batch + "with Degree Program " + getDegreeName + " has changed its part to:" + part + " All the Subjects were Assigned to part " + part;
                    ViewBag.Degrees = getAllDegrees;
                    TempData["S"] = "1";
                    //ViewBag.Message = "Sucessfully Part Changed";

                    //ViewBag.Sections = getAllSections;
                    ViewBag.Subjects = getAllSubjects;
                    //var getSubjectofBPS = r.Batch_Subjects_Parts.Where(s => s.BatchName == batch && s.Part == Part).Select(s => s);
                    return View();
                }
                else if (result == "OKNoSub")
                {
                    ViewBag.Message = "Batch " + batch + "with Degree Program " + getDegreeName + " has changed its part to:" + part + ", No Subjects Were Assigned to Part" + part;
                    ViewBag.Degrees = getAllDegrees;
                    TempData["S"] = "0";
                    //ViewBag.Message = "Sucessfully Part Changed";

                    //ViewBag.Sections = getAllSections;
                    ViewBag.Subjects = getAllSubjects;
                    //var getSubjectofBPS = r.Batch_Subjects_Parts.Where(s => s.BatchName == batch && s.Part == Part).Select(s => s);
                    return View();
                }
                else
                {
                    ViewBag.Message = result;
                    ViewBag.Degrees = getAllDegrees;
                    //ViewBag.Sections = getAllSections;
                    ViewBag.Subjects = getAllSubjects;
                    TempData["S"] = "2";
                    return View();
                }


            }
            else
            {
                ViewBag.Message = "Plz Fill All The Fields";
                ViewBag.Degrees = getAllDegrees;
                //ViewBag.Sections = getAllSections;
                ViewBag.Subjects = getAllSubjects;
                return View();
            }
        }

        public void SessionClearOnReload()
        {
            Session["Success"] = null;
        }

    }
}