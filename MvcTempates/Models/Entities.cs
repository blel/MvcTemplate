using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
namespace MvcTempates.Models
{
    public class Application
    {
        public int ID { get; set; }
        public DateTime? Sent { get; set; }
        public int FK_EmployerID {get; set;}
        public virtual Employer SentTo {get; set;}
        public virtual ICollection<Application> Remarks { get; set; }    
    }

    public class Employer
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Place { get; set; }
        public  virtual ICollection<Application> Applications {get; set;}
        public ICollection<Person> Contacts { get; set; }

    }


    public class Remark
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public DateTime? DateOfRemark { get; set; }
        public string Content { get; set; }
        public int FK_ApplicationID { get; set; }
        public virtual Application BelongsToApplication { get; set; }

    }

    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public int? FK_EmployerID { get; set; }
        public virtual Employer  Contact { get; set; }
    }

    public class EntityContext : DbContext
    {
        //call the base class' constructor
        public EntityContext() : base() { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Remark> Remarks { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Employer> Employers { get; set; }
    }




}