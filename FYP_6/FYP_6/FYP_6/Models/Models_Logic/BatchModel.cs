using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Text.RegularExpressions;

namespace FYP_6.Models.Models_Logic
{
    public class BatchModel
    {
        static RCIS2Entities1 rc = RCIS2Entities1.getinstance();

        public static IEnumerable<Batch> getAllBatches(int category)
        {
            var getRecords = rc.Batches.Where(s => s.Status.Value == category).OrderBy(s=>s.BatchName).Select(s => s);
            return getRecords;
        }
        public static string AddBatch(Batch batch, string degree, string section, string year)
        {
            //deg.LevelID = level;
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    if (degree == null || degree.ToString() == "Please select" ||
                            section == null || section.ToString() == "Please select"
                            || year == null)
                    {
                        return "Plz Select All the Fields";
                    }
                    else
                    {
                        if (rc.Batches.Any(s => s.BatchName == batch.BatchName))
                        {
                            return "This BatchName " + batch.BatchName + " Already Exists";
                        }
                        else
                        {
                            if (ValidateBatchName(batch.BatchName))
                            {
                                //int secID = int.Parse(section);
                                //int degID = int.Parse(degree);
                                //int yearID = int.Parse(year);
                                Guid deg = Guid.Parse(degree);
                                Guid sec = Guid.Parse(section);
                                Guid y = Guid.Parse(year);

                                batch.YearID = y;
                                batch.SectionID = sec;
                                batch.DegreeProgram_ID = deg;

                                batch.Degree_Program = rc.Degree_Program.Where(s => s.ProgramID == deg).Select(s => s).FirstOrDefault();
                                batch.Section = rc.Sections.Where(s => s.SectionID == sec).Select(s => s).FirstOrDefault();
                                batch.Year = rc.Years.Where(s => s.YearID == y).Select(s => s).FirstOrDefault();

                                batch.Status = 1;

                                rc.Batches.Add(batch);
                                rc.SaveChanges();
                                

                                var getDegSubjects = (from degsub in rc.Degree_Subject
                                                      where degsub.DegreeID == deg
                                                      select degsub).ToList();


                                foreach (var item in getDegSubjects)
                                {
                                    rc.Batch_Subjects_Parts.Add(new Batch_Subjects_Parts
                                    {
                                        Batch=batch,
                                        Part=item.Part,
                                        ID=Guid.NewGuid(),
                                        BatchName=batch.BatchName,
                                        Subject=item.Subject,
                                        SubjectID=item.SubjectID
                                    });
                                }
                                rc.SaveChanges();
                                t.Complete();
                                return "OK";
                            }
                            else
                            {
                                return "Plz Enter A Numeric Batch Number";
                            }
                        }

                    }
                }
                catch (Exception)
                {
                    return "Unable To Add Batch Record! Plz Check if All Subjects are Assigned OR NOT!";
                }    
            }
            
        }
        private static bool ValidateBatchName(string batch)
        {
            Match m = Regex.Match(batch, "^[0-9]*$");
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //public static int GetNewBatchID()
        //{
        //    int secId = 0;
        //    var getLastSection = rc.Batches.OrderByDescending(s => s.BatchID).Select(s => s).FirstOrDefault();
        //    if (getLastSection != null)
        //    {
        //        secId = getLastSection.BatchID;
        //        secId++;
        //        return secId;
        //    }
        //    else
        //    {
        //        secId++;
        //        return secId;
        //    }
        //}
        //public static int GetNewAssignID()
        //{
        //    int secId = 0;
        //    var getLastSection = rc.Assign_Subject.OrderByDescending(s => s.AssignID).Select(s => s).FirstOrDefault();
        //    if (getLastSection != null)
        //    {
        //        secId = getLastSection.AssignID;
        //        secId++;
        //        return secId;
        //    }
        //    else
        //    {
        //        secId++;
        //        return secId;
        //    }
        //}
        public static IEnumerable<Batch> getAllSearchSpecificBatches(string search)
        {
            if (search == "" || search == null)
            {
                return null;
            }
            else
            {
                var getBatchRecords = rc.Batches.Where(s => s.BatchName == search).Select(s => s).OrderBy(s => s.BatchName);
                return getBatchRecords;
            }
        }
        public static Batch UpdateBatchRec(Batch batchRec, Guid degID, Guid secID, Guid yearID)
        {
            var getRequestedBatchRecord = rc.Batches.Where(s => s.BatchName == batchRec.BatchName).Select(s => s).FirstOrDefault();
            try
            {
                if (getRequestedBatchRecord != null)
                {
                    getRequestedBatchRecord.DegreeProgram_ID = degID;
                    getRequestedBatchRecord.SectionID = secID;
                    getRequestedBatchRecord.YearID = yearID;
                    return getRequestedBatchRecord;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        public static string NewBatchFinalRegister(Batch bat, Batch_Subjects_Parts bps, IEnumerable<string> subj)
        {
            List<Batch_Subjects_Parts> assignSub = new List<Batch_Subjects_Parts>();
            try
            {
                rc.Batches.Add(bat);
                rc.SaveChanges();
                //int bpsID = GetBatchSubjPart();
                var getRelatedSubject = rc.Batch_Subjects_Parts.Where(s => subj.Contains(s.SubjectID.ToString())).ToList();

                foreach (var item in getRelatedSubject)
                {
                    assignSub.Add(new Batch_Subjects_Parts
                    {
                        BatchName = item.BatchName,
                        Part = item.Part,
                        Part1=item.Part1,
                        SubjectID = item.SubjectID,
                        ID = Guid.NewGuid()
                    });
                    //bpsID++;
                }
                foreach (var item in assignSub)
                {
                    rc.Batch_Subjects_Parts.Add(item);
                }
                rc.SaveChanges();
                return "Successfully Record Added";
            }
            catch (Exception e)
            {
                return "Unable to Register Student" + " " + e.Message;
            }

        }

        //private static int GetBatchSubjPart()
        //{
        //    int bpsID = 0;
        //    var getLastSubjecID = rc.Batch_Subjects_Parts.OrderByDescending(s => s.ID).Select(s => s).FirstOrDefault();
        //    if (getLastSubjecID == null)
        //    {
        //        bpsID++;
        //        return bpsID;
        //    }
        //    else
        //    {
        //        bpsID = getLastSubjecID.ID;
        //        bpsID++;
        //        return bpsID;
        //    }
        //}
        public static List<Registeration> GetAllRelevantBatchStudents(string batch)
        {
            var getStudents = rc.Registerations.Where(s => s.BatchID == batch).OrderBy(s => s.Rollno).Select(s => s).ToList();
            return getStudents;
        }

        public static string DeleteBatchesRecords_BatchModelFunc(IEnumerable<Guid> deleteBatch)
        {
            if (deleteBatch != null)
            {
                using (TransactionScope t = new TransactionScope())
                {
                    try
                    {
                        //Get Batches Objects For Selected Batch IDs
                        var getBatchesRef = rc.Batches
                            .Where(s => deleteBatch.Contains(s.BatchID))
                            .Select(s => s);

                        //Get Students of the selected Batch
                        var getStudentProfilesForThatBatch = rc.Registerations.
                            Where(s => deleteBatch.Contains(s.Batch.BatchID))
                            .Select(s => s.Student_Profile);

                        foreach (var item in getStudentProfilesForThatBatch)
                        {
                            rc.Student_Profile.Remove(item);
                        }
                        foreach (var item in getBatchesRef)
                        {
                            rc.Batches.Remove(item);
                        }
                        rc.SaveChanges();
                        t.Complete();
                        return "OK";
                    }
                    catch (Exception)
                    {
                        return "Unable To Delete Batches Records ";
                    }
                }
            }
            else
            {
                return "Plz Select Records to Delete!";
            }
        }

        //On New Subject Assignment to Batch Assign All the Students that Subject
        public static string AddBatch_Subj_PartRec(Guid degree, string batch, Guid section, Guid subjects, int part
            , List<Registeration> studentsSpecificToABatch)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    //int subjID = int.Parse(subjects);
                    //int sectionID = int.Parse(section);
                    //int Part = int.Parse(part);
                    //int degID = int.Parse(degree);
                    var getDegreeName = rc.Degree_Program.Where(s => s.ProgramID == degree).Select(s => s.Degree_ProgramName).FirstOrDefault();
                    var getSectionName = rc.Sections.Where(s => s.SectionID == section).Select(s => s.SectionName).FirstOrDefault();
                    var getSubjName = rc.Subjects.Where(s => s.SubjectID == subjects).Select(s => s.SubjectName).FirstOrDefault();


                    if (rc.Batch_Subjects_Parts.Any(s => s.Batch.BatchName == batch
                        && s.Batch.SectionID == section
                        && s.Batch.DegreeProgram_ID == degree
                        && s.Part == part
                        && s.SubjectID == subjects))
                    {
                        return "Batch "+batch+" with DegreeProgram "+getDegreeName+" has already assigned Subject "+getSubjName+" in Part "+part;
                    }
                    if (rc.Batches.Any(s => s.BatchName == batch
                        && s.SectionID == section
                        && s.DegreeProgram_ID == degree))
                    {
                        //Add New Subject To Batch
                        //int getLastID = GetBatchSubjPart();
                        Batch_Subjects_Parts bps = new Batch_Subjects_Parts();
                        bps.ID = Guid.NewGuid();
                        bps.Part = part;
                        bps.BatchName = batch;
                        bps.SubjectID = subjects;
                        rc.Batch_Subjects_Parts.Add(bps);
                        rc.SaveChanges();

                        //Check to see if there is any batch student who is studing in that part
                        if (rc.Assign_Subject.Any(s => s.Batch_Subjects_Parts.BatchName == batch
                        && s.Batch_Subjects_Parts.Batch.SectionID == section
                        && s.Batch_Subjects_Parts.Batch.DegreeProgram_ID == degree
                        && s.Batch_Subjects_Parts.Part == part)) 
                        {
                            ////Check To see on new Subject Assigned To Batch,
                            ////The Students that are not assigned that Subject,
                            ////should be assigned to that subject
                            bool checkerOnMatchedStudentThatisNotStudingThatParticularSubject = false;
                            //int AssignID = GetNewAssignID();

                            foreach (var item in studentsSpecificToABatch)
                            {
                                foreach (var item2 in item.Assign_Subject)
                                {
                                    if (item2.Batch_Subjects_Parts.BatchName == batch
                                        && item2.Batch_Subjects_Parts.Batch.SectionID == section 
                                        && item2.Batch_Subjects_Parts.Batch.DegreeProgram_ID == degree 
                                        && item2.Batch_Subjects_Parts.Part == part
                                        && item2.Batch_Subjects_Parts.SubjectID == subjects
                                        && item.Status == 1)
                                    {
                                        checkerOnMatchedStudentThatisNotStudingThatParticularSubject = true;
                                    }
                                    if (checkerOnMatchedStudentThatisNotStudingThatParticularSubject == true)
                                    {
                                        break;
                                    }
                                }

                                if (checkerOnMatchedStudentThatisNotStudingThatParticularSubject == false)
                                {
                                    rc.Assign_Subject.Add(new Assign_Subject
                                    {
                                        Rollno = item.Rollno,
                                        Batch_Subject_ID = bps.ID,
                                        Status = "Active",
                                        Registeration=item
                                    });
                                    rc.SaveChanges();
                                }
                                else
                                {
                                    checkerOnMatchedStudentThatisNotStudingThatParticularSubject = false;
                                }
                            }
                            rc.SaveChanges();
                            t.Complete();
                            return "OK";
                            //return "The Record with DegreeName: " + getDegreeName + ", Section: " + getSectionName + " Part: " + part + " ,Subject: " + getSubjName + " is Successfully added";    
                        }
                        else
                        {
                            //No Worries Subjects are still added in batch only 
                            //not To all the students
                            rc.SaveChanges();
                            t.Complete();
                            return "OK";
                        }
                        
                    }
                    else
                    {
                        return "Plz Enter Correct Batch, Section, DegreeProgram Values";
                    }

                }
                catch 
                {
                    return "Unable To Assign Subject To Batch "+batch;
                }
            }
        }

        //On Part Change Assign All The Subjects of that Part To The Specific Batch
        public static string UpdateBatch_Subj_PartRec(Guid degree, string batch, Guid section, int part)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    bool checkerForAlreadyPartPresentInAssignedSubjects = false;
                    var getDegreeName = rc.Degree_Program.Where(s => s.ProgramID == degree).Select(s => s.Degree_ProgramName).FirstOrDefault();
                    var getSectionName = rc.Sections.Where(s => s.SectionID == section).Select(s => s.SectionName).FirstOrDefault();

                    //Check For Valid Entry of Batch, Section and Degree in database
                    if (rc.Batches.Any(s => s.BatchName == batch
                        && s.SectionID == section
                        && s.DegreeProgram_ID == degree))
                    {
                        //Check if Students have already studied that part
                        var checkForAPIAS = rc.Assign_Subject.Where(s => s.Batch_Subjects_Parts.Part == part
                            && s.Batch_Subjects_Parts.BatchName == batch).Select(s => s).ToList();

                        if (checkForAPIAS.Count > 0)
                        {
                            checkerForAlreadyPartPresentInAssignedSubjects = true;
                        }
                        else
                        {
                            checkerForAlreadyPartPresentInAssignedSubjects = false;
                        }
                    }
                    else
                    {
                        return "No Record Found Related to Degree " + getDegreeName + " And BatchName " + batch;
                    }
                    if (checkerForAlreadyPartPresentInAssignedSubjects == false)
                    {
                        //Get All Students 
                        var getRelStd = rc.Registerations.Where(
                            s => s.Batch.BatchName == batch
                            && s.Part != part
                            && s.Batch.SectionID == section
                            && s.Batch.DegreeProgram_ID == degree
                            && s.Status == 1);
                     
                        //Change Students Part Here
                        foreach (var item2 in getRelStd)
                        {
                            item2.Part = part;
                        }
                        
                        //Check For Already Assigned Subjects in That Part
                        bool checkerForAlreadyAssignedSubjects = false;
                        var JoiningBPSANDAssignSubTables = (from bps in rc.Batch_Subjects_Parts
                                                            join assignS in rc.Assign_Subject
                                                             on bps.ID equals assignS.Batch_Subject_ID
                                                            where (assignS.Batch_Subjects_Parts.Part == part
                                                            && assignS.Batch_Subjects_Parts.BatchName == batch
                                                            && assignS.Batch_Subjects_Parts.Batch.SectionID == section
                                                            && degree == assignS.Batch_Subjects_Parts.Batch.DegreeProgram_ID
                                                            && bps.SubjectID == assignS.Batch_Subjects_Parts.SubjectID
                                                            && assignS.Status == "Active")
                                                            select bps).ToList();
                        if (JoiningBPSANDAssignSubTables.Count > 0)
                        {
                            checkerForAlreadyAssignedSubjects = true;
                        }
                        else
                        {
                            checkerForAlreadyAssignedSubjects = false;
                        }
                        //Assign Subjects here if Students have not Already taken That Subjects
                        if (checkerForAlreadyAssignedSubjects == false)
                        {
                        //    int AssignIDNew = GetNewAssignID();
                            foreach (var item1 in rc.Batch_Subjects_Parts)
                            {
                                foreach (var item2 in getRelStd)
                                {
                                    if (item1.Part == part && item1.BatchName == batch
                                        && item1.Batch.SectionID == section
                                        && item1.Batch.DegreeProgram_ID == degree
                                        && item2.Status == 1)
                                    {
                                        item2.Assign_Subject.Add(new Assign_Subject
                                        {
                                            Batch_Subject_ID = item1.ID,
                                            Rollno = item2.Rollno,
                                            Status = "Active",
                                            AssignID = Guid.NewGuid()
                                        });
                                        //AssignIDNew++;
                                    }
                                }
                            }
                            rc.SaveChanges();
                            t.Complete();
                            return "OK";
                        }
                        else
                        {
                            rc.SaveChanges();
                            //No Subject Assignment if no subjects with that part are present
                            t.Complete();
                            return "OKNoSub";
                        }

                    }
                    else
                    {
                        return "Students of Section " + getSectionName + " with BatchName " + batch + " Have already studied Part " + part;
                    }
                }
                catch
                {
                    return "exception";
                }   
            }
        }

        //Update Subjects in Batch_Subjects_Parts Table
        public static string SubjectAddToBPS(IEnumerable<Guid> subj, Batch batch,int part)
        {
            try
            {
                //Get List of All the selected Subjects
                List<Subject> CompleteSubjList = rc.Subjects.Where(s => subj.Contains(s.SubjectID)).ToList();                

                //Get All the assigned Subjects to DegreeProgram
                List<Subject> getAllTheSubjectOfBatch = rc.Batch_Subjects_Parts
                    .Where(s => s.BatchName== batch.BatchName
                    && s.Part==part).Select(s => s.Subject).ToList();

                //Get Records from Degree_Subject Table for the particular degree program
                List<Batch_Subjects_Parts> getBatchSubjects = rc.Batch_Subjects_Parts.
                    Where(s => s.BatchName == batch.BatchName
                    && s.Part == part).Select(s => s).ToList();


                //Match Subjects That are same in both the new selection and the old subjects in degree
                var JoinedSubjectInDegreeAndSelectedSubjects = from subjOfDegree in getAllTheSubjectOfBatch
                                                               join selectedSubjects in CompleteSubjList
                                                               on subjOfDegree.SubjectID equals selectedSubjects.SubjectID
                                                               orderby selectedSubjects.SubjectID
                                                               select selectedSubjects;
                //Get All the Subjects that are different
                var deleteUnMatchedOrDifferentSubjects =
                    getBatchSubjects.Where(s => !subj.Contains(s.SubjectID.Value) &&s.Part==part)
                    .Select(s => s);

                if (JoinedSubjectInDegreeAndSelectedSubjects.Count() == 0)
                {
                    //If there are no subjects matched then delete previous subjects of batch of specific part
                    foreach (var item in getBatchSubjects)
                    {
                        rc.Batch_Subjects_Parts.Remove(item);
                    }
                    rc.SaveChanges();
                    //Add the new subjects
                    foreach (var item in CompleteSubjList)
                    {
                        rc.Batch_Subjects_Parts.Add(
                            new Batch_Subjects_Parts
                            {
                                Batch=batch,
                                BatchName = batch.BatchName,
                                Part=part,
                                SubjectID=item.SubjectID,
                                Subject=item,
                                ID=Guid.NewGuid()
                            });
                    }
                    rc.SaveChanges();
                    return "OK";
                }
                else if (JoinedSubjectInDegreeAndSelectedSubjects.Count() == getAllTheSubjectOfBatch.Count())
                {
                    //if the same subjects are selected plus there are also other subjects that are selected Or no subject is selected
                    //Here assigning new subjects to already assigned subjects in the batch
                    if (AssignNewSubjectsToBPS(CompleteSubjList, getBatchSubjects, batch, part))
                    {
                        return "OK";
                    }
                    else
                    {
                        return "Unable to Update Subjects!";
                    }
                }
                else
                {
                    //Remove previous subjects that are different
                    foreach (var item in deleteUnMatchedOrDifferentSubjects)
                    {
                        rc.Batch_Subjects_Parts.Remove(item);
                    }
                    rc.SaveChanges();
                    //Assign New subjects but keep the old ones intact
                    if (AssignNewSubjectsToBPS(CompleteSubjList, getBatchSubjects, batch, part))
                    {
                        return "OK";
                    }
                    else
                    {
                        return "Unable to Update Subjects!";
                    }
                }
                #region Old Code
                //    bool checker = true;

                //    foreach (var item in getAllTheSubjectOfTheDegree)
                //    {
                //        foreach (var item2 in listToDelete)
                //        {
                //            //Check If the Subject is already assigned
                //            if (item.SubjectID == item2.SubjectID)
                //            {
                //                checker = false;
                //                break;
                //            }
                //        }

                //        if (checker == true)
                //        {
                //            //If not Assign The subject
                //            rc.Degree_Subject.Add(new Degree_Subject
                //            {
                //                Degree_Program = deg,
                //                Subject = rc.Subjects.Where(s => s.SubjectID == item.SubjectID).Select(s => s).FirstOrDefault(),
                //                DegreeID = degreeID,
                //                SubjectID = item.SubjectID
                //            });
                //            checker = false;
                //        }
                //    }
                //    rc.SaveChanges();
                //    t.Complete();
                //    return "OK";
                //}
                #endregion
            }
            catch
            {
                return "Unable to Update Subjects!";
            }

        }

        public static bool AssignNewSubjectsToBPS(List<Subject> CompleteSubjList,
            List<Batch_Subjects_Parts> getBPSSubjects, Batch batchForEditing,int part)
        {
            bool checker = true;

            foreach (var item in CompleteSubjList)
            {
                foreach (var item2 in getBPSSubjects)
                {
                    //Check If the Subject is already assigned
                    if (item.SubjectID == item2.SubjectID)
                    {
                        checker = false;
                        break;
                    }
                }

                if (checker == true)
                {
                    //If not Assign The subject
                    rc.Batch_Subjects_Parts.Add(
                            new Batch_Subjects_Parts
                            {
                                Batch = batchForEditing,
                                BatchName = batchForEditing.BatchName,
                                Part = part,
                                SubjectID = item.SubjectID,
                                Subject = item,
                                ID=Guid.NewGuid()
                            });
                    checker = false;
                }
                checker = true;
            }
            rc.SaveChanges();
            return true;
        }
        public static bool DeleteAllSubjectsToBPS(Batch batch ,int part)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    foreach (var item in rc.Batch_Subjects_Parts)
                    {
                        if (item.BatchName == batch.BatchName && item.Part==part)
                        {
                            rc.Batch_Subjects_Parts.Remove(item);
                        }
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static IEnumerable<Degree_Subject> getSubjectsForEditing(Batch batch)
        {
            var degSubj = rc.Degree_Subject.Where(s=>s.DegreeID==batch.DegreeProgram_ID).OrderBy(s => s.SubjectID).Select(s => s);
            return degSubj;
        }
    }
}