using ItemChanger;
using ItemChanger.Placements;
using ItemChanger.Tags;
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
        public DefaultShopItems ExcludeVanillaShopItems;
        public GrubfatherRewards ExcludeVanillaGrubfatherRewards;
        public SeerRewards ExcludeVanillaSeerRewards;

        public void ApplyCustomPoolDef(RequestBuilder rb)
        {
            if (!CustomPoolInjectorMod.GS.ActivePools.Contains(Name)) return;

            foreach (string item in IncludeItems) rb.AddItemByName(item);
            foreach (string location in IncludeLocations) rb.AddLocationByName(location);
            foreach (StringILP ilp in ExcludeVanilla) rb.Vanilla.RemoveAll(new(ilp.item, ilp.location));
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
