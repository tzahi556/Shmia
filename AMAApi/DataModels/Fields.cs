using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{

    [Table("FieldsDataTypes")]
    public class FieldsDataTypes
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NameEn { get; set; }

    }

   

    [Table("FieldsGroups")]
    public class FieldsGroups
    {
        public int Id { get; set; }

        public int FarmId { get; set; }

        public string Name { get; set; }

        public int Seq { get; set; }

        public int CountFieldsInRow { get; set; }
        

        public int StatusId { get; set; }

    }



    [Table("Fields")]
    public class Fields
    {
        public int Id { get; set; }

        public int FarmId { get; set; }

     

        public string Name { get; set; }

      

     

       

        public int StatusId { get; set; }
        

    }

    [Table("Fields2Groups")]
    public class Fields2Groups
    {
        public int Id { get; set; }

        public int FarmId { get; set; }
        public int FieldsId { get; set; }

        public int FieldsGroupsId { get; set; }

        public int FieldsDataTypesId { get; set; }

        public string Title { get; set; }

        public bool? IsWorkerShow { get; set; }

        public string DefaultValue { get; set; }

        public int Seq { get; set; }
    }




    [Table("FieldsDDL")]
    public class FieldsDDL
    {
        public int Id { get; set; }

        public int FieldsGensId { get; set; }

        public string Name { get; set; }

    }



}