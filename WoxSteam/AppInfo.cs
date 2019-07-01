using Newtonsoft.Json;
using WoxSteam.BinaryVdf;

namespace WoxSteam
{
    [JsonObject]
    public class AppInfo
    {
        public string ClientIcon { get; set; }

        public static AppInfo From(BinaryVdfItem value)
        {
            var item = value["appinfo"]["common"];
            return new AppInfo
            {
                ClientIcon = item.GetString("clienticon")
            };
        }
    }
}