using RandomizerMod.RC;
using static RandomizerMod.RandomizerData.PoolDef;

namespace CustomPoolInjector
{
    public class CustomPoolDef
    {
        public string Name;
        public float Priority;
        public string[] IncludeItems;
        public string[] IncludeLocations;
        public StringILP[] ExcludeVanilla;

        public void ApplyCustomPoolDef(RequestBuilder rb)
        {
            if (!CustomPoolInjectorMod.GlobalSettings.ActivePools.Contains(Name)) return;

            foreach (string item in IncludeItems) rb.AddItemByName(item);
            foreach (string location in IncludeLocations) rb.AddLocationByName(location);
            foreach (StringILP ilp in ExcludeVanilla) rb.Vanilla.RemoveAll(new(ilp.item, ilp.location));
        }
    }
}
