using System.ComponentModel.DataAnnotations;
using Emb.TargetProvider.Telegram.Enums;

namespace Emb.TargetProvider.Telegram.Models
{
    public class ProviderSettings
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public ProxyProtocols ProxyProtocol { get; set; }

        public string ProxyHost { get; set; }
        [Range(0, 65535)]
        public int ProxyPort { get; set; }
        public bool ProxyAuthentication { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int RetryCount { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int DelayMilliseconds { get; set; }
    }
}
