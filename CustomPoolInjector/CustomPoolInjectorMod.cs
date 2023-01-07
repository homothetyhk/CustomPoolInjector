using MenuChanger;
using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Security.Cryptography;

namespace CustomPoolInjector
{
    public class CustomPoolInjectorMod : Mod, IGlobalSettings<GlobalSettings>
    {
        public static string ModDirectory { get; }
        public static GlobalSettings GS { get; private set; } = new();
        public static readonly Dictionary<string, CustomPoolDef> Pools = new();
        public static readonly Dictionary<string, string> Hashes = new();

        public CustomPoolInjectorMod()
        {
            LoadFiles();
            GS.ActivePools.RemoveWhere(s => !Pools.ContainsKey(s));
        }

        public override void Initialize()
        {
            MenuChangerMod.OnExitMainMenu += MenuHolder.OnExitMenu;
            RandomizerMod.Menu.RandomizerMenuAPI.AddMenuPage(MenuHolder.ConstructMenu, MenuHolder.TryGetMenuButton);
            foreach (CustomPoolDef def in Pools.Values)
            {
                RandomizerMod.RC.RequestBuilder.OnUpdate.Subscribe(def.Priority, def.ApplyCustomPoolDef);
            }
            RandomizerMod.Logging.SettingsLog.AfterLogSettings += LogSettings;
            SettingsInterop.HookRSM(this);
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

                fs.Seek(0, SeekOrigin.Begin);
                using SHA256 sha = SHA256.Create();
                byte[] data = sha.ComputeHash(fs);
                Hashes.Add(def.Name, string.Concat(data.Select(b => b.ToString("X2"))));
                Modding.Logger.Log($"{def.Name}: {Hashes[def.Name]}");
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
