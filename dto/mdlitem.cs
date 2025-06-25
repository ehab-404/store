using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace testRestApi.dto
{
    public class mdlitem
    {
        [MaxLength(100)]
        public string name { get; set; }
        public string? notes { get; set; }

        public double price { get; set; }
       
        public IFormFile? image { get; set; }
       
        public int categoryId { get; set; }
    }
}
