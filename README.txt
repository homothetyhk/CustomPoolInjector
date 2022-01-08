This is a mod for adding simple user-defined pools to Randomizer 4.

To add pools, simply put .json files in the CustomPoolInjector subdirectory for each pool, copying the structure of the provided examples.
The fields are as follows:
- Name: the text that will be displayed in the menu for this pool. Different CustomPoolInjector pools must have different names.
- Priority: this determines when the pool is injected into the randomizer. Unless you have specific compatibility constraints, a value of 1.0 is recommended.
- IncludeItems: the collection of items that should be added when the pool is randomized.
- IncludeLocations: the collection of locations that should be added when the pool is randomized.
- ExcludeVanilla: the collection of placements that are not vanilla when the pool is randomized.

Pools can be toggled from the CustomPoolInjector menu, located in the Connections page of the Randomizer 4 menu.