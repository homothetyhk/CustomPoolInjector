using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using UnityEngine.SceneManagement;

namespace CustomPoolInjector
{
    public class MenuHolder
    {
        public static MenuHolder Instance { get; private set; }

        public MenuPage MainPage;
        public SmallButton JumpButton;
        public MultiGridItemPanel Panel;
        public IMenuElement[] PoolToggles;

        public static void OnExitMenu(Scene from, Scene to)
        {
            if (from.name == "Menu_Title") Instance = null;
        }

        public static void ConstructMenu(MenuPage connectionsPage)
        {
            Instance ??= new();
            Instance.OnMenuConstruction(connectionsPage);
        }

        public void OnMenuConstruction(MenuPage connectionsPage)
        {
            MainPage = new("Custom Pool Injector Main Menu", connectionsPage);
            JumpButton = new(connectionsPage, "Custom Pool Injection");
            JumpButton.AddHideAndShowEvent(MainPage);
            PoolToggles = CustomPoolInjectorMod.Pools.Values
                .Select(def => CreatePoolToggle(MainPage, def)).ToArray();
            Panel = new(MainPage, 6, 4, 50f, 400f, new(0, 300), PoolToggles);
        }

        public IMenuElement CreatePoolToggle(MenuPage page, CustomPoolDef def)
        {
            ToggleButton button = new(page, def.Name);
            button.SetValue(CustomPoolInjectorMod.GlobalSettings.ActivePools.Contains(def.Name));
            button.ValueChanged += b =>
            {
                if (b) CustomPoolInjectorMod.GlobalSettings.ActivePools.Add(def.Name);
                else CustomPoolInjectorMod.GlobalSettings.ActivePools.Remove(def.Name);
            };
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

    }
}
