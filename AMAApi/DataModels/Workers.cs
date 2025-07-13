using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
  
    public class Workers
    {
        [Key]
        public int Id { get; set; }//מזהה עובד
        public int FarmId { get; set; }//ארגון

        public int? DivisionsId { get; set; }// אגף
        public int? SubDivisionsId { get; set; }//תת אגף
        public int? DepartmentsId { get; set; }//מחלקה
        public int? SubDepartmentsId { get; set; }//מחלקה תת
       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Taz { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PhoneSelular { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Mikud { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int StatusId { get; set; }
       
        [NotMapped]
        public bool IsHaveSignature { get; set; }
        [NotMapped]
        public string ImgData { get; set; }
        public string FullName
        {
            get
            {


                return this.LastName + ' ' + this.FirstName;

            }
        }
        public string FullAddress
        {
            get
            {


                return this.City + ' ' + this.Street + ' ' + this.HouseNumber + ' ' + this.Mikud;

            }
        }
        public object this[string propertyName]
        {
            get
            {

                var PropertInfo = this.GetType().GetProperty(propertyName);
                if (PropertInfo == null)
                    return null;

                return PropertInfo.GetValue(this, null);


            }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }

    

}