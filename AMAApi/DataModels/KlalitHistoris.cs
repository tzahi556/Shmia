using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class KlalitHistoris
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [NotMapped]
        public int? KlalitHistorisId { get; set; }
        
        public int FarmId { get; set; }

        public DateTime DateSend { get; set; }
        public DateTime DateLesson { get; set; }

        public string Result { get; set; }
        public string ResultXML { get; set; }

        public string ResultNumber { get; set; }

        public string ClaimNumber { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string Taz { get; set; }
      
        public int? Instructor_Id { get; set; }

        public int? CounterSend { get; set; }
        

    }
}