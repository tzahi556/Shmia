using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class Logs
    {
        [Key]
        public int Id { get; set; }

        //public int? WorkersId { get; set; }

      
        //public Workers Workers { get; set; }
        public string Action { get; set; }
        public DateTime DateTime { get; set; }
        public string Device { get; set; }
        public string UserAgent { get; set; }
        public string Expetion { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }

        public string WorkerName { get; set; }


      

    }
}