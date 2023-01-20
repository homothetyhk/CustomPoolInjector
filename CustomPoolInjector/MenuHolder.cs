using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;

namespace CustomPoolInjector
{
    public class MenuHolder
    {
        public static MenuHolder Instance { get; private set; }

        public MenuPage ConnectionsPage;
        public MenuPage MainPage;
        public SmallButton JumpButton;
        public MultiGridItemPanel Panel;
        public IMenuElement[] PoolToggles;
        public Dictionary<string, ToggleButton> PoolToggleLookup = new();
        public SmallButton? RestoreLocalPacks;

        public static void OnExitMenu()
        {
            Instance = null;
        }

        public static void ConstructMenu(MenuPage connectionsPage)
        {
            Instance ??= new();
            Instance.OnConstructMenuFirstTime(connectionsPage);
            Instance.OnMenuConstruction();
        }

        public void ReconstructMenu()
        {
            UnityEngine.Object.Destroy(MainPage.self);
            JumpButton.ClearOnClick();
            OnMenuConstruction();
        }

        public void OnConstructMenuFirstTime(MenuPage connectionsPage)
        {
            ConnectionsPage = connectionsPage;
            JumpButton = new(connectionsPage, "Custom Pool Injection");
            connectionsPage.BeforeShow += () => JumpButton.Text.color = CustomPoolInjectorMod.GS.ActivePools.Count != 0 ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }

        public void OnMenuConstruction()
        {
            MainPage = new("Custom Pool Injector Main Menu", ConnectionsPage);
            JumpButton.AddHideAndShowEvent(MainPage);
            PoolToggles = CustomPoolInjectorMod.Pools.Values
                .Select(def => (IMenuElement)CreatePoolToggle(MainPage, def)).ToArray();
            Panel = new(MainPage, 5, 3, 60f, 650f, new(0, 300), PoolToggles);
        }

        public void CreateRestoreLocalPacksButton()
        {
            RestoreLocalPacks = new SmallButton(MainPage, "Restore Local Packs");
            RestoreLocalPacks.OnClick += () =>
            {
                MainPage.Hide();
                CustomPoolInjectorMod.LoadFiles();
                ReconstructMenu();
                MainPage.Show();
            };
            RestoreLocalPacks.MoveTo(new(0f, -300f));
            RestoreLocalPacks.SymSetNeighbor(Neighbor.Up, Panel);
            RestoreLocalPacks.SymSetNeighbor(Neighbor.Down, MainPage.backButton);
        }

        public ToggleButton CreatePoolToggle(MenuPage page, CustomPoolDef def)
        {
            ToggleButton button = new(page, def.Name);
            button.SetValue(CustomPoolInjectorMod.GS.ActivePools.Contains(def.Name));
            button.ValueChanged += b =>
            {
                if (b) CustomPoolInjectorMod.GS.ActivePools.Add(def.Name);
                else CustomPoolInjectorMod.GS.ActivePools.Remove(def.Name);
            };
            PoolToggleLookup[def.Name] = button;
            if (Panel is not null)
            {
                Panel.Add(button);
            }
            return button;
        }

        public static bool TryGetMenuButton(MenuPage connectionsPage, out SmallButton button)
        {
            return Instance.TryGetJumpButton(connectionsPage, out button);
        }

        public bool TryGetJumpButton(MenuPage connectionsPage, out SmallButton button)
        {
            button = JumpButton;
            return true;
        }

        public bool TryGetPoolToggle(string name, out ToggleButton button)
        {
            return PoolToggleLookup.TryGetValue(name, out button);
        }

        public void ToggleAllOff()
        {
            foreach (ToggleButton b in PoolToggleLookup.Values) b.SetValue(false);
        }
    }
}
