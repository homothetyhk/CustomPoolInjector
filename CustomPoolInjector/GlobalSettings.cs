namespace CustomPoolInjector
{
    public class GlobalSettings
    {
        public HashSet<string> ActivePools = new();

        public GlobalSettings GetDisplayableSettings()
        {
            HashSet<string> activePools = new(ActivePools.Where(p => CustomPoolInjectorMod.Pools.ContainsKey(p)));
            return new() { ActivePools = activePools };
        }
    }
}
