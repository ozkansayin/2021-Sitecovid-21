using System;
using Sitecore.Sites;
using Sitecore.Web.UI.HtmlControls;

namespace Feature.SmartNavigation.Services
{
    public class RegistryService : IRegistryService
    {
        private const string ShellSiteName = "shell";
        private string prefix;
        private string Prefix => prefix ?? (prefix = "registry_/" + Sitecore.Context.GetUserName().ToLowerInvariant() + "/");

        private static SiteContext ShellSiteContext =>
            Sitecore.Context.Site?.Name.Equals(ShellSiteName, StringComparison.OrdinalIgnoreCase) ?? false
                ? Sitecore.Context.Site
                : SiteContext.GetSite(ShellSiteName);

        public string GetValue(string key) => Registry.GetValue("/Current_User/" + key.TrimStart('/').Trim());

        public void SaveValue(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (key.StartsWith(Prefix))
            {
                key = key.Substring(Prefix.Length - 1).Trim();
            }

            using (new SiteContextSwitcher(ShellSiteContext))
            {
                Registry.SetValue("/Current_User/" + key.TrimStart('/').Trim(), value);
            }
        }

        public Guid? TryGetGuid(string key)
        {
            var value = GetValue(key);
            if (Guid.TryParse(value, out var parsedGuid))
            {
                return parsedGuid;
            }

            return null;
        }
    }
}