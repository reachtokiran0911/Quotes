using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuotesApi.Models
{
    public class Quote
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Title { get; set; }
        [Required]
        [StringLength(300)]
        public string Author { get; set; }
        [Required]
        [StringLength(3000)]
        public string Description { get; set; }
        [Required]
        [StringLength(30)]
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
    }
}
