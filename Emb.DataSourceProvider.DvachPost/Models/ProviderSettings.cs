using System.ComponentModel.DataAnnotations;

namespace Emb.DataSourceProvider.DvachPost.Models
{
    public class ProviderSettings
    {
        [Required]
        public string Hostname { get; set; }
    }
}
