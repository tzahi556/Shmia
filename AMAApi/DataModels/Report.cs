using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class Report
    {
        
        public int Id { get; set; }
        public string AreaId { get; set; }
     
        public string AreaId2 { get; set; }
        public string ShnatMas { get; set; }
        public string ManagerName { get; set; }
     
        public string Total { get; set; }
        public string TotalDone { get; set; }

      





    }
}