using System;
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
        [NotMapped]
        public string Sign { get; set; }

    }



    public class FarmPDFFiles
    {
        public int Id { get; set; }
        public int FarmId { get; set; }

        public int CampainsId { get; set; }
        
        public string FileName { get; set; }
        public int Seq { get; set; }
        public int StatusId { get; set; }

        public bool Is101 { get; set; }

        [NotMapped]
        public string FullLink { get; set; }

    }


    [Table("Campains")]
    public class Campains
    {
        public int Id { get; set; }

        public int FarmId { get; set; }

        public string Name { get; set; }

        public DateTime DateRigster { get; set; }

        public int CountSend { get; set; }

        public int StatusId { get; set; }

    }
}