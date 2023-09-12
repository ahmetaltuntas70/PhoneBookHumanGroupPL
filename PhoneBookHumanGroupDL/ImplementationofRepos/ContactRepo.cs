using PhoneBookHumanGroupDL.ContextInfo;
using PhoneBookHumanGroupDL.ImplementationofRepos;
using PhoneBookHumanGroupDL.InterfacesofRepos;
using PhoneBookHumanGroupEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookHumanGroupDL.ImplementationofRepos
{
    public class ContactRepo:Repository<Contact, int>, IContactRepo
    {
        public ContactRepo(PhoneBookHumanGroupContext context):base(context) 
        {

        }

    }
}
