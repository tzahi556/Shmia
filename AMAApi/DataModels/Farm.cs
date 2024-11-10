using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class Farm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Meta { get; set; }

        public int? IsHiyuvInHashlama { get; set; }

        public int? Style { get; set; }


        

        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string Password { get; set; }


    }

   
}