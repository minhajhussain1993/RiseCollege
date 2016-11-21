using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace FYP_6.Models.Models_Logic
{
    public class CoursesModel
    {
          static RCIS3Entities rc = RCIS3Entities.getinstance();
        public   bool AddCourse(Guid level, Degree_Program deg)
        {
            //int degID = 0;
            try
            {
                //var getDegree = rc.Degree_Program.OrderByDescending(s => s.ProgramID).Select(s => s).FirstOrDefault();
                //if (getDegree != null)
                //{
                //    degID = getDegree.ProgramID;
                //    degID++;
                //}
                //else
                //{
                //    degID++;
                //}
                //deg.ProgramID = degID;
                deg.Level = rc.Levels.Where(s => s.LevelID == level).Select(s => s).FirstOrDefault();
                deg.LevelID = level;
                //var getSameRecords=rc.Degree_Program.Any(s=>s.Degree_ProgramName==deg.Degree_ProgramName);
                if (rc.Degree_Program.Any(s => s.Degree_ProgramName == deg.Degree_ProgramName
                    && s.LevelID == deg.LevelID))
                {
                    return false;
                }
                else
                {
                    deg.ProgramID = Guid.NewGuid();
                    rc.Degree_Program.Add(deg);
                    rc.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }


        }
        public   bool CheckToSeeIfThereExistARecordWithSameDegreeNameAndSubject(Degree_Program deg, IEnumerable<Subject> subj)
        {
            foreach (var item in subj)
            {
                foreach (var item2 in rc.Degree_Subject)
                {
                    if (item.SubjectID == item2.SubjectID && item2.DegreeID == deg.ProgramID)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public   bool DeleteRelatedBPSRecordsOnDeletionOfSubjects(IEnumerable<Degree_Subject> subjDeg)
        {
            try
            {
                foreach (var item in subjDeg)
                {
                    foreach (var item2 in rc.Batch_Subjects_Parts)
                    {
                        if (item.SubjectID == item2.SubjectID && item.DegreeID == item2.Batch.DegreeProgram_ID)
                        {
                            rc.Batch_Subjects_Parts.Remove(item2);
                        }
                    }
                }
                rc.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public   IEnumerable<Degree_Subject> getSubjectsRelatedToDegree(Guid degID)
        {
            var degSubj = rc.Degree_Subject.Where(s => s.DegreeID == degID).OrderBy(s=>s.Part).Select(s => s);
            return degSubj;
        }
        public   IEnumerable<Subject> getSubjectsRelatedToDegreeForEditing()
        {
            var degSubj = rc.Subjects.OrderBy(s => s.SubjectName).Select(s => s);
            return degSubj;
        }


        //Subject Updation in Degree Subjects

        public   string SubjectAddToDegree(IEnumerable<Guid> subj, Guid degID,int part)
        {
                try
                {
                    //int degreeID = int.Parse(degID);

                    //Get List of All the selected Subjects
                    List<Subject> listToDelete = rc.Subjects.Where(s => subj.Contains(s.SubjectID)).ToList();

                    //Get Degree Program Reference
                    Degree_Program deg = rc.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();

                    //Get All the assigned Subjects to DegreeProgram
                    List<Subject> getAllTheSubjectOfTheDegree = rc.Degree_Subject
                        .Where(s => s.DegreeID == degID
                        && s.Part== part).Select(s => s.Subject).ToList();

                     
                    //Get Records from Degree_Subject Table for the particular degree program
                    List<Degree_Subject> getDegreeSubjects = rc.Degree_Subject
                        .Where(s => s.DegreeID == degID
                        && s.Part == part).Select(s => s).ToList();

                    //foreach (var item in subj)
                    //{
                    //    if (getDegreeSubjects.Any(s=>s.SubjectID==item && s.Part==part))
                    //    {
                    //        return "Subject with Name: "+rc.Subjects.Where(s=>s.SubjectID==item).Select(s=>s.SubjectName).FirstOrDefault()??""
                    //            +" already exists in part "+part;
                    //    }
                    //}
                    //Match Subjects That are same in both the new selection and the old subjects in degree
                    var JoinedSubjectInDegreeAndSelectedSubjects = from subjOfDegree in getAllTheSubjectOfTheDegree
                                                                   join selectedSubjects in listToDelete
                                                                   on subjOfDegree.SubjectID equals selectedSubjects.SubjectID
                                                                   orderby selectedSubjects.SubjectID
                                                                   select selectedSubjects;
                           //Get All the Subjects that are different
                    var deleteUnMatchedOrDifferentSubjects =
                        getDegreeSubjects.Where(s => !subj.Contains(s.SubjectID.Value) && s.Part == part)
                        .Select(s=>s);
                          //var deleteUnMatchedOrDifferentSubjects = from selectedSubjects in listToDelete 
                          //                                         from subjOfDegree in getDegreeSubjects 
                          //                                         where(subjOfDegree.SubjectID != selectedSubjects.SubjectID)
                          //                                         select subjOfDegree;

                    if (JoinedSubjectInDegreeAndSelectedSubjects.Count()==0)
                    {
                        foreach (var item in getDegreeSubjects)
                        {
                            rc.Degree_Subject.Remove(item);
                        }
                        rc.SaveChanges();

                        foreach (var item in listToDelete)
                        {
                            rc.Degree_Subject.Add(
                                new Degree_Subject
                            {
                                Degree_Program=deg,
                                DegreeID=deg.ProgramID,
                                Subject=item,
                                SubjectID=item.SubjectID,
                                ID=Guid.NewGuid(),
                                Part=part
                            });
                        }
                        rc.SaveChanges();
                        return "OK";   
                    }
                    else if(JoinedSubjectInDegreeAndSelectedSubjects.Count()==getAllTheSubjectOfTheDegree.Count())
                    {
                        if (AssignNewSubjects(listToDelete, getDegreeSubjects, deg,part))
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
                            rc.Degree_Subject.Remove(item);
                        }
                        rc.SaveChanges();
                        if (AssignNewSubjects(listToDelete, getDegreeSubjects, deg, part))
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

        public   bool AssignNewSubjects(List<Subject> listToDelete, 
            List<Degree_Subject> getDegreeSubjects, Degree_Program deg,int part)
        {
            bool checker = true;

            foreach (var item in listToDelete)
            {
                foreach (var item2 in getDegreeSubjects)
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
                    rc.Degree_Subject.Add(new Degree_Subject
                    {
                        Degree_Program = deg,
                        Subject = rc.Subjects.Where(s => s.SubjectID == item.SubjectID).Select(s => s).FirstOrDefault(),
                        DegreeID = deg.ProgramID,
                        SubjectID = item.SubjectID,
                        ID = Guid.NewGuid(),
                        Part = part
                    });
                    checker = false;
                }
                checker = true;
            }
            rc.SaveChanges();
            return true;
        }
        public   bool DeleteAllSubjects(Degree_Program deg,int part)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    foreach (var item in rc.Degree_Subject)
                    {
                        if (item.DegreeID == deg.ProgramID && item.Part == part)
                        {
                            rc.Degree_Subject.Remove(item);
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

        public   string DeleteAllDegreePrograms(IEnumerable<Guid> deleteCourses)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    List<Degree_Program> listToDelete = rc.Degree_Program.Where(s => deleteCourses.Contains(s.ProgramID)).ToList();
                    foreach (var item in listToDelete)
                    {
                        if (item.Batches.Count>0)
                        {
                            return "Unable to delete Record! The Degree Program "+ item.Degree_ProgramName +" has "+item.Batches.Count+" Batches!";
                        }
                    }

                    foreach (var item in listToDelete)
                    {
                        rc.Degree_Program.Remove(item);                        
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch 
                {
                    t.Dispose();
                    return "Unable To Delete Degree Programs!";
                }
            }
        }

    }
}