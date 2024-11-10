using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class EsekConfiguraions
    {
        [Key]
        public int Id { get; set; }

        public string EsekName { get; set; }
        public string EsekAdress { get; set; }
        public string EsekPhone { get; set; }
        public string EsekNikuim { get; set; }
       



    }
}