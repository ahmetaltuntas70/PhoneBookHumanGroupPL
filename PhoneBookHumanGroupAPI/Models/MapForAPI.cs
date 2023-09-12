using AutoMapper;
using PhoneBookHumanGroupEL.Entities;
using PhoneBookHumanGroupEL.ViewModels;

namespace PhoneBookHumanGroupAPI.Models
{
    public class MapForAPI:Profile  
    {
        public MapForAPI()
        {
            CreateMap<AddNewContactViewModel, MemberPhoneVM>().ReverseMap();

        }
    }
}
