using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using FYP_6.Models.ViewModels;
using System.Transactions;
using FYP_6.Models.Report_Models;

namespace FYP_6.Models.Models_Logic
{
    public class TeacherModel
    {
        private string[] MonthsNames ={"","January","February","March","April","May","June","July",
                                         "August","September","October","November","December"};
        private int[] MonthsNamesInInts = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        static RCIS3Entities rc = RCIS3Entities.getinstance();
        public string[] WelcomeTeacherScreenResults(string id)
        {
            string[] mainScreenItems = new string[5];
            //int t_id = int.Parse(id);
            var subjectsTaught = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == id).Select(s => s.Subject).Distinct().ToList();
            var teacherInfo = rc.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
            int noOfSubjects = 0;

            foreach (var items in subjectsTaught)
            {
                noOfSubjects++;
            }
            if (teacherInfo != null)
            {
                mainScreenItems[0] = teacherInfo.Graduation_Degree_Level + " " + teacherInfo.Graduation_Degree_Name;
                //mainScreenItems[0] = teacherInfo.Graduation_Details;
                mainScreenItems[1] = (teacherInfo.Major_Subject);
                mainScreenItems[2] = (teacherInfo.Status.ToString());
            }
            if (noOfSubjects > 0 && subjectsTaught != null)
            {
                mainScreenItems[3] = (noOfSubjects.ToString());
            }

            return mainScreenItems;
        }
        public string ChangePassword_TeacherModelFunction(string oldpass, string newpass, string id)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    var getStudent = rc.Teachers.Where(s => s.TeacherID == id).Select(s => s).FirstOrDefault();
                    string getUserPassword = rc.Teachers.Where(s => s.TeacherID == id).Select(s => s.Password).FirstOrDefault();

                    string getPasswordChangeResult = ValidatePassword(oldpass, newpass, getUserPassword);

                    if (getPasswordChangeResult == "")
                    {
                        getStudent.Password = newpass;
                        rc.SaveChanges();
                        t.Complete();
                        return "Password Changed";
                    }
                    else
                    {
                        return getPasswordChangeResult;
                    }
                }
                catch (Exception)
                {
                    return "Unable to change Password";
                }
            }
        }
        private string ValidatePassword(string oldentered, string newpass, string actualPassword)
        {
            if (oldentered.Length <= 30)
            {
                if (newpass.Length <= 30 && newpass.Length >= 5)
                {
                    if (oldentered == null || oldentered == "")
                    {
                        return "Plz Fill All The Fields!";
                    }
                    else if (newpass == null || newpass == "")
                    {
                        return "Plz Fill All The Fields!";
                    }
                    else if (oldentered == actualPassword)
                    {
                        return "";
                    }
                    else
                    {
                        return "Old Password is Incorrect";
                    }
                }
                else
                {
                    return "New Password must be 5 to 30 characters long";
                }
            }
            else
            {
                return "Old Password must be less than 30 characters";
            }
        }


        public IEnumerable<Student_Marks> getResultRecords(string teacherID)
        {
            List<Student_Marks> stMarksRec = new List<Student_Marks>();
            //string t_id = teacherID.ToString();
            
            //var getSpecificRecordsOfTeacher = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == teacherID).Select(s => s);

            //var stMarksRec = from stdAttRec in rc.Student_Marks
            //                 from recordsOfTeacher in getSpecificRecordsOfTeacher

            //                 where (stdAttRec.Assign_Subject.Batch_Subjects_Parts.PartID == recordsOfTeacher.Batch_Subjects_Parts.PartID
            //                 && stdAttRec.Assign_Subject.Registeration.Batch.SectionID == recordsOfTeacher.Batch_Subjects_Parts.Batch.SectionID
            //                 && stdAttRec.Assign_Subject.Batch_Subjects_Parts.SubjectID == recordsOfTeacher.Batch_Subjects_Parts.SubjectID
            //                 && stdAttRec.Assign_Subject.Registeration.Status == "Active")
            //                 orderby stdAttRec.Assign_Subject.Rollno
            //                 select stdAttRec;
            //return stMarksRec;
            try
            { 
            var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == teacherID)
                      .Select(s => s.SubjectID).Distinct();

            var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == teacherID).Select(s => s);

            foreach (var item in getSpecificRecordsOfTeacher2)
            {
                //var getTeacherSubjects2 = item.Teacher_Subject
                //      .Where(s => s.Teachers_Batches.TeacherID == teacherID)
                //      .Select(s => s.SubjectID).Distinct();
                foreach (var item2 in getTeacherSubjects)
                {
                    if (rc.Teacher_Subject.Any(s=>s.Teachers_Batches.TeacherID==teacherID 
                        &&
                        s.SubjectID==item2
                        && s.Teachers_Batches.BatchName==item.BatchName))
                    {
                        //var q1 = rc.Student_Marks.Where(s => s.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                        //&& item2 == s.Assign_Subject.Batch_Subjects_Parts.SubjectID).Select(s=>s);
                        foreach (var item3 in rc.Student_Marks)
                        {
                            if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName==item.BatchName
                                && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                && item3.Assign_Subject.Registeration.Status==1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1)
                            {
                                stMarksRec.Add(item3);
                            }
                        }
                         
                    }
                }
                //var q2=item.Teacher_Subject.Where(s=>s.SubjectID)
                 
                 
            }
            //foreach (var item in getSpecificRecordsOfTeacher)
            //{
            //    foreach (var result in rc.Student_Marks)
            //    {
            //        if (item.Teachers_Batches.BatchName == result.Assign_Subject.Batch_Subjects_Parts.BatchName &&
            //            item.Teachers_Batches.Batch.Section.SectionID == result.Assign_Subject.Batch_Subjects_Parts.Batch.Section.SectionID
            //            && item.SubjectID == result.Assign_Subject.Batch_Subjects_Parts.SubjectID)
            //        {
            //            stMarksRec.Add(result);
            //        }
            //    }
            //}
            return stMarksRec.OrderBy(s => s.Assign_Subject.Rollno);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IEnumerable<Students_Attendance> getResultRecordsAttendance(string teacherID)
        {
            List<Students_Attendance> stMarksRec = new List<Students_Attendance>();
            //var getSpecificRecordsOfTeacher = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == teacherID).Select(s => s);

            //var stAttendanceRec = from stdAttRec in rc.Students_Attendance
            //                 from recordsOfTeacher in getSpecificRecordsOfTeacher

            //                 where (stdAttRec.Assign_Subject.Batch_Subjects_Parts.PartID == recordsOfTeacher.Batch_Subjects_Parts.PartID
            //                 && stdAttRec.Assign_Subject.Registeration.Batch.SectionID == recordsOfTeacher.Batch_Subjects_Parts.Batch.SectionID
            //                 && stdAttRec.Assign_Subject.Batch_Subjects_Parts.SubjectID == recordsOfTeacher.Batch_Subjects_Parts.SubjectID
            //                 && stdAttRec.Assign_Subject.Registeration.Status == "Active")
            //                 orderby stdAttRec.Assign_Subject.Rollno
            //                 select stdAttRec;
            //return stAttendanceRec;
            //var q1 = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == teacherID).Select(s => s.Subject).Distinct();
            try
            {
                 
            var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == teacherID)
                      .Select(s => s.SubjectID).Distinct();

            var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == teacherID).Select(s => s);

            foreach (var item in getSpecificRecordsOfTeacher2)
            {
                 
                foreach (var item2 in getTeacherSubjects)
                {
                    if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == teacherID
                        &&
                        s.SubjectID == item2
                        && s.Teachers_Batches.BatchName == item.BatchName))
                    {
                         
                        foreach (var item3 in rc.Students_Attendance)
                        {
                            if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1)
                            {
                                stMarksRec.Add(item3);
                            }
                        }

                    }
                }
                //var q2=item.Teacher_Subject.Where(s=>s.SubjectID)


            }
            return stMarksRec.OrderBy(s => s.Assign_Subject.Rollno);
            //foreach (var item in getSpecificRecordsOfTeacher)
            //{
            //    foreach (var result in rc.Students_Attendance)
            //    {
            //        if (item.Teachers_Batches.BatchName == result.Assign_Subject.Batch_Subjects_Parts.BatchName &&
            //            item.Teachers_Batches.Batch.Section.SectionID == result.Assign_Subject.Batch_Subjects_Parts.Batch.Section.SectionID
            //            && item.SubjectID == result.Assign_Subject.Batch_Subjects_Parts.SubjectID)
            //        {
            //            stMarksRec.Add(result);
            //        }
            //    }
            //}
            //return stMarksRec.OrderBy(s => s.Assign_Subject.Rollno);

            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<Student_Marks> showResults_TeacherModelFunction(string Month, string batch, Guid SectionID, Guid degID, string search, string t_id, string year,
            string subjectID,string part)
        {
            try
            {
                int yearNo = 0;
                Guid subjID = Guid.NewGuid();
                string MonthName = Month;
                int? partNo = int.Parse(part);
                List<Student_Marks> sectionQuery = new List<Student_Marks>();
                //{ }
                if (Month == "none" && (year == null || year == "") &&
                    (search == null || search == "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

            var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                && s.BatchName ==batch
                &&s.Batch.DegreeProgram_ID==degID
                &&s.Batch.SectionID==SectionID).Select(s => s);

            foreach (var item in getSpecificRecordsOfTeacher2)
            {

                foreach (var item2 in getTeacherSubjects)
                {
                    if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                        &&
                        s.SubjectID == item2
                        && s.Teachers_Batches.BatchName == item.BatchName))
                    {

                        foreach (var item3 in rc.Student_Marks)
                        {
                            if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                && item3.Assign_Subject.Batch_Subjects_Parts.Part==partNo)
                            {
                                if (!sectionQuery.Contains(item3))
                                {
                                    sectionQuery.Add(item3);  
                                } 
                            }
                        }

                    }
                }
            }
            return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //        && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();

                    ////var getTeacherSubjects2 = rc.Teacher_Subject
                    ////    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    ////    .Select(s => s);
                     

                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                     
                }
                //{1 }
                else if (Month != "none" && (year == null || year == "") && (search == null || search == "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Month == Month && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Month == Month
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //        &&  s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //{2 }
                else if (Month == "none" && (year != null && year != "") && (search == null || search == "") && (subjectID == null || subjectID == "Please select" || subjectID == ""))
                {
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && yearNo == item3.Year && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Year == yearNo
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //          && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //3
                else if (Month == "none" && (year == null || year == "") && (search != null &&
                    search != "") && (subjectID == null || subjectID == "Please select" || subjectID == ""))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //         && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //4
                else if (Month == "none" && (year == null || year == "") && (search == null ||
                    search == "") && (subjectID != null && subjectID != "Please select" && subjectID != ""))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //         && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,2
                else if (Month != "none" && (year != null && year != "")
                    && (search == null || search == "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {

                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Month==Month &&item3.Year==yearNo
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Month == Month
                    //        && s.Year == yearNo
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //          && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,3
                else if (Month != "none" && (year == null || year == "") &&
                    (search != null && search != "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        &&item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Month == Month && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Month == Month
                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //         && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,4
                else if (Month != "none" && (year == null || year == "") &&
                    (search == null || search == "") && (subjectID != null &&
                    subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Month == Month && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Month == Month
                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //          && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //2,3
                else if (Month == "none" && (year != null && year != "")
                    && (search != null && search != "") &&
                    (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {

                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        &&item3.Year==yearNo
                                        && item3.Assign_Subject.Rollno.StartsWith(search) && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Year == yearNo
                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //          && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //2,4
                else if (Month == "none" && (year != null && year != "") &&
                    (search == null || search == "") &&
                    (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Year == yearNo
                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //          && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year==yearNo
                                         && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //3,4
                else if (Month == "none" && (year == null || year == "") &&
                    (search != null && search != "") &&
                    (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }

                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //          && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,2,3
                else if (Month != "none" && (year != null && year != "")
                    && (search != null && search != "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {

                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Month==Month
                                        && item3.Year==yearNo
                                        && item3.Assign_Subject.Rollno.StartsWith(search) && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Year == yearNo
                    //        && s.Month == Month
                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //         && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,3,4
                else if (Month != "none" && (year == null || year == "")
                    && (search != null || search != "") &&
                    (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }

                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Month == Month

                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //         && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                       && item3.Month==Month
                                    && item3.Assign_Subject.Rollno.StartsWith(search) && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,2,4
                else if (Month != "none" &&
                    (year != null || year != "") && (search == null || search == "")
                    && (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        &&item3.Year==yearNo
                                        && item3.Month == Month && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Month == Month
                    //        && s.Year == yearNo

                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //         && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //2,3,4
                else if (Month == "none" && (year != null && year != "") &&
                    (search != null && search != "") && (subjectID != null && subjectID != "" &&
                    subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID

                    //        && s.Year == yearNo
                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //         && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year==yearNo
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,2,3,4
                else
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    //var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    //        && s.Month == Month
                    //        && s.Year == yearNo
                    //        && s.Assign_Subject.Rollno.StartsWith(search)
                    //        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjID
                    //        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    //          && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    //        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                    //var getTeacherSubjects = rc.Teacher_Subject
                    //    .Where(s => s.Teachers_Batches.TeacherID == t_id)
                    //    .Select(s => s.SubjectID).Distinct();
                    //foreach (var item in query)
                    //{
                    //    foreach (var tsubj in getTeacherSubjects)
                    //    {
                    //        if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                    //        {
                    //            sectionQuery.Add(item);
                    //        }
                    //    }

                    //}
                    //return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    var getTeacherSubjects = rc.Teacher_Subject
                      .Where(s => s.Teachers_Batches.TeacherID == t_id)
                      .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Student_Marks)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year==yearNo
                                        &&item3.Month==Month
                                        &&item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                } 

            }
            catch (Exception)
            {
                return null; 
            }
             
        }
        public IEnumerable<Students_Attendance> showResultsAttendance_TeacherModelFunction(string Month, string batch, Guid SectionID, Guid degID, string search, string t_id
            , string year,string subjectID,string part)
        {
            try
            {
                int yearNo = 0;
                Guid subjID = Guid.NewGuid();
                string MonthName = Month;
                int? partNo = int.Parse(part);
                List<Students_Attendance> sectionQuery = new List<Students_Attendance>();
                //{ }
                if (Month == "none" && (year == null || year == "") &&
                    (search == null || search == "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {
                     
                    var getTeacherSubjects = rc.Teacher_Subject
          .Where(s => s.Teachers_Batches.TeacherID == t_id)
          .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                         && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //{1 }
                else if (Month != "none" && (year == null || year == "") && (search == null || search == "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
          .Where(s => s.Teachers_Batches.TeacherID == t_id)
          .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        //&& item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        //&& item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //{2 }
                else if (Month == "none" && (year != null && year != "") && (search == null || search == "") && (subjectID == null || subjectID == "Please select" || subjectID == ""))
                {
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
        .Where(s => s.Teachers_Batches.TeacherID == t_id)
        .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        //&& item3.Month == Month
                                        //&& item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //3
                else if (Month == "none" && (year == null || year == "") && (search != null &&
                    search != "") && (subjectID == null || subjectID == "Please select" || subjectID == ""))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
         .Where(s => s.Teachers_Batches.TeacherID == t_id)
         .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                         
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //4
                else if (Month == "none" && (year == null || year == "") && (search == null ||
                    search == "") && (subjectID != null && subjectID != "Please select" && subjectID != ""))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
        .Where(s => s.Teachers_Batches.TeacherID == t_id)
        .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                         && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,2
                else if (Month != "none" && (year != null && year != "")
                    && (search == null || search == "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {

                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
        .Where(s => s.Teachers_Batches.TeacherID == t_id)
        .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        //&& item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,3
                else if (Month != "none" && (year == null || year == "") &&
                    (search != null && search != "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {
                    var getTeacherSubjects = rc.Teacher_Subject
        .Where(s => s.Teachers_Batches.TeacherID == t_id)
        .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        //&& item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //1,4
                else if (Month != "none" && (year == null || year == "") &&
                    (search == null || search == "") && (subjectID != null &&
                    subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
        .Where(s => s.Teachers_Batches.TeacherID == t_id)
        .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        //&& item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        //&& item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //2,3
                else if (Month == "none" && (year != null && year != "")
                    && (search != null && search != "") &&
                    (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {

                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
        .Where(s => s.Teachers_Batches.TeacherID == t_id)
        .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        //&& item3.Month == Month
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //2,4
                else if (Month == "none" && (year != null && year != "") &&
                    (search == null || search == "") &&
                    (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
        .Where(s => s.Teachers_Batches.TeacherID == t_id)
        .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        //&& item3.Month == Month
                                        //&& item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                }
                //3,4
                else if (Month == "none" && (year == null || year == "") &&
                    (search != null && search != "") &&
                    (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
         .Where(s => s.Teachers_Batches.TeacherID == t_id)
         .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        //&& item3.Year == yearNo
                                        //&& item3.Month == Month
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                     
                }
                //1,2,3
                else if (Month != "none" && (year != null && year != "")
                    && (search != null && search != "") && (subjectID == null || subjectID == "" || subjectID == "Please select"))
                {

                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
         .Where(s => s.Teachers_Batches.TeacherID == t_id)
         .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && item2 == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        sectionQuery.Add(item3);
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                     
                }
                //1,3,4
                else if (Month != "none" && (year == null || year == "")
                    && (search != null || search != "") &&
                    (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
         .Where(s => s.Teachers_Batches.TeacherID == t_id)
         .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        //&& item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                     
                }
                //1,2,4
                else if (Month != "none" &&
                    (year != null || year != "") && (search == null || search == "")
                    && (subjectID != null && subjectID != "" && subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
         .Where(s => s.Teachers_Batches.TeacherID == t_id)
         .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        //&& item3.Assign_Subject.Rollno.StartsWith(search))
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                     
                }
                //2,3,4
                else if (Month == "none" && (year != null && year != "") &&
                    (search != null && search != "") && (subjectID != null && subjectID != "" &&
                    subjectID != "Please select"))
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
         .Where(s => s.Teachers_Batches.TeacherID == t_id)
         .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        //&& item3.Month == Month
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                     
                }
                //1,2,3,4
                else
                {
                    if (!Guid.TryParse(subjectID, out subjID))
                    {
                        return null;
                    }
                    if (!int.TryParse(year, out yearNo))
                    {
                        return null;
                    }
                    var getTeacherSubjects = rc.Teacher_Subject
         .Where(s => s.Teachers_Batches.TeacherID == t_id)
         .Select(s => s.SubjectID).Distinct();

                    var getSpecificRecordsOfTeacher2 = rc.Teachers_Batches.Where(s => s.TeacherID == t_id
                        && s.BatchName == batch
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == SectionID).Select(s => s);

                    foreach (var item in getSpecificRecordsOfTeacher2)
                    {

                        foreach (var item2 in getTeacherSubjects)
                        {
                            if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                                &&
                                s.SubjectID == item2
                                && s.Teachers_Batches.BatchName == item.BatchName))
                            {

                                foreach (var item3 in rc.Students_Attendance)
                                {
                                    if (item3.Assign_Subject.Batch_Subjects_Parts.BatchName == item.BatchName
                                        && subjID == item3.Assign_Subject.Batch_Subjects_Parts.SubjectID
                                        && item3.Year == yearNo
                                        && item3.Month == Month
                                        && item3.Assign_Subject.Rollno.StartsWith(search)
                                        && item3.Assign_Subject.Registeration.Status == 1
                                && item3.Assign_Subject.Registeration.Student_Profile.Status == 1
                                        && item3.Assign_Subject.Batch_Subjects_Parts.Part == partNo)
                                    {
                                        if (!sectionQuery.Contains(item3))
                                        {
                                            sectionQuery.Add(item3);
                                        } 
                                    }
                                }

                            }
                        }
                    }
                    return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                } 
             
            }
            catch (Exception)
            {
                return null;
            }
              
        }
             
          
             
         
        public List<Subject> getRelatedSubjects(string t_id)
        {
            var getTeacherSubjectRelatedRecord = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == t_id)
                .Select(s => s.Subject).Distinct();

            List<Subject> tsubj = new List<Subject>();

            foreach (var item in getTeacherSubjectRelatedRecord)
            {
                tsubj.Add(new Subject { SubjectID = item.SubjectID, SubjectName = item.SubjectName });
            }
            return tsubj;
        }
        public List<Batch> getRelatedStudentBatches(string t_id)
        {
            List<Batch> batchesInTeachers = new List<Batch>();
            var ListofDegreePrograms = rc.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s);
            List<Batch> joinTeacherBatchs = (from tb in ListofDegreePrograms
                                             join batch in rc.Batches
                                             on tb.BatchName equals batch.BatchName
                                             where (tb.BatchName == batch.BatchName &&batch.Status==1)
                                             select batch).ToList();
            //foreach (var item in ListofDegreePrograms)
            //{
            //    foreach (var item2 in rc.Batches)
            //    {
            //        if (item.Batch.DegreeProgram_ID == item2.DegreeProgram_ID)
            //        {
            //            batchesInTeachers.Add(item2);
            //        }
            //    }
            //}
            //return batchesInTeachers;
            return joinTeacherBatchs;
        }
        public List<string> getRelatedStudentRollNos(List<Batch> batchList)
        {
            //List<string> rollNos = new List<string>();
            List<string> joinBatchReg = (from bt in batchList
                                         join reg in rc.Registerations
                                         on bt.BatchName equals reg.BatchID
                                         where (bt.BatchName == reg.BatchID
                                         && reg.Status == 1)
                                         orderby reg.Rollno
                                         select reg.Rollno).ToList();
            return joinBatchReg;
            //foreach (var item in batchList)
            //{
            //    foreach (var item2 in item.Registerations)
            //    {
            //        if (item2.Status=="Active")
            //        {

            //        }
            //        rollNos.Add(item2.Rollno);
            //    }
            //}
            //return rollNos;
            //List<Batch> batchesInTeachers = new List<Batch>();
            //var ListofDegreePrograms = rc.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s);

            ////List<Batch> RollnosQuery = (from items in ListofDegreePrograms
            ////                                    join regStudents in rc.Batches
            ////                                    on items.Batch.DegreeProgram_ID equals regStudents.DegreeProgram_ID
            ////                                    where (regStudents.DegreeProgram_ID == items.Batch.DegreeProgram_ID
            ////                                        //&& regStudents.Registerations == "Active")
            ////                                    )
            ////                                    select regStudents);
            ////return RollnosQuery;
            //foreach (var item in ListofDegreePrograms)
            //{
            //    foreach (var item2 in rc.Batches)
            //    {
            //        if (item.Batch.DegreeProgram_ID == item2.DegreeProgram_ID)
            //        {
            //            batchesInTeachers.Add(item2);
            //        }
            //    }
            //}
            //return batchesInTeachers;
        }
        public bool CheckForDuplicateRecords(string rollno, string subjectID, string batch, string month2, string year)
        {
            try
            {
                bool checkIfNotMatching = true;
                //int roll = int.Parse(rollno);
                Guid subjectsID = Guid.Parse(subjectID);
                int Selectedyear = int.Parse(year);
                //int student_SubjectPart = int.Parse(part);
                var getSpecificStudentRollnoMatchedRecord = rc.Student_Marks.Where(s => s.Assign_Subject.Rollno == rollno).Select(s => s);

                var checkingQuery = (from item in getSpecificStudentRollnoMatchedRecord

                                     where (month2 == item.Month
                                     && item.Assign_Subject.Rollno == rollno

                                     && item.Assign_Subject.Batch_Subjects_Parts.BatchName == batch &&

                                     item.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjectsID

                                     && item.Assign_Subject.Registeration.Status == 1

                                     && item.Year == Selectedyear)
                                     select item).ToList();
                if (checkingQuery.Count == 0)
                {
                    checkIfNotMatching = true;
                }
                else
                {
                    checkIfNotMatching = false;
                }
                //foreach (var item in getSpecificStudentRollnoMatchedRecord)
                //{
                //    if (month2 == item.Month && item.Assign_Subject.Rollno == rollno
                //        && item.Assign_Subject.PartID == student_SubjectPart &&
                //        item.Assign_Subject.SubjectID == subjectsID)
                //    {
                //        checkIfNotMatching = false;
                //        break;
                //    }
                //}

                return checkIfNotMatching;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool CheckForDuplicateRecordsInAttendance(string rollno, string subjectID, string month2
            , string year, string batch)
        {
            try
            {
                bool checkIfNotMatching = true;
                //int roll = int.Parse(rollno);
                Guid subjectsID = Guid.Parse(subjectID);
                int Selectedyear = int.Parse(year);
                //int student_SubjectPart = int.Parse(part);
                var getSpecificStudentRollnoMatchedRecord = rc.Students_Attendance.Where(s => s.Assign_Subject.Rollno == rollno).Select(s => s);
                var checkingQuery = (from item in getSpecificStudentRollnoMatchedRecord

                                     where (month2 == item.Month && item.Assign_Subject.Rollno == rollno
                                     && item.Assign_Subject.Batch_Subjects_Parts.SubjectID == subjectsID
                                      && item.Assign_Subject.Registeration.Status == 1
                                      && item.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                                      && item.Year == Selectedyear)

                                     select item).ToList();
                if (checkingQuery.Count > 0)
                {
                    checkIfNotMatching = false;
                }
                else
                {
                    checkIfNotMatching = true;
                }
                //foreach (var item in getSpecificStudentRollnoMatchedRecord)
                //{
                //    if (month2 == item.Month && item.Assign_Subject.PartID == student_SubjectPart
                //        && item.Assign_Subject.SubjectID == subjectsID)
                //    {
                //        checkIfNotMatching = false;
                //        break;
                //    }
                //}
                return checkIfNotMatching;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public bool AddMarksRecord(Student_Marks res, string roll, Guid subjectsID, string batch, string month2
            , string year)
        {
            try
            {
                var MarksqueryForID = rc.Assign_Subject.
                Where(s => s.Batch_Subjects_Parts.SubjectID == subjectsID
                && s.Rollno == roll && s.Batch_Subjects_Parts.BatchName == batch
                && s.Registeration.Status == 1)
                .Select(s => s).FirstOrDefault();

                if (MarksqueryForID != null)
                {
                    res.SubjectAssignID = MarksqueryForID.AssignID;
                    res.Month = month2;
                    res.Year = int.Parse(year);
                    rc.Student_Marks.Add(res);
                    rc.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public string AddIndiAttRecord(Students_Attendance stdAtt, DateTime? date1234,
            string rollnohereIndi, string partInIndiMarks, string subjectMarksIDIndi, string t_id)
        {
            try
            {

                int partNo = 0;
                Guid gd = Guid.NewGuid();
                var checkRoll = rc.Registerations.Where(s => s.Rollno == rollnohereIndi 
                    && s.Student_Profile.Status == 1 && s.Status == 1).Select(s => s).FirstOrDefault();

                if (rollnohereIndi == null || rollnohereIndi == "")
                {
                    return "Roll no is required!";
                }
                else if (checkRoll == null)
                {
                    return "Roll no Doesn't exists";
                }
                else if (date1234 == null)
                {
                    return "Date is Required!";
                }
                else if (!int.TryParse(partInIndiMarks, out partNo))
                {
                    return "Part is Not Valid!";
                }
                else if (subjectMarksIDIndi == "Please select")
                {
                    return "Subject Needs To be Selected!";
                }
                else if (!Guid.TryParse(subjectMarksIDIndi, out gd))
                {
                    return "Subject Does not Exists!";
                }
                else if (stdAtt.Attended_Lectures > stdAtt.Total_lectures)
                {
                    return "Attended Lectures Must be less than or Equal to Total Lectures!";
                }
                else
                {
                    string batchno = checkRoll.BatchID;
                    string MonthNaam = MonthsNames[date1234.Value.Month];
                    string subjName = rc.Subjects.Where(s => s.SubjectID == gd).Select(s => s.SubjectName).FirstOrDefault();
                    if (partNo > checkRoll.Part || partNo < 0 || partNo == 0)
                    {
                        return "Plz Enter a Valid Part No For this Roll no!";
                    }
                    if (checkRoll.Assign_Subject != null)
                    {
                        if (checkRoll.Assign_Subject.Count == 0)
                        {
                            return "This student has no subject assigned!";
                        }
                        else if (!checkRoll.Assign_Subject.Any(s => s.Batch_Subjects_Parts.SubjectID == gd
                            && s.Batch_Subjects_Parts.Part == partNo))
                        {
                            return "Student is not assigned Subject:" + subjName + " in part " + partNo;
                        }
                    }
                    if (!rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                        && s.Teachers_Batches.BatchName == batchno
                        && s.SubjectID == gd))
                    {
                        return "You are not Teaching The Subject: " + subjName + " to Batch " + batchno;
                    }
                    if (rc.Students_Attendance.Any(s => s.Month == MonthNaam
                        && s.Year == date1234.Value.Year
                        && s.Assign_Subject.Rollno == rollnohereIndi
                        && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == gd
                        && s.Assign_Subject.Batch_Subjects_Parts.Part == partNo))
                    {
                        return "Record already exists!";
                    }
                    if (!(date1234.Value.Year >= checkRoll.Batch.Year.FromYear && date1234.Value.Year <= checkRoll.Batch.Year.ToYear))
                    {
                        return "Plz Enter a Year that is between " + checkRoll.Batch.Year.FromYear + " - " + checkRoll.Batch.Year.ToYear;
                    }
                    CalculatePercenatageOfObtainedAttIndi(ref stdAtt);

                    var assignSubj = checkRoll.Assign_Subject.Where(s => s.Batch_Subjects_Parts.SubjectID == gd
                            && s.Batch_Subjects_Parts.Part == partNo).Select(s => s).FirstOrDefault();

                    rc.Students_Attendance.Add(new Students_Attendance
                    {
                        Month = MonthsNames[date1234.Value.Month],
                        Year = date1234.Value.Year,
                        Status = "Active",
                        Total_lectures = stdAtt.Total_lectures,
                        Attended_Lectures = stdAtt.Attended_Lectures,
                        AttendanceID = Guid.NewGuid(),
                        Attendance_Percentage = stdAtt.Attendance_Percentage,
                        SubjectAssignID = assignSubj.AssignID,
                        Assign_Subject = assignSubj
                    });
                    rc.SaveChanges();
                    return "OK";
                }
            }
            catch (Exception)
            {
                return "Unable to Upload Attendance! Plz Try Again!";
            }
        }
        private void CalculatePercenatageOfObtainedAttIndi(ref Students_Attendance stdAtt)
        {
            double? Totallec = stdAtt.Total_lectures;
            double? d = 0.0d;

            d = (stdAtt.Attended_Lectures / Totallec);
            d *= 100;

            stdAtt.Attendance_Percentage = Math.Round(d ?? 0, 2).ToString();
        }
        public string AddIndiMarksRecord(Student_Marks stdMarks, DateTime? date1234,
            string rollnohereIndi, string partInIndiMarks, string subjectMarksIDIndi,string t_id)
        {
            try
            {
                 
            int partNo = 0;
            Guid gd = Guid.NewGuid();
            var checkRoll= rc.Registerations.Where(s=>s.Rollno==rollnohereIndi && s.Student_Profile.Status==1 &&s.Status==1).Select(s=>s).FirstOrDefault();

            if (rollnohereIndi==null||rollnohereIndi=="")
            {
                return "Roll no is required!";
            }
            else if (checkRoll==null)
            {
                return "Roll no Doesn't exists";
            }
            else if (date1234==null)
            {
                return "Date is Required!";
            }
            else if (!int.TryParse(partInIndiMarks,out partNo))
            {
                return "Part is Not Valid!";
            }
            else if (subjectMarksIDIndi=="Please select")
            {
                return "Subject Needs To be Selected!";
            }
            else if (!Guid.TryParse(subjectMarksIDIndi,out gd))
            {
                return "Subject Does not Exists!";
            }
            else if (stdMarks.Obtained_Marks>stdMarks.Total_Marks)
            {
                return "Obtained marks Must be less than or Equal to Total Marks!";
            }
            else
            {
                string batchno = checkRoll.BatchID;
                string MonthNaam =MonthsNames[date1234.Value.Month];
                string subjName= rc.Subjects.Where(s=>s.SubjectID==gd).Select(s=>s.SubjectName).FirstOrDefault();
                if (partNo>checkRoll.Part ||partNo<0 || partNo==0)
                {
                    return "Plz Enter a Valid Part No For this Roll no!";
                }
                if (checkRoll.Assign_Subject!=null)
                {
                    if (checkRoll.Assign_Subject.Count==0)
                    {
                        return "This student has no subject assigned!";
                    }
                    else if (!checkRoll.Assign_Subject.Any(s=>s.Batch_Subjects_Parts.SubjectID==gd 
                        && s.Batch_Subjects_Parts.Part==partNo))
                    {
                        return "Student is not assigned Subject:"+ subjName+" in part "+partNo;
                    }
                }
                if (!rc.Teacher_Subject.Any(s => s.Teachers_Batches.TeacherID == t_id
                    && s.Teachers_Batches.BatchName == batchno
                    && s.SubjectID==gd))
                {
                    return "You are not Teaching The Subject: "+subjName+" to Batch "+batchno;
                }
                if (rc.Student_Marks.Any(s=>s.Month== MonthNaam
                    &&s.Year==date1234.Value.Year
                    && s.Assign_Subject.Rollno==rollnohereIndi
                    && s.Assign_Subject.Batch_Subjects_Parts.SubjectID==gd
                    && s.Assign_Subject.Batch_Subjects_Parts.Part==partNo))
                {
                    return "Record already exists!";
                }
                if (!(date1234.Value.Year>=checkRoll.Batch.Year.FromYear &&date1234.Value.Year<=checkRoll.Batch.Year.ToYear))
                {
                    return "Plz Enter a Year that is between " + checkRoll.Batch.Year.FromYear + " - " + checkRoll.Batch.Year.ToYear;    
                }
                CalculatePercenatageOfObtainedMarksIndi(ref stdMarks);

                var assignSubj=checkRoll.Assign_Subject.Where(s=>s.Batch_Subjects_Parts.SubjectID==gd 
                        && s.Batch_Subjects_Parts.Part==partNo).Select(s=>s).FirstOrDefault();

                rc.Student_Marks.Add(new Student_Marks
                { 
                    Month=MonthsNames[ date1234.Value.Month],
                    Year=date1234.Value.Year,
                    Status="Active",
                    Total_Marks=stdMarks.Total_Marks,
                    Obtained_Marks=stdMarks.Obtained_Marks,
                    ResultID=Guid.NewGuid(),
                    Marks_Percentage=stdMarks.Marks_Percentage,
                    SubjectAssignID=assignSubj.AssignID,
                    Assign_Subject=assignSubj
                });
                rc.SaveChanges();
                return "OK";
            } 
            }
            catch (Exception)
            {
                return "Unable to Upload Marks! Plz Try Again!";
            }
        }
        private void CalculatePercenatageOfObtainedMarksIndi(ref Student_Marks stdMarks)
        {
            double? TotalMarks = stdMarks.Total_Marks;
            double? d = 0.0d;
             
                d = (stdMarks.Obtained_Marks / TotalMarks);
                d *= 100;

                stdMarks.Marks_Percentage = Math.Round(d ?? 0, 2).ToString(); 
        }
        public string AddAllMarksRecord(string batch, Nullable<DateTime> date,
            int part, Guid subj, IEnumerable<ViewModelMarks> VM1, int total)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    string monthName = MonthsNames[date.Value.Month];
                    //If The teacher is teaching the subject to this batch
                    if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.BatchName == batch
                        && s.Subject.SubjectID == subj))
                    {
                        //If the Marks of batch have already been Uploaded

                        if (rc.Student_Marks.Any(s =>
                            s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                            && s.Month == monthName
                            && s.Year == date.Value.Year
                            && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subj
                            && s.Assign_Subject.Batch_Subjects_Parts.Part == part))
                        {
                            return "Marks of Batch " + batch + " of Part: " + part +
                                " of Subject :" + rc.Subjects.
                                Where(s => s.SubjectID == subj)
                                .Select(s => s.SubjectName).FirstOrDefault()
                                + " has Already been Uploaded";
                        }

                            //If Any obtained Marks entered is Greater Than Total Marks
                        else if (VM1.Any(s => s.Obtained_Marks > total))
                        {
                            return "Please Make Sure That Obtained Marks are always less Or Equal To Total Marks";
                        } 
                        else
                        {
                            Guid gd1=VM1.FirstOrDefault().AssignID;
                            var getVm1First = rc.Assign_Subject.Where(s => s.AssignID ==  gd1).Select(s => s.Registeration).FirstOrDefault();
                            if (getVm1First==null)
                            {
                                return "No Record Found for Uploading";
                            }

                            if (!(date.Value.Year >= getVm1First.Batch.Year.FromYear && date.Value.Year <= getVm1First.Batch.Year.ToYear))
                            {
                                return "Plz Enter a Year that is between " + getVm1First.Batch.Year.FromYear + " - " + getVm1First.Batch.Year.ToYear;
                            }
                            CalculatePercenatageOfObtainedMarks(ref VM1, total);
                            //Upload Marks
                            foreach (var item in VM1)
                            {
                                rc.Student_Marks.Add(new Student_Marks
                                {
                                    Total_Marks = total,
                                    Obtained_Marks = Math.Round( item.Obtained_Marks??0,2),
                                    Marks_Percentage = item.Marks_Percentage,
                                    Month = MonthsNames[date.Value.Month],
                                    Year = date.Value.Year,
                                    SubjectAssignID = item.AssignID,
                                    Assign_Subject = rc.Assign_Subject.Where(s => s.AssignID == item.AssignID).Select(s => s).FirstOrDefault(),
                                    ResultID = Guid.NewGuid()
                                });
                            }
                            rc.SaveChanges();
                            t.Complete();
                            return "OK";
                        }
                    }
                    else
                    {
                        return "Please Select The Subject That You are Teaching to The Batch: " + batch;
                    }
                }
                catch (Exception)
                {
                    return "Unable To Upload Marks!";
                }
            }
        }
        private void CalculatePercenatageOfObtainedMarks(ref IEnumerable<ViewModelMarks> VM1, int total)
        {
            double? TotalMarks = total;
            double? d = 0.0d;
            foreach (var item in VM1)
            {
                d = (item.Obtained_Marks / TotalMarks);
                d *= 100;

                item.Marks_Percentage = Math.Round(d??0,2).ToString();
            }
        }


        public string AddAllAttendanceRecord(string batch, Nullable<DateTime> date,
            int part, Guid subj, IEnumerable<ViewModelAttendance> VM1, int total)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    string monthName = MonthsNames[date.Value.Month];
                    //If The teacher is teaching the subject to this batch
                    if (rc.Teacher_Subject.Any(s => s.Teachers_Batches.BatchName == batch
                        && s.Subject.SubjectID == subj))
                    {
                        //If the Marks of batch have already been Uploaded
                        if (rc.Students_Attendance.Any(s =>
                            s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                            && s.Month == monthName
                            && s.Year == date.Value.Year
                            && s.Assign_Subject.Batch_Subjects_Parts.SubjectID == subj
                            && s.Assign_Subject.Batch_Subjects_Parts.Part == part))
                        {
                            return "Attendance of Batch " + batch + " of Part: " + part +
                                " of Subject :" + rc.Subjects.
                                Where(s => s.SubjectID == subj)
                                .Select(s => s.SubjectName).FirstOrDefault()
                                + " has Already been Uploaded";
                        }
                         
                        else if (VM1.Any(s => s.Attended_Lectures > total))
                        {
                            return "Please Make Sure That Attended Lectures are always less Or Equal To Total Lectures";
                        }
                        else
                        {
                            Guid gd1 = VM1.FirstOrDefault().AssignID;
                            var getVm1First = rc.Assign_Subject.Where(s => s.AssignID == gd1).Select(s => s.Registeration).FirstOrDefault();
                            if (getVm1First == null)
                            {
                                return "No Record Found for Uploading";
                            }
                            if (!(date.Value.Year >= getVm1First.Batch.Year.FromYear && date.Value.Year <= getVm1First.Batch.Year.ToYear))
                            {
                                return "Plz Enter a Year that is between " + getVm1First.Batch.Year.FromYear + " - " + getVm1First.Batch.Year.ToYear;
                            }
                            //Calculate Attendance Percentage First
                            CalculatePercenatageOfAttended_Lectures(ref VM1, total);
                            //Upload Marks
                            foreach (var item in VM1)
                            {
                                rc.Students_Attendance.Add(new Students_Attendance
                                {
                                    AttendanceID = Guid.NewGuid(),
                                    Total_lectures = total,
                                    Attended_Lectures = item.Attended_Lectures,
                                    Attendance_Percentage = item.Attendance_Percentage,
                                    Month = MonthsNames[date.Value.Month],
                                    Year = date.Value.Year,
                                    SubjectAssignID = item.AssignID,
                                    Assign_Subject = rc.Assign_Subject.Where(s => s.AssignID == item.AssignID).Select(s => s).FirstOrDefault()
                                });
                            }
                            rc.SaveChanges();
                            t.Complete();
                            return "OK";
                        }
                    }
                    else
                    {
                        return "Please Select The Subject That You are Teaching to The Batch: " + batch;
                    }
                }
                catch (Exception)
                {
                    t.Dispose();
                    return "Unable To Upload Attendance!";
                }
            }
        }
        private void CalculatePercenatageOfAttended_Lectures(ref IEnumerable<ViewModelAttendance> VM1, int total)
        {
            int? totalLec = total;
            foreach (var item in VM1)
            {
                item.Attendance_Percentage = ((item.Attended_Lectures * 100) / totalLec).ToString();
            }
        }


        public bool AddAttendanceRecord(Students_Attendance res, string roll, Guid subjectsID, string month2
            , string year, string batch)
        {
            try
            {
                var MarksqueryForID = rc.Assign_Subject.
                Where(s => s.Batch_Subjects_Parts.SubjectID == subjectsID
                && s.Rollno == roll
                && s.Batch_Subjects_Parts.BatchName == batch
                && s.Registeration.Status == 1).Select(s => s).FirstOrDefault();
                if (MarksqueryForID != null)
                {
                    res.SubjectAssignID = MarksqueryForID.AssignID;
                    res.Month = month2;
                    res.Year = int.Parse(year);
                    rc.Students_Attendance.Add(res);
                    rc.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public List<Registeration> ViewStudents(string t_id)
        {
            try
            {
                //Get Batches and Subjects of Current Teacher
                var ListofBatches = rc.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s);

                var getSubjOfTeacher = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == t_id).Select(s => s);

                List<Registeration> updatedSTudentList = new List<Registeration>();


                //Get All students of Batches of a Teacher that it is teaching to
                List<Registeration> viewStudentsQuery = (from tBatches in ListofBatches
                                                         join regStudents in rc.Registerations on tBatches.BatchName equals regStudents.BatchID
                                                         where (regStudents.Student_Profile.Status == 1
                                                         && regStudents.Status == 1)
                                                         select regStudents).ToList();

                //Now Get Assigned Subjects for each student
                List<Assign_Subject> assignSubjectOfStudents = (from regStudents in viewStudentsQuery
                                                                join subj in rc.Assign_Subject on regStudents.Rollno equals subj.Rollno
                                                                where (regStudents.Student_Profile.Status == 1)
                                                                select subj).ToList();

                foreach (var item in getSubjOfTeacher)
                {
                    foreach (var item2 in assignSubjectOfStudents)
                    {
                        if (item.Teachers_Batches.BatchName == item2.Batch_Subjects_Parts.BatchName
                            && item2.Batch_Subjects_Parts.SubjectID == item.SubjectID)
                        {

                            if (!updatedSTudentList.Any(s => s.Rollno == item2.Rollno))
                            {
                                updatedSTudentList.Add(item2.Registeration);
                            }
                        }
                    }
                }
                return updatedSTudentList;
            }
            catch (Exception)
            {
                return null;
            }


        }
        public List<Registeration> ViewStudents_Searched(string t_id, string search)
        {
            try
            {
                //Get Batches and Subjects of Current Teacher

                var ListofBatches = rc.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s);
                var getSubjOfTeacher = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == t_id).Select(s => s);


                List<Registeration> updatedSTudentList = new List<Registeration>();


                //Get Searched students of Batches of a Teacher that it is teaching to
                //List<Registeration> stdsInBatchesSearched = rc.Registerations.Where(s => s.Rollno.StartsWith(search)).OrderBy(s=>s.Rollno).Select(s => s).ToList();

                List<Registeration> viewStudentsQuery = (from tBatches in ListofBatches
                                                         join regStudents in rc.Registerations on tBatches.BatchName equals regStudents.BatchID
                                                         where (regStudents.Student_Profile.Status == 1 && regStudents.Rollno.StartsWith(search)
                                                         && regStudents.Status==1 )
                                                         select regStudents).ToList();

                //Now Get Assigned Subjects for each student
                List<Assign_Subject> assignSubjectOfStudents = (from regStudents in viewStudentsQuery
                                                                join subj in rc.Assign_Subject on regStudents.Rollno equals subj.Rollno
                                                                where (regStudents.Student_Profile.Status == 1)
                                                                select subj).ToList();

                foreach (var item in getSubjOfTeacher)
                {
                    foreach (var item2 in assignSubjectOfStudents)
                    {
                        if (item.Teachers_Batches.BatchName == item2.Batch_Subjects_Parts.BatchName
                            && item2.Batch_Subjects_Parts.SubjectID == item.SubjectID)
                        { 
                            if (!updatedSTudentList.Any(s => s.Rollno == item2.Rollno))
                            {
                                updatedSTudentList.Add(item2.Registeration);
                            }
                        }
                    }
                }
                return updatedSTudentList;
            }
            catch (Exception)
            {
                return null;
            }


            //return viewStudentsQuery;

            //foreach (var item in rc.Registerations)
            //{
            //    foreach (var item2 in ListofSubj)
            //    {
            //        if (item2.DegreeProgram_ID == item.Degree_Program.ProgramID
            //            && item2.SectionID == item.SectionID
            //            && item2.Part == item.PartID)
            //        {

            //            rollnos.Add(item);
            //        }
            //    }
            //}
            //return rollnos;
        }
        public List<string[]> GetDataBasedOnRollnosForViewing(string id)
        {
            List<string[]> rollnos = new List<string[]>();
            var ListofDegreePrograms = rc.Teachers_Batches.Where(s => s.TeacherID == id).Select(s => s);
            //List<string> getDataQuery = (from item1 in rc.Registerations
            //                 join item2 in ListofDegreePrograms 
            //                 on item1.Degree_Program.ProgramID equals item2.DegreeProgram_ID
            //                 where(item2.DegreeProgram_ID == item1.Degree_Program.ProgramID
            //                 && item2.SectionID == item1.SectionID
            //                 && item2.Part == item1.PartID)
            //                 select new {
            //                     item1.Rollno,
            //                     item1.Student_Profile.FirstName,
            //                     item1.Student_Profile.Gender,
            //                     item1.Degree_Program.Degree_ProgramName,
            //                     item1.PartID,
            //                     item1.Section.SectionName}.ToString()).ToList();
            //return getDataQuery;
            foreach (var item in rc.Registerations)
            {
                foreach (var item2 in ListofDegreePrograms)
                {
                    if (item2.Batch.DegreeProgram_ID == item.Batch.DegreeProgram_ID
                        && item2.Batch.SectionID == item.Batch.SectionID
                        && item.Student_Profile.Status == 1)
                    {

                        rollnos.Add(new string[]
                        {
                            item.Rollno.ToString(),
                    item.Student_Profile.FirstName+" "+item.Student_Profile.LastName,
                    item.Student_Profile.Gender,
                    item.Batch.Degree_Program.Degree_ProgramName,
                    item.Part.ToString(),
                    item.Batch.Section.SectionName
                        });
                    }
                }
            }
            return rollnos;
        }
        public List<string> getSpecificSearchRecord(string rollno)
        {
            List<string> list = new List<string>();
            var query = rc.Registerations.Where(s => s.Rollno.StartsWith(rollno) && s.Status == 1).Select(s => s).FirstOrDefault();
            list.Add(query.Rollno.ToString());
            list.Add(query.Student_Profile.FirstName + " " + query.Student_Profile.LastName);
            list.Add(query.Student_Profile.Gender);
            list.Add(query.Batch.Degree_Program.Degree_ProgramName);
            list.Add(query.Part.ToString());
            list.Add(query.Batch.Section.SectionName);
            return list;
        }
        public IEnumerable<Teacher_Attendance> getResultRecordsForTeacherAttendance(string t_id)
        {
            var getSpecificRecordsOfTeacher = rc.Teacher_Attendance.Where(s => s.TeacherID == t_id).Select(s => s).OrderBy(s => s.TeacherID);
            return getSpecificRecordsOfTeacher;
        }
        public IEnumerable<Teacher_Attendance> showResultsTeacherAttendance_TeacherModelFunction(string t_id, string Month, string year)
        {
            int count = MonthsNames.ToList().Count;
            int index2 = 0;
            for (int i = 0; i < count; i++)
            {
                if (MonthsNames[i] == Month)
                {
                    index2 = i;
                    break;
                }
            }
            //int index2 = MonthsNames.Select(s=>s.IndexOf(Month)).FirstOrDefault();
            if (year == null || year == "")
            {
                var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                      where tAtt.TeacherID == t_id
                                      && tAtt.Date.Month == index2
                                      select tAtt;
                return TeacherAttQuery;
            }
            else
            {
                Match m = Regex.Match(year, "^[0-9]*$");
                if (m.Success)
                {
                    int Selectedyear = int.Parse(year);
                    var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                          where tAtt.TeacherID == t_id
                                          && tAtt.Date.Month == index2
                                          && tAtt.Date.Year == Selectedyear
                                          select tAtt;
                    return TeacherAttQuery;
                }
                else
                {
                    var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                          where tAtt.TeacherID == t_id
                                          && tAtt.Date.Month == index2
                                          select tAtt;
                    return TeacherAttQuery;
                }
            }
        }
        public string checkerIfIamTeachingthisSubjectTothisBatch(string batch,
            string subjectID,string t_id)
        {
            //int partID = int.Parse(part);
            Guid subjID =  Guid.NewGuid();
            if (!Guid.TryParse(subjectID,out subjID))
            {
                return "Subject Name is not Valid!";
            }
            if (!rc.Subjects.Any(s=>s.SubjectID==subjID))
            {
                return "Subject Does not Exists!";   
            }

            var getteacherBatchesSubjects = rc.Teacher_Subject.Where(s => s.Teachers_Batches.BatchName == batch
                && s.SubjectID == subjID
                &&s.Teachers_Batches.TeacherID==t_id
                ).Select(s => s);

            if (getteacherBatchesSubjects == null || getteacherBatchesSubjects.Count() == 0)
            {
                return "You are not assigned to the batch: " + batch +" for Subject "+
                    rc.Subjects.Where(s=>s.SubjectID==subjID).Select(s=>s.SubjectName).FirstOrDefault()??"";
            }
             
                return "OK"; 
        }

        public IEnumerable<ViewModelMarks> getListofStudentsAccordingToBatch
            (string batch, string part, string subjectID)
        {
            int partID = int.Parse(part);
            Guid subjID = Guid.Parse(subjectID);
             
                var getStudents = rc.Assign_Subject
                .Where(s => s.Batch_Subjects_Parts.BatchName == batch
                && s.Batch_Subjects_Parts.Part == partID
                && s.Batch_Subjects_Parts.SubjectID == subjID)
                .Select(s => s);

                List<ViewModelMarks> Vm = new List<ViewModelMarks>();
                foreach (var item in getStudents)
                {
                    if (item.Registeration.Status == 1 && item.Registeration.Student_Profile.Status == 1)
                    {
                        if (!Vm.Any(s => s.Registeration == item.Registeration))
                        {
                            Vm.Add(new ViewModelMarks
                            {
                                AssignID = item.AssignID,
                                Assign_Subject = item,
                                Batch_Subjects_Parts = item.Batch_Subjects_Parts,
                                Batch_Subject_ID = item.Batch_Subject_ID,
                                Rollno = item.Rollno,
                                Registeration = item.Registeration,
                                Student_Marks = item.Student_Marks,
                                Students_Attendance = item.Students_Attendance
                            });   
                        }
                         
                    }
                     
                }
                return Vm.OrderBy(s => s.Rollno);
             

        }
        public IEnumerable<ViewModelAttendance> getListofStudentsAttendanceAccordingToBatch
            (string batch, string part, string subjectID)
        {
            int partID = int.Parse(part);
            Guid subjID = Guid.Parse(subjectID);

             

                var getStudents = rc.Assign_Subject
                    .Where(s => s.Batch_Subjects_Parts.BatchName == batch
                    && s.Batch_Subjects_Parts.Part == partID
                    && s.Batch_Subjects_Parts.SubjectID == subjID)
                    .Select(s => s);

                List<ViewModelAttendance> Vm = new List<ViewModelAttendance>();
                foreach (var item in getStudents)
                {
                    if (item.Registeration.Status == 1 && item.Registeration.Student_Profile.Status == 1 )
                    {
                        if (!Vm.Any(s=>s.Registeration==item.Registeration))
                        {
                            Vm.Add(new ViewModelAttendance
                            {
                                AssignID = item.AssignID,
                                Assign_Subject = item,
                                Batch_Subjects_Parts = item.Batch_Subjects_Parts,
                                Batch_Subject_ID = item.Batch_Subject_ID,
                                Rollno = item.Rollno,
                                Registeration = item.Registeration,
                                Student_Marks = item.Student_Marks,
                                Students_Attendance = item.Students_Attendance
                            });      
                        }
                         
                    }
                     
                }
                return Vm.OrderBy(s => s.Rollno);
             

        }

        public IEnumerable<Teachers_Batches> GetAllTeacher_batchesRecordsForTeacher(string id, int choice, string BatchesType
            , string degforTeacher)
        {
            int batchStatus = 0;
            Guid gd = new Guid();

            if (choice == 1)
            {
                if (!int.TryParse(BatchesType, out batchStatus))
                {
                    return null;
                }
                IEnumerable<Teachers_Batches> getRecords = rc.Teachers_Batches.Where(s => s.TeacherID == id
                        && s.Batch.Status == batchStatus).OrderBy(s => s.TeacherID).Select(s => s);
                return getRecords;
            }
            else if (choice == 2)
            {
                if (!Guid.TryParse(degforTeacher, out gd))
                {
                    return null;
                }
                if (!int.TryParse(BatchesType, out batchStatus))
                {
                    return null;
                }
                IEnumerable<Teachers_Batches> getRecords2 = rc.Teachers_Batches.Where(s => s.TeacherID == id
                        && s.Batch.Status == batchStatus
                        && s.Batch.Degree_Program.ProgramID == gd).OrderBy(s => s.TeacherID).Select(s => s);
                return getRecords2;
            }
            else
            {
                IEnumerable<Teachers_Batches> getRecords = rc.Teachers_Batches.Where(s => s.TeacherID == id
                        && s.Batch.Status == 1).OrderBy(s => s.TeacherID).Select(s => s);
                return getRecords;
            }
        }
        
        #region Report Generation
        //Student Marks Report BY TEACHER
        public List<MarksBYTeacherReportClass> GetStudentMarksListForReport(string month, string batch, Guid section, Guid degree,
            string t_id,
              string search, string year,string subjectID,string part)
        {
            List<MarksBYTeacherReportClass> mbtrc = new List<MarksBYTeacherReportClass>();

            //Guid secGuid = new Guid();
            //Guid subjGuid = new Guid();
            //Guid degGuid = new Guid();
            //int yearInNumber = 0;

            try
            {
                IEnumerable<Student_Marks> studentMarks = showResults_TeacherModelFunction(month,batch,section,degree,
                    search,t_id,year,subjectID,part);

                if (month=="none" && (year==null ||year==""))
                {
                    foreach (var p in studentMarks)
                    {

                        MarksBYTeacherReportClass marksReportView = new MarksBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            TotalMarks = p.Total_Marks ?? 0,
                            ObtainedMarks = p.Obtained_Marks ?? 0,
                            Marks_Percentage = p.Marks_Percentage,
                            //Month = p.Month,
                            //Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part=(p.Assign_Subject.Batch_Subjects_Parts.Part??1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                else if (month!="none" && (year==null ||year==""))
                {
                    foreach (var p in studentMarks)
                    {

                        MarksBYTeacherReportClass marksReportView = new MarksBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            TotalMarks = p.Total_Marks ?? 0,
                            ObtainedMarks = p.Obtained_Marks ?? 0,
                            Marks_Percentage = p.Marks_Percentage,
                            Month = p.Month,
                            //Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part = (p.Assign_Subject.Batch_Subjects_Parts.Part ?? 1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                else if (month == "none" && (year != null && year != ""))
                {
                    foreach (var p in studentMarks)
                    {

                        MarksBYTeacherReportClass marksReportView = new MarksBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            TotalMarks = p.Total_Marks ?? 0,
                            ObtainedMarks = p.Obtained_Marks ?? 0,
                            Marks_Percentage = p.Marks_Percentage,
                            //Month = p.Month,
                            Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part = (p.Assign_Subject.Batch_Subjects_Parts.Part ?? 1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                else
                {
                    foreach (var p in studentMarks)
                    {

                        MarksBYTeacherReportClass marksReportView = new MarksBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            TotalMarks = p.Total_Marks ?? 0,
                            ObtainedMarks = p.Obtained_Marks ?? 0,
                            Marks_Percentage = p.Marks_Percentage,
                            Month = p.Month,
                            Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part = (p.Assign_Subject.Batch_Subjects_Parts.Part ?? 1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                    return mbtrc; 
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Student Attendance Report BY TEACHER
        public List<AttendanceBYTeacherReportClass> GetStudentAttendanceListForReport(string month,
            string batch, string section, string degree, string t_id,
              string search, string year, string subjectID,string part)
        {
            List<AttendanceBYTeacherReportClass> mbtrc = new List<AttendanceBYTeacherReportClass>();
              
            try
            {
                Guid gd1 = Guid.Parse(section);
                Guid gd2 = Guid.Parse(degree);
                 
                IEnumerable<Students_Attendance> studentMarks = showResultsAttendance_TeacherModelFunction(month, batch, gd1, gd2,
                    search, t_id, year, subjectID,part);

                if (month == "none" && (year == null || year == ""))
                {
                    foreach (var p in studentMarks)
                    {

                        AttendanceBYTeacherReportClass marksReportView = new AttendanceBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            Total_lectures = p.Total_lectures?? 0,
                            Attended_Lectures = p.Attended_Lectures ?? 0,
                            Attendance_Percentage = p.Attendance_Percentage,
                            //Month = p.Month,
                            //Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part = (p.Assign_Subject.Batch_Subjects_Parts.Part ?? 1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                else if (month != "none" && (year == null || year == ""))
                {
                    foreach (var p in studentMarks)
                    {

                        AttendanceBYTeacherReportClass marksReportView = new AttendanceBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            Total_lectures = p.Total_lectures ?? 0,
                            Attended_Lectures = p.Attended_Lectures ?? 0,
                            Attendance_Percentage = p.Attendance_Percentage,
                            Month = p.Month,
                            //Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part = (p.Assign_Subject.Batch_Subjects_Parts.Part ?? 1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                else if (month == "none" && (year != null && year != ""))
                {
                    foreach (var p in studentMarks)
                    {

                        AttendanceBYTeacherReportClass marksReportView = new AttendanceBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            Total_lectures = p.Total_lectures ?? 0,
                            Attended_Lectures = p.Attended_Lectures ?? 0,
                            Attendance_Percentage = p.Attendance_Percentage,
                            //Month = p.Month,
                            Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part = (p.Assign_Subject.Batch_Subjects_Parts.Part ?? 1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                else
                {
                    foreach (var p in studentMarks)
                    {

                        AttendanceBYTeacherReportClass marksReportView = new AttendanceBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            Total_lectures = p.Total_lectures ?? 0,
                            Attended_Lectures = p.Attended_Lectures ?? 0,
                            Attendance_Percentage = p.Attendance_Percentage,
                            Month = p.Month,
                            Year = (p.Year ?? 0).ToString(),
                            //extra
                            Part = (p.Assign_Subject.Batch_Subjects_Parts.Part ?? 1).ToString(),
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                    }
                }
                return mbtrc; 
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public bool CheckerIfAllFieldsAreSelectedOrNot(string batch, string section, string degree, string subjectID
            ,string month,string year)
        {
            //Guid secGuid = new Guid();
            //Guid degGuid = new Guid();
            if ((batch == null || batch == "Please select" || batch == "")
                || (section == null || section == "Please select" || section == "")
            || (degree == null || degree == "Please select" || degree == "")
            || (subjectID == null || subjectID == "Please select" || subjectID == "")
                || (month == null || month == "none" || month == "")
                || (year == null|| year == ""))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<ViewStudentsBYTeacher> GetStudentsOfTeacherForReport(string search,string tid)
        {
            try
            {
                if (search == "" || search == null)
                {
                    ViewStudents_Searched(tid, search);
                    List<Registeration> listOfStds = ViewStudents(tid);
                    List<ViewStudentsBYTeacher> listViewModelStds = new List<ViewStudentsBYTeacher>();
                    foreach (var item in listOfStds)
                    {
                        listViewModelStds.Add(new ViewStudentsBYTeacher
                        {
                            Rollno = item.Rollno,
                            Name = item.Student_Profile.FirstName + " " + item.Student_Profile.LastName,
                            Gender = item.Student_Profile.Gender,
                            Degree_ProgramName = item.Batch.Degree_Program.Degree_ProgramName,
                            Part = item.Part ?? 1,
                            SectionName = item.Batch.Section.SectionName
                        });
                    }
                    return listViewModelStds;

                }
                else
                {
                    List<Registeration> listOfStds = ViewStudents_Searched(tid, search);
                    List<ViewStudentsBYTeacher> listViewModelStds = new List<ViewStudentsBYTeacher>();
                    foreach (var item in listOfStds)
                    {
                        listViewModelStds.Add(new ViewStudentsBYTeacher
                        {
                            Rollno = item.Rollno,
                            Name = item.Student_Profile.FirstName + " " + item.Student_Profile.LastName,
                            Gender = item.Student_Profile.Gender,
                            Degree_ProgramName = item.Batch.Degree_Program.Degree_ProgramName,
                            Part = item.Part ?? 1,
                            SectionName = item.Batch.Section.SectionName
                        });
                    }
                    return listViewModelStds;
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        #endregion
    }
}