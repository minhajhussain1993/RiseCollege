using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace FYP_6.Models.Models_Logic
{
    public class SubjectsModel
    {
        static  RCIS3Entities rc = RCIS3Entities.getinstance();
        public   Guid GetSubjectID()
        {
            Guid g = new Guid();
            g = Guid.NewGuid();
            return g;
        }
        public   Guid GetSubjectDegreeID()
        {
            Guid g = new Guid();
            g = Guid.NewGuid();
            return g;
        }
        public   string AddSubject(Subject sub)
        {
            Guid subID = GetSubjectID();
            try
            {

                if (rc.Subjects.Any(s => s.SubjectName.ToLower() == sub.SubjectName.ToLower()))
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

        public   string DeleteSubjects(IEnumerable<Guid> deletesub)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    List<Subject> listToDelete = rc.Subjects.Where(s => deletesub.Contains(s.SubjectID)).ToList();

                    foreach (var item in listToDelete)
                    {
                        if (item.Batch_Subjects_Parts.Count > 0)
                        {
                            return "Unable to delete Record! The Subject " +item.SubjectName+ " has related " + "Records in the Database!";
                        }
                    }

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
                    t.Dispose();
                    return "Unable to Delete Records!";
                }
            }
        }
    }
}