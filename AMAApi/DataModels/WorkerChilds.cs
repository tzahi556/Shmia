using System;

namespace FarmsApi.DataModels
{
    public class WorkerChilds
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        
        public string Name { get; set; }
        public string Taz { get; set; }
        public DateTime? BirthDate { get; set; }

        public bool IsInHouse { get; set; }

        public bool IsBituaLeumi { get; set; }


        public object this[string propertyName]
        {
            get
            {

                var PropertInfo = this.GetType().GetProperty(propertyName);
                if (PropertInfo == null)
                    return null;

                return PropertInfo.GetValue(this, null);


            }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

    }
}