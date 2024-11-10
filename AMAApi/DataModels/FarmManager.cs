using System;

namespace FarmsApi.DataModels
{
    public class FarmManagers
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int SupplierID { get; set; }
        public string SectionCode { get; set; }
        public string CareCode { get; set; }
        public string MefarzelUser { get; set; }
        public string VetrinarUser { get; set; }
        
    }
}