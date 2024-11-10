using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{

    //https://stackoverflow.com/questions/26305273/there-is-already-an-object-named-in-the-database
    public class Workers
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserManager{ get; set; }
        public string ShnatMas { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Taz { get; set; }

        public string Darkon { get; set; }

        public DateTime? BirthDate { get; set; }
        public DateTime? AliaDate { get; set; }
        
        public string PhoneSelular { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Mikud { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string ToshavIsrael { get; set; }
        public string Kibutz { get; set; }

        public string KupatHolim { get; set; }
        public string Kupa { get; set; }
        public string StatusFamely { get; set; }
        public string SugMaskuret { get; set; }
        public DateTime? StartWorkDate { get; set; }
        public string AnotherMaskuret { get; set; }
        public string AnotherSugMaskuret { get; set; }
        public string Nkudutzikuy { get; set; }
        public string AnotherMaskuretHafrashot1 { get; set; }
        public string AnotherMaskuretHafrashot2 { get; set; }


        public string ZoogFirstName { get; set; }
        public string ZoogLastName { get; set; }
        public string ZoogTaz { get; set; }
        public string ZoogDarkon { get; set; }
        public DateTime? ZoogBirthDate { get; set; }
        public DateTime? ZoogAliaDate { get; set; }
        public string ZoogMaskuret { get; set; }
        public string ZoogMaskuret1 { get; set; }
        public string ZoogMaskuret2 { get; set; }
        public string ZikuyToshavIsrael { get; set; }

        public bool ZikuyNeke { get; set; }
        public bool ZikuyToshavMas { get; set; }
        public DateTime? ZikuyYeshuvStartDate { get; set; }
        public bool ZikuyOleHadash { get; set; }

        public DateTime? ZikuyOleHadashDate { get; set; }
        public DateTime? ZikuyOleHadashFromStartYearDate { get; set; }

        public bool ZikuyZoog { get; set; }
        public bool ZikuyHoreHayNefrad { get; set; }
        public bool ZikuyLuladBehzka { get; set; }
       
        public int? ZikuyLuladNuldoShnatMas { get; set; }
        public int? ZikuyLulad_1_5 { get; set; }
        public int? ZikuyLulad_6_17 { get; set; }
        public int? ZikuyLulad_18 { get; set; }
        public bool ZikuyLuladPeutim { get; set; }


        public int? ZikuyLuladNuldoShnatMas2 { get; set; }
        public int? ZikuyLulad2_1_5 { get; set; }
        public bool ZikuyHoreYahid { get; set; }
        public bool ZikuyPsakDinMezonot { get; set; }
       
      
        public bool ZikuyLuladMugbalut { get; set; }
        public int? ZikuyLuladMugbalutNumber { get; set; }
        public bool ZikuyTashlumMezonot { get; set; }
        public bool ZikuyBetween16_18 { get; set; }

        public bool ZikuyHayalEnd { get; set; }
      
        public DateTime? ZikuyHayalEndStartDate { get; set; }
        public DateTime? ZikuyHayalEndEndDate { get; set; }
        public bool ZikuyToarAkdemi { get; set; }


        public string TiumMas { get; set; }
        public string TiumMasBakasha { get; set; }
        public string TiumMasAnotherMaskuretName { get; set; }
        public string TiumMasAnotherMaskuretKtuvet { get; set; }
        public string TiumMasAnotherMaskuretTikNikuim { get; set; }
        public string TiumMasAnotherMaskuretSug { get; set; }
        public int? TiumMasAnotherMaskuretSum { get; set; }
        public int? TiumMasAnotherMaskuretMas { get; set; }
        public string HeskemYom { get; set; }
        public string HeskemHodesh { get; set; }
        public string HeskemShana { get; set; }
        public string HeskemWorkerName { get; set; }

        public string HeskemWorkerTaz { get; set; }
        public string HeskemWorkerStreet { get; set; }
        public string HeskemWorkerPhone { get; set; }
        public string HeskemTafkid { get; set; }

        public string HeskemSamkut { get; set; }
        public DateTime? HeskemStartDate { get; set; }
        public string HeskemStartHour { get; set; }
        public string HeskemEndHour { get; set; }

        public bool HeskemBriutEnKlumCheckbox { get; set; }
        public bool HeskemWorkerMigbalotCheckbox { get; set; }
        public string HeskemWorkerMigbalot { get; set; }
        public bool HeskemWorkerTrufotCheckbox { get; set; }
        public string HeskemWorkerTrufot { get; set; }


        public bool HeskemWorkerRgishutCheckbox { get; set; }
        public string HeskemWorkerRgishut { get; set; }
        public bool HeskemWorkerCronitCheckbox { get; set; }
        public string HeskemWorkerCronit { get; set; }
        public int? HeskemWorkerMaskuert3 { get; set; }

        public int? HeskemWorkerMaskuert4 { get; set; }

        public int? HeskemWorkerMaskuert5 { get; set; }
        

        public bool Deleted { get; set; }
      
        public string Status { get; set; }
      
        public DateTime? DateRigster { get; set; }

        public string ImgData { get; set; }

        public string BankNumName { get; set; }
        public string BrunchNumName { get; set; }
        public string BankAccountNumber { get; set; }
        public string Comments { get; set; }
        public string UniqNumber { get; set; }
        public bool IsNew { get; set; }

     

        public string ManagerName
        {
            get
            {
                //var DBNumber = mispar_rechev;

                //if (DBNumber.StartsWith('0'))
                //{

                //    return DBNumber.Substring(1, 7);
                //}

                if (this.UserManager == null) return null;
                return this.UserManager.FirstName + ' ' + this.UserManager.LastName;

            }
        }
        public object this[string propertyName]
        {
            get {

                var PropertInfo = this.GetType().GetProperty(propertyName);
                if (PropertInfo == null)
                    return null;
                
                return PropertInfo.GetValue(this, null);
            
            
            }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}