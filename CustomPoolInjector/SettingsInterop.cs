using MenuChanger.MenuElements;
using Modding;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;

namespace CustomPoolInjector
{
    internal class SettingsInterop
    {
        internal static void HookRSM(Mod mod)
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new SimpleSettingsProxy<Dictionary<string, string>>(
                mod,
                ReceiveSettings,
                SendSettings
                ));
        }

        internal static void ReceiveSettings(Dictionary<string, string> activeHashes) 
        {
            activeHashes ??= new();
            foreach (KeyValuePair<string, string> kvp in activeHashes)
            {
                if (!CustomPoolInjectorMod.Hashes.TryGetValue(kvp.Key, out string hash))
                {
                    throw new InvalidOperationException($"Received settings contain unrecognized active pack: {kvp.Key}");
                }
                else if (hash != kvp.Value)
                {
                    throw new InvalidOperationException($"Received settings contain hash mismatch for active pack: {kvp.Key}");
                }
            }
            foreach (KeyValuePair<string, ToggleButton> kvp in MenuHolder.Instance.PoolToggleLookup)
            {
                bool value = activeHashes.ContainsKey(kvp.Key);
                if (value != kvp.Value.Value)
                {
                    kvp.Value.SetValue(value);
                }
            }
        }

        internal static Dictionary<string, string> SendSettings()
        {
            if (CustomPoolInjectorMod.GS.ActivePools.Count == 0) return null;
            return CustomPoolInjectorMod.GS.ActivePools.ToDictionary(p => p, p => CustomPoolInjectorMod.Hashes[p]);
        }
    }
}
