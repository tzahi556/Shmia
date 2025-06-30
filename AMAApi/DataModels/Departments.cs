using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    [Table("Departments")]
    public class Departments
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int FarmId { get; set; }

        public int TypeId { get; set; }
        // 1 אגף
        // 2 מחלקה
        // 3 תת מחלקה

        public int StatusId { get; set; }

    }
}