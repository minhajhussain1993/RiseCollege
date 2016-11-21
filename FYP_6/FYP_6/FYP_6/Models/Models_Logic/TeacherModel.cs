using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using FYP_6.Models.ViewModels;
using System.Transactions;

namespace FYP_6.Models.Models_Logic
{
    public static class TeacherModel
    {
        private static string[] MonthsNames ={"","January","February","March","April","May","June","July",
                                         "August","September","October","November","December"};
        private static int[] MonthsNamesInInts ={0,1,2,3,4,5,6,7,8,9,10,11,12};

        static RCIS2Entities1 rc = RCIS2Entities1.getinstance();
        public static string[] WelcomeTeacherScreenResults(string id)
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
                mainScreenItems[0] = teacherInfo.Graduation_Details;
                mainScreenItems[1] = (teacherInfo.Major_Subject);
                mainScreenItems[2] = (teacherInfo.Status.ToString());
            }
            if (noOfSubjects > 0 && subjectsTaught != null)
            {
                mainScreenItems[3] = (noOfSubjects.ToString());
            }

            return mainScreenItems;
        }
        public static string ChangePassword_TeacherModelFunction(string oldpass, string newpass, string id)
        {
            using (TransactionScope t =new TransactionScope())
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
        private static string ValidatePassword(string oldentered, string newpass, string actualPassword)
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


        public static IEnumerable<Student_Marks> getResultRecords(string teacherID)
        {
            List<Student_Marks> stMarksRec = new List<Student_Marks>();
            //string t_id = teacherID.ToString();
            var getSpecificRecordsOfTeacher = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == teacherID).Select(s => s);

            //var stMarksRec = from stdAttRec in rc.Student_Marks
            //                 from recordsOfTeacher in getSpecificRecordsOfTeacher

            //                 where (stdAttRec.Assign_Subject.Batch_Subjects_Parts.PartID == recordsOfTeacher.Batch_Subjects_Parts.PartID
            //                 && stdAttRec.Assign_Subject.Registeration.Batch.SectionID == recordsOfTeacher.Batch_Subjects_Parts.Batch.SectionID
            //                 && stdAttRec.Assign_Subject.Batch_Subjects_Parts.SubjectID == recordsOfTeacher.Batch_Subjects_Parts.SubjectID
            //                 && stdAttRec.Assign_Subject.Registeration.Status == "Active")
            //                 orderby stdAttRec.Assign_Subject.Rollno
            //                 select stdAttRec;
            //return stMarksRec;
            foreach (var item in getSpecificRecordsOfTeacher)
            {
                foreach (var result in rc.Student_Marks)
                {
                    if (item.Teachers_Batches.BatchName == result.Assign_Subject.Batch_Subjects_Parts.BatchName &&
                        item.Teachers_Batches.Batch.Section.SectionID == result.Assign_Subject.Batch_Subjects_Parts.Batch.Section.SectionID
                        && item.SubjectID == result.Assign_Subject.Batch_Subjects_Parts.SubjectID)
                    {
                        stMarksRec.Add(result);
                    }
                }
            }
            return stMarksRec.OrderBy(s => s.Assign_Subject.Rollno);
        }
        public static IEnumerable<Students_Attendance> getResultRecordsAttendance(string teacherID)
        {
            List<Students_Attendance> stMarksRec = new List<Students_Attendance>();
            var getSpecificRecordsOfTeacher = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == teacherID).Select(s => s);

            //var stAttendanceRec = from stdAttRec in rc.Students_Attendance
            //                 from recordsOfTeacher in getSpecificRecordsOfTeacher

            //                 where (stdAttRec.Assign_Subject.Batch_Subjects_Parts.PartID == recordsOfTeacher.Batch_Subjects_Parts.PartID
            //                 && stdAttRec.Assign_Subject.Registeration.Batch.SectionID == recordsOfTeacher.Batch_Subjects_Parts.Batch.SectionID
            //                 && stdAttRec.Assign_Subject.Batch_Subjects_Parts.SubjectID == recordsOfTeacher.Batch_Subjects_Parts.SubjectID
            //                 && stdAttRec.Assign_Subject.Registeration.Status == "Active")
            //                 orderby stdAttRec.Assign_Subject.Rollno
            //                 select stdAttRec;
            //return stAttendanceRec;
            foreach (var item in getSpecificRecordsOfTeacher)
            {
                foreach (var result in rc.Students_Attendance)
                {
                    if (item.Teachers_Batches.BatchName == result.Assign_Subject.Batch_Subjects_Parts.BatchName &&
                        item.Teachers_Batches.Batch.Section.SectionID == result.Assign_Subject.Batch_Subjects_Parts.Batch.Section.SectionID
                        && item.SubjectID == result.Assign_Subject.Batch_Subjects_Parts.SubjectID)
                    {
                        stMarksRec.Add(result);
                    }
                }
            }
            return stMarksRec.OrderBy(s=>s.Assign_Subject.Rollno);
        }

        public static IEnumerable<Student_Marks> showResults_TeacherModelFunction(string Month, string batch, Guid SectionID, Guid degID, string search, string t_id, string year)
        {
            string MonthName = Month;
            List<Student_Marks> sectionQuery = new List<Student_Marks>();

            Match m = Regex.Match(year, "^[0-9]*$");
            if (m.Success)
            {
                if (search == "" || search == null)
                {
                    if (year == null || year == "")
                    {
                        var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                        && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                        var getTeacherSubjects = rc.Teacher_Subject
                            .Where(s => s.Teachers_Batches.TeacherID == t_id)
                            .Select(s => s.SubjectID).Distinct();
                        foreach (var item in query)
                        {
                            foreach (var tsubj in getTeacherSubjects)
                            {
                                if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                {
                                    sectionQuery.Add(item);
                                }
                            }

                        }
                        return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    }
                    else
                    {
                            int SelectedYear = int.Parse(year);
                            var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                            && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                            && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                            && s.Assign_Subject.Registeration.Status == 1
                            && s.Year == SelectedYear).Select(s => s).ToList();

                            var getTeacherSubjects = rc.Teacher_Subject
                                .Where(s => s.Teachers_Batches.TeacherID == t_id)
                                .Select(s => s.SubjectID).Distinct();

                            foreach (var item in query)
                            {
                                foreach (var tsubj in getTeacherSubjects)
                                {
                                    if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                    {
                                        sectionQuery.Add(item);
                                    }
                                }

                            }
                            return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                        
                    }
                }
                else
                {
                    if (year == null || year == "")
                    {
                        var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                       && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                       && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                       && s.Assign_Subject.Rollno == search
                       && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                        var getTeacherSubjects = rc.Teacher_Subject
                            .Where(s => s.Teachers_Batches.TeacherID == t_id)
                            .Select(s => s.SubjectID).Distinct();
                        //var searchBarQueryIfNotNull = (from tsubj in getTeacherSubjects
                        //                                                            join item in query
                        //                                                            on tsubj equals item.Assign_Subject.SubjectID
                        //                                                            orderby item.Assign_Subject.Rollno
                        //                                                            select item).ToList();
                        //return searchBarQueryIfNotNull;
                        foreach (var item in query)
                        {
                            foreach (var tsubj in getTeacherSubjects)
                            {
                                if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                {
                                    sectionQuery.Add(item);
                                }
                            }

                        }
                        return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    }
                    else
                    {
                        int SelectedYear = int.Parse(year);
                        var query = rc.Student_Marks.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    && s.Assign_Subject.Rollno == search
                    && s.Assign_Subject.Registeration.Status == 1
                    && s.Year == SelectedYear).Select(s => s).ToList();

                        var getTeacherSubjects = rc.Teacher_Subject
                            .Where(s => s.Teachers_Batches.TeacherID == t_id)
                            .Select(s => s.SubjectID).Distinct();
                        //var searchBarQueryIfNotNull = (from tsubj in getTeacherSubjects
                        //                                                            join item in query
                        //                                                            on tsubj equals item.Assign_Subject.SubjectID
                        //                                                            orderby item.Assign_Subject.Rollno
                        //                                                            select item).ToList();
                        //return searchBarQueryIfNotNull;
                        foreach (var item in query)
                        {
                            foreach (var tsubj in getTeacherSubjects)
                            {
                                if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                {
                                    sectionQuery.Add(item);
                                }
                            }

                        }
                        return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    }
                }
            }
            else
            {
                return null;
            }

        }
        public static IEnumerable<Students_Attendance> showResultsAttendance_TeacherModelFunction(string Month, string batch, Guid SectionID, Guid degID, string search, string t_id
            , string year)
        {
            //int SelectedYear = int.Parse(year);
            string MonthName = Month;
            List<Students_Attendance> sectionQuery = new List<Students_Attendance>();
            Match m = Regex.Match(year, "^[0-9]*$");
            if (m.Success)
            {
                if (search == "" || search == null)
                {
                    if (year == null || year == "")
                    {
                        var query = rc.Students_Attendance.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                        && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                        && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                        var getTeacherSubjects = rc.Teacher_Subject
                            .Where(s => s.Teachers_Batches.TeacherID == t_id)
                            .Select(s => s.SubjectID).Distinct();
                        foreach (var item in query)
                        {
                            foreach (var tsubj in getTeacherSubjects)
                            {
                                if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                {
                                    sectionQuery.Add(item);
                                }
                            }

                        }
                        return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    }
                    else
                    {
                        int SelectedYear = int.Parse(year);
                        var query = rc.Students_Attendance.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                        && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                        && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                        && s.Assign_Subject.Registeration.Status == 1
                        && s.Year == SelectedYear).Select(s => s).ToList();

                        var getTeacherSubjects = rc.Teacher_Subject
                            .Where(s => s.Teachers_Batches.TeacherID == t_id)
                            .Select(s => s.SubjectID).Distinct();
                        foreach (var item in query)
                        {
                            foreach (var tsubj in getTeacherSubjects)
                            {
                                if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                {
                                    sectionQuery.Add(item);
                                }
                            }

                        }
                        return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    }

                }
                else
                {
                    if (year == null || year == "")
                    {
                        var query = rc.Students_Attendance.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                       && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                       && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                       && s.Assign_Subject.Rollno == search
                       && s.Assign_Subject.Registeration.Status == 1).Select(s => s).ToList();

                        var getTeacherSubjects = rc.Teacher_Subject
                            .Where(s => s.Teachers_Batches.TeacherID == t_id)
                            .Select(s => s.SubjectID).Distinct();
                        foreach (var item in query)
                        {
                            foreach (var tsubj in getTeacherSubjects)
                            {
                                if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                {
                                    sectionQuery.Add(item);
                                }
                            }

                        }
                        return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    }
                    else
                    {
                        int SelectedYear = int.Parse(year);
                        var query = rc.Students_Attendance.Where(s => s.Assign_Subject.Registeration.Batch.SectionID == SectionID
                    && s.Assign_Subject.Registeration.Batch.DegreeProgram_ID == degID
                    && s.Month == MonthName && s.Assign_Subject.Batch_Subjects_Parts.BatchName == batch
                    && s.Assign_Subject.Rollno == search
                    && s.Assign_Subject.Registeration.Status == 1
                    && s.Year == SelectedYear).Select(s => s).ToList();

                        var getTeacherSubjects = rc.Teacher_Subject
                            .Where(s => s.Teachers_Batches.TeacherID == t_id)
                            .Select(s => s.SubjectID).Distinct();
                        foreach (var item in query)
                        {
                            foreach (var tsubj in getTeacherSubjects)
                            {
                                if (item.Assign_Subject.Batch_Subjects_Parts.SubjectID == tsubj)
                                {
                                    sectionQuery.Add(item);
                                }
                            }

                        }
                        return sectionQuery.OrderBy(s => s.Assign_Subject.Rollno);
                    }
                }
            }
            else
            {
                return null;
            }


        }
        public static List<Subject> getRelatedSubjects(string t_id)
        {
            var getTeacherSubjectRelatedRecord = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == t_id).Select(s => s.Subject).Distinct();

            List<Subject> tsubj = new List<Subject>();

            foreach (var item in getTeacherSubjectRelatedRecord)
            {
                tsubj.Add(new Subject { SubjectID = item.SubjectID, SubjectName = item.SubjectName });
            }
            return tsubj;
        }
        public static List<Batch> getRelatedStudentBatches(string t_id)
        {
            List<Batch> batchesInTeachers = new List<Batch>();
            var ListofDegreePrograms = rc.Teachers_Batches.Where(s => s.TeacherID == t_id).Select(s => s);
            List<Batch> joinTeacherBatchs = (from tb in ListofDegreePrograms
                                             join batch in rc.Batches
                                             on tb.BatchName equals batch.BatchName
                                             where (tb.BatchName == batch.BatchName)
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
        public static List<string> getRelatedStudentRollNos(List<Batch> batchList)
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
        public static bool CheckForDuplicateRecords(string rollno, string subjectID, string batch, string month2, string year)
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
        public static bool CheckForDuplicateRecordsInAttendance(string rollno, string subjectID, string month2
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
        public static bool AddMarksRecord(Student_Marks res, string roll, Guid subjectsID, string batch, string month2
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
        
        
        public static string AddAllMarksRecord(string batch, Nullable<DateTime> date,
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
                        else if (VM1.Any(s=>s.Obtained_Marks>total))
                        {
                            return "Please Make Sure That Obtained Marks are always less Or Equal To Total Marks";
                        }
                        else
                        {
                            CalculatePercenatageOfObtainedMarks(ref VM1, total);
                            //Upload Marks
                            foreach (var item in VM1)
                            {
                                rc.Student_Marks.Add(new Student_Marks
                                {
                                    Total_Marks = total,
                                    Obtained_Marks = item.Obtained_Marks,
                                    Marks_Percentage = item.Marks_Percentage,
                                    Month = MonthsNames[date.Value.Month],
                                    Year = date.Value.Year,
                                    SubjectAssignID = item.AssignID,
                                    Assign_Subject=rc.Assign_Subject.Where(s=>s.AssignID==item.AssignID).Select(s=>s).FirstOrDefault(),
                                    ResultID=Guid.NewGuid()
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
        private static void CalculatePercenatageOfObtainedMarks(ref IEnumerable<ViewModelMarks> VM1,int total)
        {
            double? TotalMarks = total;
            double? d = 0.0d;
            foreach (var item in VM1)
            {
                d = (item.Obtained_Marks / TotalMarks);
                d *= 100;
                item.Marks_Percentage = d.ToString();
            }
        }


        public static string AddAllAttendanceRecord(string batch, Nullable<DateTime> date,
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
                        else if(VM1.Any(s=>s.Attended_Lectures>total))
                        {
                            return "Please Make Sure That Attended Lectures are always less Or Equal To Total Lectures"; 
                        }
                        else
                        {
                            //Calculate Attendance Percentage First
                            CalculatePercenatageOfAttended_Lectures(ref VM1, total);
                            //Upload Marks
                            foreach (var item in VM1)
                            {
                                rc.Students_Attendance.Add(new Students_Attendance
                                {
                                    AttendanceID=Guid.NewGuid(),
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
                    return "Unable To Upload Attendance!";
                }
            }
        }
        private static void CalculatePercenatageOfAttended_Lectures(ref IEnumerable<ViewModelAttendance> VM1, int total)
        {
            int? totalLec = total;
            foreach (var item in VM1)
            {
                item.Attendance_Percentage = ((item.Attended_Lectures * 100) / totalLec).ToString();
            }
        }


        public static bool AddAttendanceRecord(Students_Attendance res, string roll, Guid subjectsID, string month2
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
        public static List<Registeration> ViewStudents(string t_id)
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
                                                         where (regStudents.Student_Profile.Status == 1)
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
        public static List<Registeration> ViewStudents_Searched(string t_id,string search)
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
                                                         where (regStudents.Student_Profile.Status == 1 && regStudents.Rollno.StartsWith(search))
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
        public static List<string[]> GetDataBasedOnRollnosForViewing(string id)
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
        public static List<string> getSpecificSearchRecord(string rollno)
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
        public static IEnumerable<Teacher_Attendance> getResultRecordsForTeacherAttendance(string t_id)
        {
            var getSpecificRecordsOfTeacher = rc.Teacher_Attendance.Where(s => s.TeacherID == t_id).Select(s => s).OrderBy(s => s.TeacherID);
            return getSpecificRecordsOfTeacher;
        }
        public static IEnumerable<Teacher_Attendance> showResultsTeacherAttendance_TeacherModelFunction(string t_id,string Month,string year)
        {
            int count = MonthsNames.ToList().Count;
            int index2 = 0;
            for (int i = 0; i < count; i++)
            {
                if (MonthsNames[i]==Month)
                {
                    index2 = i;
                    break;
                }
            }
            //int index2 = MonthsNames.Select(s=>s.IndexOf(Month)).FirstOrDefault();
            if (year==null ||year=="")
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

        public static IEnumerable<ViewModelMarks> getListofStudentsAccordingToBatch
            (string batch, string part, string subjectID)
        {
            int partID = int.Parse(part);
            Guid subjID = Guid.Parse(subjectID);

            var getteacherBatchesSubjects = rc.Teacher_Subject.Where(s => s.Teachers_Batches.BatchName == batch
                && s.SubjectID == subjID
                ).Select(s => s);

            if (getteacherBatchesSubjects==null ||getteacherBatchesSubjects.Count()==0)
            {
                return null;
            }

            else
            {
                var getStudents = rc.Assign_Subject
                .Where(s => s.Batch_Subjects_Parts.BatchName == batch
                && s.Batch_Subjects_Parts.Part == partID
                && s.Batch_Subjects_Parts.SubjectID == subjID)
                .Select(s => s);

                List<ViewModelMarks> Vm = new List<ViewModelMarks>();
                foreach (var item in getStudents)
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
                return Vm;
            }
            
        }
        public static IEnumerable<ViewModelAttendance> getListofStudentsAttendanceAccordingToBatch
            (string batch, string part, string subjectID)
        {
            int partID = int.Parse(part);
            Guid subjID = Guid.Parse(subjectID);

            var getteacherBatchesSubjects = rc.Teacher_Subject.Where(s => s.Teachers_Batches.BatchName == batch
                && s.SubjectID == subjID
                ).Select(s => s);

            if (getteacherBatchesSubjects == null || getteacherBatchesSubjects.Count() == 0)
            {
                return null;
            }
            else
            {

                var getStudents = rc.Assign_Subject
                    .Where(s => s.Batch_Subjects_Parts.BatchName == batch
                    && s.Batch_Subjects_Parts.Part == partID
                    && s.Batch_Subjects_Parts.SubjectID == subjID)
                    .Select(s => s);

                List<ViewModelAttendance> Vm = new List<ViewModelAttendance>();
                foreach (var item in getStudents)
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
                return Vm;
            }

        }
    }
}