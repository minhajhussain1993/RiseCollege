using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace FYP_6.Models.Models_Logic
{
    public static class StudentModel
    {
        static RCIS2Entities1 rc = RCIS2Entities1.getinstance();
        public static string[] WelcomeStudentScreenResults(string id)
        {
            string[] mainScreenItems = new string[5];

            var currentStudentDegree = rc.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();

            var getStudentSection = rc.Registerations.Where(s => s.BatchID == currentStudentDegree.BatchID).Select(s => s.Batch.Section.SectionName).FirstOrDefault();
            var subjects = rc.Assign_Subject.Where(s => s.Rollno == id).Select(s => s).ToList();
            int noOfSubjects = 0;

            foreach (var items in subjects)
            {
                noOfSubjects++;
            }
            if (currentStudentDegree != null && noOfSubjects > 0)
            {
                mainScreenItems[0] = currentStudentDegree.Batch.Degree_Program.Degree_ProgramName.ToString();
                mainScreenItems[1] = getStudentSection.ToString();
                mainScreenItems[2] = noOfSubjects.ToString();
                mainScreenItems[3] = currentStudentDegree.Status.ToString();
            }
            return mainScreenItems;
        }
        public static List<Student_Marks> showMarks_StudentModelFunction(string Month, string Part, string id, int y)
        {
            List<Student_Marks> listOfresults = new List<Student_Marks>();
            //int stdid = int.Parse(id);
            int part = int.Parse(Part);
            string MonthName = Month;
            foreach (var item in rc.Student_Marks)
            {
                if (item.Month == MonthName && 
                    id == item.Assign_Subject.Rollno && item.Assign_Subject.Batch_Subjects_Parts.Part == part
                    && item.Year == y)
                {
                    listOfresults.Add(item);
                }
            }
            return listOfresults;
        }
        public static List<Students_Attendance> showAttendance_StudentModelFunction(string Month, string Part, string id, int y)
        {
            List<Students_Attendance> listOfresults = new List<Students_Attendance>();
            //int stdid = int.Parse(id);
            int part = int.Parse(Part);
            string MonthName = Month;
            foreach (var item in rc.Students_Attendance)
            {
                if (item.Month == MonthName && id == item.Assign_Subject.Rollno && item.Assign_Subject.Batch_Subjects_Parts.Part == part
                    && item.Year == y)
                {
                    listOfresults.Add(item);
                }
            }
            return listOfresults;
        }
        public static List<Student_Marks> showMarks_StudentModelFunction(string id)
        {
            List<Student_Marks> listOfresults = new List<Student_Marks>();

            foreach (var item in rc.Student_Marks)
            {
                if (item.Assign_Subject.Rollno==id)
                {
                    listOfresults.Add(item);   
                }
            }
            return listOfresults;
        }
        public static List<Students_Attendance> showAttendance_StudentModelFunction(string id)
        {
            List<Students_Attendance> listOfresults = new List<Students_Attendance>();

            foreach (var item in rc.Students_Attendance)
            {
                if (item.Assign_Subject.Rollno == id)
                {
                    listOfresults.Add(item);
                }
            }
            return listOfresults;
        }
        public static List<Fee> ShowFeeRecords_StudentModelFunction(string id, int choice, int selectedYear)
        {
            //int stdid = int.Parse(id);
            List<Fee> FeeList = new List<Fee>();
            if (choice == 1)
            {
                foreach (var item in rc.Fees)
                {
                    if (item.Rollno == id)
                    {
                        FeeList.Add(item);
                    }
                }
                return FeeList;
            }
            else
            {
                foreach (var item in rc.Fees)
                {
                    if ( id == item.Rollno && item.Dated.Value.Year == selectedYear)
                    {
                        FeeList.Add(item);
                    }
                }
                return FeeList;
            }
        }
        public static string ChangePassword_StudentModelFunction(string oldpass, string newpass, string id)
        {
            using (TransactionScope t =new TransactionScope())
            {
                try
                {
                    var getStudent = rc.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();
                    string getUserPassword = getStudent.Password;

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
                    return "Unable To Change Password!";
                }
            }
            
        }
        private static string ValidatePassword(string oldentered, string newpass, string actualPassword)
        {
            if (oldentered.Length <= 30)
            {
                if (newpass.Length <= 30 & newpass.Length>=5)
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

        public static List<string> StudentDegreeDetail_StudentModel(string id)
        {
            List<string> student = new List<string>();

            //int stdrollno = int.Parse(id);
            var student_Query = rc.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();
            string degreeNameQuery = student_Query.Batch.Degree_Program.Degree_ProgramName;
            //var student_Query = rc.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();

            student.Add(student_Query.Rollno.ToString());
            student.Add(student_Query.Student_Profile.FirstName + " " + student_Query.Student_Profile.LastName);
            student.Add(degreeNameQuery);
            return student;
        }
        public static List<string> getAllSubjectNames(string id)
        {
            List<string> subjectsNames = new List<string>();

            //int stdrollno = int.Parse(id);
            var degree_Query = rc.Registerations.Where(s => s.Rollno == id).Select(s => s).FirstOrDefault();
            var subjects_Query = rc.Assign_Subject.Where(s => s.Rollno == id).Select(s => s.Batch_Subjects_Parts.Subject.SubjectID);

            foreach (var MatchingSubjids in subjects_Query)
            {
                foreach (var subjects in rc.Subjects)
                {
                    if (MatchingSubjids == subjects.SubjectID)
                    {
                        subjectsNames.Add(subjects.SubjectName);
                    }
                }
            }

            return subjectsNames;

        }
        public static List<int?> getStudentYears(string id)
        {
            if (id != null)
            {
                List<int?> l = new List<int?>();
                var getYear = rc.Registerations.Where(s => s.Rollno == id).Select(s => s.Batch.Year).FirstOrDefault();
                for (int? i = getYear.FromYear; i <= getYear.ToYear; i++)
                {
                    l.Add(i);
                }
                return l;
            }
            else
            {
                return null;
            }
        }
    }
}