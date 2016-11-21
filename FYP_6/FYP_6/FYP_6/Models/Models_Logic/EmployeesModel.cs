using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections;
using System.Transactions;
using System.Text;
using FYP_6.Models.ViewModels;


namespace FYP_6.Models.Models_Logic
{
    public class EmployeesModel
    {
        public static ViewModel_Registeration vmReg;

        private static string[] MonthsNames ={"","January","February","March","April","May","June","July",
                                         "August","September","October","November","December"};
        static RCIS2Entities1 rc = RCIS2Entities1.getinstance();


        #region Employee Functions
        public static string[] WelcomeEmployeesScreenResults()
        {
            string[] mainScreenItems = new string[5];
            //int t_id = int.Parse(id);
            var DegreeProgramsInCollege = rc.Degree_Program.Select(s => s).ToList();
            var SubjectsInCollege = rc.Subjects.Select(s => s).ToList();
            var studentsInCollege = rc.Student_Profile.Where(s => s.Status == 1).Select(s => s).ToList();
            var TeachersInCollege = rc.Teachers.Where(s => s.Status == "Active").Select(s => s).ToList();

            mainScreenItems[0] = DegreeProgramsInCollege.Count.ToString();
            mainScreenItems[1] = (SubjectsInCollege.Count.ToString());
            mainScreenItems[2] = (studentsInCollege.Count.ToString());
            mainScreenItems[3] = (TeachersInCollege.Count.ToString());

            return mainScreenItems;
        }
        public static string ChangePassword_EmployeeModelFunction(string oldpass, string newpass, Guid id)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {

                var getEmployee = rc.Employees.Where(s => s.EmpID == id).Select(s => s).FirstOrDefault();
                string getUserPassword = getEmployee.Password;
                string getPasswordChangeResult = ValidatePassword(oldpass, newpass, getUserPassword);

                if (getPasswordChangeResult == "")
                {
                    getEmployee.Password = newpass;
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
                    return "UnAble to Change Password";
                }
            }
            
        }
        public static string ChangePassword_AdminFunction(string oldpass, string newpass, Guid emp_id)
        {
            using (TransactionScope t =new TransactionScope())
            {
                try
                {
                    //int emp_id = int.Parse(id);
                    var getAdmin = rc.Admins.Where(s => s.ID == emp_id).Select(s => s).FirstOrDefault();
                    string getUserPassword = getAdmin.Password;
                    string getPasswordChangeResult = ValidatePassword(oldpass, newpass, getUserPassword);
                    if (getPasswordChangeResult == "")
                    {
                        getAdmin.Password = newpass;
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
                    return "Unable To Change Password";
                }   
            }
        }
        private static string ValidatePassword(string oldentered, string newpass, string actualPassword)
        {
            if (oldentered.Length <= 30)
            {
                if (newpass.Length <= 30)
                {
                    if (oldentered==null ||oldentered=="")
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
                    return "New Password must be less than 30 characters";
                }
            }
            else
            {
                return "Old Password must be less than 30 characters";
            }
        }

        #endregion

        #region Teacher Relevant Functions
        public static string getTeacherID()
        {
            long t_ID = 90000;
            string geLastTeacherID = rc.Teachers.OrderByDescending(s => s.TeacherID).Select(s => s.TeacherID).FirstOrDefault();
            if (geLastTeacherID != null)
            {
                t_ID = int.Parse(geLastTeacherID);
                t_ID++;
                return t_ID.ToString();
            }
            else
            {
                t_ID++;
                return t_ID.ToString();
            }
        }
        
        public static string UpdateTeacherRecord(string ID, Teacher std, HttpPostedFileBase file)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    var getTeacher = rc.Teachers.Where(s => s.TeacherID == ID).Select(s => s).FirstOrDefault();
                    getTeacher.Name = std.Name;
                    getTeacher.Address = std.Address;
                    getTeacher.Gender = std.Gender;
                    getTeacher.Date_of_Birth = std.Date_of_Birth;
                    getTeacher.ContactNo = std.ContactNo;
                    getTeacher.Graduation_Details = std.Graduation_Details;
                    getTeacher.Address = std.Address;
                    getTeacher.Major_Subject = std.Major_Subject;
                    getTeacher.Year_of_Graduation = std.Year_of_Graduation;
                    getTeacher.Year_of_Post_Graduation = std.Year_of_Post_Graduation;
                    getTeacher.Salary = std.Salary;
                    getTeacher.Post_Graduation_Details = std.Post_Graduation_Details;
                    getTeacher.Password = std.Password;
                    getTeacher.Martial_Status = std.Martial_Status;
                    if (file != null && file.ContentType.Contains("image") && file.ContentLength > 0 && file.ContentLength <= 3048576)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            getTeacher.Picture = array;
                        }
                    }
                    else if (file != null)
                    {
                        if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                        {
                            return "Plz Select image File less than 3 MB";
                        }
                    }
                    //UpdateModel(getStudent, new string[] { "FirstName", "LastName", "Gender", "ContactNo", "CNIC", "Address", "Nationality"
                    //,"Domicile","Date_of_Birth"});
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable To Update Record!";
                }    
            }
        }
        public static IEnumerable<Teacher> GetAllTeacherRecords()
        {
            var getRecords = rc.Teachers.Where(s => s.Status == "Active").OrderBy(s => s.TeacherID).Select(s => s);
            return getRecords;
        }
        public static IEnumerable<Teacher> getSpecificSearchRecordForTeacher(string ID, string TeacherType)
        {
            if (ID == "" || ID == null)
            {
                var query = rc.Teachers.Where(s => s.Status == TeacherType).OrderBy(s => s.TeacherID).Select(s => s);
                return query;
            }
            else
            {
                var query = rc.Teachers.Where(s => s.TeacherID.StartsWith(ID) && s.Status == TeacherType).OrderBy(s => s.TeacherID).Select(s => s);
                return query;
            }

        }
        public static IEnumerable<Teacher_Attendance> getResultRecordsForTeacherAttendance()
        {
            var getSpecificRecordsOfTeacher = rc.Teacher_Attendance.Where(s => s.Teacher.Status == "Active").Select(s => s).OrderBy(s => s.TeacherID);
            return getSpecificRecordsOfTeacher;
        }
        public static IEnumerable<Teacher_Attendance> showResultsTeacherAttendance_EmployeeModelFunction(string search,string Month,string year)
        {
            #region Previous Attendance Code
            //     var query = rc.Teacher_Subject.Where(s => s.Batch_Subjects_Parts.Batch.DegreeProgram_ID == degID
            //&& s.Batch_Subjects_Parts.Batch.SectionID == secID
            //&& s.Batch_Subjects_Parts.BatchName == batch
            //&& s.Teacher.Status == "Active").Select(s => s.ID);

            //     var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
            //                           join values in query on tAtt.CourseSubjectID equals values
            //                           select tAtt;
            //     return TeacherAttQuery;
            // }
            // else
            // {
            //     var query = rc.Teacher_Subject.Where(s => s.Batch_Subjects_Parts.Batch.DegreeProgram_ID == degID
            // && s.Batch_Subjects_Parts.Batch.SectionID == secID
            // && s.Batch_Subjects_Parts.BatchName == batch && s.TeacherID == search
            // && s.Teacher.Status == "Active").Select(s => s.ID);

            //     var TeacherAttQuery = from tAtt in rc.Teacher_Attendances
            //                           join values in query on tAtt.CourseSubjectID equals values
            //                           select tAtt;
            //     return TeacherAttQuery;
            #endregion
            if ((year == null || year == "") && Month == "None Selected")
            {
                var TeacherAttQuery = from tAtt in rc.Teacher_Attendance where tAtt.TeacherID == search select tAtt;
                return TeacherAttQuery;
            }
            else if ((year != null || year != "") && Month == "None Selected")
            {
                int yearInNumbers = 0;
                if (int.TryParse(year,out yearInNumbers))
                {
                    var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                          where tAtt.TeacherID == search
                                          && tAtt.Date.Year == yearInNumbers
                                          select tAtt;
                    return TeacherAttQuery;    
                }
                else
                {
                    return null;
                }
                
            }
            else if ((year == null || year == "") && Month != "None Selected")
            {
                int monthNo = GetMonthNoForMonthName(Month);

                if (monthNo==13)
                {
                    return null;
                }
                else
                {
                    var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                          where tAtt.TeacherID == search
                                          && tAtt.Date.Month==monthNo
                                          select tAtt;
                    return TeacherAttQuery;
                }
                
            }
            else
            {
                int yearInNumbers = 0;
                int monthNo = GetMonthNoForMonthName(Month);

                if (int.TryParse(year, out yearInNumbers) || monthNo != 13)
                {
                    var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                          where tAtt.TeacherID == search
                                          && tAtt.Date.Year == yearInNumbers
                                          && tAtt.Date.Month==monthNo
                                          select tAtt;
                    return TeacherAttQuery;
                }
                else
                {
                    return null;
                }
            }
            
        }
        public static string EditTeacherAttWithDateAndStatus(Teacher_Attendance tAtt, string TeacherID, string Date, string PresentStatus)
        {
            try
            {
                DateTime dt = DateTime.Parse(Date);

                var getUpdatedRecord = rc.Teacher_Attendance.Where(s => s.Date == dt
                    && s.TeacherID == TeacherID).Select(s => s).FirstOrDefault();

                getUpdatedRecord.Present = PresentStatus;
                rc.SaveChanges();
                return "OK";
            }
            catch (Exception)
            {
                return "Unable to Update Record";
            }
        }
        public static string AddTeacherAtt(Teacher_Attendance tRec, string t_id, Nullable<DateTime> date)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    //int subjID = int.Parse(subjects);
                    //int sectionID = int.Parse(section);

                    //var SearchTeacherSubjectsAndBatches =
                    //    rc.Teacher_Subject.Where(s => s.Batch_Subjects_Parts.Batch.BatchName == batch
                    //    && s.Batch_Subjects_Parts.Batch.SectionID == sectionID
                    //    && s.TeacherID == t_id
                    //    && s.Batch_Subjects_Parts.SubjectID == subjID
                    //    && s.Teacher.Status == "Active").Select(s => s).FirstOrDefault();

                    if (rc.Teacher_Attendance.Any(s => s.TeacherID == t_id && s.Date == date))
                    {
                        return "This Record is Already present";
                    }
                    else
                    {
                        //int listID = GetBatchIDAutoIncrementWali();
                        //tRec.CourseSubjectID = int.Parse(SearchTeacherSubjectsAndBatches.Batch_Subject_ID.ToString());
                        tRec.TeacherID = t_id;
                        tRec.Date = date.Value;
                        rc.Teacher_Attendance.Add(tRec);
                        t.Complete();
                        rc.SaveChanges();
                        return "OK";
                    }

                }
                catch (Exception e)
                {
                    return "Unable to Add Records" + " " + e.Message;
                }
            }
        }
        public static IEnumerable<Teacher_Subject> GetAllTeacher_SubjectsRecords()
        {
            var rec = from tsubj in rc.Teacher_Subject
                      where tsubj.Teachers_Batches.Teacher.Status == "Active"
                      orderby tsubj.Teachers_Batches.TeacherID
                      select tsubj;
            //var getRecords = rc.Teacher_Subject.Where(s => s.Teachers_Batches.Teacher.Status == "Active")
              //  .OrderBy(s => s.Teachers_Batches.Teacher).Select(s => s).ToList();
            return rec;
        }
        public static IEnumerable<Teacher_Subject> GetAllSearchSpecificTeacher_SubjectsRecords(Guid degree, Guid section, string batch, string teacherID)
        {
            if (teacherID == "" || teacherID == null)
            {
                var query = from tsubj in rc.Teacher_Subject
                      where tsubj.Teachers_Batches.BatchName== batch 
                      && tsubj.Teachers_Batches.Batch.SectionID == section
                      orderby tsubj.Teachers_Batches.TeacherID
                      select tsubj;
                //var query = rc.Teacher_Subject.Where(s => s.Teachers_Batches.BatchName == batch
                //    && s.Teachers_Batches.Batch.SectionID == section).OrderBy(s => s.ID).Select(s => s);
                return query;
            }
            else
            {
                var query = from tsubj in rc.Teacher_Subject
                            where tsubj.Teachers_Batches.BatchName == batch
                            && tsubj.Teachers_Batches.Batch.SectionID == section
                            && tsubj.Teachers_Batches.TeacherID.StartsWith(teacherID)
                            orderby tsubj.Teachers_Batches.TeacherID
                            select tsubj;

                //var query = rc.Teacher_Subject.Where(
                //    s => s.Teachers_Batches.BatchName == batch
                //    && s.Teachers_Batches.Batch.SectionID == section
                //    && s.Teachers_Batches.TeacherID.StartsWith(teacherID)
                //    ).OrderBy(s => s.ID).Select(s => s);
                return query;
            }
            //var getRecords = rc.Teacher_Subject.Where(s => s.Teacher.Status == "Active").OrderBy(s => s.TeacherID).Select(s => s);
            //return getRecords;
        }
        public static IEnumerable<Teacher_Subject> GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(string teacherID)
        {
            if (teacherID == "" || teacherID == null)
            {
                return null;
            }
            else
            {
                var query = from tsubj in rc.Teacher_Subject
                            where tsubj.Teachers_Batches.TeacherID.StartsWith(teacherID)
                            orderby tsubj.Teachers_Batches.TeacherID
                            select tsubj;
                //var query = rc.Teacher_Subject.Where(
                //    s => s.Teachers_Batches.TeacherID.StartsWith(teacherID)
                //    ).OrderBy(s => s.ID).Select(s => s);
                return query;
            }
        }
        public static IEnumerable<Teachers_Batches> GetAllTeacher_batchesRecords()
        {
            var getRecords = rc.Teachers_Batches.Where(s => s.Teacher.Status == "Active").OrderBy(s => s.TeacherID).Select(s => s);
            return getRecords;
        }
        //public static IEnumerable<Teachers_Batches> GetAllSearchSpecificTeacher_BatchesRecords(int degree, int section, string batch, string teacherID)
        public static IEnumerable<Teachers_Batches> GetAllSearchSpecificTeacher_BatchesRecords(string teacherID)
        {
            if (teacherID == "" || teacherID == null)
            {
                var query = rc.Teachers_Batches.OrderBy(s => s.TeacherID).Select(s => s);
                return query;
            }
            else
            {
                var query = rc.Teachers_Batches.Where(
                    s => s.TeacherID.StartsWith(teacherID)
                    ).OrderBy(s => s.TeacherID).Select(s => s);
                return query;
            }
        }
        //public static int GetNewTeacherSubjectID()
        //{
        //    int secId = 0;
        //    var getLastSection = rc.Teacher_Subject.OrderByDescending(s => s.ID).Select(s => s).FirstOrDefault();
        //    if (getLastSection != null)
        //    {
        //        secId = getLastSection.ID;
        //        secId++;
        //        return secId;
        //    }
        //    else
        //    {
        //        secId++;
        //        return secId;
        //    }
        //}
        //public static int GetNewTeacherBatchID()
        //{
        //    int secId = 0;
        //    var getLastSection = rc.Teachers_Batches.OrderByDescending(s => s.ID).Select(s => s).FirstOrDefault();
        //    if (getLastSection != null)
        //    {
        //        secId = getLastSection.ID;
        //        secId++;
        //        return secId;
        //    }
        //    else
        //    {
        //        secId++;
        //        return secId;
        //    }
        //}
        public static string AddTeacherSubject(string teacherID, string degree, string section, string batch, string subjectID, string part)
        {
            //deg.LevelID = level;
            try
            {
                bool checkIfTeacherIsTeachingTheBatch = rc.Teachers_Batches.Any(s => s.TeacherID == teacherID && s.BatchName == batch);

                if (degree == null || degree == "Please select"
                    || section == null || section == "Please select"
                    || batch == null || batch == "Please select"
                    || teacherID == null || teacherID == "")
                {
                    return "Plz Select All the Fields!";
                }
                else if (checkIfTeacherIsTeachingTheBatch == false)
                {
                    return "The Teacher is not currently Teaching this batch!";
                }
                else
                {
                    #region Old Code of Teacher_Subject Assignment
                    //int subjID = int.Parse(subjectID);
                    //int sectionID = int.Parse(section);
                    //int partID = int.Parse(part);
                    //int degID = int.Parse(degree);
                    //var getDegreeName = rc.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s.Degree_ProgramName).FirstOrDefault();
                    //var getSectionName = rc.Sections.Where(s => s.SectionID == sectionID).Select(s => s.SectionName).FirstOrDefault();
                    //var getSubjectName = rc.Subjects.Where(s => s.SubjectID == subjID).Select(s => s.SubjectName).FirstOrDefault();



                    //if (rc.Teacher_Subject.Any(s => s.Batch_Subjects_Parts.BatchName == batch && s.TeacherID == teacherID
                    //    && s.Batch_Subjects_Parts.SubjectID == subjID
                    //    && s.Batch_Subjects_Parts.Batch.DegreeProgram_ID == degID
                    //    && s.Batch_Subjects_Parts.Batch.SectionID == sectionID
                    //    && s.Batch_Subjects_Parts.PartID == partID
                    //    && s.Teacher.Status == "Active"))
                    //{
                    //    return "This Teacher Already Teaches this Subject to Batch: " + batch;
                    //}
                    //else
                    //{
                    //    //Check to see if there is such a batch with specified credentials in database
                    //    bool checkerForSubjectWithPArtExists = false;
                    //    if (rc.Batch_Subjects_Parts.Any(s => s.BatchName == batch
                    //        && s.SubjectID == subjID
                    //        && s.Batch.DegreeProgram_ID == degID
                    //        && s.Batch.SectionID == sectionID
                    //        && s.PartID == partID))
                    //    {
                    //        checkerForSubjectWithPArtExists = true;
                    //    }
                    //    if (checkerForSubjectWithPArtExists)
                    //    {
                    //        //int TeacherSubjID = GetNewTeacherSubjectID();

                    //        Teacher_Subject ts = new Teacher_Subject();
                    //        //ts.ID = TeacherSubjID;
                    //        ts.TeacherID = teacherID;
                    //        var bpsID = rc.Batch_Subjects_Parts.Where(s => s.BatchName == batch
                    //            && s.SubjectID == subjID
                    //            && s.Batch.DegreeProgram_ID == degID
                    //            && s.Batch.SectionID == sectionID).Select(s => s).FirstOrDefault();
                    //        ts.Batch_Subjects_Parts = bpsID;
                    //        ts.Teacher = rc.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();
                    //        ts.Batch_Subject_ID = bpsID.ID;

                    //        //ts.Batch_Subjects_Parts.Batch.DegreeProgram_ID = degID;
                    //        ts.Batch_Subjects_Parts.Batch = rc.Batches.Where(s => s.BatchName == batch).Select(s => s).FirstOrDefault();
                    //        ts.Batch_Subjects_Parts.Batch.Degree_Program = rc.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
                    //        ts.Batch_Subjects_Parts.Batch.Section = rc.Sections.Where(s => s.SectionID == sectionID).Select(s => s).FirstOrDefault();
                    //        ts.Batch_Subjects_Parts.Subject = rc.Subjects.Where(s => s.SubjectID == subjID).Select(s => s).FirstOrDefault();

                    //        //ts.Batch_Subjects_Parts.Batch.SectionID = sectionID;
                    //        //ts.Batch_Subjects_Parts.SubjectID = subjID;


                    //        rc.Teacher_Subject.Add(ts);
                    //        rc.SaveChanges();

                    //        return "OK";
                    //    }
                    //    else
                    //    {
                    //        return "Degree Program " + getDegreeName + " with batch " + batch + " Doesn't have Subject: " + getSubjectName + " in Part:" + part;
                    //    }

                    //}
                    #endregion
                    return "OK";
                }

            }
            catch (Exception)
            {
                return "Unable To Assign Subjects!";
            }
        }
        public static string AddTeacherBat(string teacherID, string degree, string section, string batch)
        {
            //deg.LevelID = level;
            try
            {

                if (degree == null || degree == "Please select"
                    || section == null ||
                    batch == null || teacherID == null)
                {
                    return "Plz Select All the Fields";
                }
                else
                {
                    Guid sectionID = Guid.Parse(section);
                    Guid degID = Guid.Parse(degree);

                    if (rc.Teachers_Batches.Any(s => s.BatchName == batch && s.TeacherID == teacherID
                        && s.Batch.DegreeProgram_ID == degID
                        && s.Batch.SectionID == sectionID
                        && s.Teacher.Status == "Active"))
                    {
                        return "This Teacher Already Teaches to Batch: " + batch;
                    }
                    else
                    {
                        //Get Teacher Record
                        var TeacherGet = rc.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();

                        //Get Batch Record
                        var batchGet = rc.Batches.Where(s => s.BatchName == batch).Select(s => s).FirstOrDefault();

                        Teachers_Batches ts = new Teachers_Batches();
                        //ts.ID = TeacherSubjID;
                        ts.TeacherID = teacherID;
                        ts.BatchName = batch;
                        ts.ID = Guid.NewGuid();
                        ts.Teacher = TeacherGet;
                        ts.Batch = batchGet;

                        
                        rc.Teachers_Batches.Add(ts);
                        rc.SaveChanges();

                        return "OK";
                    }
                }

            }
            catch (Exception e)
            {
                return "Unable To Add Batch Record " + e.Message;
            }
        }
        
        public static string EnrollTeacherForCollege(Teacher tRec, HttpPostedFileBase file)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    if (file != null && file.ContentType.Contains("image") && file.ContentLength > 0 && file.ContentLength <= 3048576)
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.InputStream.CopyTo(ms);
                                byte[] array = ms.GetBuffer();
                                tRec.Picture = array;
                            }
                        }
                        catch (Exception)
                        {
                            return "Unable to Upload Image!";
                        }
                    }
                    else if (file !=null)
                    {
                        if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                        {
                            return "Plz Select image File less than 3 MB";   
                        }
                    }
                    tRec.Status = "Active";
                    rc.Teachers.Add(tRec);
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable to Add Record!";
                }
            }
        }

        public static IEnumerable<Batch_Subjects_Parts> getBPSForAssigningToTeachers(Batch batch)
        {
            var batchSubj = rc.Batch_Subjects_Parts.Where(s => s.BatchName == batch.BatchName).OrderBy(s => s.SubjectID).Select(s => s);
            return batchSubj;
        }

        //Update Assigning Subjects To Teachers
        #region Previous Code Of Assigning Subjects To Teachers
        //public static string SubjectAddToBPS(IEnumerable<Guid> subj, Batch batch, string teacherID)
        //{
        //    try
        //    {
        //        //Get Teacher Record First
        //        var TeacherGet = rc.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();

        //        //Get List of All the selected Subjects for particular batch
        //        List<Batch_Subjects_Parts> CompleteSubjList = rc.Batch_Subjects_Parts
        //            .Where(s => subj.Contains(s.SubjectID.Value) && s.BatchName == batch.BatchName).ToList();

        //        //Get All Teacher Subjects for particular teacher and batch
        //        List<Teacher_Subject> tSubjParticularTeacher =
        //            rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == teacherID
        //            && s.Teachers_Batches.BatchName == batch.BatchName)
        //            .Select(s => s).ToList();

        //        //Get Records from Batch_Subject_Part Table for the particular batch
        //        List<Batch_Subjects_Parts> getBatchSubjects = rc.Batch_Subjects_Parts.
        //            Where(s => s.BatchName == batch.BatchName).Select(s => s).ToList();


        //        //Match Subjects That are same in both the new selection and the old subjects in batch
        //        var JoinedSubjectInDegreeAndSelectedSubjects = from subjOfDegree in tSubjParticularTeacher
        //                                                       join selectedSubjects in CompleteSubjList

        //                                                       on subjOfDegree.Batch_Subjects_Parts.SubjectID
        //                                                       equals selectedSubjects.SubjectID

        //                                                       orderby selectedSubjects.SubjectID
        //                                                       select selectedSubjects;
        //        //Get All the Subjects that are different
        //        var deleteUnMatchedOrDifferentSubjects =
        //            tSubjParticularTeacher.Where(s =>
        //                !subj.Contains(s.Batch_Subjects_Parts.SubjectID.Value))
        //            .Select(s => s);

        //        if (JoinedSubjectInDegreeAndSelectedSubjects.Count() == 0)
        //        {
        //            //If there are no subjects matched then delete previous subjects of batch of specific part
        //            foreach (var item in tSubjParticularTeacher)
        //            {
        //                rc.Teacher_Subject.Remove(item);
        //            }
        //            rc.SaveChanges();
        //            //Add the new subjects
        //            foreach (var item in CompleteSubjList)
        //            {
        //                rc.Teacher_Subject.Add(
        //                    new Teacher_Subject
        //                    {
        //                        Batch_Subjects_Parts = item,
        //                        TeacherID = teacherID,
        //                        Batch_Subject_ID = item.ID,
        //                        ID=Guid.NewGuid(),
        //                        Teacher=TeacherGet
        //                    });
        //            }
        //            rc.SaveChanges();
        //            return "OK";
        //        }
        //        else if (JoinedSubjectInDegreeAndSelectedSubjects.Count() == tSubjParticularTeacher.Count())
        //        {
        //            //if the same subjects are selected plus there are also other subjects that are selected Or no subject is selected
        //            //Here assigning new subjects to already assigned subjects in the batch
        //            if (AssignNewSubjectsToBPS(CompleteSubjList, tSubjParticularTeacher, batch, TeacherGet))
        //            {
        //                return "OK";
        //            }
        //            else
        //            {
        //                return "Unable to Update Subjects!";
        //            }
        //        }
        //        else
        //        {
        //            //Remove previous subjects that are different
        //            foreach (var item in deleteUnMatchedOrDifferentSubjects)
        //            {
        //                rc.Teacher_Subject.Remove(item);
        //            }
        //            rc.SaveChanges();
        //            //Assign New subjects but keep the old ones intact
        //            if (AssignNewSubjectsToBPS(CompleteSubjList, tSubjParticularTeacher, batch, TeacherGet))
        //            {
        //                return "OK";
        //            }
        //            else
        //            {
        //                return "Unable to Update Subjects!";
        //            }
        //        }
        //        #region Old Code
        //        //    bool checker = true;

        //        //    foreach (var item in getAllTheSubjectOfTheDegree)
        //        //    {
        //        //        foreach (var item2 in listToDelete)
        //        //        {
        //        //            //Check If the Subject is already assigned
        //        //            if (item.SubjectID == item2.SubjectID)
        //        //            {
        //        //                checker = false;
        //        //                break;
        //        //            }
        //        //        }

        //        //        if (checker == true)
        //        //        {
        //        //            //If not Assign The subject
        //        //            rc.Degree_Subject.Add(new Degree_Subject
        //        //            {
        //        //                Degree_Program = deg,
        //        //                Subject = rc.Subjects.Where(s => s.SubjectID == item.SubjectID).Select(s => s).FirstOrDefault(),
        //        //                DegreeID = degreeID,
        //        //                SubjectID = item.SubjectID
        //        //            });
        //        //            checker = false;
        //        //        }
        //        //    }
        //        //    rc.SaveChanges();
        //        //    t.Complete();
        //        //    return "OK";
        //        //}
        //        #endregion
        //    }
        //    catch
        //    {
        //        return "Unable to Update Subjects!";
        //    }

        //}
        //public static bool AssignNewSubjectsToBPS(List<Batch_Subjects_Parts> CompleteSubjList,
        //    List<Teacher_Subject> getBPSSubjects, Batch batchForEditing, Teacher teach)
        //{
        //    bool checker = true;
            
        //    foreach (var item in CompleteSubjList)
        //    {
        //        foreach (var item2 in getBPSSubjects)
        //        {
        //            //Check If the Subject is already assigned
        //            if (item.SubjectID == item2.Batch_Subjects_Parts.SubjectID)
        //            {
        //                checker = false;
        //                break;
        //            }
        //        }

        //        if (checker == true)
        //        {
        //            //If not Assign The subject
        //            rc.Teacher_Subject.Add(
        //                    new Teacher_Subject
        //                    {
        //                        Batch_Subjects_Parts = item,
        //                        Batch_Subject_ID = item.ID,
        //                        TeacherID = teach.TeacherID,
        //                        ID=Guid.NewGuid(),
        //                        Teacher = teach
        //                    });
        //            checker = false;
        //        }
        //        checker = true;
        //    }
        //    rc.SaveChanges();
        //    return true;
        //}
        //public static bool DeleteAllSubjectsToBPS(Batch batch, string teacherID)
        //{
        //    using (TransactionScope t = new TransactionScope())
        //    {
        //        try
        //        {
        //            foreach (var item in rc.Teacher_Subject)
        //            {
        //                if (item.TeacherID == teacherID)
        //                {
        //                    rc.Teacher_Subject.Remove(item);
        //                }
        //            }
        //            rc.SaveChanges();
        //            t.Complete();
        //            return true;
        //        }
        //        catch (Exception)
        //        {
        //            return false;
        //        }
        //    }
        //}

        public static bool DeleteBatchPlusBatchRelatedSubjectsOfTeacher(List<Teachers_Batches> listToDelete)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    foreach (var item in listToDelete)
                    {
                        rc.Teachers_Batches.Remove(item);
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

#endregion

        public static string AssignTeacherSubjects_NewFunction(IEnumerable<Guid> subjSelect, Teachers_Batches tbRec)
        {
            if (subjSelect == null)
            {
                return "Plz Select At least One Subject To Continue!";
            }
            // Check If Any Teacher is Teaching thisSubject
            var checkerOneIfAnyTeacherisTeachingthisSubject = rc.Teacher_Subject
                .Where(s => s.Teachers_Batches.BatchName == tbRec.BatchName
                && subjSelect.Contains(s.SubjectID.Value)
                && s.Teachers_Batches.TeacherID != tbRec.Teacher.TeacherID).Select(s => s).FirstOrDefault();

            
            

            if (checkerOneIfAnyTeacherisTeachingthisSubject!=null)
            {
                return @"The Subject "+checkerOneIfAnyTeacherisTeachingthisSubject.Subject.SubjectName
                    +" of Batch "+checkerOneIfAnyTeacherisTeachingthisSubject.Teachers_Batches.BatchName
                    +"is being taught by Teacher: "+checkerOneIfAnyTeacherisTeachingthisSubject.Teachers_Batches.TeacherID;
            }
            else
            {
                using (TransactionScope t=new TransactionScope())
                {
                    try
                    {
                        var getTeacherSubjects = rc.Teacher_Subject.Where(s=>s.Teacher_BatchID==tbRec.ID).Select(s=>s).ToList();
                        //Remove Previous Subjects, Assign New Subjects

                        foreach (var item in getTeacherSubjects)
                        {
                            rc.Teacher_Subject.Remove(item);
                        }

                        foreach (var item in subjSelect)
                        {
                            rc.Teacher_Subject.Add(new Teacher_Subject
                            {
                                ID=Guid.NewGuid(),
                                Teacher_BatchID=tbRec.ID,
                                Teachers_Batches=tbRec,
                                SubjectID=item,
                                Subject=rc.Subjects.Where(s=>s.SubjectID==item).Select(s=>s).FirstOrDefault()
                            });
                        }
                        rc.SaveChanges();
                        t.Complete();
                        return "OK";
                    }
                    catch (Exception)
                    {
                        return "Unable to Assign Subjects";
                    }
                }
            }
            #region Old Teacher Subject Assignment Code
            //if (subjSelect == null || batchNames == null)
            //{
            //    return "Plz Select At least one Batch And Subject For Subject Registeration!";

            //}
            //else if (teacherID == null||teacherID=="")
            //{
            //    return "Plz Enter Teacher ID to Assign Subjects";

            //}
            //else
            //{
            //    var getTeacherRec = rc.Teachers.Where(s => s.TeacherID == teacherID).Select(s => s).FirstOrDefault();
            //    if (getTeacherRec==null)
            //    {
            //        return "Teacher Record Not Found!";
            //    }
            //    else
            //    {
            //        var getSpecificTeacherBatchSubjects = rc.Teacher_Subject.Where(s => s.Teachers_Batches.TeacherID == teacherID).Select(s => s);
            //        foreach (var item in batchNames)
            //        {
            //            var getBSPs = rc.Batch_Subjects_Parts.Where(s => s.BatchName == item).Select(s => s);
            //            var getAllTeacherSubjectsForBatchForSubjectChecking=rc.Teacher_Subject.Where(s => s.Teachers_Batches.BatchName == item).Select(s => s);

            //            foreach (var item2 in subjSelect)
            //            {
            //                if (!getBSPs.Any(s=>s.SubjectID==item2))
            //                {
            //                    return "Batch " + item + " Does not study " + rc.Subjects.Where(s => s.SubjectID == item2).Select(s => s.SubjectName).FirstOrDefault() + ", Plz Select Another Subject For this Batch! ";
            //                }
            //                if (getAllTeacherSubjectsForBatchForSubjectChecking.Any(s => s.Teachers_Batches.TeacherID != teacherID && s.SubjectID == item2))
            //                {
            //                    return "The Subject " + rc.Subjects.Where(s => s.SubjectID == item2).Select(s => s.SubjectName).FirstOrDefault()+" of Batch "+item +" is being taught by another Teacher!";
            //                }
            //            }
            //        }
            //        using (TransactionScope t=new TransactionScope())
            //        {
            //            try
            //            {
            //                foreach (var item in batchNames)
            //                {
            //                    var getTeacherBatch=rc.Teachers_Batches.Where(s => s.BatchName == item && s.TeacherID == teacherID).Select(s => s).FirstOrDefault();
            //                    foreach (var item2 in subjSelect)
            //                    {
            //                        if (!getSpecificTeacherBatchSubjects.Any(s => s.Teachers_Batches.BatchName 
            //                            == item && s.SubjectID == item2))
            //                        {
            //                            rc.Teacher_Subject.Add(new Teacher_Subject
            //                            {
            //                                ID=Guid.NewGuid(),
            //                                SubjectID=item2,
            //                                Teacher_BatchID=getTeacherBatch.ID,
            //                                Teachers_Batches=getTeacherBatch,
            //                                Subject = rc.Subjects.Where(s => s.SubjectID == item2).Select(s => s).FirstOrDefault()
            //                            });
            //                        }
            //                    }
            //                }
            //                rc.SaveChanges();
            //                t.Complete();
            //                return "OK";
            //            }
            //            catch (Exception)
            //            {
            //                return "Unable To Assign Subjects to "+teacherID;
            //            }
            //        }
                    
            //        //return "OK";
            //    }
            //}
            #endregion
        }
        public static IEnumerable<Teacher_Attendance> GetAllTeacherIDsNamesForUploadingTeacherAttendance()
        {
            List<Teacher_Attendance> tAtt = new List<Teacher_Attendance>();
            foreach (var item in rc.Teachers)
            {
                tAtt.Add(new Teacher_Attendance
                {
                    Teacher = item,
                    TeacherID = item.TeacherID
                    
                });
            }
            return tAtt;
        }

        public static string NewTeacherAttendancesAdditionCode(IEnumerable<string> status,
            Nullable<DateTime> date, IEnumerable<string> T_IDS)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    if (rc.Teacher_Attendance.Any(s => s.Date == date && T_IDS.FirstOrDefault() == s.TeacherID))
                    {
                        return "Attendance of Date " + date.Value + " has already been Uploaded!";
                    }
                    else
                    {
                        var ListOfStatus = status.ToList();
                        var ListOfT_IDS = T_IDS.ToList();
                        var getListOfTeacherRefrences =
                            rc.Teachers.Where(s => T_IDS.Contains(s.TeacherID)).ToList();

                        for (int i = 0; i < ListOfStatus.Count; i++)
                        {
                            string st = ListOfT_IDS[i];
                            rc.Teacher_Attendance.Add(new Teacher_Attendance
                            {
                                Date = date.Value,
                                Present = ListOfStatus[i],
                                Teacher = getListOfTeacherRefrences[i],
                                TeacherID = getListOfTeacherRefrences[i].TeacherID
                                ,ID=Guid.NewGuid()
                                
                            });
                        }
                        rc.SaveChanges();
                        t.Complete();
                        return "OK";
                    }
                }
                catch (Exception)
                {
                    return "Unable to Mark Teacher Attendance! Plz Try Again!";
                }
            }
        }


        //Subject Updation in Degree Subjects

        public static string TeacherBatchAssign(IEnumerable<string> listOfBatches,Teacher teacher)
        {
            try
            {
                //int degreeID = int.Parse(degID);

                //Get List of All the selected Subjects
                List<Batch> listToDelete = rc.Batches.Where(s => listOfBatches.Contains(s.BatchName)).ToList();

                //Get Degree Program Reference
                //Degree_Program deg = rc.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();

                //Get All the assigned Subjects to DegreeProgram
                List<Teachers_Batches> getAllTheBatchesofTeacher = rc.Teachers_Batches.Where(s=>s.TeacherID==teacher.TeacherID)
                    .Select(s => s).ToList();

                //Match Subjects That are same in both the new selection and the old subjects in degree
                var JoinedSubjectInDegreeAndSelectedSubjects = from subjOfDegree in getAllTheBatchesofTeacher
                                                               join selectedSubjects in listToDelete
                                                               on subjOfDegree.BatchName equals selectedSubjects.BatchName
                                                               orderby selectedSubjects.BatchName
                                                               select selectedSubjects;
                //Get All the Subjects that are different
                var deleteUnMatchedOrDifferentSubjects =
                    getAllTheBatchesofTeacher.Where(s => !listOfBatches.Contains(s.BatchName))
                    .Select(s => s);
                //var deleteUnMatchedOrDifferentSubjects = from selectedSubjects in listToDelete 
                //                                         from subjOfDegree in getDegreeSubjects 
                //                                         where(subjOfDegree.SubjectID != selectedSubjects.SubjectID)
                //                                         select subjOfDegree;

                if (JoinedSubjectInDegreeAndSelectedSubjects.Count() == 0)
                {
                    using (TransactionScope t=new TransactionScope())
                    {
                        try
                        {
                            foreach (var item in getAllTheBatchesofTeacher)
                            {
                                rc.Teachers_Batches.Remove(item);
                            }
                            rc.SaveChanges();

                            foreach (var item in listToDelete)
                            {
                                rc.Teachers_Batches.Add(
                                    new Teachers_Batches
                                    {
                                        BatchName = item.BatchName,
                                        Batch = item,
                                        TeacherID = teacher.TeacherID,
                                        Teacher = teacher,
                                        ID = Guid.NewGuid()

                                    });
                            }
                            rc.SaveChanges();
                            t.Complete();
                            return "OK";
                        }
                        catch (Exception)
                        {
                            return "Unable to Update Teacher Batches!";
                        }
                    }
                    
                }
                else if (JoinedSubjectInDegreeAndSelectedSubjects.Count() == getAllTheBatchesofTeacher.Count())
                {
                    using (TransactionScope t=new TransactionScope())
                    {
                        try
                        {
                            if (AssignNewBatchesToTeacher(listToDelete, getAllTheBatchesofTeacher, teacher))
                            {
                                rc.SaveChanges();
                                t.Complete();
                                return "OK";
                            }
                            else
                            {
                                return "Unable to Update Teacher Batches!";
                            }
                        }
                        catch (Exception)
                        {
                            return "Unable to Update Teacher Batches!";
                        }
                    }                    
                }
                else
                {
                    //Remove previous subjects that are different
                    using (TransactionScope t=new TransactionScope())
                    {
                        try
                        {
                            foreach (var item in deleteUnMatchedOrDifferentSubjects)
                            {
                                rc.Teachers_Batches.Remove(item);
                            }
                            rc.SaveChanges();
                            t.Complete();
                        }
                        catch (Exception)
                        {
                            return "Unable to Update Teacher Batches!";
                        }
                    
                    }

                    if (AssignNewBatchesToTeacher(listToDelete, getAllTheBatchesofTeacher, teacher))
                    {
                        return "OK";
                    }
                    else
                    {
                        return "Unable to Update Teacher Batches!";
                    }
                }
            }
            catch
            {
                return "Unable to Update Teacher Batches!";
            }

        }

        public static bool AssignNewBatchesToTeacher(List<Batch> selectedListofBatches,
            List<Teachers_Batches> getTeacherBatches, Teacher teacher)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    bool checker = true;

                    foreach (var item in selectedListofBatches)
                    {
                        foreach (var item2 in getTeacherBatches)
                        {
                            //Check If the Subject is already assigned
                            if (item.BatchName == item2.BatchName)
                            {
                                checker = false;
                                break;
                            }
                        }

                        if (checker == true)
                        {
                            //If not Assign The subject
                            rc.Teachers_Batches.Add(
                                    new Teachers_Batches
                                    {
                                        BatchName = item.BatchName,
                                        Batch = item,
                                        TeacherID = teacher.TeacherID,
                                        Teacher = teacher,
                                        ID = Guid.NewGuid()

                                    });
                            checker = false;
                        }
                        checker = true;
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

        
        public static bool DeleteAllTeacher_Batches(Teacher teacher)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    foreach (var item in rc.Teachers_Batches)
                    {
                        if (item.TeacherID == teacher.TeacherID)
                        {
                            rc.Teachers_Batches.Remove(item);
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

        
        public static string DeleteTeacherRecords_EMP_ModelFunction(IEnumerable<string> deleteTeacher)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Teacher> listToDelete = rc.Teachers.Where(s => deleteTeacher.Contains(s.TeacherID)).ToList();

                    foreach (var item in listToDelete)
                    {
                        if (item.Status == "UnActive")
                        {
                            rc.Teachers.Remove(item);
                        }
                        else
                        {
                            item.Status = "UnActive";
                        }
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable to Delete Records";
                }
            }
        }
        public static string DeleteTeacherAttendanceRecords_EMP_ModelFunction(IEnumerable<Guid> deleteTatt)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Teacher_Attendance> listToDelete = rc.Teacher_Attendance.Where(s => deleteTatt.Contains(s.ID)).ToList();
                    foreach (var item in listToDelete)
                    {
                        rc.Teacher_Attendance.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable to Delete Records";
                }
            }
        }
        public static string DeleteTeacherSubjectRecords_EMP_ModelFunction(IEnumerable<Guid> deleteTSub)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Teacher_Subject> listToDelete = rc.Teacher_Subject.Where(s => deleteTSub.Contains(s.ID)).ToList();

                    foreach (var item in listToDelete)
                    {
                        rc.Teacher_Subject.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable to Delete Records";
                }
            }
        }


        #endregion

        #region Student Relevant Functions
        public static IEnumerable<Registeration> GetAllStudentRecords()
        {
            var getRecords = rc.Registerations.Where(s => s.Student_Profile.Status == 1)
                .OrderBy(s => s.Rollno).Select(s => s);
            return getRecords;
        }
        public static IEnumerable<Registeration> getSpecificSearchRecord(string rollno, int? StudentType)
        {
            if (rollno == null || rollno == "")
            {
                if (StudentType != null)
                {
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType.Value)
                       .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }
                else
                {
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == 1)
                       .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }
            }
            else
            {
                if (StudentType != null)
                {
                    var query = rc.Registerations.Where(s => s.Rollno.StartsWith(rollno)
                       && s.Student_Profile.Status == StudentType.Value)
                       .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }
                else
                {
                    var query = rc.Registerations.Where(s => s.Rollno.StartsWith(rollno)
                       && s.Student_Profile.Status == 1)
                       .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }
            }
        }
        public static IEnumerable<Fee> showFee_EmployeeModelFunction(string search,
            string month, int? StudentType)
        {
            if (search == "" || search == null)
            {
                var getFeeRecords = rc.Fees.Where(s => s.Month == month &&
                    s.Registeration.Student_Profile.Status.Value == StudentType.Value).Select(s => s).OrderBy(s => s.Rollno);
                return getFeeRecords;
            }
            else
            {
                var getFeeRecords = rc.Fees.Where(s => s.Month == month &&
                    s.Rollno.StartsWith(search)
                    && s.Registeration.Student_Profile.Status == StudentType.Value)
                    .Select(s => s).OrderBy(s => s.Rollno);
                return getFeeRecords;
            }

        }
        public static string AddFeeRec(int month, Fee feeRec)
        {
            
                try
                {
                    var getIfPresentRollno = rc.Registerations.Where(s => s.Rollno == feeRec.Rollno
                        && s.Status == 1).Select(s => s).FirstOrDefault();
                    if (getIfPresentRollno == null)
                    {
                        return "Roll no is invalid";
                    }
                    else if (rc.Fees.Any(s => s.Bill_No == feeRec.Bill_No))
                    {
                        return "Bill No Already Exists";

                    }
                    else
                    {
                        feeRec.Month = MonthsNames[month];

                        feeRec.Registeration = rc.Registerations.Where
                            (s => s.Rollno == feeRec.Rollno &&
                            s.Status == 1).Select(s => s).FirstOrDefault();

                        feeRec.StudentName = rc.Registerations.Where
                            (s => s.Rollno == feeRec.Rollno &&
                            s.Status == 1).Select(s => s.Student_Profile.FirstName)
                            .FirstOrDefault();
                        
                        rc.Fees.Add(feeRec);
                        rc.SaveChanges();
                        //t.Complete();
                        return "OK";
                    }
                }
                catch (Exception)
                {
                    return "Unable to Add Fee Record";
                    throw;
                }

            
        }
        public static string NewAdmissionRegister(string batch,
            string section, string part, string degree)
        {
            Guid sectionID = Guid.Parse(section);
            int partID = int.Parse(part);
            Guid degreeID = Guid.Parse(degree);
            try
            {
                //Check to see data entered is valid
                if (rc.Batches.Any(s => s.SectionID == sectionID
                    && s.DegreeProgram_ID == degreeID
                    && s.BatchName == batch))
                {
                    //Check to see if there are other students of the same batch and part
                    if (rc.Registerations.Any(s => s.BatchID == batch
                    && s.Part == partID
                    && s.Status == 1))
                    {
                        return "OK";
                    }
                    else
                    {
                        int batchRegCount = rc.Registerations.Where(s => s.BatchID == batch).Count();
                        //if there are no students in the batch then part 1 must be assigned
                        if (batchRegCount == 0)
                        {
                            if (partID == 1)
                            {
                                return "OK";
                            }
                            else
                            {
                                return "For New Admission, The Batch: " + batch + " Must be Assigned Part 1";
                            }
                        }
                        //If there are students then relevent part must be assigned
                        else
                        {
                            return "Plz Select the Part: " + rc.Registerations.Where(s => s.Batch.BatchName == batch).FirstOrDefault().Part + ", For the Batch: " + batch;
                        }
                        #region Old Code
                        //if there is no student of that batch or part
                        //var checkerMine = rc.Registerations.Where(s => s.BatchID == batch
                        //    && s.PartID == partID
                        //    && s.Batch.SectionID == sectionID
                        //    && s.Batch.DegreeProgram_ID == degreeID
                        //    && s.Status == 1)
                        //    .Select(s => s).FirstOrDefault();

                        ////add this as new student
                        //if (checkerMine == null)
                        //{
                        //    return "OK";
                        //}
                        //else
                        //{
                        //    //Students are having other part number
                        //    return "There is no such Section that has BatchName" + batch + " And part: " + part;
                        //}
                        #endregion
                    }
                }
                else
                {
                    return "There is no such Section that has BatchName" + batch;
                }

            }
            catch (Exception)
            {
                return "Failed Connection Plz Try Again!";
            }
        }

        //public static int GetNewStudentID()
        //{
        //    var getLastStudenID = rc.Registerations.OrderByDescending(s => s.StudentID).Select(s => s).FirstOrDefault();
        //    int studentID = 0;
        //    if (getLastStudenID != null)
        //    {
        //        studentID = getLastStudenID.StudentID;
        //        studentID++;
        //        return studentID;
        //    }
        //    else
        //    {
        //        studentID++;
        //        return studentID;
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
        public static IEnumerable<Assign_Subject> GetAllStudentSubjectRecords()
        {
            var getRecords = rc.Assign_Subject.Where(s => s.Registeration.Status.Value == 1 &&
                s.Registeration.Student_Profile.Status.Value == 1).OrderBy(s => s.Rollno).Select(s => s);
            return getRecords;
        }
        public static string CheckFeeEditied(Fee feeRec, string bill)
        {
            return "OK";
        }
        public static string UpdateFeeRec(int month, Fee feeRec, string rollno,string billno)
        {
                try
                {
                    feeRec.Bill_No = billno;
                    var getRequestedFeeRecord = rc.Fees.Where(s => s.Bill_No == feeRec.Bill_No).Select(s => s).FirstOrDefault();

                    if (getRequestedFeeRecord != null)
                    {
                        feeRec.Month = MonthsNames[month];
                        getRequestedFeeRecord.Month = feeRec.Month;
                        //getRequestedFeeRecord = feeRec;
                        getRequestedFeeRecord.Admission_Fee = feeRec.Admission_Fee;
                        getRequestedFeeRecord.Dated = feeRec.Dated;
                        getRequestedFeeRecord.Exam_Fee = feeRec.Exam_Fee;
                        getRequestedFeeRecord.Fine = feeRec.Fine;
                        getRequestedFeeRecord.Installment = feeRec.Installment;
                        getRequestedFeeRecord.Registeration_Fee = feeRec.Registeration_Fee;
                        getRequestedFeeRecord.Total_Fee = feeRec.Total_Fee;
                        getRequestedFeeRecord.Total_Installments = feeRec.Total_Installments;
                        getRequestedFeeRecord.Tution_Fee = feeRec.Tution_Fee;
                        //getRequestedFeeRecord.Rollno = rollno;
                        //feeRec.StudentName = rc.Registerations.Where(s => s.Rollno == feeRec.Rollno).Select(s => s.Student_Profile.FirstName).FirstOrDefault();
                        //getRequestedFeeRecord.StudentName = feeRec.StudentName;
                        rc.SaveChanges();
                        //t.Complete();
                        return "OK";
                    }
                    else
                    {
                        return "Unable to Update Fee Record!";
                    }
                }
                catch (Exception)
                {
                    return "Unable to Update Fee Record!";
                }    
            
            

        }
        public static string NewAdmissionStudentFinalRegister(IEnumerable<Guid> subj)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Assign_Subject> assignSub = new List<Assign_Subject>();
                    vmReg.stdProfile.Status = 1;
                    vmReg.stdProfile.ProfileID = Guid.NewGuid();

                    rc.Student_Profile.Add(vmReg.stdProfile);
                    rc.SaveChanges();

                    vmReg.stdRegisteration.ProfileID = vmReg.stdProfile.ProfileID;
                    vmReg.stdRegisteration.Student_Profile = vmReg.stdProfile;
                    vmReg.stdRegisteration.Status = 1;

                    rc.Registerations.Add(vmReg.stdRegisteration);
                    rc.SaveChanges();

                    var getRelatedSubject = rc.Batch_Subjects_Parts.Where(s => subj.Contains(s.SubjectID.Value)).ToList();

                    foreach (var item in getRelatedSubject)
                    {
                        if (item.BatchName == vmReg.stdRegisteration.BatchID
                            && item.Part == vmReg.stdRegisteration.Part)
                        {
                            assignSub.Add(new Assign_Subject
                            {
                                Batch_Subject_ID = item.ID,
                                Rollno = vmReg.stdRegisteration.Rollno,
                                Status = "Active",
                                Registeration = vmReg.stdRegisteration,
                                AssignID=Guid.NewGuid()
                            });
                        }
                    }
                    foreach (var item in assignSub)
                    {
                        rc.Assign_Subject.Add(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "Successfully Record Added";
                }
                catch (Exception e)
                {
                    return "Unable to Register Student";
                }

            }


        }
        public static string UpdateStudentRecord(string roll, Registeration std, HttpPostedFileBase file)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    var getStudent = rc.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();

                    getStudent.Student_Profile.FirstName = std.Student_Profile.FirstName;
                    getStudent.Student_Profile.LastName = std.Student_Profile.LastName;
                    getStudent.Student_Profile.Gender = std.Student_Profile.Gender;
                    getStudent.Student_Profile.Date_of_Birth = std.Student_Profile.Date_of_Birth;
                    getStudent.Student_Profile.ContactNo = std.Student_Profile.ContactNo;
                    getStudent.Student_Profile.CNIC = std.Student_Profile.CNIC;
                    getStudent.Student_Profile.Address = std.Student_Profile.Address;
                    getStudent.Student_Profile.Nationality = std.Student_Profile.Nationality;
                    getStudent.Student_Profile.Domicile = std.Student_Profile.Domicile;
                    if (file != null && file.ContentType.Contains("image") && file.ContentLength > 0 && file.ContentLength <= 3048576)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            getStudent.Student_Profile.Picture = array;
                        }
                    }
                    if (file != null)
                    {
                        if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                        {
                            return "Plz Select image File less than 3 MB";

                        }
                    }
                    //UpdateModel(getStudent, new string[] { "FirstName", "LastName", "Gender", "ContactNo", "CNIC", "Address", "Nationality"
                    //,"Domicile","Date_of_Birth"});
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable To Update Record!";
                }    
            }
            

        }
        private static string getNewRollNoForStudentOnUpdatingRegisteration(string batch)
        {
            var getLastRegisterationBatchRecord = rc.Registerations.
                Where(s => s.BatchID == batch).
                OrderByDescending(s => s.Rollno).
                Select(s => s.Rollno).FirstOrDefault();

            //getLastRegisterationBatchRecord= 
            string[] s2 = getLastRegisterationBatchRecord.Split(new char[] { '-' });
            int newRollno = 0;
            //int newRollno = int.Parse(s2[1]);
            if (s2 == null)
            {
                string s3 = batch + "-" + "001";
                //newRollno = int.Parse(s3);
                return s3;
            }
            else
            {
                newRollno = int.Parse(s2[1]);
                newRollno++;
                string s4 = s2[0] + "-" + newRollno.ToString();
                //newRollno = int.Parse(s4);
                return s4;
            }
        }
        private static bool CompleteNewRegOnDegreeBatchChange(string newRoll,
            Registeration oldReg, string batch
            , Guid secID, Guid degID, int partID)
        {
            //Create New StudentProfile using Existing Student Record
            Registeration newStdReg = new Registeration();

            //Profile Remains Same
            newStdReg.ProfileID = oldReg.Student_Profile.ProfileID;
            newStdReg.Student_Profile = oldReg.Student_Profile;

            //Registeration
            newStdReg.Rollno = newRoll;
            newStdReg.BatchID = batch;
            newStdReg.Part1 = rc.Parts.Where(s => s.PartID == partID).Select(s => s).FirstOrDefault();
            newStdReg.Part = partID;
            newStdReg.Batch = rc.Batches.Where(s => s.BatchName == batch).Select(s => s).FirstOrDefault();
            newStdReg.Batch.Section = rc.Sections.Where(s => s.SectionID == secID).Select(s => s).FirstOrDefault();
            newStdReg.Batch.SectionID = secID;
            newStdReg.Batch.Degree_Program = rc.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();
            newStdReg.Batch.DegreeProgram_ID = degID;
            newStdReg.Password = oldReg.Password;
            //newStdReg.StudentID = GetNewStudentID();

            //New Registeration is Set
            newStdReg.Status = 1;

            //Previous record is saved but it is not included in current Active Registeration
            oldReg.Status = 0;

            rc.Registerations.Add(newStdReg);
            rc.SaveChanges();

            return true;
        }
        public static string[] UpdateStudentRegRecord(string roll, string degree,
            string batch, string section, string degName, string secName, string part)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    Guid degID = Guid.Parse(degree);
                    Guid secID = Guid.Parse(section);
                    int partID = int.Parse(part);
                    var getStudent = rc.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();

                    //If the Entered Values of batch , degree and section are correct
                    if (rc.Batches.Any(s => s.BatchName == batch
                        && s.DegreeProgram_ID == degID
                        && s.SectionID == secID))
                    {
                        //If the student is already studing the entered batch,degree and section
                        if (getStudent.BatchID == batch
                            && getStudent.Batch.SectionID == secID
                            && getStudent.Batch.DegreeProgram_ID == degID
                            && getStudent.Status == 1)
                        {
                            return new string[]{"NotOK","Student with Rollno " +getStudent.Rollno
                                + " is Already studing in " + degName
                                + " ,Batch: " + batch + " And Section: " + secName};
                        }
                        //If the student re transfers to the old registeration
                        else if (
                            getStudent.Student_Profile.Registerations.Any(s => s.BatchID == batch
                            && s.Batch.SectionID == secID
                            && s.Batch.DegreeProgram_ID == degID
                            && s.Status == 0))
                        {
                            StringBuilder newRoll = new StringBuilder();
                            foreach (var item in getStudent.Student_Profile.Registerations)
                            {
                                if (item.Status == 1)
                                {
                                    item.Status = 0;
                                }
                                else
                                {
                                    newRoll.Append(item.Rollno);
                                    item.Status = 1;
                                }
                            }
                            rc.SaveChanges();
                            t.Complete();
                            return new string[] { "OK", newRoll.ToString() };
                        }
                        else
                        {
                            if (rc.Registerations.Any(s => s.BatchID == batch
                            && s.Batch.SectionID == secID
                            && s.Batch.DegreeProgram_ID == degID
                            && s.Part == partID))
                            {
                                string getNewRoll = getNewRollNoForStudentOnUpdatingRegisteration(batch);
                                if (CompleteNewRegOnDegreeBatchChange(getNewRoll, getStudent, batch, secID, degID, partID))
                                {
                                    //Transaction Completed Successfull batch,degree or section Changed
                                    t.Complete();
                                    return new string[] { "OK", getNewRoll };
                                }
                                else
                                {
                                    return new string[] { "NotOK", "Unable to Update Student Registeration" };
                                }
                            }
                            else
                            {
                                return new string[] { "NotOK",
                                    "There is No Student in Degree Program "+degName+
                                    " Section: "+secName+" And Batch: "+batch+ 
                                    "That is studing in Part: "+part};
                            }
                        }
                    }
                    else
                    {
                        return new string[] { "NotOK", "Record with DegreeName " + degName + ", Batch:" + batch + " And Section " + secName + " Doesnot Exists!" };
                    }

                }
                catch (Exception)
                {
                    return new string[] { "NotOK", "Unable to Update Student Registeration" };
                }

            }

        }

        public static bool AssignSubjectsOnUpdationOfBatchOrDegreeOfStudent(IEnumerable<int?> subjDeg,
            Registeration regUpdatedStudent,
            List<Batch_Subjects_Parts> SubjectsListForBatch)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    //int assignID = GetNewAssignID();

                    if (regUpdatedStudent.Assign_Subject == null ||
                        regUpdatedStudent.Assign_Subject.Count == 0)
                    {
                        foreach (var item in SubjectsListForBatch)
                        {
                            rc.Assign_Subject.Add(new Assign_Subject
                            {
                                Rollno = regUpdatedStudent.Rollno,
                                Batch_Subject_ID = item.ID,
                                Registeration = regUpdatedStudent,
                                AssignID=Guid.NewGuid(),
                                Batch_Subjects_Parts=rc.Batch_Subjects_Parts.Where(s=>s.ID==item.ID).Select(s=>s).FirstOrDefault()
                                //AssignID = assignID
                            });
                            rc.SaveChanges();
                            //assignID++;
                        }
                        rc.SaveChanges();
                        t.Complete();
                        return true;
                    }
                    else
                    {
                        //Get All Assigned Subject to the Subjects that are going to be assigned
                        var getAllAssignedSubjs = SubjectsListForBatch.Select(s => s.Assign_Subject);

                        foreach (var item in getAllAssignedSubjs)
                        {
                            foreach (var item2 in item)
                            {
                                //Match Subjects That are Already assigned and still user asks them to be assigned
                                if (item2.Rollno == regUpdatedStudent.Rollno
                                    && item2.Batch_Subjects_Parts.Part == regUpdatedStudent.Part
                                    && item2.Batch_Subjects_Parts.BatchName == regUpdatedStudent.BatchID)
                                {
                                    continue;
                                }
                                else if (item2.Rollno != regUpdatedStudent.Rollno)
                                {
                                    continue;
                                }
                                else
                                {
                                    //add other subjects that are not assigned
                                    rc.Assign_Subject.Add(new Assign_Subject
                                    {
                                        Rollno = item2.Registeration.Rollno,
                                        Batch_Subject_ID = item2.Batch_Subject_ID,
                                        Batch_Subjects_Parts=rc.Batch_Subjects_Parts.Where(s=>s.ID==item2.Batch_Subject_ID).Select(s=>s).FirstOrDefault(),
                                        //AssignID = assignID,
                                        Registeration = item2.Registeration
                                        ,AssignID=Guid.NewGuid()
                                    });
                                    rc.SaveChanges();
                                    //assignID++;
                                }

                            }
                        }
                        rc.SaveChanges();
                        t.Complete();
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            }

        }
        public static IEnumerable<Assign_Subject> getSpecificSearchStudentSubjRecord
            (string rollno, int? StudentType)
        {
            if (rollno == null || rollno == "")
            {
                var query = rc.Assign_Subject.Where(
                    s => s.Registeration.Student_Profile.Status.Value == StudentType.Value
                    ).OrderBy(s => s.Rollno).Select(s => s);
                return query;
            }
            else
            {
                var query = rc.Assign_Subject.Where(s => s.Rollno.StartsWith(rollno)
                    && s.Registeration.Student_Profile.Status.Value == StudentType.Value
                    ).OrderBy(s => s.Rollno).Select(s => s);
                return query;
            }
        }
        public static string AddStudentSubject_In_Student_SubjectsView(Assign_Subject std, string batch, string section, string part
            , string degree, string rollno, string subjectID)
        {
            //int sectionID = int.Parse(section);

            try
            {
                Guid secID = Guid.Parse(section);
                Guid degID = Guid.Parse(degree);

                var degRecord = rc.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s.Degree_ProgramName).FirstOrDefault();

                int partID = int.Parse(part);
                Guid subjID = Guid.Parse(subjectID);

                var getRequestedStudent = rc.Registerations.Where(s => s.Rollno == rollno
                    && s.Status == 1).Select(s => s).FirstOrDefault();

                var getDegreeSubjectBatch = rc.Batches.Where(s => s.BatchName == batch
                    && s.SectionID == secID
                    && s.DegreeProgram_ID == degID).Select(s => s).FirstOrDefault();

                if (rc.Assign_Subject.Any(
                    s => s.Batch_Subjects_Parts.BatchName == batch &&
                    degID == s.Batch_Subjects_Parts.Batch.DegreeProgram_ID
                    && partID == s.Batch_Subjects_Parts.Part
                    && secID == s.Batch_Subjects_Parts.Batch.SectionID
                    && s.Batch_Subjects_Parts.SubjectID == subjID
                    && s.Rollno == rollno
                    ))
                {
                    return "Record Already Exists";
                }
                else if (getRequestedStudent == null)
                {
                    return "Roll no Doesnot exists";
                }
                else if (getDegreeSubjectBatch == null)
                {
                    return "Batch " + batch + " doesnot have the Selected Section Or Degree";
                }
                else
                {
                    if (getRequestedStudent.BatchID == batch && getRequestedStudent.Batch.SectionID == secID
                    && getRequestedStudent.Batch.DegreeProgram_ID == degID)
                    {
                        var getBpsID = rc.Batch_Subjects_Parts.Where(
                        s => s.BatchName == batch &&
                    degID == s.Batch.DegreeProgram_ID
                    && partID == s.Part
                    && secID == s.Batch.SectionID
                    && s.SubjectID == subjID
                    ).Select(s => s).FirstOrDefault();

                        if (getBpsID==null)
                        {
                            return "The Batch "+batch+" has not been assigned the Subject: "+rc.Subjects.Where(s=>s.SubjectID==subjID).Select(s=>s.SubjectName).FirstOrDefault();
                        }
                        else
                        {
                            //int assignID = GetNewAssignID();
                            rc.Assign_Subject.Add(new Assign_Subject
                            {
                                //AssignID = assignID,
                                Rollno = rollno,
                                Status = "Active",
                                Batch_Subject_ID = getBpsID.ID,
                                Batch_Subjects_Parts = rc.Batch_Subjects_Parts.Where(s => s.ID == getBpsID.ID).Select(s => s).FirstOrDefault(),
                                AssignID = Guid.NewGuid()

                            });
                            rc.SaveChanges();
                            //t.Complete();
                            return "OK";
                        }
                        
                    }
                    else
                    {
                        return "The Student is not Studing the Entered Degree Program " + degRecord + " Or Batch " + batch;
                    }


                }
            }
            catch (Exception ex)
            {
                //t.Dispose();

                return "Unable TO Assign Subject";
            }
            

        }

        public static IEnumerable<Student_Marks> GetAllStudentMarksRecords()
        {
            var getRecords = rc.Student_Marks
                .Where(s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == 1)
                .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);

            return getRecords;
        }
        public static IEnumerable<Student_Marks> getSpecificSearchStudentMarksRecord
            (string rollno, int? StudentType,string year, string Month)
        {
            try
            {
                if ((rollno == null || rollno == "") && Month == "None Selected" && (year == null || year == ""))
                {
                    var query = rc.Student_Marks.Where(s => s
                        .Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value).OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
                else if ((rollno != null && rollno != "") && Month == "None Selected" && (year == null || year == ""))
                {
                    var query = rc.Student_Marks.Where(
                        s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                            && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value)
                        .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
                else if ((rollno != null && rollno != "") && Month != "None Selected" && (year == null || year == ""))
                {

                    var query = rc.Student_Marks.Where(
                        s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                            && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                            && s.Month == Month)
                        .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
                else if ((rollno != null && rollno != "") && Month != "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Student_Marks.Where(
                            s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                                && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                && s.Month == Month
                                && s.Year == yearInNumbers)
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                    }
                    else
                    {
                        return null;
                    }

                }
                else if ((rollno != null && rollno != "") && Month == "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Student_Marks.Where(
                            s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                                && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                && s.Year==yearInNumbers)
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var query = rc.Student_Marks.Where(s => s
                        .Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value).OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
            }
            catch (Exception)
            {

                return null;
            }

            
        }
        public static IEnumerable<Students_Attendance> GetAllStudentAttendanceRecords()
        {
            var getRecords = rc.Students_Attendance.Where(
                s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == 1).
                OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
            return getRecords;
        }
        public static IEnumerable<Students_Attendance> getSpecificSearchStudentAttendanceRecord
            (string rollno, int? StudentType,string year,string Month)
        {
            try
            {
                if ((rollno == null || rollno == "") && Month == "None Selected" && (year == null || year == ""))
                {
                    var query = rc.Students_Attendance.Where(s => s
                        .Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value).OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
                else if ((rollno != null && rollno != "") && Month == "None Selected" && (year == null || year == ""))
                {
                    var query = rc.Students_Attendance.Where(
                        s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                            && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value)
                        .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
                else if ((rollno != null && rollno != "") && Month != "None Selected" && (year == null || year == ""))
                {

                    var query = rc.Students_Attendance.Where(
                        s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                            && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                            && s.Month == Month)
                        .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
                else if ((rollno != null && rollno != "") && Month != "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Students_Attendance.Where(
                            s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                                && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                && s.Month == Month
                                && s.Year == yearInNumbers)
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                    }
                    else
                    {
                        return null;
                    }

                }
                else if ((rollno != null && rollno != "") && Month == "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Students_Attendance.Where(
                            s => s.Assign_Subject.Registeration.Rollno.StartsWith(rollno)
                                && s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                && s.Year == yearInNumbers)
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var query = rc.Students_Attendance.Where(s => s
                        .Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value).OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                    return query;
                }
            }
            catch (Exception)
            {

                return null;
            }            
        }

        public static string DeleteStudentRecords(IEnumerable<string> deleteroll)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Student_Profile> listToDelete = rc.Registerations.
                    Where(s => deleteroll.Contains(s.Rollno))
                    .Select(s => s.Student_Profile).ToList();

                    foreach (var item in listToDelete)
                    {
                        rc.Student_Profile.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch
                {
                    return "Unable To Delete Student Records";
                }
            }
        }
        public static string DeleteStudentSubjects(IEnumerable<Guid> deleteroll)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Assign_Subject> listToDelete =
                        rc.Assign_Subject.Where(s => deleteroll.Contains(s.AssignID)).ToList();

                    foreach (var item in listToDelete)
                    {
                        rc.Assign_Subject.Remove(item);
                        rc.SaveChanges();
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch
                {
                    return "Unable To Delete Student Subject Records";
                }
            }
        }

        public static string DeleteFeeRecordsOfStudents(IEnumerable<string> deletefee)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Fee> listToDelete = rc.Fees.Where(s => deletefee.Contains(s.Bill_No)).ToList();
                    foreach (var item in listToDelete)
                    {
                        rc.Fees.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable To Delete Records!";
                }
            }
        }
        public static string DeleteStudentMarksRecords(IEnumerable<Guid> deleteroll)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Student_Marks> listToDelete = rc.Student_Marks
                        .Where(s => deleteroll.Contains(s.ResultID)).ToList();

                    foreach (var item in listToDelete)
                    {
                        rc.Student_Marks.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable To Delete Records!";
                }
            }
        }
        public static string DeleteStudentAttendanceRecords(IEnumerable<Guid> deleteroll)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Students_Attendance> listToDelete =
                        rc.Students_Attendance.Where(s => deleteroll.Contains(s.AttendanceID)).ToList();

                    foreach (var item in listToDelete)
                    {
                        rc.Students_Attendance.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable To Delete Records!";
                }
            }
        }
        #endregion

        #region Employee Manangement By Admin
        public static Guid getNewEmployeeID()
        {
            Guid gd = new Guid();
            gd = Guid.NewGuid();
            return gd;
        }
        public static bool UpdateEmployeeRecord(Guid ID, Employee std, HttpPostedFileBase file,
            Nullable<System.DateTime> date1)
        {
            try
            {
                var getEMP = rc.Employees.Where(s => s.EmpID == ID).Select(s => s).FirstOrDefault();
                getEMP.Name = std.Name;
                getEMP.Address = std.Address;
                getEMP.Gender = std.Gender;
                getEMP.Date_of_Birth = std.Date_of_Birth;
                getEMP.ContactNo = std.ContactNo;
                getEMP.Salary = std.Salary;
                getEMP.CNIC = std.CNIC;
                getEMP.Password = std.Password;
                getEMP.Martial_Status = std.Martial_Status;
                getEMP.Date_of_Birth = date1;
                getEMP.UserName = std.UserName;
                if (file != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        getEMP.Picture = array;
                    }
                }

                //UpdateModel(getStudent, new string[] { "FirstName", "LastName", "Gender", "ContactNo", "CNIC", "Address", "Nationality"
                //,"Domicile","Date_of_Birth"});
                rc.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static IEnumerable<Employee> getSpecificSearchRecordForEmployees(string name)
        {
            var query = rc.Employees.Where(s => s.Name.StartsWith(name)).OrderBy(s => s.Name).Select(s => s);
            return query;
        }
        public static bool NewEmployeeAddition(Employee tRec, HttpPostedFileBase file, Nullable<System.DateTime> date1)
        {
            try
            {
                tRec.EmpID = getNewEmployeeID();
                if (file != null && file.ContentType.Contains("image") && file.ContentLength > 0 && file.ContentLength <= 3048576)
                {
                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            tRec.Picture = array;
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }
                tRec.Date_of_Birth = date1;
                tRec.Status = "Active";
                rc.Employees.Add(tRec);
                rc.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IEnumerable<Employee> GetAllEmployeesRecords()
        {
            var getRecords = rc.Employees.OrderBy(s => s.Name).Select(s => s);
            return getRecords;
        }

        #endregion

        #region Others
        //public static ViewModel_Registeration getRegisterationInstanceForNewAdmission()
        //{
        //    if (vmReg == null)
        //    {
        //        vmReg = new ViewModel_Registeration();
        //        return vmReg;
        //    }
        //    return vmReg;
        //}
        public static void SetStudentProfileNewAdmission(Student_Profile std)
        {
            vmReg.stdProfile = std;
        }
        public static void SetStudentProfileNewAdmissionEducationDetails(Student_Profile std)
        {
            vmReg.stdProfile.Matric_Marks = std.Matric_Marks;
            vmReg.stdProfile.Intermediate_Marks = std.Intermediate_Marks;
            vmReg.stdProfile.MatricFrom = std.MatricFrom;
            vmReg.stdProfile.IntermediateFrom = std.IntermediateFrom;
        }
        public static void SetRegisterationNewAdmission(Registeration reg)
        {
            vmReg.stdRegisteration = reg;
        }
        public static void ClearOrNotToClearValues(int val)
        {
            if (val == 1)
            {
                if (vmReg == null)
                {
                    vmReg = new ViewModel_Registeration();
                }
            }
            else
            {
                vmReg = null;
            }
        }

        public static int GetMonthNoForMonthName(string Month)
        {
            switch (Month)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
                default:
                    return 13;
                    break;
            }
        }
        #endregion
    }
}