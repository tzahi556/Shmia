using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class Farm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public int? Style { get; set; }
        public string Address { get; set; }
        public string TikNikuim { get; set; }
        public string IdNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string OfficeMail { get; set; }
        public bool OfficeIsMail { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public string ContactMail { get; set; }
        public bool ContactIsMail { get; set; }



        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string Logo { get; set; }

    }

   
}