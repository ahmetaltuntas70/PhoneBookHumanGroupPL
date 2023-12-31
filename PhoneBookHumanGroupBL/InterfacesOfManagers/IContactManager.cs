﻿using PhoneBookHumanGroupEL.Entities;
using PhoneBookHumanGroupEL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookHumanGroupBL.InterfacesOfManagers
{
    public interface IContactManager: IManager<ContactVM, int>
    {
    }
}
