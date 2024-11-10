using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class FarmInstructors
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ClalitNumber { get; set; }
        [NotMapped]
        public string InstructorName { get; set; }


}
}