using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace ValhiemHotkeys {
    [BepInPlugin(UID, NAME, VERSION)]
    public class ValhiemHotkeys : BaseUnityPlugin {
        const string UID = "com.ashnel3.ValhiemHotkeys";
        const string NAME = "Valhiem Hotkeys";
        const string VERSION = "0.1.0";

        private readonly ConfigEntry<KeyboardShortcut> ConfigQuit;
        private readonly ConfigEntry<KeyboardShortcut> ConfigDisconnect;
        private readonly Harmony Harmony = new Harmony(UID);

        public ValhiemHotkeys() {
            ConfigQuit = Config.Bind("General", "Quit", new KeyboardShortcut(KeyCode.Delete), "Quit Hotkey");
            ConfigDisconnect = Config.Bind("General", "Disconnect", new KeyboardShortcut(KeyCode.End), "Disconnect Hotkey");
        }

        public void Awake() {
            Harmony.PatchAll();
        }

        public void Update() {
            if (ConfigDisconnect.Value.IsPressed()) {
                Game.instance.Logout();
            }
            if (ConfigQuit.Value.IsPressed()) {
                Application.Quit();
            }
        }
    }
}
