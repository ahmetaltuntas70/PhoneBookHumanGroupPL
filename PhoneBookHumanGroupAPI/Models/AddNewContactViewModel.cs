using PhoneBookHumanGroupEL.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace PhoneBookHumanGroupAPI.Models
{
    public class AddNewContactViewModel
    {
        public int PhoneGroupId { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string PhoneGroupNameSurname { get; set; }

        //REgularExpression gerekli 
        //05396796650   (0539) 679 66 50  0539-679-66-50 5396796650 +90 539 679 66 50 
        public string? PhoneNumber { get; set; }

        public int MemberId { get; set; }

        public string? AnotherPhoneGroupName { get; set; } //ViewModel ya da diğer adıyla DTO classları ön yüzdeki istediğimiz işlemleri yapmak için yardımcıdır. Böylece ön yüzden aldığımız bilgileri ENTITIlerimize aktarırız.
        public string? CountryPhoneCode { get; set; } // bu aslında başka tabloda olmalı
        public string? Phone { get; set; } // bu aslında başka tabloda olmalı
    }
}
