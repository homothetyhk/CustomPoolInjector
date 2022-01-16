# CustomPoolInjector

CustomLogicInjector is an add-on to Randomizer 4 which allows adding custom item/location pools, so that different combinations of items and locations can be toggled to be randomized.

# Examples

This mod should have been distributed along with an Examples folder. To use the examples, simply move the contents of the Examples folder up one level (in other words, the folders inside Examples should be moved so they are next to the mod dll).

# Usage

To add pools, simply put .json files in the CustomPoolInjector subdirectory for each pool, copying the structure of the provided examples.
The fields are as follows:
- Name: the text that will be displayed in the menu for this pool. Different CustomPoolInjector pools must have different names.
- Priority: this determines when the pool is injected into the randomizer. This can be omitted in general, unless needed for specific compatibility constraints.
- IncludeItems: the collection of items that should be added when the pool is randomized.
- IncludeLocations: the collection of locations that should be added when the pool is randomized.
- ExcludeVanilla: the collection of placements that are not vanilla when the pool is randomized.
- ExcludeVanillaShopItems: this is used to communicate when vanilla versions of an item should be removed from shops.
  - The value should be a string taking the form of a comma-separated list of flags from the DefaultShopItems enum defined in ItemChanger.
  - This property must be set **in addition** to listing the vanilla placements in ExcludeVanilla.
  - If the pool does not contain any shop items, this property can be omitted.
- ExcludeVanillaGrubfatherRewards: this is used to communicate when vanilla versions of a reward should be removed from Grubfather.
  - The value should be a string taking the form of a comma-separated list of flags from the GrubfatherRewards enum defined in ItemChanger.
  - This property must be set **in addition** to listing the vanilla placements in ExcludeVanilla.
  - If the pool does not contain any Grubfather rewards, this property can be omitted.
- ExcludeVanillaSeerRewards: this is used to communicate when vanilla versions of a reward should be removed from Seer.
  - The value should be a string taking the form of a comma-separated list of flags from the SeerRewards enum defined in ItemChanger.
  - This property must be set **in addition** to listing the vanilla placements in ExcludeVanilla.
  - If the pool does not contain any Seer rewards, this property can be omitted.
- Documentation for the above enums can be found at https://github.com/homothetyhk/HollowKnight.ItemChanger/blob/master/ItemChanger/Enums.cs

Pools can be toggled from the CustomPoolInjector menu, located in the Connections page of the Randomizer 4 menu.