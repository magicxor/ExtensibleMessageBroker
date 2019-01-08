using System.ComponentModel.DataAnnotations;

namespace Emb.DataSourceProvider.VkFeed.Models
{
    public class ProviderSettings
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
