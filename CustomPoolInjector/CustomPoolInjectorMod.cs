using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CustomPoolInjector
{
    public class CustomPoolInjectorMod : Mod, IGlobalSettings<GlobalSettings>
    {
        public static GlobalSettings GlobalSettings { get; private set; } = new();
        public static readonly Dictionary<string, CustomPoolDef> Pools = new();

        public override void Initialize()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += MenuHolder.OnExitMenu;
            RandomizerMod.Menu.RandomizerMenuAPI.AddMenuPage(MenuHolder.ConstructMenu, MenuHolder.TryGetMenuButton);
            LoadFiles();
            foreach (CustomPoolDef def in Pools.Values)
            {
                RandomizerMod.RC.RequestBuilder.OnUpdate.Subscribe(def.Priority, def.ApplyCustomPoolDef);
            }
        }

        public override string GetVersion()
        {
            return "1.0.0";
        }

        public static void LoadFiles()
        {
            DirectoryInfo di = new(Path.GetDirectoryName(typeof(CustomPoolInjectorMod).Assembly.Location));
            foreach (var f in di.EnumerateFiles("*.json"))
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

        void IGlobalSettings<GlobalSettings>.OnLoadGlobal(GlobalSettings s)
        {
            GlobalSettings = s ?? new();
        }

        GlobalSettings IGlobalSettings<GlobalSettings>.OnSaveGlobal()
        {
            return GlobalSettings;
        }
    }
}
