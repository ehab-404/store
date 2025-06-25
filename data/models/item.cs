using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testRestApi.data.models
{
    public class item:ISoftDeletable,ITrackable
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string name { get; set; }
        public string? notes { get; set; }

        public double price { get; set; }
        [NotMapped]
        public IFormFile? image { get; set; }
        public string? imagepath {get; set;}

        [ForeignKey(nameof(category))]
        public int categoryId { get; set; }
        
        public Category category { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted
        {
            get; set;
        }



    }
}
