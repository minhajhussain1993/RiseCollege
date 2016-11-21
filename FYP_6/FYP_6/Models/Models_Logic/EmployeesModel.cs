using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections;
using System.Transactions;
using System.Text;
using FYP_6.Models.ViewModels;
using System.Data.Entity.Validation;
using FYP_6.Models.Report_Models;
using System.Data.Entity;


namespace FYP_6.Models.Models_Logic
{
    public class EmployeesModel
    {
        public static ViewModel_Registeration vmReg;

        private static string[] MonthsNames ={"","January","February","March","April","May","June","July",
                                         "August","September","October","November","December"};
        static RCIS3Entities rc = RCIS3Entities.getinstance();


        #region Employee Functions
        public string[] WelcomeEmployeesScreenResults()
        {
            string[] mainScreenItems = new string[5];
            //int t_id = int.Parse(id);
            var DegreeProgramsInCollege = rc.Degree_Program.Select(s => s).ToList();
            var SubjectsInCollege = rc.Subjects.Select(s => s).ToList();
            var studentsInCollege = rc.Registerations.Where(s => s.Status == 1 && s.Student_Profile.Status == 1).Select(s => s).ToList();
            var TeachersInCollege = rc.Teachers.Where(s => s.Status == "Active").Select(s => s).ToList();

            mainScreenItems[0] = DegreeProgramsInCollege.Count.ToString();
            mainScreenItems[1] = (SubjectsInCollege.Count.ToString());
            mainScreenItems[2] = (studentsInCollege.Count.ToString());
            mainScreenItems[3] = (TeachersInCollege.Count.ToString());

            return mainScreenItems;
        }
        public string ChangePassword_EmployeeModelFunction(string oldpass, string newpass, Guid id)
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
        public string ChangePassword_AdminFunction(string oldpass, string newpass, Guid emp_id)
        {
            using (TransactionScope t = new TransactionScope())
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
        private string ValidatePassword(string oldentered, string newpass, string actualPassword)
        {
            if (oldentered.Length <= 30)
            {
                if (newpass.Length <= 30 && newpass.Length >=5)
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
                    return "New Password must be 5 to 30 characters";
                }
            }
            else
            {
                return "Old Password must be less than 30 characters";
            }
        }

        #endregion

        #region Teacher Relevant Functions
        public string getTeacherID()
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

        public string UpdateTeacherRecord(string ID, Teacher std, HttpPostedFileBase file, string gender, Nullable<System.DateTime> date1
            , string Marriedstatus
            , string PostDeg, string gradDeg, string religion)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    var getTeacher = rc.Teachers.Where(s => s.TeacherID == ID).Select(s => s).FirstOrDefault();

                    int graddegreeID = 0;
                    int postdegID = 0;

                    if (int.TryParse(gradDeg, out graddegreeID) && int.TryParse(PostDeg, out postdegID))
                    {
                        if (graddegreeID == 0)
                        {

                        }
                        else
                        {
                            getTeacher.Graduation_Degree_Level = rc.GradPostDegrees.Where(s => s.ID == graddegreeID).Select(s => s.DegreeName).FirstOrDefault() ?? "";
                        }
                    }
                    else
                    {
                        return "Plz Enter Valid Degree Details!";
                    }
                    if (religion == "Please select")
                    {

                    }
                    else
                    {
                        getTeacher.Religion = religion;
                    }
                    getTeacher.Name = std.Name;
                    getTeacher.Address = std.Address;
                    getTeacher.Gender = gender;
                    getTeacher.Date_of_Birth = date1;
                    getTeacher.ContactNo = std.ContactNo;
                    //getTeacher.Graduation_Details = std.Graduation_Details;
                    getTeacher.Address = std.Address;
                    getTeacher.Major_Subject = std.Major_Subject;
                    getTeacher.Year_of_Graduation = std.Year_of_Graduation;


                    getTeacher.Salary = std.Salary;
                    //getTeacher.Post_Graduation_Details = std.Post_Graduation_Details;
                    getTeacher.Password = std.Password;
                    getTeacher.Martial_Status = Marriedstatus;

                    if ((std.Post_Graduation_Degree_Institution != "" &&
                        std.Post_Graduation_Degree_Institution != null)

                        && (std.Post_Graduation_Degree_Name != "" && std.Post_Graduation_Degree_Name != null)

                        && (postdegID != 0) &&

                        (std.Year_of_Post_Graduation.ToString() != "" && std.Year_of_Post_Graduation.ToString() != null))
                    {
                        getTeacher.Year_of_Post_Graduation = std.Year_of_Post_Graduation;
                        getTeacher.Post_Graduation_Degree_Name = std.Post_Graduation_Degree_Name;
                        getTeacher.Post_Graduation_Degree_Institution = std.Post_Graduation_Degree_Institution;
                        getTeacher.Post_Graduation_Level = rc.GradPostDegrees.Where(s => s.ID == postdegID).Select(s => s.DegreeName).FirstOrDefault() ?? "";
                    }
                    else if ((std.Post_Graduation_Degree_Institution == "" || std.Post_Graduation_Degree_Institution == null)
                        &&
                        (std.Post_Graduation_Degree_Name == "" || std.Post_Graduation_Degree_Name == null)
                        &&
                        (postdegID == 0)
                        &&
                        (std.Year_of_Post_Graduation.ToString() == "" || std.Year_of_Post_Graduation == null))
                    {
                        //getTeacher.Year_of_Post_Graduation = std.Year_of_Post_Graduation;
                        //getTeacher.Post_Graduation_Degree_Name = std.Post_Graduation_Degree_Name;
                        //getTeacher.Post_Graduation_Degree_Institution = std.Post_Graduation_Degree_Institution;
                        //getTeacher.Post_Graduation_Level = rc.GradPostDegrees.Where(s => s.ID == postdegID).Select(s => s.DegreeName).FirstOrDefault() ?? "";
                    }
                    else if ((std.Post_Graduation_Degree_Institution != "" &&
                        std.Post_Graduation_Degree_Institution != null)

                        && (std.Post_Graduation_Degree_Name != "" && std.Post_Graduation_Degree_Name != null)

                        && (postdegID == 0) &&

                        (std.Year_of_Post_Graduation.ToString() != "" && std.Year_of_Post_Graduation.ToString() != null))
                    {
                        getTeacher.Year_of_Post_Graduation = std.Year_of_Post_Graduation;
                        getTeacher.Post_Graduation_Degree_Name = std.Post_Graduation_Degree_Name;
                        getTeacher.Post_Graduation_Degree_Institution = std.Post_Graduation_Degree_Institution;
                        //getTeacher.Post_Graduation_Level = rc.GradPostDegrees.Where(s => s.ID == postdegID).Select(s => s.DegreeName).FirstOrDefault() ?? "";
                    }
                    else
                    {
                        return "Plz Enter All Post Graduation Details or Leave All of them Empty!";
                    }

                    //getTeacher.
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
                    t.Dispose();
                    return "Unable To Update Record!";
                }
            }
        }
        public IEnumerable<Teacher> GetAllTeacherRecords()
        {
            var getRecords = rc.Teachers.Where(s => s.Status == "Active").OrderBy(s => s.TeacherID).Select(s => s);
            return getRecords;
        }
        public IEnumerable<Teacher> getSpecificSearchRecordForTeacher(string ID, string TeacherType)
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
        public IEnumerable<Teacher_Attendance> getResultRecordsForTeacherAttendance()
        {
            var getSpecificRecordsOfTeacher = rc.Teacher_Attendance.Where(s => s.Teacher.Status == "Active").Select(s => s).OrderBy(s => s.TeacherID);
            return getSpecificRecordsOfTeacher;
        }
        public IEnumerable<Teacher_Attendance> showResultsTeacherAttendance_EmployeeModelFunction(string search, string Month, string year)
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

            try
            {
                if (search == null || search == "")// Check if search is empty then
                {
                    //if no year and no month is entered then return no record
                    //if year is entered but no month is entered then return year related records
                    //if month is entered but no year is entered then return month related records
                    if ((year == null || year == "") && Month == "None Selected")
                    {
                        return null;
                    }
                    else if ((year != null || year != "") && Month == "None Selected")
                    {
                        int yearInNumbers = 0;
                        if (int.TryParse(year, out yearInNumbers))
                        {
                            var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                                  where tAtt.Date.Year == yearInNumbers
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

                        if (monthNo == 13)
                        {
                            return null;
                        }
                        else
                        {
                            var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                                  where tAtt.Date.Month == monthNo
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
                                                  where tAtt.Date.Year == yearInNumbers
                                                  && tAtt.Date.Month == monthNo
                                                  select tAtt;
                            return TeacherAttQuery;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    if ((year == null || year == "") && Month == "None Selected")
                    {
                        var TeacherAttQuery = from tAtt in rc.Teacher_Attendance where tAtt.TeacherID == search select tAtt;
                        return TeacherAttQuery;
                    }
                    else if ((year != null || year != "") && Month == "None Selected")
                    {
                        int yearInNumbers = 0;
                        if (int.TryParse(year, out yearInNumbers))
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

                        if (monthNo == 13)
                        {
                            return null;
                        }
                        else
                        {
                            var TeacherAttQuery = from tAtt in rc.Teacher_Attendance
                                                  where tAtt.TeacherID == search
                                                  && tAtt.Date.Month == monthNo
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
                                                  && tAtt.Date.Month == monthNo
                                                  select tAtt;
                            return TeacherAttQuery;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        public string EditTeacherAttWithDateAndStatus(Teacher_Attendance tAtt, string TeacherID, Nullable<DateTime> Date, string PresentStatus)
        {
            try
            {
                //DateTime dt = DateTime.Parse(Date);
                 
                var getUpdatedRecord = rc.Teacher_Attendance.Where(s => s.Date == Date
                    && s.TeacherID == TeacherID).Select(s => s).FirstOrDefault();

                if (getUpdatedRecord == null)
                {
                    return "Teacher Record Not Found For Updation!";
                }
                getUpdatedRecord.Present = PresentStatus;
                //getUpdatedRecord.
                rc.SaveChanges();
                return "OK";
            }
            catch (Exception)
            {
                return "Unable to Update Record";
            }
        }
        public string AddTeacherAtt(Teacher_Attendance tRec, string t_id, Nullable<DateTime> date)
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
        public IEnumerable<Teacher_Subject> GetAllTeacher_SubjectsRecords()
        {
            var rec = from tsubj in rc.Teacher_Subject
                      where tsubj.Teachers_Batches.Teacher.Status == "Active"
                      orderby tsubj.Teachers_Batches.TeacherID
                      select tsubj;
            //var getRecords = rc.Teacher_Subject.Where(s => s.Teachers_Batches.Teacher.Status == "Active")
            //  .OrderBy(s => s.Teachers_Batches.Teacher).Select(s => s).ToList();
            return rec;
        }
        public IEnumerable<Teacher_Subject> GetAllSearchSpecificTeacher_SubjectsRecords(Guid degree, Guid section, string batch, string teacherID)
        {
            if (teacherID == "" || teacherID == null)
            {
                var query = from tsubj in rc.Teacher_Subject
                            where tsubj.Teachers_Batches.BatchName == batch
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
        public IEnumerable<Teacher_Subject> GetAllSearchSpecificTeacher_SubjectsRecordsAccordingToTeacherID(string teacherID)
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
        public IEnumerable<Teachers_Batches> GetAllTeacher_batchesRecords()
        {
            var getRecords = rc.Teachers_Batches.Where(s => s.Teacher.Status == "Active").OrderBy(s => s.TeacherID).Select(s => s);
            return getRecords;
        }
        //public   IEnumerable<Teachers_Batches> GetAllSearchSpecificTeacher_BatchesRecords(int degree, int section, string batch, string teacherID)
        public IEnumerable<Teachers_Batches> GetAllSearchSpecificTeacher_BatchesRecords(string teacherID)
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
        //public   int GetNewTeacherSubjectID()
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
        //public   int GetNewTeacherBatchID()
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
        public string AddTeacherSubject(string teacherID, string degree, string section, string batch, string subjectID, string part)
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
        public string AddTeacherBat(string teacherID, string degree, string section, string batch)
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

        public List<GradPostDegree> getGradPostDegreesType1(string gradDeg)
        {
            try
            {
                int graddegreeID = 0;
                if (int.TryParse(gradDeg, out graddegreeID))
                {
                    return rc.GradPostDegrees.Where(s => s.Type == 1).OrderByDescending(s=>s.ID==graddegreeID).ThenBy(s => s).ToList();
                }
                else
                {
                    return rc.GradPostDegrees.Where(s => s.Type == 1).Select(s => s).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        public List<GradPostDegree> getGradPostDegreesType2(string PostDeg)
        {
            try
            {
                int postdegreeID = 0;
                if (int.TryParse(PostDeg, out postdegreeID))
                {
                    if (postdegreeID==0)
                    {
                        return rc.GradPostDegrees.Where(s => s.Type == 2).Select(s => s).ToList();    
                    }
                    return rc.GradPostDegrees.Where(s => s.Type == 2).OrderByDescending(s => s.ID == postdegreeID).ThenBy(s => s).ToList();
                }
                else
                {
                    return rc.GradPostDegrees.Where(s => s.Type == 2).Select(s => s).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        public string EnrollTeacherForCollege(Teacher tRec, HttpPostedFileBase file, string gender, Nullable<System.DateTime> date1
            , string gradDeg, string PostDeg, string Marriedstatus)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    int graddegreeID = 0;
                    int postdegID = 0;


                    if (int.TryParse(gradDeg, out graddegreeID) && int.TryParse(PostDeg, out postdegID))
                    {
                        tRec.Graduation_Degree_Level = rc.GradPostDegrees.Where(s => s.ID == graddegreeID).Select(s => s.DegreeName).FirstOrDefault() ?? "";
                    }
                    else
                    {
                        return "Plz Enter Valid Degree Details!";
                    }

                    if ((tRec.Post_Graduation_Degree_Institution != "" &&
                       tRec.Post_Graduation_Degree_Institution != null)

                       && (tRec.Post_Graduation_Degree_Name != "" && tRec.Post_Graduation_Degree_Name != null)

                       && (postdegID != 0) &&

                       (tRec.Year_of_Post_Graduation.ToString() != "" && tRec.Year_of_Post_Graduation != null))
                    {
                        tRec.Post_Graduation_Level = rc.GradPostDegrees.Where(s => s.ID == postdegID).Select(s => s.DegreeName).FirstOrDefault() ?? "";
                    }
                    else if ((tRec.Post_Graduation_Degree_Institution == "" || tRec.Post_Graduation_Degree_Institution == null)
                        &&
                        (tRec.Post_Graduation_Degree_Name == "" || tRec.Post_Graduation_Degree_Name == null)
                        &&
                        (postdegID == 0)
                        &&
                        (tRec.Year_of_Post_Graduation.ToString() == "" || tRec.Year_of_Post_Graduation == null))
                    {

                    }
                    else
                    {
                        return "Plz Enter All Post Graduation Details or Leave All of them Empty!";
                    }
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
                    else if (file != null)
                    {
                        if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                        {
                            return "Plz Select image File less than 3 MB";
                        }
                    }
                    tRec.Martial_Status = Marriedstatus;
                    tRec.Gender = gender;
                    tRec.Date_of_Birth = date1;
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

        public IEnumerable<Batch_Subjects_Parts> getBPSForAssigningToTeachers(Batch batch)
        {
            var batchSubj = rc.Batch_Subjects_Parts.Where(s => s.BatchName == batch.BatchName).OrderBy(s => s.SubjectID).Select(s => s);
            return batchSubj;
        }

        //Update Assigning Subjects To Teachers
        #region Previous Code Of Assigning Subjects To Teachers
        //public   string SubjectAddToBPS(IEnumerable<Guid> subj, Batch batch, string teacherID)
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
        //public   bool AssignNewSubjectsToBPS(List<Batch_Subjects_Parts> CompleteSubjList,
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
        //public   bool DeleteAllSubjectsToBPS(Batch batch, string teacherID)
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

        public bool DeleteBatchPlusBatchRelatedSubjectsOfTeacher(List<Teachers_Batches> listToDelete)
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

        public string AssignTeacherSubjects_NewFunction(IEnumerable<Guid> subjSelect, Teachers_Batches tbRec)
        {
            if (tbRec.Teacher.Status!="Active")
            {
                return "This Past Teacher's Batch Assignment must be Deleted! ";
            }
            if (subjSelect == null)
            {
                return "Plz Select At least One Subject To Continue!";
            }
            // Check If Any Teacher is Teaching thisSubject
            var checkerOneIfAnyTeacherisTeachingthisSubject = rc.Teacher_Subject
                .Where(s => s.Teachers_Batches.BatchName == tbRec.BatchName
                && subjSelect.Contains(s.SubjectID.Value)
                && s.Teachers_Batches.TeacherID != tbRec.Teacher.TeacherID).Select(s => s).FirstOrDefault();




            if (checkerOneIfAnyTeacherisTeachingthisSubject != null)
            {
                return @"The Subject " + checkerOneIfAnyTeacherisTeachingthisSubject.Subject.SubjectName
                    + " of Batch " + checkerOneIfAnyTeacherisTeachingthisSubject.Teachers_Batches.BatchName
                    + "is being taught by Teacher: " + checkerOneIfAnyTeacherisTeachingthisSubject.Teachers_Batches.TeacherID;
            }
            else
            {
                using (TransactionScope t = new TransactionScope())
                {
                    try
                    {
                        var getTeacherSubjects = rc.Teacher_Subject.Where(s => s.Teacher_BatchID == tbRec.ID).Select(s => s).ToList();
                        //Remove Previous Subjects, Assign New Subjects

                        foreach (var item in getTeacherSubjects)
                        {
                            rc.Teacher_Subject.Remove(item);
                        }

                        foreach (var item in subjSelect)
                        {
                            rc.Teacher_Subject.Add(new Teacher_Subject
                            {
                                ID = Guid.NewGuid(),
                                Teacher_BatchID = tbRec.ID,
                                Teachers_Batches = tbRec,
                                SubjectID = item,
                                Subject = rc.Subjects.Where(s => s.SubjectID == item).Select(s => s).FirstOrDefault()
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
        public IEnumerable<Teacher_Attendance> GetAllTeacherIDsNamesForUploadingTeacherAttendance()
        {
            List<Teacher_Attendance> tAtt = new List<Teacher_Attendance>();
            foreach (var item in rc.Teachers)
            {
                if (item.Status=="Active")
                {
                    tAtt.Add(new Teacher_Attendance
                    {
                        Teacher = item,
                        TeacherID = item.TeacherID

                    });   
                }
                 
            }
            return tAtt;
        }
        public string NewTeacherIndividualAttendancesAdditionCode(string status,
            Nullable<System.DateTime> date, string T_IDS)
        {

            try
            {
                if (T_IDS==null ||T_IDS=="")
                {
                    return "Teacher ID is required!";   
                }
                else if (!rc.Teachers.Any(s=>s.TeacherID==T_IDS &&s.Status=="Active"))
                {
                    return "Teacher ID doesn't exists or is UnActive!";
                }
                else if (date==null)
                {
                    return "Date is required!";
                }
                else if (rc.Teacher_Attendance.Any(s => s.Date == date && T_IDS == s.TeacherID))
                {
                    return "Attendance of Teacher ID :"+T_IDS+" of Date " + date.Value.ToShortDateString() + " has already been Uploaded!";
                }
                else
                {
                    var getObj = rc.Teachers.Where(s => s.TeacherID == T_IDS).Select(s => s).FirstOrDefault();
                        rc.Teacher_Attendance.Add(new Teacher_Attendance
                        {
                            Date = date.Value,
                            Present = status,
                            Teacher =  getObj,
                            TeacherID = getObj.TeacherID
                            ,
                            ID = Guid.NewGuid()

                        }); 
                    rc.SaveChanges();
                    //t.Complete();
                    return "OK";
                }
            }
            catch (Exception)
            {
                //t.Dispose();
                return "Unable to Mark Teacher Attendance! Plz Try Again!";
            }

        }
        public string NewTeacherAttendancesAdditionCode(IEnumerable<string> status,
            Nullable<System.DateTime> date, IEnumerable<string> T_IDS)
        {

            try
            {
                if (rc.Teacher_Attendance.Any(s => s.Date == date && T_IDS.FirstOrDefault() == s.TeacherID))
                {
                    return "Attendance of Date " + date.Value.ToShortDateString() + " has already been Uploaded!";
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
                            ,
                            ID = Guid.NewGuid()

                        });
                    }
                    rc.SaveChanges();
                    //t.Complete();
                    return "OK";
                }
            }
            catch (Exception)
            {
                //t.Dispose();
                return "Unable to Mark Teacher Attendance! Plz Try Again!";
            }

        }


        //Subject Updation in Degree Subjects

        public string TeacherBatchAssign(IEnumerable<string> listOfBatches, Teacher teacher)
        {
            try
            {
                //int degreeID = int.Parse(degID);

                //Get List of All the selected Subjects
                List<Batch> listToDelete = rc.Batches.Where(s => listOfBatches.Contains(s.BatchName)).ToList();

                //Get Degree Program Reference
                //Degree_Program deg = rc.Degree_Program.Where(s => s.ProgramID == degID).Select(s => s).FirstOrDefault();

                //Get All the assigned Subjects to DegreeProgram
                List<Teachers_Batches> getAllTheBatchesofTeacher = rc.Teachers_Batches.Where(s => s.TeacherID == teacher.TeacherID)
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
                    using (TransactionScope t = new TransactionScope())
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
                    using (TransactionScope t = new TransactionScope())
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
                    using (TransactionScope t = new TransactionScope())
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

        public bool AssignNewBatchesToTeacher(List<Batch> selectedListofBatches,
            List<Teachers_Batches> getTeacherBatches, Teacher teacher)
        {
            using (TransactionScope t = new TransactionScope())
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


        public bool DeleteAllTeacher_Batches(Teacher teacher)
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


        public string DeleteTeacherRecords_EMP_ModelFunction(IEnumerable<string> deleteTeacher)
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
        public string DeleteTeacherAttendanceRecords_EMP_ModelFunction(IEnumerable<Guid> deleteTatt)
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
        public string DeleteTeacherSubjectRecords_EMP_ModelFunction(IEnumerable<Guid> deleteTSub)
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
        public IEnumerable<Registeration> GetAllStudentRecords()
        {
            var getRecords = rc.Registerations.Where(s => s.Student_Profile.Status == 1)
                .OrderBy(s => s.Rollno).Select(s => s);
            return getRecords;
        }
        public IEnumerable<Registeration> getSpecificSearchRecord(string rollno, int? StudentType
            , string searchfname, string searchdeg
            , string searchsection, string searchpart)
        {
            try
            { 
                //{}
                if ((searchfname==null ||searchfname=="")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType)
                       .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }//{1}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        &&s.Rollno.StartsWith(rollno))
                       .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }//{2}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower()))
                       .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }//{3}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                      
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                       )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{4}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,2}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno)
                        )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,3}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                      
                        && s.Rollno.StartsWith(rollno)
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                       )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,4}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Rollno.StartsWith(rollno)
                         
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }
                //{1,5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Rollno.StartsWith(rollno)
                         
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{2,3}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                         
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{2,4}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                         
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{2,5}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                         
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }
                //{3,4}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                       
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                      )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{3,5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                         
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{4,5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,2,3}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno)
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,3,4}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno != null || rollno != "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Rollno.StartsWith(rollno)
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,4,5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Rollno.StartsWith(rollno)
                         
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{2,3,4}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                         
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{2,3,5}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                         
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                         
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{3,4,5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{2,4,5}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                         
                         
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,2,5}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno)                          
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }
                //{1,3,5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Rollno.StartsWith(rollno)
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                         
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,2,3,4}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno)
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                         )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }
                //{1,2,3,5}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno != null || rollno != "")
                    && (searchsection == null || searchsection == "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno)
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                         
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,3,4,5}
                else if ((searchfname == null || searchfname == "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno != null || rollno != "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                         
                        && s.Rollno.StartsWith(rollno)
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,2,4,5}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno)
                         
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{2,3,4,5}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg != null || searchdeg != "")
                    && (rollno == null || rollno == "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart != null || searchpart != ""))
                {
                    int part = 0;
                    if (!int.TryParse(searchpart, out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                         
                        && s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part == part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,2,4}
                else if ((searchfname != null || searchfname != "")
                    && (searchdeg == null || searchdeg == "")
                    && (rollno != null || rollno != "")
                    && (searchsection != null || searchsection != "")
                    && (searchpart == null || searchpart == ""))
                {
                     
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno) 
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower()) )
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }//{1,2,3,4,5}
                else  
                {
                    int part=0;
                    if(!int.TryParse(searchpart,out part))
                    {
                        return null;
                    }
                    var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType
                        && s.Student_Profile.FirstName.ToLower().StartsWith(searchfname.ToLower())
                        && s.Rollno.StartsWith(rollno)
                        &&s.Batch.Degree_Program.Degree_ProgramName.ToLower().StartsWith(searchdeg.ToLower())
                        && s.Batch.Section.SectionName.ToLower().StartsWith(searchsection.ToLower())
                        && s.Part==part)
                       .OrderBy(s => s.Rollno).Select(s => s);

                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
            //if (rollno == null || rollno == "")
            //{
            //    if (StudentType != null)
            //    {
            //        var query = rc.Registerations.Where(s => s.Student_Profile.Status == StudentType.Value)
            //           .OrderBy(s => s.Rollno).Select(s => s);
            //        return query;
            //    }
            //    else
            //    {
            //        var query = rc.Registerations.Where(s => s.Student_Profile.Status == 1)
            //           .OrderBy(s => s.Rollno).Select(s => s);
            //        return query;
            //    }
            //}
            //else
            //{
            //    if (StudentType != null)
            //    {
            //        var query = rc.Registerations.Where(s => s.Rollno.StartsWith(rollno)
            //           && s.Student_Profile.Status == StudentType.Value)
            //           .OrderBy(s => s.Rollno).Select(s => s);
            //        return query;
            //    }
            //    else
            //    {
            //        var query = rc.Registerations.Where(s => s.Rollno.StartsWith(rollno)
            //           && s.Student_Profile.Status == 1)
            //           .OrderBy(s => s.Rollno).Select(s => s);
            //        return query;
            //    }
            //}
        }
         
         
        public string NewAdmissionRegister(string batch,
            string section, string part, string degree, string rollno)
        {
            Guid sectionID = Guid.Parse(section);
            int partID = int.Parse(part);
            Guid degreeID = Guid.Parse(degree);
            try
            {
                if (rollno != getNewRollNoForStudentOnUpdatingRegisteration(batch))
                {
                    return "Roll no is not Valid!";
                }
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

        //public   int GetNewStudentID()
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
        //public   int GetNewAssignID()
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
        public IEnumerable<Assign_Subject> GetAllStudentSubjectRecords()
        {
            var getRecords = rc.Assign_Subject.Where(s => s.Registeration.Status.Value == 1 &&
                s.Registeration.Student_Profile.Status.Value == 1).OrderBy(s => s.Rollno).Select(s => s);
            return getRecords;
        }

        public string NewAdmissionStudentFinalRegister(ViewModel_FeeManagement feeRec, Nullable<System.DateTime> date1 , string paidInst)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Assign_Subject> assignSub = new List<Assign_Subject>();
                    vmReg.stdProfile.Status = 1;
                    vmReg.stdProfile.ProfileID = Guid.NewGuid();

                    rc.Student_Profile.Add(vmReg.stdProfile);
                    //rc.SaveChanges();

                    vmReg.stdRegisteration.ProfileID = vmReg.stdProfile.ProfileID;
                    vmReg.stdRegisteration.Student_Profile = vmReg.stdProfile;
                    vmReg.stdRegisteration.Status = 1;

                    rc.Registerations.Add(vmReg.stdRegisteration);
                    //rc.SaveChanges();

                    //var getRelatedSubject = rc.Batch_Subjects_Parts.Where(s => subj.Contains(s.SubjectID.Value)).ToList();

                    foreach (var item in rc.Batch_Subjects_Parts)
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
                                AssignID = Guid.NewGuid()
                            });
                        }
                    }
                    foreach (var item in assignSub)
                    {
                        rc.Assign_Subject.Add(item);
                    }
                     
                    //Sum For COmparison with Total FEE
                    decimal[] values = new decimal[10];
                    values[0] = feeRec.yearlyFee.Tution_Fee ?? 0;
                    values[1] = feeRec.yearlyFee.Registeration_Fee ?? 0;
                    //values[2] = feeRec.yearlyFee.Fine ?? 0;
                    values[3] = feeRec.yearlyFee.Admission_Fee ?? 0;
                    values[4] = feeRec.yearlyFee.Exam_Fee ?? 0;

                    decimal sum = values.Sum();

                    var getRecordsOFFEEForStudent = rc.Fees.Where(s => s.RollNo == feeRec.feeSummary.RollNo).Select(s => s).Count();

                    
                    //var getIfPresentRollno = rc.Registerations.Where(s => s.Rollno == feeRec.feeSummary.RollNo && s.Student_Profile.Status == 1
                    //    && s.Status == 1).Select(s => s).FirstOrDefault();

                    var getFeeSummary = rc.Overall_Fees.Where(s => s.RollNo == feeRec.feeSummary.RollNo).Select(s => s).FirstOrDefault();

                    string rollCheck = getNewRollNoForStudentOnUpdatingRegisteration(vmReg.stdRegisteration.BatchID);

                    if (feeRec.feeSummary.RollNo!=vmReg.stdRegisteration.Rollno)
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();
                         
                        return "Roll no is invalid!";
                    }
                    else if (rc.Fees.Any(s => s.Bill_No == feeRec.yearlyFee.Bill_No))
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();

                        return "Bill No Already Exists";

                    }
                    else if (feeRec.yearlyFee.Bill_No.Length > 20)
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();
                        return "Bill No must be less than 20 characters!";
                    }
                    else if (sum != feeRec.yearlyFee.Total_Fee)
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();
                        return "Plz Correct Total Fee value!";
                    }
                    else if (feeRec.yearlyFee.Total_Fee > feeRec.feeSummary.Total_Degree_Fee)
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();
                        return "Current Total Fee Must be less Than OR Equal to Total Degree Fee!";
                    }

                    if (feeRec.feeSummary.Total_Installments==null)
                    {
                        if (feeRec.yearlyFee.Installment != null)
                        {
                            foreach (var item in assignSub)
                            {
                                rc.Assign_Subject.Remove(item);
                            }
                            t.Dispose();
                            return "Plz Set Installment Related Fields to Empty For Non Installment Type Student!";
                        }
                    }
                    if (feeRec.feeSummary.Total_Installments != null)
                    {
                        if (feeRec.yearlyFee.Installment == null)
                        {
                            foreach (var item in assignSub)
                            {
                                rc.Assign_Subject.Remove(item);
                            }
                            t.Dispose();
                            return "Plz Fill Installment Related Fields to Empty For Installment Type Student!";
                        }
                        int count = getRecordsOFFEEForStudent;
                        count++;
                        feeRec.feeSummary.Paid_Installments = count;
                    }

                    if (feeRec.feeSummary.Total_Installments!=null && feeRec.yearlyFee.Installment != null)
                    {
                        if (getRecordsOFFEEForStudent == 0)
                        {
                            if (feeRec.yearlyFee.Installment != 1)
                            {
                                foreach (var item in assignSub)
                                {
                                    rc.Assign_Subject.Remove(item);
                                }
                                t.Dispose();
                                return "Installment One Needs to be Paid First!";
                            }
                        }
                        else
                        {
                            var getPaidInstallmentLastWali = rc.Fees
                                .Where(s => s.RollNo == feeRec.feeSummary.RollNo)
                                 .OrderByDescending(s => s.Installment).Select(s => s.Installment).FirstOrDefault();

                            for (int i = 1; i <= getPaidInstallmentLastWali + 1; i++)
                            {
                                if (!rc.Fees.Any(s => s.RollNo == feeRec.feeSummary.RollNo && s.Installment == i))
                                {
                                    if (feeRec.yearlyFee.Installment != i)
                                    {
                                        foreach (var item in assignSub)
                                        {
                                            rc.Assign_Subject.Remove(item);
                                        }
                                        t.Dispose();
                                        return "Plz Enter Installment " + i + "To Continue!";
                                    }
                                }
                            }
 
                        }
                    }

                    decimal v3 = feeRec.yearlyFee.Total_Fee ?? 0;
                    decimal v2 = feeRec.feeSummary.SubmittedFee ?? 0;
                    if (decimal.Truncate(v2)!=0)
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();
                        return "Plz Dont Change Submitted Fee value!";
                    }
                    if ((v2+ v3) > feeRec.feeSummary.Total_Degree_Fee)
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();
                        return "Plz Adjust Total Fee So that Submitted fee is always less than or equal to Total Degree Fee! ";
                    }
                    if (feeRec.feeSummary.RemainingFee < 0)
                    {
                        foreach (var item in assignSub)
                        {
                            rc.Assign_Subject.Remove(item);
                        }
                        t.Dispose();
                        return "Remaining Fee Must be always be Greater Than 0!";
                    }
                    //feeRec.feeSummary.Total_Degree_Fee = values2[0];

                    feeRec.feeSummary.SubmittedFee += v3;
                    feeRec.feeSummary.RemainingFee = feeRec.feeSummary.Total_Degree_Fee - feeRec.feeSummary.SubmittedFee;

                    feeRec.feeSummary.Registeration = vmReg.stdRegisteration;
                    feeRec.yearlyFee.Overall_Fees = feeRec.feeSummary;

                    feeRec.yearlyFee.Dated = date1;
                    feeRec.yearlyFee.Month = MonthsNames[date1.Value.Month];
                    feeRec.yearlyFee.RollNo = feeRec.feeSummary.RollNo;

                    //feeRec.feeSummary.Registeration = rc.Registerations.Where
                    //    (s => s.Rollno == feeRec.feeSummary.RollNo &&
                    //    s.Status == 1).Select(s => s).FirstOrDefault();

                    if (getFeeSummary == null)
                    {
                        rc.Overall_Fees.Add(feeRec.feeSummary);
                        rc.SaveChanges();
                    }
                    else
                    {
                        getFeeSummary.SubmittedFee = feeRec.feeSummary.SubmittedFee;
                        getFeeSummary.RemainingFee = feeRec.feeSummary.RemainingFee;
                        getFeeSummary.Paid_Installments = feeRec.feeSummary.Paid_Installments;
                    }

                    rc.Fees.Add(feeRec.yearlyFee);

                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception e)
                {
                    t.Dispose();
                    return "Unable to Register Student Plz Try Again!";
                }

            }


        }
        public string UpdateStudentRecord(string roll,
            Registeration std, HttpPostedFileBase file,
            string gender, Nullable<System.DateTime> date1
            , string province, string domicile, string dom, string religion)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    var getStudent = rc.Registerations.Where(s => s.Rollno == roll).Select(s => s).FirstOrDefault();

                    getStudent.Student_Profile.FirstName = std.Student_Profile.FirstName;
                    getStudent.Student_Profile.LastName = std.Student_Profile.LastName;
                    getStudent.Student_Profile.Gender = gender;
                    //getStudent.Student_Profile.Date_of_Birth = std.Student_Profile.Date_of_Birth;
                    getStudent.Student_Profile.ContactNo = std.Student_Profile.ContactNo;
                    getStudent.Student_Profile.CNIC = std.Student_Profile.CNIC;
                    getStudent.Student_Profile.Address = std.Student_Profile.Address;
                    //getStudent.Student_Profile.Nationality = std.Student_Profile.Nationality;
                    //getStudent.Student_Profile.Domicile = std.Student_Profile.Domicile;
                    getStudent.Student_Profile.Father_Guardian_Contact = std.Student_Profile.Father_Guardian_Contact;
                    getStudent.Student_Profile.FatherName_Guardian_Name = std.Student_Profile.FatherName_Guardian_Name;
                    getStudent.Student_Profile.Father_Guardian_Occupation = std.Student_Profile.Father_Guardian_Occupation;
                    getStudent.Student_Profile.Date_of_Birth = date1;

                    if (std.Student_Profile.Matric_Marks > std.Student_Profile.Total_Matric_Marks)
                    {
                        return "Obtained Matric Marks Must be Less than Total Matric Marks!";
                    }
                    if ((std.Student_Profile.IntermediateFrom != ""
                        && std.Student_Profile.IntermediateFrom != null)
                        && (std.Student_Profile.Intermediate_Marks.ToString() != ""
                        && std.Student_Profile.Intermediate_Marks != null)
                        && (std.Student_Profile.Total_Inter_Marks.ToString() != "" &&
                        std.Student_Profile.Total_Inter_Marks != null))
                    {
                         
                        if (std.Student_Profile.Intermediate_Marks > std.Student_Profile.Total_Inter_Marks)
                        {
                            return "Obtained Intermediate Marks Must be Less than Total Intermediate Marks!";
                        }
                        getStudent.Student_Profile.IntermediateFrom = std.Student_Profile.IntermediateFrom;
                        getStudent.Student_Profile.Total_Inter_Marks = std.Student_Profile.Total_Inter_Marks;
                        getStudent.Student_Profile.Intermediate_Marks = std.Student_Profile.Intermediate_Marks;
                    }
                    else if ((std.Student_Profile.IntermediateFrom == ""
                        || std.Student_Profile.IntermediateFrom == null)
                        && (std.Student_Profile.Intermediate_Marks.ToString() == ""
                        || std.Student_Profile.Intermediate_Marks == null)
                        && (std.Student_Profile.Total_Inter_Marks.ToString() == "" ||
                        std.Student_Profile.Total_Inter_Marks == null))
                    {

                        getStudent.Student_Profile.IntermediateFrom = std.Student_Profile.IntermediateFrom;
                        getStudent.Student_Profile.Total_Inter_Marks = std.Student_Profile.Total_Inter_Marks;
                        getStudent.Student_Profile.Intermediate_Marks = std.Student_Profile.Intermediate_Marks;
                    }
                    else
                    {
                        return "Plz Enter All The Intermediate Details OR Leave All of Them Empty";
                    }


                    getStudent.Student_Profile.MatricFrom = std.Student_Profile.MatricFrom;
                    getStudent.Student_Profile.Total_Matric_Marks = std.Student_Profile.Total_Matric_Marks;
                    getStudent.Student_Profile.Matric_Marks = std.Student_Profile.Matric_Marks;


                    if (religion == "Please select" || religion == "" || religion == null)
                    {

                    }
                    else
                    {
                        getStudent.Student_Profile.Religion = religion;
                    }

                    if ((province == "Please select" || province == "" || province == null) && (domicile == null || domicile == ""))
                    {
                        //domicile = "";
                        province = getStudent.Student_Profile.Province;
                        dom = getStudent.Student_Profile.Domicile;

                        getStudent.Student_Profile.Domicile = dom;
                        getStudent.Student_Profile.Province = province;
                        //getStudent.Student_Profile.Domicile = domicile;
                    }
                    else
                    {
                        int? provIDprovince = Convert.ToInt32(province);
                        int? domID = Convert.ToInt32(domicile);

                        getStudent.Student_Profile.Province = rc.Provinces.Where(s => s.ID == provIDprovince).Select(s => s.Province_Name).FirstOrDefault() ?? "";
                        getStudent.Student_Profile.Domicile = rc.Districts.Where(s => s.DistrictID == domID).Select(s => s.DistrictName).FirstOrDefault() ?? "";
                    }
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
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }

                catch (Exception e)
                {
                    t.Dispose();
                    return "Unable To Update Record!";
                }

            }


        }
        private string getNewRollNoForStudentOnUpdatingRegisteration(string batch)
        {
            var getLastRegisterationBatchRecord = rc.Registerations.
                Where(s => s.BatchID == batch).
                OrderByDescending(s => s.Rollno).
                Select(s => s.Rollno).FirstOrDefault();
            if (getLastRegisterationBatchRecord == null)
            {
                string s3 = batch + "-" + "1";
                //newRollno = int.Parse(s3);
                return s3;
            }
            //getLastRegisterationBatchRecord= 
            string[] s2 = getLastRegisterationBatchRecord.Split(new char[] { '-' });
            int newRollno = 0;
            //int newRollno = int.Parse(s2[1]);
            if (s2 == null)
            {
                string s3 = batch + "-" + "1";
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
        private bool CompleteNewRegOnDegreeBatchChange(string newRoll,
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
        public string[] UpdateStudentRegRecord(string roll, string degree,
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
                    if (getStudent.Student_Profile.Status==0)
                    {
                        return new string[]{"NotOK","Past Student's Registeration cannot be changed!"};
                    }

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

        public bool AssignSubjectsOnUpdationOfBatchOrDegreeOfStudent(IEnumerable<int?> subjDeg,
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
                                AssignID = Guid.NewGuid(),
                                Batch_Subjects_Parts = rc.Batch_Subjects_Parts.Where(s => s.ID == item.ID).Select(s => s).FirstOrDefault()
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
                                        Batch_Subjects_Parts = rc.Batch_Subjects_Parts.Where(s => s.ID == item2.Batch_Subject_ID).Select(s => s).FirstOrDefault(),
                                        //AssignID = assignID,
                                        Registeration = item2.Registeration
                                        ,
                                        AssignID = Guid.NewGuid()
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
        public IEnumerable<Assign_Subject> getSpecificSearchStudentSubjRecord
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
        public string AddStudentSubject_In_Student_SubjectsView(Assign_Subject std, string rollno, string subjectID,Registeration rec)
        {
            //int sectionID = int.Parse(section);

            try
            {
                //Guid secID = Guid.Parse(section);
                //Guid degID = Guid.Parse(degree);

                var degRecord = rc.Degree_Program.Where(s => s.ProgramID == rec.Batch.DegreeProgram_ID).Select(s => s.Degree_ProgramName).FirstOrDefault();

                //int partID = int.Parse(part);
                Guid subjID = Guid.Parse(subjectID);

                var getRequestedStudent = rc.Registerations.Where(s => s.Rollno == rollno
                    && s.Status == 1).Select(s => s).FirstOrDefault();

                //var getDegreeSubjectBatch = rc.Batches.Where(s => s.BatchName == std.Registeration.BatchID
                //    && s.SectionID == std.Registeration.Batch.SectionID
                //    && s.DegreeProgram_ID == std.Registeration.Batch.DegreeProgram_ID).Select(s => s).FirstOrDefault();

                if (rc.Assign_Subject.Any(
                    s => s.Batch_Subjects_Parts.BatchName == rec.BatchID &&
                    rec.Batch.DegreeProgram_ID == s.Batch_Subjects_Parts.Batch.DegreeProgram_ID
                    && rec.Part == s.Batch_Subjects_Parts.Part
                    && rec.Batch.SectionID == s.Batch_Subjects_Parts.Batch.SectionID
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
                //else if (getDegreeSubjectBatch == null)
                //{
                //    return "Batch " + batch + " doesnot have the Selected Section Or Degree";
                //}
                else
                {
                    //if (getRequestedStudent.BatchID == batch && getRequestedStudent.Batch.SectionID == secID
                    //&& getRequestedStudent.Batch.DegreeProgram_ID == degID)
                    //{
                    var getBpsID = rc.Batch_Subjects_Parts.Where(
                    s => s.BatchName == rec.BatchID &&
                rec.Batch.DegreeProgram_ID == s.Batch.DegreeProgram_ID
                && rec.Part == s.Part
                && rec.Batch.SectionID == s.Batch.SectionID
                && s.SubjectID == subjID
                ).Select(s => s).FirstOrDefault();

                        //if (getBpsID == null)
                        //{
                        //    return "The Batch " + batch + " has not been assigned the Subject: " + rc.Subjects.Where(s => s.SubjectID == subjID).Select(s => s.SubjectName).FirstOrDefault();
                        //}
                        //else
                        //{
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
                        //}

                    //}
                    //else
                    //{
                    //    return "The Student is not Studing the Entered Degree Program " + degRecord + " Or Batch " + batch;
                    //}


                }
            }
            catch (Exception ex)
            {
                //t.Dispose();

                return "Unable TO Assign Subject";
            }


        }

        public IEnumerable<Student_Marks> GetAllStudentMarksRecords()
        {
            var getRecords = rc.Student_Marks
                .Where(s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == 1)
                .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);

            return getRecords;
        }
        public IEnumerable<Student_Marks> getSpecificSearchStudentMarksRecord
            (string rollno, int? StudentType, string year, string Month)
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
                                && s.Year == yearInNumbers)
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if ((rollno == null || rollno == "") && Month != "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Student_Marks.Where(
                            s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                && s.Year == yearInNumbers
                                && s.Month==Month)
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if ((rollno == null || rollno == "") && Month != "None Selected" && (year == null || year == ""))
                {
                     
                        var query = rc.Student_Marks.Where(
                            s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                 
                                && s.Month == Month)
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                     
                }
                else if ((rollno == null || rollno == "") && Month == "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Student_Marks.Where(
                            s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                && s.Year == yearInNumbers
                                )
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
        public IEnumerable<Students_Attendance> GetAllStudentAttendanceRecords()
        {
            var getRecords = rc.Students_Attendance.Where(
                s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == 1).
                OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
            return getRecords;
        }
        public IEnumerable<Students_Attendance> getSpecificSearchStudentAttendanceRecord
            (string rollno, int? StudentType, string year, string Month)
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
                else if ((rollno == null || rollno == "") && Month != "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Students_Attendance.Where(
                            s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
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
                else if ((rollno == null || rollno == "") && Month != "None Selected" && (year == null || year == ""))
                {
                 
                        var query = rc.Students_Attendance.Where(
                            s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                && s.Month == Month )
                            .OrderBy(s => s.Assign_Subject.Registeration.Rollno).Select(s => s);
                        return query;
                  
                }
                else if ((rollno == null || rollno == "") && Month == "None Selected" && (year != null && year != ""))
                {
                    int yearInNumbers = 0;
                    if (int.TryParse(year, out yearInNumbers))
                    {
                        var query = rc.Students_Attendance.Where(
                            s => s.Assign_Subject.Registeration.Student_Profile.Status.Value == StudentType.Value
                                 
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

        public string DeleteStudentRecords(IEnumerable<string> deleteroll)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Student_Profile> distinctStudentProfileList = new List<Student_Profile>();

                    var listToDelete = rc.Registerations.
                    Where(s => deleteroll.Contains(s.Rollno))
                    .Select(s => s.Student_Profile).ToList();

                    foreach (var item in listToDelete)
                    {
                        if (distinctStudentProfileList.Contains(item))
                        {
                            
                        }
                        else
                        {
                            distinctStudentProfileList.Add(item);
                        }
                    }
                    //var listToDelete2 = rc.Student_Profile.
                    //Where(s => deleteroll.Contains(s.Registerations.Rollno))
                    //.Select(s => s).Distinct();
                     
                    foreach (var item in distinctStudentProfileList)
                    {
                        if (item!=null)
                        { 
                                if (item.Status == 0)
                                {
                                    rc.Student_Profile.Remove(item);
                                }
                                else
                                {
                                    item.Status = 0;
                                }      
                        }      
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch
                {
                    t.Dispose();
                    return "Unable To Delete Student Records";
                }
            }
        }
        public string DeleteStudentSubjects(IEnumerable<Guid> deleteroll,int? partcheck)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Assign_Subject> listToDelete =
                        rc.Assign_Subject.Where(s => deleteroll.Contains(s.AssignID)).ToList();
                     
                    if (listToDelete.Any(s=>s.Registeration.Student_Profile.Status == 0))
                    {
                        t.Dispose();
                        return "Past Students Subject Registeration cannot be Changed!";
                    }
                    if (listToDelete != null)
                    {
                        if (listToDelete.Count > 0)
                        {
                            if (listToDelete.Any(s => s.Batch_Subjects_Parts.Part < partcheck))
                            {
                                return "Previous part's Subjects cannot be deleted!";
                            }
                        }
                    }
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

         
        public string DeleteStudentMarksRecords(IEnumerable<Guid> deleteroll)
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
                    t.Dispose();
                    return "Unable To Delete Records!";
                }
            }
        }
        public string DeleteStudentAttendanceRecords(IEnumerable<Guid> deleteroll)
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
                    t.Dispose();
                    return "Unable To Delete Records!";
                }
            }
        }
        #endregion

        #region Fee Module 
        public IEnumerable<Overall_Fees> getSpecificSearchRecordFeeSummary(string rollno, int? StudentType)
        {
            if (rollno == null || rollno == "")
            {
                if (StudentType != null)
                {
                    var query = rc.Overall_Fees.Where(s => s.Registeration.Student_Profile.Status == StudentType.Value)
                       .OrderBy(s => s.RollNo).Select(s => s);
                    return query;
                }
                else
                {
                    var query = rc.Overall_Fees.Where(s => s.Registeration.Student_Profile.Status == 1)
                       .OrderBy(s => s.RollNo).Select(s => s);
                    //var query = rc.Registerations.Where(s => s.re.Student_Profile.Status == 1)
                    //   .OrderBy(s => s.Rollno).Select(s => s);
                    return query;
                }
            }
            else
            {
                if (StudentType != null)
                {
                    var query = rc.Overall_Fees.Where(s => s.RollNo.StartsWith(rollno)
                       && s.Registeration.Student_Profile.Status == StudentType.Value)
                       .OrderBy(s => s.RollNo).Select(s => s);
                    return query;
                }
                else
                {
                    var query = rc.Overall_Fees.Where(s => s.RollNo.StartsWith(rollno)
                       && s.Registeration.Student_Profile.Status == 1)
                       .OrderBy(s => s.RollNo).Select(s => s);
                    return query;
                }
            }
        }
        public IEnumerable<Overall_Fees> getSpecificSearchRecordOverAllFees(string rollno, int? StudentType)
        {
            if (rollno == null || rollno == "")
            {
                if (StudentType != null)
                {
                    var query = rc.Overall_Fees.Where(s => s.Registeration.Student_Profile.Status == StudentType.Value)
                       .OrderBy(s => s.RollNo).Select(s => s);
                    return query;
                }
                else
                {
                    var query = rc.Overall_Fees.Where(s => s.Registeration.Student_Profile.Status == 1)
                       .OrderBy(s => s.RollNo).Select(s => s);
                     
                    return query;
                }
            }
            else
            {
                if (StudentType != null)
                {
                    var query = rc.Overall_Fees.Where(s => s.RollNo.StartsWith(rollno)
                        && s.Registeration.Student_Profile.Status == StudentType.Value)
                   .OrderBy(s => s.RollNo).Select(s => s); 
                    return query;
                }
                else
                {
                    var query = rc.Overall_Fees.Where(s => s.RollNo.StartsWith(rollno)
                        && s.Registeration.Student_Profile.Status == 1)
                   .OrderBy(s => s.RollNo).Select(s => s); 
                    return query;
                }
            }
        }

        public IEnumerable<Overall_Fees> getFeeRecordsOverall()
        {
            var getRecords = rc.Overall_Fees.Where(s => s.Registeration.Student_Profile.Status == 1)
                .OrderBy(s => s.RollNo).Select(s => s);
            return getRecords;
        }
        public IEnumerable<Fee> showFee_EmployeeModelFunction(string search,
            string Month, int? StudentType, string year)
        {
            try
            {
                if (search == null || search == "")// Check if search is empty then
                {
                    //if no year and no month is entered then return search by student type
                    //if year is entered but no month is entered then return year related records
                    //if month is entered but no year is entered then return month related records
                    if ((year == null || year == "") && Month == "None Selected")
                    {
                        var getFeeRecords = rc.Fees.Where(s =>
                            s.Overall_Fees.Registeration.Student_Profile.Status == StudentType).Select(s => s).OrderBy(s => s.Overall_Fees.RollNo);
                        return getFeeRecords;
                    }
                    else if ((year != null || year != "") && Month == "None Selected")
                    {
                        int yearInNumbers = 0;
                        if (int.TryParse(year, out yearInNumbers))
                        {
                            var getFeeRecords = rc.Fees.Where(s =>
                            s.Overall_Fees.Registeration.Student_Profile.Status == StudentType
                            && s.Dated.Value.Year == yearInNumbers).Select(s => s).OrderBy(s => s.Overall_Fees.RollNo);

                            return getFeeRecords;
                        }
                        else
                        {
                            return null;
                        }

                    }
                    else if ((year == null || year == "") && Month != "None Selected")
                    {
                        int monthNo = GetMonthNoForMonthName(Month);

                        if (monthNo == 13)
                        {
                            return null;
                        }
                        else
                        {
                            var getFeeRecords = rc.Fees.Where(s =>
                            s.Overall_Fees.Registeration.Student_Profile.Status == StudentType
                            && s.Dated.Value.Month == monthNo).Select(s => s).OrderBy(s => s.Overall_Fees.RollNo);

                            return getFeeRecords;
                        }

                    }
                    else
                    {
                        int yearInNumbers = 0;
                        int monthNo = GetMonthNoForMonthName(Month);

                        if (int.TryParse(year, out yearInNumbers) || monthNo != 13)
                        {
                            var getFeeRecords = rc.Fees.Where(s =>
                            s.Overall_Fees.Registeration.Student_Profile.Status == StudentType
                            && s.Dated.Value.Year == yearInNumbers
                            && s.Dated.Value.Month == monthNo).Select(s => s).OrderBy(s => s.Overall_Fees.RollNo);

                            return getFeeRecords;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    if ((year == null || year == "") && Month == "None Selected")
                    {
                        var getFeeRecords = rc.Fees.Where(s =>
                            s.Overall_Fees.Registeration.Student_Profile.Status == StudentType
                            && s.Overall_Fees.RollNo.StartsWith(search)).Select(s => s).OrderBy(s => s.Overall_Fees.RollNo);
                        return getFeeRecords;
                    }
                    else if ((year != null || year != "") && Month == "None Selected")
                    {
                        int yearInNumbers = 0;
                        if (int.TryParse(year, out yearInNumbers))
                        {
                            var getFeeRecords = rc.Fees.Where(s =>
    s.Overall_Fees.Registeration.Student_Profile.Status == StudentType
    && s.Dated.Value.Year == yearInNumbers
    && s.Overall_Fees.RollNo.StartsWith(search)).Select(s => s).OrderBy(s => s.Overall_Fees.RollNo);

                            return getFeeRecords;
                        }
                        else
                        {
                            return null;
                        }

                    }
                    else if ((year == null || year == "") && Month != "None Selected")
                    {
                        int monthNo = GetMonthNoForMonthName(Month);

                        if (monthNo == 13)
                        {
                            return null;
                        }
                        else
                        {
                            var getFeeRecords = rc.Fees.Where(s =>
                            s.Overall_Fees.Registeration.Student_Profile.Status == StudentType
                            && s.Dated.Value.Month == monthNo
                            && s.Overall_Fees.RollNo.StartsWith(search)).Select(s => s).OrderBy(s => s.Overall_Fees.RollNo);

                            return getFeeRecords;
                        }

                    }
                    else
                    {
                        int yearInNumbers = 0;
                        int monthNo = GetMonthNoForMonthName(Month);

                        if (int.TryParse(year, out yearInNumbers) || monthNo != 13)
                        {
                            var getFeeRecords = rc.Fees.Where(s =>
                            s.Overall_Fees.Registeration.Student_Profile.Status == StudentType
                            && s.Dated.Value.Year == yearInNumbers
                            && s.Dated.Value.Month == monthNo
                            && s.Overall_Fees.RollNo.StartsWith(search)).Select(s => s).OrderBy(s =>s.Overall_Fees.RollNo);

                            return getFeeRecords;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }


            }
            catch (Exception)
            {
                return null;
            }

            //if (search == "" || search == null)
            //{
            //    var getFeeRecords = rc.Fees.Where(s => s.Month == month &&
            //        s.Registeration.Student_Profile.Status.Value == StudentType.Value).Select(s => s).OrderBy(s => s.Rollno);
            //    return getFeeRecords;
            //}
            //else
            //{
            //    var getFeeRecords = rc.Fees.Where(s => s.Month == month &&
            //        s.Rollno.StartsWith(search)
            //        && s.Registeration.Student_Profile.Status == StudentType.Value)
            //        .Select(s => s).OrderBy(s => s.Rollno);
            //    return getFeeRecords;
            //}

        }
        public string AddFeeRec(ViewModel_FeeManagement feeRec, Nullable<System.DateTime> date1,
            string totaldegfee, string totalSubmitfee, string totalremfee, 
            string totalinstall, string paidInst)
        { 
            using (TransactionScope t=new TransactionScope())
            {
                try
            {
                    decimal[] values2 =new decimal[10];
                    if (!(decimal.TryParse(totaldegfee, out values2[0]) && 
                        decimal.TryParse(totalSubmitfee, out values2[1]) &&
                        decimal.TryParse(totalremfee, out values2[2])))
                {
                    return "Plz Enter Correct Fee Summary!";
                }
                    //Sum For COmparison with Total FEE
                decimal[] values = new decimal[10];
                    values[0]=feeRec.yearlyFee.Tution_Fee ?? 0;
                    values[1]=feeRec.yearlyFee.Registeration_Fee ?? 0;
                    //values[2]=feeRec.yearlyFee.Fine ?? 0;
                    values[3]=feeRec.yearlyFee.Admission_Fee ?? 0 ;
                    values[4] = feeRec.yearlyFee.Exam_Fee ?? 0;

                    decimal sum2 = decimal.Round(feeRec.yearlyFee.Total_Fee??0);
                    //sum2 = decimal.Round(sum2);
                    decimal sum = values[0] + values[1]  + values[3] + values[4];
                    sum = decimal.Round(sum);

                var getRecordsOFFEEForStudent = rc.Fees.Where(s => s.RollNo == feeRec.feeSummary.RollNo).Select(s => s).Count();

                var getIfPresentRollno = rc.Registerations.Where(s => s.Rollno == feeRec.feeSummary.RollNo &&s.Student_Profile.Status==1
                    && s.Status == 1).Select(s => s).FirstOrDefault();

                var getFeeSummary = rc.Overall_Fees.Where(s => s.RollNo == feeRec.feeSummary.RollNo).Select(s => s).FirstOrDefault();

                if (getFeeSummary == null || getFeeSummary != null)
                {
                    if (getFeeSummary == null)
                    {
                        if (values2[1] != 0 || values2[2] != 0 || feeRec.feeSummary.Paid_Installments!=null)
                        {
                            return "Plz Make Sure that Student Fee Summary is Accurate!";
                        }
                    }
                    else
                    {
                        if (getFeeSummary.SubmittedFee != values2[1] ||
                            getFeeSummary.RemainingFee != values2[2]
                            ||
                            getFeeSummary.Total_Degree_Fee != values2[0] )
                        {
                            return "Plz Make Sure that Student Fee Summary is Accurate!";
                        }
                        if (totalinstall !=null &&totalinstall!="")
                        {
                            if (int.Parse(totalinstall)!=getFeeSummary.Total_Installments)
                            {
                                return "Plz Make Sure that Student Fee Summary is Accurate!";
                            }
                        }
                        if (paidInst != null && paidInst != "")
                        {
                            if (int.Parse(paidInst)!=getFeeSummary.Paid_Installments)
                            {
                                return "Plz Make Sure that Student Fee Summary is Accurate!";
                            }
                        }
                    }
                }
                if (getIfPresentRollno == null)
                {
                    return "Roll no is invalid Or UnActive";
                }
                else if (rc.Fees.Any(s => s.Bill_No == feeRec.yearlyFee.Bill_No))
                {
                    return "Bill No Already Exists";

                }
                else if (feeRec.yearlyFee.Bill_No.Length>20)
                {
                    return "Bill No must be less than 20 characters!";
                }
                else if (sum != sum2)
                {
                    return "Plz Correct Total Fee value!";
                }
                else if (feeRec.yearlyFee.Total_Fee>values2[0])
                {
                    return "Current Total Fee Must be less Than OR Equal to Total Degree Fee!";
                }
                 
                if ((totalinstall == null || totalinstall==""))
                {
                    if (feeRec.yearlyFee.Installment!=null)
                    {
                        return "Plz Set Installment Related Fields to Empty For Non Installment Type Student!";
                    }
                }
                if ((totalinstall != null && totalinstall != ""))
                {
                    if (feeRec.yearlyFee.Installment == null )
                    {
                        return "Plz Fill Installment Related Fields to Empty For Installment Type Student!";
                    }
                     
                }

                if ((totalinstall != null && totalinstall != "") && feeRec.yearlyFee.Installment != null)
                  {
                      int totalInstallmentsVal = int.Parse(totalinstall);
                       
                       if (getRecordsOFFEEForStudent == 0)
                       {
                           if (feeRec.yearlyFee.Installment != 1)
                           {
                               return "Installment One Needs to be Paid First!";
                           }
                       }
                       else
                       {
                           int k = 0;
                           var getPaidInstallmentLastWali = rc.Fees
                               .Where(s => s.RollNo == feeRec.feeSummary.RollNo)
                                .OrderByDescending(s => s.Installment).Select(s => s.Installment).FirstOrDefault();

                           for (int i = 1; i <= getPaidInstallmentLastWali+1; i++)
                           {
                               if (!rc.Fees.Any(s => s.RollNo == feeRec.feeSummary.RollNo 
                                   && s.Installment == i))
                               {
                                   k = i;
                                   break;
                               }
                           }
                            
                           if (k > totalInstallmentsVal)
                           {
                               return "All the Installments have been Paid!";
                           }
                           if (feeRec.yearlyFee.Installment != k)
                           {
                               return "Plz Enter Installment " + k + "To Continue!";
                           }
                             
                       }
                       feeRec.feeSummary.Total_Installments = totalInstallmentsVal;
                       int count = getRecordsOFFEEForStudent;
                       count++;
                       feeRec.feeSummary.Paid_Installments = count;
                  }
                   
                    decimal v3=feeRec.yearlyFee.Total_Fee??0;
                  if ((values2[1]+v3)>values2[0])
                  {
                      return "Plz Adjust Total Fee So that Submitted fee is always less than or equal to Total Degree Fee! ";
                  }
                  if (values2[2]<0)
                  {
                      return "Remaining Fee Must be always be Greater Than 0!";
                  }
                  feeRec.feeSummary.Total_Degree_Fee = values2[0];
                  feeRec.feeSummary.SubmittedFee = values2[1];
                  feeRec.feeSummary.RemainingFee = values2[2];
                   
                    feeRec.feeSummary.SubmittedFee+=v3;
                    feeRec.feeSummary.RemainingFee = feeRec.feeSummary.Total_Degree_Fee - feeRec.feeSummary.SubmittedFee;

                    feeRec.yearlyFee.Dated = date1;
                    feeRec.yearlyFee.Month = MonthsNames[date1.Value.Month];
                    feeRec.yearlyFee.RollNo = feeRec.feeSummary.RollNo;

                    feeRec.feeSummary.Registeration = rc.Registerations.Where
                        (s => s.Rollno == feeRec.feeSummary.RollNo &&
                        s.Status == 1).Select(s => s).FirstOrDefault(); 

                    if (getFeeSummary==null)
                    {
                        rc.Overall_Fees.Add(feeRec.feeSummary);
                        rc.SaveChanges();
                    }
                    else
                    {
                        getFeeSummary.SubmittedFee = feeRec.feeSummary.SubmittedFee;
                        getFeeSummary.RemainingFee = feeRec.feeSummary.RemainingFee;
                        getFeeSummary.Paid_Installments = feeRec.feeSummary.Paid_Installments;
                        getFeeSummary.Total_Degree_Fee = values2[0];
                    }
                     
                    rc.Fees.Add(feeRec.yearlyFee);
                     
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                 
            }
                catch (DbEntityValidationException e)
                {
                    t.Dispose(); 
                    foreach (var eve in e.EntityValidationErrors)
                    {
                      //  eve.ValidationErrors
                        //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            return ve.ErrorMessage;
                        }
                    }
                    return "Validation Failed for one or more Entries!";
                    //    throw;
                }
            catch (Exception)
            {
                t.Dispose();
                return "Unable to Add Fee Record";
                //   throw;
            }
  
            } 
        }

        public string CheckFeeEditied(Fee feeRec, string bill)
        {
            return "OK";
        }
        public string UpdateFeeRec(ViewModel_FeeManagement feeRec, string rollno, string billno, Nullable<System.DateTime> date1)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    var getRecordStd1 = rc.Registerations.Where(s => s.Rollno == rollno).Select(s => s).FirstOrDefault();

                    if (getRecordStd1==null)
                    {
                        return "Student Roll no is invalid!";
                    }
                    else if (getRecordStd1.Student_Profile.Status==0)
                    {
                        return "Past Student Fee Cannot be Changed!";
                    }

                    feeRec.yearlyFee.Bill_No = billno;
                    var getRequestedFeeRecord = rc.Fees.Where(s => s.Bill_No == feeRec.yearlyFee.Bill_No).Select(s => s).FirstOrDefault();

                    var getSummary = getRequestedFeeRecord.Overall_Fees;
                    var getRecordsForRollNoFee = getRequestedFeeRecord.Overall_Fees.Fees.Where(s=>s.Bill_No!=billno).Select(s=>s);

                    decimal feeTotalSub=0;
                    if (getRequestedFeeRecord != null)
                    {
                        foreach (var item in getRecordsForRollNoFee)
                        { 
                            feeTotalSub += item.Total_Fee??0;
                        }
                        decimal finalSubFee = feeTotalSub + feeRec.yearlyFee.Total_Fee??0;
                        if (finalSubFee>getSummary.Total_Degree_Fee)
                        {
                            return "Plz Adjust Total Fee So that Submitted fee is always less than or equal to Total Degree Fee! ";
                        }

                        decimal[] values = new decimal[10];
                        values[0] = feeRec.yearlyFee.Tution_Fee ?? 0;
                        values[1] = feeRec.yearlyFee.Registeration_Fee ?? 0;
                        //values[2] = feeRec.yearlyFee.Fine ?? 0;
                        values[3] = feeRec.yearlyFee.Admission_Fee ?? 0;
                        values[4] = feeRec.yearlyFee.Exam_Fee ?? 0;

                        values[5] = getRequestedFeeRecord.Total_Fee??0;
                        decimal sum = values[0]+values[1]+values[3]+values[4];
                        if (sum!=feeRec.yearlyFee.Total_Fee)
                        {
                            return "Plz Correct Total Fee value!";        
                        }          
                        if (values[5] !=feeRec.yearlyFee.Total_Fee)
                        { 
                            getSummary.SubmittedFee -= getRequestedFeeRecord.Total_Fee;
                            getSummary.SubmittedFee += feeRec.yearlyFee.Total_Fee;
                            getSummary.RemainingFee = getSummary.Total_Degree_Fee - getSummary.SubmittedFee;

                            getRequestedFeeRecord.Overall_Fees.Total_Degree_Fee =decimal.Truncate( getSummary.Total_Degree_Fee??0);
                            getRequestedFeeRecord.Overall_Fees.SubmittedFee = decimal.Truncate( getSummary.SubmittedFee??0);
                            getRequestedFeeRecord.Overall_Fees.RemainingFee = decimal.Truncate( getSummary.RemainingFee??0);
                            //rc.Entry(getSummary).State = EntityState.Modified;
                            //rc.SaveChanges();
                        }
                        getRequestedFeeRecord.Admission_Fee = feeRec.yearlyFee.Admission_Fee;
                        //getRequestedFeeRecord.Dated = date1;
                        getRequestedFeeRecord.Exam_Fee = feeRec.yearlyFee.Exam_Fee;
                        getRequestedFeeRecord.Fine = feeRec.yearlyFee.Fine;
                        //getRequestedFeeRecord.Installment = feeRec.yearlyFee.Installment;
                        getRequestedFeeRecord.Registeration_Fee = feeRec.yearlyFee.Registeration_Fee;
                        getRequestedFeeRecord.Total_Fee = feeRec.yearlyFee.Total_Fee;
                        //getRequestedFeeRecord.Total_Installments = feeRec.Total_Installments;
                        getRequestedFeeRecord.Tution_Fee = feeRec.yearlyFee.Tution_Fee;
                         
                        rc.SaveChanges();
                        t.Complete();
                        return "OK";
                    }
                    else
                    {
                        return "Unable to Update Fee Record!";
                    }
                }
                catch (DbEntityValidationException e)
                {
                    t.Dispose();
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        //  eve.ValidationErrors
                        //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            return ve.ErrorMessage;
                        }
                    }
                    return "Validation Failed for one or more Entries!";
                //    throw;
                }

                catch (Exception)
                {
                    t.Dispose();
                    return "Unable to Update Fee Record!";
                }  
            }
              
        }
        public string DeleteFeeSummaryRecordsOfStudents(IEnumerable<string> deleteRoll)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Overall_Fees> listToDelete = rc.Overall_Fees.Where(s => deleteRoll.Contains(s.RollNo)).ToList();

                    if (listToDelete.Any(s=>s.Registeration.Status==0))
                    {
                        return "Fee Summary of Students having Deactivated Registeration cannot be Deleted!";
                    }
                    if (listToDelete.Any(s => s.Registeration.Student_Profile.Status == 0))
                    {
                        return "Fee Summary of Past Students cannot be Deleted!";
                    }

                    foreach (var item in listToDelete)
                    { 
                        rc.Overall_Fees.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    t.Dispose();
                    return "Unable To Delete Records! Plz Try Again!";
                }
            }
        }
        public string DeleteFeeRecordsOfStudents(IEnumerable<string> deletefee)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    List<Fee> listToDelete = rc.Fees.Where(s => deletefee.Contains(s.Bill_No)).OrderByDescending(s=>s.Total_Fee).Select(s=>s).ToList();
                    if (listToDelete.Any(s=>s.Overall_Fees.Registeration.Status==0))
                    {
                        return "Fee Records of Students having Deactivated Registeration cannot be Deleted!";
                    }
                    if (listToDelete.Any(s => s.Overall_Fees.Registeration.Student_Profile.Status == 0))
                    {
                        return "Fee Records of Past Students cannot be Deleted!";
                    }
                    decimal? getSumRec = 0;
                    foreach (var item in listToDelete)
                    {
                        getSumRec = 0;
                        var getSummaryFeeRec = rc.Overall_Fees.Where(s => s.RollNo == item.RollNo).Select(s => s).FirstOrDefault();
                        var recTotalFees = rc.Fees.Where(s => s.Bill_No != item.Bill_No && s.RollNo == item.RollNo).Select(s => s);
                        if (recTotalFees!=null)
                        {
                            if (recTotalFees.Count()>0)
                            {
                                foreach (var item12 in recTotalFees)
                                {
                                    getSumRec += item12.Total_Fee;
                                }   
                            }
                            else
                            {
                                getSumRec = 0;
                            }
                             
                        }
                        else
                        {
                            getSumRec = 0;
                        }
                         

                        //vals[0] = getSumRec??0;
                        //string s123 = decimal.Truncate(getSumRec ?? 0).ToString();
                        //vals[0] =
                        getSummaryFeeRec.SubmittedFee = int.Parse(decimal.Truncate(getSumRec ?? 0).ToString()); ;
                        getSummaryFeeRec.RemainingFee = 
                            int.Parse(decimal.Truncate(getSummaryFeeRec.Total_Degree_Fee??0).ToString()) - int.Parse(decimal.Truncate(getSummaryFeeRec.SubmittedFee??0).ToString());

                        item.Overall_Fees.SubmittedFee = getSummaryFeeRec.SubmittedFee;
                        item.Overall_Fees.RemainingFee = getSummaryFeeRec.RemainingFee;
                        item.Overall_Fees.Total_Degree_Fee =int.Parse( decimal.Truncate(getSummaryFeeRec.Total_Degree_Fee??0).ToString());
                        //getSummaryFeeRec.SubmittedFee -= item.Total_Fee;
                        //dValues[0] += getSummaryFeeRec.SubmittedFee;
                        //getSummaryFeeRec.SubmittedFee = dValues[0];
                        //getSummaryFeeRec.RemainingFee = getSummaryFeeRec.Total_Degree_Fee - getSummaryFeeRec.SubmittedFee;
                        //dValues[1] += getSummaryFeeRec.RemainingFee;
                        //getSummaryFeeRec.RemainingFee = dValues[1];
                        if (getSummaryFeeRec.Total_Installments!=null)
                        {
                            getSummaryFeeRec.Paid_Installments--;
                            //item.Overall_Fees.Paid_Installments = getSummaryFeeRec.Paid_Installments;
                        } 
                        rc.Fees.Remove(item);
                        rc.SaveChanges();
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (DbEntityValidationException e)
                {

                     
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        //foreach (var ve in eve.ValidationErrors)
                        //{
                        //    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        //        ve.PropertyName, ve.ErrorMessage);
                        //}
                    }
                    return "Validation Failed for one or more Entries!";
                    //throw;
                }
                catch (Exception)
                {
                    t.Dispose();
                    return "Unable To Delete Records!";
                }
            }
        }
        #endregion

        #region Employee Manangement By Admin
        public Guid getNewEmployeeID()
        {
            Guid gd = new Guid();
            gd = Guid.NewGuid();
            return gd;
        }
        public string UpdateEmployeeRecord(Guid ID, Employee std, HttpPostedFileBase file,
            Nullable<System.DateTime> date1, string gender, string religion, string Marriedstatus)
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    var getEMP = rc.Employees.Where(s => s.EmpID == ID).Select(s => s).FirstOrDefault();
                    if (religion != "Please select")
                    {
                        getEMP.Religion = religion;
                    }
                    if (getEMP.UserName!=std.UserName)
                    {
                        var getAllEMPusernamediff = rc.Employees.Where(s => s.EmpID != ID).Select(s => s.UserName);

                        if (getAllEMPusernamediff.Any(s=>s==std.UserName))
                        {
                            return "UserName already Exists!";
                        }
                    }
                    getEMP.Name = std.Name;
                    getEMP.Address = std.Address;
                    getEMP.Gender = gender;
                    getEMP.Date_of_Birth = date1;
                    getEMP.ContactNo = std.ContactNo;
                    getEMP.Salary = std.Salary;
                    getEMP.CNIC = std.CNIC;
                    getEMP.Password = std.Password;
                    getEMP.Martial_Status = Marriedstatus;
                    // getEMP.Date_of_Birth = std.Date_of_Birth;
                    getEMP.UserName = std.UserName;

                    if (file != null && file.ContentType.Contains("image") && file.ContentLength > 0 && file.ContentLength <= 3048576)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            getEMP.Picture = array;
                        }
                    }
                    else if (file != null)
                    {
                        if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                        {
                            return "Plz Select Image File Less than 3 MB";
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
                    t.Dispose();
                    return "Unable to Update Employee";
                }
            }
        }
        public IEnumerable<Employee> getSpecificSearchRecordForEmployees(string name)
        {
            var query = rc.Employees.Where(s => s.Name.StartsWith(name)).OrderBy(s => s.Name).Select(s => s);
            return query;
        }
        public string NewEmployeeAddition(Employee tRec, HttpPostedFileBase file, Nullable<System.DateTime> date1
            , string gender, string religion, string Marriedstatus)
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
                        return "Unable to Upload Image!";
                    }

                }
                else if (file != null)
                {
                    if ((file.ContentLength > 0 && !file.ContentType.Contains("image")) || file.ContentLength > 3048576)
                    {
                        return "Plz Select Image File Less than 3 MB";
                    }
                }
                if (rc.Employees.Any(s => s.UserName == tRec.UserName))
                {
                    return "An Employee with UserName: " + tRec.UserName + " already Exists!";
                }
                tRec.Martial_Status = Marriedstatus;
                tRec.Religion = religion;
                tRec.Gender = gender;
                tRec.Date_of_Birth = date1;
                tRec.Status = "Active";
                rc.Employees.Add(tRec);
                rc.SaveChanges();
                return "OK";
            }
            catch (Exception)
            {
                return "Unable to Add Employee";
            }
        }

        public IEnumerable<Employee> GetAllEmployeesRecords()
        {
            var getRecords = rc.Employees.OrderBy(s => s.Name).Select(s => s);
            return getRecords;
        }

        #endregion

        #region Reports
        public List<MarksBYTeacherReportClass> GetStudentMarksListForReport(string month, int? StudentType ,
              string search, string year)
        {
            List<MarksBYTeacherReportClass> mbtrc = new List<MarksBYTeacherReportClass>();
            IEnumerable<Student_Marks> sttMarks = getSpecificSearchStudentMarksRecord(search, StudentType, year, month);
            //Guid secGuid = new Guid();
            //Guid degGuid = new Guid();
            //int yearInNumber = 0;

            try
            {
                if (sttMarks != null)
                {
                    foreach (var p in sttMarks)
                    { 
                        if(p.Year.HasValue)
                        { 
                        MarksBYTeacherReportClass marksReportView = new MarksBYTeacherReportClass()
                        {
                            RollNo = p.Assign_Subject.Registeration.Rollno,
                            Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                            TotalMarks = p.Total_Marks ?? 0,
                            ObtainedMarks = p.Obtained_Marks ?? 0,
                            Marks_Percentage = p.Marks_Percentage,
                            Month = p.Month,
                            Year = p.Year.Value.ToString(),
                            //extra
                            SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                            SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                            BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                            Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                        };
                        mbtrc.Add(marksReportView);
                              }
                        else
                        {
                            MarksBYTeacherReportClass marksReportView = new MarksBYTeacherReportClass()
                            {
                                RollNo = p.Assign_Subject.Registeration.Rollno,
                                Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                                TotalMarks = p.Total_Marks ?? 0,
                                ObtainedMarks = p.Obtained_Marks ?? 0,
                                Marks_Percentage = p.Marks_Percentage,
                                Month = p.Month,
                                Year = "",
                                //extra
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
        public List<AttendanceBYTeacherReportClass> GetStudentAttendanceListForReport(string month, int? StudentType,
              string search, string year)
        {
            List<AttendanceBYTeacherReportClass> mbtrc = new List<AttendanceBYTeacherReportClass>();
            IEnumerable<Students_Attendance> sttAtt = getSpecificSearchStudentAttendanceRecord(search, StudentType, year, month);
            //Guid secGuid = new Guid();
            //Guid degGuid = new Guid();
            //int yearInNumber = 0;

            try
            {
                if (sttAtt != null)
                {
                    foreach (var p in sttAtt)
                    {
                        if (p.Year.HasValue)
                        {
                            AttendanceBYTeacherReportClass marksReportView = new AttendanceBYTeacherReportClass()
                            {
                                RollNo = p.Assign_Subject.Registeration.Rollno,
                                Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                                Total_lectures = p.Total_lectures ?? 0,
                                Attended_Lectures = p.Attended_Lectures ?? 0,
                                Attendance_Percentage = p.Attendance_Percentage,
                                Month = p.Month,
                                Year = p.Year.Value.ToString(),
                                //extra
                                SubjectName = p.Assign_Subject.Batch_Subjects_Parts.Subject.SubjectName,
                                SectionName = p.Assign_Subject.Registeration.Batch.Section.SectionName,
                                BatchName = p.Assign_Subject.Registeration.Batch.BatchName,
                                Degree_Programme = p.Assign_Subject.Registeration.Batch.Degree_Program.Degree_ProgramName
                            };
                            mbtrc.Add(marksReportView);
                        }
                        else
                        {
                            AttendanceBYTeacherReportClass marksReportView = new AttendanceBYTeacherReportClass()
                            {
                                RollNo = p.Assign_Subject.Registeration.Rollno,
                                Name = p.Assign_Subject.Registeration.Student_Profile.FirstName + " " + p.Assign_Subject.Registeration.Student_Profile.LastName,

                                Total_lectures = p.Total_lectures ?? 0,
                                Attended_Lectures = p.Attended_Lectures ?? 0,
                                Attendance_Percentage = p.Attendance_Percentage,
                                Month = p.Month,
                                Year = "",
                                //extra
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
        public IEnumerable<RptFeeSummaryClass> GetReportModelForFeeSummary(string search2, int? StudentType)
        {
            try
            {
                List<RptFeeSummaryClass> lpc2 = new List<RptFeeSummaryClass>();

                IEnumerable<Overall_Fees> lpc = getSpecificSearchRecordFeeSummary(search2, StudentType);
                if (lpc == null)
                {
                    return null;
                }
                foreach (var item in lpc)
                {
                    lpc2.Add(new RptFeeSummaryClass
                    {
                         RollNo=item.RollNo,
                         Total_Degree_Fee=item.Total_Degree_Fee??0,
                         Total_Installments=item.Total_Installments??0,
                         Paid_Installments=item.Paid_Installments??0,
                         RemainingFee=item.RemainingFee??0,
                         SubmittedFee=item.SubmittedFee??0,
                    });
                }
                return lpc2;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public IEnumerable<TeacherAttendanceReportClass> GetReportModelForTeacherAttendance(string search, string year, string month)
        {
            //int yearInNumber = 0;
            try
            {
                List<TeacherAttendanceReportClass> lpc = new List<TeacherAttendanceReportClass>();
                IEnumerable<Teacher_Attendance> teacherAttlist = showResultsTeacherAttendance_EmployeeModelFunction(search, month, year);

                if (teacherAttlist != null)
                {
                    foreach (var item in teacherAttlist)
                    {
                        lpc.Add(new TeacherAttendanceReportClass
                        {
                            TeacherID = item.TeacherID,
                            Name = item.Teacher.Name,
                            Date = item.Date,
                            Present = item.Present
                        });
                    }
                    return lpc;
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

        public IEnumerable<ReportClassForStudentsListByEmployee> GetReportModelForStudentList(string search2, int? StudentType, string searchfname, string searchdeg
            , string searchsection, string searchpart)
        {
            try
            {
                List<ReportClassForStudentsListByEmployee> lpc2 = new List<ReportClassForStudentsListByEmployee>();

                IEnumerable<Registeration> lpc = getSpecificSearchRecord(search2, StudentType,searchfname,searchdeg
                    ,searchsection,searchpart);
                if (lpc == null)
                {
                    return null;
                }
                foreach (var item in lpc)
                {
                    lpc2.Add(new ReportClassForStudentsListByEmployee
                    {
                        Rollno = item.Rollno,
                        Name = item.Student_Profile.FirstName + " " + item.Student_Profile.LastName,
                        Gender = item.Student_Profile.Gender,
                        Degree_ProgramName = item.Batch.Degree_Program.Degree_ProgramName,
                        Part = item.Part.Value,
                        Section=item.Batch.Section.SectionName,
                        Duration = item.Batch.Year.FromYear.Value + "-" + item.Batch.Year.ToYear.Value
                    });
                }
                return lpc2;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public IEnumerable<FeeReportCLass> GetReportDataForStudentFeesRecords(string search2, string month2, int? StudentType, string year)
        {
            try
            {
                List<FeeReportCLass> lpc2 = new List<FeeReportCLass>();

                IEnumerable<Fee> lpc = showFee_EmployeeModelFunction(search2, month2, StudentType, year);
                if (lpc == null)
                {
                    return null;
                }
                foreach (var item in lpc)
                {
                    lpc2.Add(new FeeReportCLass
                    {
                        Rollno = item.Overall_Fees.RollNo,
                        StudentName = item.Overall_Fees.Registeration.Student_Profile.FirstName + " " + item.Overall_Fees.Registeration.Student_Profile.LastName,
                        Admission_Fee = item.Admission_Fee ?? 0,
                        Bill_No = item.Bill_No,
                        Dated = item.Dated.Value,
                        Exam_Fee = item.Exam_Fee ?? 0,
                        Fine = item.Fine ?? 0,
                        Installment = item.Installment ?? 0,
                        Total_Fee = item.Total_Fee ?? 0,
                        Total_Installments = 0,
                        Month = item.Month,
                        Registeration_Fee = item.Registeration_Fee ?? 0,
                        Tution_Fee = item.Tution_Fee ?? 0
                    });
                }
                return lpc2;
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion

        #region Others
        //public   ViewModel_Registeration getRegisterationInstanceForNewAdmission()
        //{
        //    if (vmReg == null)
        //    {
        //        vmReg = new ViewModel_Registeration();
        //        return vmReg;
        //    }
        //    return vmReg;
        //}
        public void SetStudentProfileNewAdmission(Student_Profile std, string gender, string religion)
        {
            vmReg.stdProfile = std;
            vmReg.stdProfile.Gender = gender;
            vmReg.stdProfile.Religion = religion;
        }
        public void SetStudentProfileNewAdmissionEducationDetails(Student_Profile std)
        {
            vmReg.stdProfile.Total_Matric_Marks = std.Total_Matric_Marks;
            vmReg.stdProfile.Total_Inter_Marks = std.Total_Inter_Marks;
            vmReg.stdProfile.Matric_Marks = std.Matric_Marks;
            vmReg.stdProfile.Intermediate_Marks = std.Intermediate_Marks;
            vmReg.stdProfile.MatricFrom = std.MatricFrom;
            vmReg.stdProfile.IntermediateFrom = std.IntermediateFrom;
            //   vmReg.stdProfile.Province=
        }
        public void SetRegisterationNewAdmission(Registeration reg)
        {
            try
            {
                vmReg.stdRegisteration = reg;
            }
            catch (Exception)
            {
                return;
            }

        }
        public void ClearOrNotToClearValues(int val)
        {
            if (val == 1)
            {
                vmReg = new ViewModel_Registeration();
            }
            else
            {
                vmReg = null;
            }
        }

        public int GetMonthNoForMonthName(string Month)
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