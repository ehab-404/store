using System.ComponentModel.DataAnnotations;

namespace testRestApi.data.models
{
    public class Category:ITrackable,ISoftDeletable
    {

        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string name { get; set; }
        public string? notes { get; set; }

        public List<item> items { get; set; }



        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted
        {
            get; set;
        }


        }
}
