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

        public CustomPoolInjectorMod()
        {
            LoadFiles();
        }

        public override void Initialize()
        {
            MenuChangerMod.OnExitMainMenu += MenuHolder.OnExitMenu;
            RandomizerMod.Menu.RandomizerMenuAPI.AddMenuPage(MenuHolder.ConstructMenu, MenuHolder.TryGetMenuButton);
            RequestBuilderHookManager.Setup();
            RandomizerMod.Logging.SettingsLog.AfterLogSettings += LogSettings;
            SettingsInterop.Setup(this);
        }

        public override string GetVersion()
        {
            Version v = GetType().Assembly.GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public static void LoadFiles()
        {
            Pools.Clear();
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
            GS.ActivePools.RemoveWhere(s => !Pools.ContainsKey(s));
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
