using Emb.TargetProvider.Telegram.Enums;

namespace Emb.TargetProvider.Telegram.Models
{
    public class ProviderSettings
    {
        public string Token { get; set; }
        public ProxyProtocols ProxyProtocol { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public bool ProxyAuthentication { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
    }
}
