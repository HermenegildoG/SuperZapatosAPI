using DB;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperZapatosAPI.Dtos
{
    public class CreateArticleDto
    {
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public int total_in_shelf { get; set; }
        public int total_in_vault { get; set; }
        public int store_id { get; set; }
        
    }
}
