using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClientGraph.Models
{
    public class ClientModel : ModelBase
    {
        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("Telephone Number")]
        public string TelephoneNumber { get; set; }
    }
}