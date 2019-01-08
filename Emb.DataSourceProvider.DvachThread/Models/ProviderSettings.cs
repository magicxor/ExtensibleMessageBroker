using System.ComponentModel.DataAnnotations;

namespace Emb.DataSourceProvider.DvachThread.Models
{
    public class ProviderSettings
    {
        [Required]
        public string Hostname { get; set; }
    }
}
