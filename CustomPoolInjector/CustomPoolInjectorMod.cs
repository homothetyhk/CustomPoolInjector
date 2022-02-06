using MenuChanger;
using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CustomPoolInjector
{
    public class CustomPoolInjectorMod : Mod, IGlobalSettings<GlobalSettings>
    {
        public static string ModDirectory { get; }
        public static GlobalSettings GS { get; private set; } = new();
        public static readonly Dictionary<string, CustomPoolDef> Pools = new();

        public override void Initialize()
        {
            MenuChangerMod.OnExitMainMenu += MenuHolder.OnExitMenu;
            RandomizerMod.Menu.RandomizerMenuAPI.AddMenuPage(MenuHolder.ConstructMenu, MenuHolder.TryGetMenuButton);
            LoadFiles();
            foreach (CustomPoolDef def in Pools.Values)
            {
                RandomizerMod.RC.RequestBuilder.OnUpdate.Subscribe(def.Priority, def.ApplyCustomPoolDef);
            }
            RandomizerMod.Logging.SettingsLog.AfterLogSettings += LogSettings;
        }

        public override string GetVersion()
        {
            return "1.0.2";
        }

        public static void LoadFiles()
        {
            DirectoryInfo main = new(ModDirectory);

            foreach (var f in main.EnumerateFiles("*.json"))
            {
                using FileStream fs = f.OpenRead();
                using StreamReader sr = new(fs);
                using JsonTextReader jtr = new(sr);
                JsonSerializer serializer = new()
                {
                    DefaultValueHandling = DefaultValueHandling.Include,
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto,
                };
                serializer.Converters.Add(new StringEnumConverter());
                CustomPoolDef def = serializer.Deserialize<CustomPoolDef>(jtr);
                Pools.Add(def.Name, def);
            }
        }

        private static void LogSettings(RandomizerMod.Logging.LogArguments arg1, TextWriter tw)
        {
            tw.WriteLine("Logging CustomPoolInjector settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false, };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, GS);
            tw.WriteLine();
        }

        void IGlobalSettings<GlobalSettings>.OnLoadGlobal(GlobalSettings s)
        {
            GS = s ?? new();
        }

        GlobalSettings IGlobalSettings<GlobalSettings>.OnSaveGlobal()
        {
            return GS;
        }

        static CustomPoolInjectorMod()
        {
            ModDirectory = Path.GetDirectoryName(typeof(CustomPoolInjectorMod).Assembly.Location);
        }
    }
}
