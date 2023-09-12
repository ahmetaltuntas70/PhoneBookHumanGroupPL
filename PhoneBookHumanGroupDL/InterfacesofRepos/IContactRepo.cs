using PhoneBookHumanGroupEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookHumanGroupDL.InterfacesofRepos
{
    public interface IContactRepo: IRepository<Contact, int>
    {
    }
}
