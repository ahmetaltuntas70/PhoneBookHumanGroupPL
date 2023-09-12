using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookHumanGroupEL.Entities
{
    [Table("CONTACT")]
    public class Contact : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        [MinLength(2)]
        public string Subject { get; set; }
        [Required]
        [StringLength(500)]
        [MinLength(2)]
        public string Message { get; set; }
        public bool IsActive { get; set; }
    }
}
