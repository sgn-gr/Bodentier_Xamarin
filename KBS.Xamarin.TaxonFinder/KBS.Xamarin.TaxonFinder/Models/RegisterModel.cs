using SQLite;

namespace KBS.App.TaxonFinder.Models
{
    public class RegisterModel
    {

        [PrimaryKey]
        public int ID { get; set; }
        public string UserName { get; set; }
        public string DeviceHash { get; set; }
    }
}