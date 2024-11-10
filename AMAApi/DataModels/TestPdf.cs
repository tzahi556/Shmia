using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class Testpdfs
    {
        [Key]
        public int Id { get; set; }
        public int llx { get; set; }
        public int lly { get; set; }
        public int urx { get; set; }
        public int ury { get; set; }
        public int Space { get; set; }
        public int? Font { get; set; }
        public string Word { get; set; }
        public string Comment { get; set; }

        public string Value { get; set; }

        

        public int PageNumber { get; set; }
        


    }

}