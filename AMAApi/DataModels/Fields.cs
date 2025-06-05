using System.ComponentModel.DataAnnotations;
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

        public int CampainsId { get; set; }

        public string Name { get; set; }

        public int Seq { get; set; }

        public int CountFieldsInRow { get; set; }

        public int TitleTypeId { get; set; }

        public int? ParentId { get; set; }
        
        public int StatusId { get; set; }

    }



    [Table("Fields")]
    public class Fields
    {
        public int Id { get; set; }

        public int? FarmId { get; set; }

        public int? CampainsId { get; set; }
        

        public string Name { get; set; }

        public string WorkerTableField { get; set; }

        public int StatusId { get; set; }
        


    }

    [Table("Fields2Groups")]
    public class Fields2Groups
    {
        public int Id { get; set; }

        public int FarmId { get; set; }
        public int FieldsId { get; set; }

        public int CampainsId { get; set; }
        public int FieldsGroupsId { get; set; }

        public int FieldsDataTypesId { get; set; }

        public string Title { get; set; }

        public bool? IsWorkerHide { get; set; }

        public string DefaultValue { get; set; }
        public int StatusId { get; set; }
        public int Seq { get; set; }
    }






    [Table("Fields2PDF")]
    public class Fields2PDF
    {
        public int Id { get; set; }

        public int Fields2GroupsId { get; set; }

        public int FarmPDFFilesId { get; set; }

        public int PageNumber { get; set; }

        public double? PdfX { get; set; }

        public double? PdfY { get; set; }

        public double? PdfWidthX { get; set; }

        public double? PdfHeightY { get; set; }

        public int? FieldsId { get; set; }
        
        public int StatusId { get; set; }

    }



    [Table("Fields2GroupsWorkerData")]
    public class Fields2GroupsWorkerData
    {
        public int Id { get; set; }

        public int WorkersId { get; set; }

        public int Fields2GroupsId { get; set; }

        public string Value { get; set; }

        [NotMapped]
        public int Type { get; set; }
        [NotMapped]
        public string SourceValue { get; set; }

    }

    [Table("Fields2PDF_101")]
    public class Fields2PDF_101
    {
        [Key]
        public int Id { get; set; }
        public double? llx { get; set; }
        public double? lly { get; set; }
        public double? urx { get; set; }
        public double? ury { get; set; }
        public int Space { get; set; }
        public int? Font { get; set; }
        public string Word { get; set; }
        public string Comment { get; set; }

        public string Value { get; set; }

        public int PageNumber { get; set; }

        [NotMapped]
        public string Fields2GroupsWorkerDataValue { get; set; }

        [NotMapped]
        public int FieldsDataTypesId { get; set; }


    }




    [Table("FieldsDDL")]
    public class FieldsDDL
    {
        public int Id { get; set; }

        public int FieldsGensId { get; set; }

        public string Name { get; set; }

    }

    
    public class ResultObjectFields
    {
        public Fields2Groups f2g { get; set; }

        public FieldsGroups fg { get; set; }

        public Fields f { get; set; }

        public Fields2GroupsWorkerData f2gwd { get; set; }

    }

}