using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int? RanadId { get; set; }//מפעל
        public int RolesId { get; set; }
        public string Email { get; set; }
      
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
       
        public int FarmId { get; set; }

        public int StatusId { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public string HomePage { get; set; }
        public string FullName
        {
            get
            {
              

                if (this.FirstName == null) return null;
                return this.FirstName + ' ' + this.LastName;

            }
        }


    }

    public class UserDto
    {
       
        public int Id { get; set; }

        public int RolesId { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public int FarmId { get; set; }

        public int StatusId { get; set; }

        public string HomePage { get; set; }

        public string FullName
        {
            get
            {


                if (this.FirstName == null) return null;
                return this.FirstName + ' ' + this.LastName;

            }
        }


    }

    public class UserResult
    {
        public UserDto User { get; set; }
        public List<Departments> Departments { get; set; } = new List<Departments>();
    }


    public class Genders
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }


    }



    [Table("Roles")]
    public class Roles
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? FarmId { get; set; }

        public string HomePage { get; set; }
        
        public int StatusId { get; set; }

    }



    [Table("UsersDepartments")]
    public class UsersDepartments
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public int TypeId { get; set; }
        // מפעל 0 
        // 1 אגף
        // 2 אגף תת
        // 3 מחלקה
        // 4 תת מחלקה

        public int RanadId { get; set; }
        public int UsersId { get; set; }

        public int DepartmentsId { get; set; }

        public int StatusId { get; set; }

    }


    public class UserAPIENTER
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}