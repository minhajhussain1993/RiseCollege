using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace FYP_6.Models.Models_Logic
{
    public class SectionModel
    {
        static RCIS3Entities rc = RCIS3Entities.getinstance();
        public static string AddSection(Section sec)
        {
            //deg.LevelID = level;
            try
            {
                if (rc.Sections.Any(s => s.SectionName.ToLower() == sec.SectionName.ToLower()))
                {
                    return "Section Name Already Exists!";
                }
                sec.SectionID = Guid.NewGuid();
                rc.Sections.Add(sec);
                rc.SaveChanges();
                return "OK";
            }
            catch (Exception)
            {
                return "Unable to Add Section Record!";
            }
        }

        public static string DeleteSection_SectionModelFunction(IEnumerable<Guid> deletesec)
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    List<Section> listToDelete = rc.Sections.Where(s => deletesec.Contains(s.SectionID)).ToList();
                    foreach (var item in listToDelete)
                    {
                        if (item.Batches.Count > 0)
                        {
                            return "Unable to delete Record! The Section " + item.SectionName + " has " + item.Batches.Count + " Batches!";
                        }
                    }
                    foreach (var item in listToDelete)
                    {
                        rc.Sections.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    t.Dispose();
                    return "Unable To Delete Sections!";
                }
            }
        }
    }
}