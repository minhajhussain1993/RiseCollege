﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYP_6
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RCIS2Entities1 : DbContext
    {
        private RCIS2Entities1()
            : base("name=RCIS2Entities1")
        {
        }

        static RCIS2Entities1 rc = new RCIS2Entities1();
        public static RCIS2Entities1 getinstance()
        {
            if (rc == null)
            {
                rc = new RCIS2Entities1();
                return rc;
            }
            return rc;
        }

    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Assign_Subject> Assign_Subject { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Batch_Subjects_Parts> Batch_Subjects_Parts { get; set; }
        public virtual DbSet<BoardResult> BoardResults { get; set; }
        public virtual DbSet<Degree_Program> Degree_Program { get; set; }
        public virtual DbSet<Degree_Subject> Degree_Subject { get; set; }
        public virtual DbSet<Duration> Durations { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Fee> Fees { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<Registeration> Registerations { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Student_Marks> Student_Marks { get; set; }
        public virtual DbSet<Student_Profile> Student_Profile { get; set; }
        public virtual DbSet<Students_Attendance> Students_Attendance { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Teacher_Attendance> Teacher_Attendance { get; set; }
        public virtual DbSet<Teacher_Subject> Teacher_Subject { get; set; }
        public virtual DbSet<Teachers_Batches> Teachers_Batches { get; set; }
        public virtual DbSet<Time_Table> Time_Table { get; set; }
        public virtual DbSet<Year> Years { get; set; }
    }
}
