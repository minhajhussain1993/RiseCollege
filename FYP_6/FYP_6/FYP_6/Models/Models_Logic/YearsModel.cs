using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace FYP_6.Models.Models_Logic
{
    public class YearsModel
    {
        static RCIS2Entities1 rc = RCIS2Entities1.getinstance();
        public static bool AddYear(Year year)
        {
            try
            {
                year.Status = 1;
                year.YearID = Guid.NewGuid();
                rc.Years.Add(year);
                rc.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static IEnumerable<Year> getAllYears(int category)
        {
            var getRecords = rc.Years.Where(s => s.Status == category).OrderBy(s => s.YearID)
                .Select(s => s);
            return getRecords;
        }

        public static string BackUpYearRecords(IEnumerable<Guid> deleteyear)
        {
            if (deleteyear != null)
            {
                using (TransactionScope t = new TransactionScope())
                {
                    try
                    {
                        //Get Years For Selected Year IDs
                        var getSelectedYears = rc.Years
                            .Where(s => deleteyear.Contains(s.YearID))
                            .Select(s => s);

                        //Get Batches of the selected Years
                        var getBatchesOfThatYear = rc.Batches.
                            Where(s => deleteyear.Contains(s.YearID.Value)).Select(s => s);

                        //Get Students of the selected Years
                        var getStudentProfilesForThatYear = rc.Registerations.
                            Where(s => deleteyear.Contains(s.Batch.YearID.Value)).Select(s => s.Student_Profile);

                        foreach (var item in getSelectedYears)
                        {
                            item.Status = 0;
                        }
                        foreach (var item in getBatchesOfThatYear)
                        {
                            item.Status = 0;
                        }
                        foreach (var item in getStudentProfilesForThatYear)
                        {
                            item.Status = 0;
                        }
                        rc.SaveChanges();
                        t.Complete();
                        return "OK";
                    }
                    catch (Exception)
                    {
                        return "Unable To BackUp Records ";
                    }
                }
            }
            else
            {
                return "Plz Select Records to BackUp!";
            }
        }
        public static string DeleteYearRecords(IEnumerable<Guid> deleteyear)
        {
            try
            {
                using (TransactionScope t = new TransactionScope())
                {
                    if (deleteyear != null)
                    {
                        //Get Years For Selected Year IDs
                        var getSelectedYears = rc.Years
                            .Where(s => deleteyear.Contains(s.YearID))
                            .Select(s => s);

                        //Get Students of the selected Years
                        var getStudentProfilesForThatYear = rc.Registerations.
                            Where(s => deleteyear.Contains(s.Batch.YearID.Value))
                            .Select(s => s.Student_Profile);


                        //Then Profiles which deletes profiles,registeration,Assigned Subjects,Marks,Attendance,Fee
                        //except batches and years
                        foreach (var item in getStudentProfilesForThatYear)
                        {
                            rc.Student_Profile.Remove(item);
                        }
                        rc.SaveChanges();

                        //Batches and Years Plus Subjects of Batches Are deleted Here
                        foreach (var item in getSelectedYears)
                        {
                            rc.Years.Remove(item);
                        }
                        rc.SaveChanges();
                        t.Complete();
                        return "OK";
                    }
                    else
                    {
                        return "Plz Select Records to Delete!";
                    }
                }
            }
            catch (Exception)
            {
                return "Unable to Delete Records!";
            }
        }
    }
}