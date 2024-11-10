using System.ComponentModel.DataAnnotations;

namespace FarmsApi.DataModels
{
    public class BanksBrunchs
    {
        [Key]
        public int KeyId { get; set; }
        public int Id { get; set; }

        public int BankId { get; set; }
        public string Name { get; set; }



    }
}