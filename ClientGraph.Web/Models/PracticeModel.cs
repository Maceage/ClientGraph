using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClientGraph.Models
{
    public class PracticeModel : ModelBase
    {
        [DisplayName("Company Name")]
        [Required]
        public string CompanyName { get; set; }

        [DisplayName("Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DisplayName("Website URL")]
        [DataType(DataType.Url)]
        public string WebsiteUrl { get; set; }
    }
}