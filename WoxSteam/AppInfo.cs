using WoxSteam.BinaryVdf;

namespace WoxSteam
{
    public class AppInfo
    {
        public string ClientIcon { get; }

        private AppInfo(BinaryVdfItem value)
        {
            var item = value["appinfo"]["common"];
            ClientIcon = item.GetString("clienticon");
        }

        public static AppInfo From(BinaryVdfItem value)
        {
            return new AppInfo(value);
        }
    }
}