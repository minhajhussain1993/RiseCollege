using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace FYP_6.Models.Models_Logic
{
    public class SubjectsModel
    {
        static RCIS2Entities1 rc = RCIS2Entities1.getinstance();
        public static Guid GetSubjectID()
        {
            Guid g = new Guid();
            g = Guid.NewGuid();
            return g;
        }
        public static Guid GetSubjectDegreeID()
        {
            Guid g = new Guid();
            g = Guid.NewGuid();
            return g;
        }
        public static string AddSubject(Subject sub)
        {
            Guid subID = GetSubjectID();
            try
            {

                if (rc.Subjects.Any(s => s.SubjectName == sub.SubjectName))
                {
                    return "Record Already Exists in The Database with " + sub.SubjectName + " ";
                }
                else
                {
                    sub.SubjectID = subID;
                    rc.Subjects.Add(sub);
                    rc.SaveChanges();
                    return "OK";
                }
            }
            catch (Exception e)
            {

                return "Unable to Add Subject ";
            }


        }

        public static string DeleteSubjects(IEnumerable<Guid> deletesub)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    List<Subject> listToDelete = rc.Subjects.Where(s => deletesub.Contains(s.SubjectID)).ToList();

                    foreach (var item in listToDelete)
                    {
                        rc.Subjects.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch 
                {
                    return "Unable to Delete Records!";
                }
            }
        }
    }
}