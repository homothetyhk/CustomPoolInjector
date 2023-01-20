using Modding;
using MonoMod.ModInterop;

namespace CustomPoolInjector
{
    internal class SettingsInterop
    {
        [ModImportName("RandoSettingsManager")]
        internal static class RSMImport
        {
            public static Action<Mod, Type, Delegate, Delegate>? RegisterConnectionSimple = null;
            static RSMImport() => typeof(RSMImport).ModInterop();
        }

        public class RSMData
        {
            public RSMData() { }
            public RSMData(GlobalSettings gs)
            {
                SharedPools = CustomPoolInjectorMod.Pools.Values.Where(p => gs.ActivePools.Contains(p.Name)).ToList();
            }

            public List<CustomPoolDef> SharedPools;
        }

        internal static void Setup(Mod mod)
        {
            RSMImport.RegisterConnectionSimple?.Invoke(mod, typeof(RSMData), ReceiveSettings, SendSettings);
        }

        internal static void ReceiveSettings(RSMData? data) 
        {
            MenuHolder.Instance.ToggleAllOff();
            RequestBuilderHookManager.Reset(); // unnecessary check for safety

            if (data is not null)
            {
                CustomPoolInjectorMod.Pools.Clear();
                foreach (CustomPoolDef pool in data.SharedPools) CustomPoolInjectorMod.Pools.Add(pool.Name, pool);
                CustomPoolInjectorMod.GS.ActivePools.UnionWith(data.SharedPools.Select(p => p.Name));
                MenuHolder.Instance.ReconstructMenu();
                MenuHolder.Instance.CreateRestoreLocalPacksButton();
                RequestBuilderHookManager.Setup();
            }
        }

        internal static RSMData? SendSettings()
        {
            return CustomPoolInjectorMod.GS.ActivePools.Count > 0 ? new(CustomPoolInjectorMod.GS) : null;
        }
    }
}
