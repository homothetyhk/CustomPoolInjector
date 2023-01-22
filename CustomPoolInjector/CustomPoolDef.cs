using ItemChanger;
using ItemChanger.Placements;
using ItemChanger.Tags;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace CustomPoolInjector
{
    public class CustomPoolDef
    {
        public string Name;
        public float Priority;
        public string[]? IncludeItems;
        public string[]? IncludeLocations;
        public VanillaDef[]? ExcludeVanilla;
        public string[]? ExcludeItems;
        public string[]? ExcludeLocations;
        public VanillaDef[]? IncludeVanilla;
        public DefaultShopItems ExcludeVanillaShopItems;
        public GrubfatherRewards ExcludeVanillaGrubfatherRewards;
        public SeerRewards ExcludeVanillaSeerRewards;

        public void ApplyCustomPoolDef(RequestBuilder rb)
        {
            if (!CustomPoolInjectorMod.GS.ActivePools.Contains(Name)) return;

            if (ExcludeItems is not null) foreach (string item in ExcludeItems) rb.GetItemGroupFor(item).Items.Remove(item, 1);
            if (ExcludeLocations is not null) foreach (string location in ExcludeLocations) rb.GetLocationGroupFor(location).Locations.Remove(location, 1);
            if (IncludeVanilla is not null) foreach (VanillaDef def in IncludeVanilla) rb.AddToVanilla(def);

            if (IncludeItems is not null) foreach (string item in IncludeItems) rb.AddItemByName(item);
            if (IncludeLocations is not null) foreach (string location in IncludeLocations) rb.AddLocationByName(location);
            if (ExcludeVanilla is not null) foreach (VanillaDef def in ExcludeVanilla)
            {
                if (def.Costs == null) rb.RemoveFromVanilla(def.Item, def.Location);
                else rb.RemoveFromVanilla(def);
            }

            if (ExcludeVanillaShopItems != DefaultShopItems.None)
            {
                foreach (string s in new[] { LocationNames.Sly, LocationNames.Sly_Key, LocationNames.Iselda, LocationNames.Salubra, LocationNames.Leg_Eater })
                {
                    rb.EditLocationRequest(s, info => info.onPlacementFetch += (f, r, p) => ((ShopPlacement)p).defaultShopItems &= ~ExcludeVanillaShopItems);
                }
            }
            if (ExcludeVanillaGrubfatherRewards != GrubfatherRewards.None)
            {
                rb.EditLocationRequest(LocationNames.Grubfather, info => info.onPlacementFetch += (f, r, p) => p.GetTag<DestroyGrubRewardTag>().destroyRewards |= ExcludeVanillaGrubfatherRewards);
            }
            if (ExcludeVanillaSeerRewards != SeerRewards.None)
            {
                rb.EditLocationRequest(LocationNames.Seer, info => info.onPlacementFetch += (f, r, p) => p.GetTag<DestroySeerRewardTag>().destroyRewards |= ExcludeVanillaSeerRewards);
            }
        }
    }
}
