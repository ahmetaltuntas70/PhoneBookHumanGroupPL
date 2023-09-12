using PhoneBookHumanGroupEL.ViewModels;

namespace PhoneBookHumanGroupAPI.Models
{
    public class GetNumberAsSavedViewModel
    {
        //05396796650 şeklindeki numarayı kim nasıl kaydetmiş?
        //Sait Çakır 05396796650'yi hocam diye kaydetmiş.
        //Kübra 05396796650 kankam diye kaydetmiş.
        public string WhoSaved { get; set; }  //MemberVM
        public string HowSaved { get; set; }  //PhoneGroupVM
    }
}
