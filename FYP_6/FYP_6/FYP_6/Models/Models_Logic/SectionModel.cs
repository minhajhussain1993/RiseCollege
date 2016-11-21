using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace FYP_6.Models.Models_Logic
{
    public class SectionModel
    {
        static RCIS2Entities1 rc = RCIS2Entities1.getinstance();
        public static bool AddSection(Section sec)
        {
            //deg.LevelID = level;
            try
            {
                sec.SectionID = Guid.NewGuid();
                rc.Sections.Add(sec);
                rc.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
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
                        rc.Sections.Remove(item);
                    }
                    rc.SaveChanges();
                    t.Complete();
                    return "OK";
                }
                catch (Exception)
                {
                    return "Unable To Delete Sections!";
                }
            }
        }
    }
}