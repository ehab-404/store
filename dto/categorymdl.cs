using System.ComponentModel.DataAnnotations;

namespace testRestApi.dto
{
    public class categorymdl
    {


        [MaxLength(50)]
        public string name { get; set; }
        public string? notes { get; set; }

    }
}
