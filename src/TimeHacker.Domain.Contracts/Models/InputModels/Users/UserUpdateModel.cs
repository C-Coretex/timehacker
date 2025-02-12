using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeHacker.Domain.Contracts.Models.InputModels.Users
{
    public class UserUpdateModel
    {
        public string Name { get; set; }
        public string PhoneNumberForNotifications { get; set; }
        public string EmailForNotifications { get; set; }
    }
}
