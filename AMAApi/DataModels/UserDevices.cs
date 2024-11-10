namespace FarmsApi.DataModels
{
    public class UserDevices
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public string DeviceToken { get; set; }
    }
}