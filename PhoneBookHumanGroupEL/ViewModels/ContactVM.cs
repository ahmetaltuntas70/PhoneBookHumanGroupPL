using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookHumanGroupEL.ViewModels
{
    public class ContactVM
    {
        [Required]
        [StringLength(50)]
        [MinLength(2)]
        public string UserName { get; set; }
        [Required]
        [StringLength(50)]
        [MinLength(2)]
        public string Subject { get; set; }
        [Required]
        [StringLength(500)]
        [MinLength(2)]
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsAktive { get; set; }
    }
}
