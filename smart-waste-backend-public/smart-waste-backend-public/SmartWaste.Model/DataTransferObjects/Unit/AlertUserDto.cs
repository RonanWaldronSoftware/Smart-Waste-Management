using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.DataTransferObjects.WebConfiguration;

namespace SmartWaste.Model.DataTransferObjects.Unit
{
    public class AlertUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public List<AlertTypeDto> Alerts { get; set;} = new();
    }
}
