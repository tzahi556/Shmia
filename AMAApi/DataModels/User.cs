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
        public string Role { get; set; }
      
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string Active { get; set; }
        public int Farm_Id { get; set; }
        public bool Deleted { get; set; }

        public string AreaId { get; set; }
        public string AreaId2 { get; set; }

        public string FullName
        {
            get
            {
              

                if (this.FirstName == null) return null;
                return this.FirstName + ' ' + this.LastName;

            }
        }


    }
}