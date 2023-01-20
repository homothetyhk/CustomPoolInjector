using RandomizerMod.RC;

namespace CustomPoolInjector
{
    internal static class RequestBuilderHookManager
    {
        private static readonly HashSet<string> subscribedPools = new();

        public static void Setup()
        {
            foreach (CustomPoolDef pool in CustomPoolInjectorMod.Pools.Values)
            {
                if (CustomPoolInjectorMod.GS.ActivePools.Contains(pool.Name)) AddPoolHook(pool);
            }
        }

        public static void Reset()
        {
            foreach (CustomPoolDef pool in CustomPoolInjectorMod.Pools.Values) RemovePoolHook(pool);
            if (subscribedPools.Count > 0) throw new InvalidOperationException("Unable to locate subscribed pools: " + string.Join(", ", subscribedPools));
        }

        public static void AddPoolHook(CustomPoolDef pool)
        {
            if (!subscribedPools.Add(pool.Name)) return;
            RequestBuilder.OnUpdate.Subscribe(pool.Priority, pool.ApplyCustomPoolDef);
        }

        public static void RemovePoolHook(CustomPoolDef pool)
        {
            if (!subscribedPools.Remove(pool.Name)) return;
            RequestBuilder.OnUpdate.Unsubscribe(pool.Priority, pool.ApplyCustomPoolDef);
        }
    }
}
