using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ValhiemHotkeys {
    [BepInPlugin(UID, NAME, VERSION)]
    public class HotkeysPlugin : BaseUnityPlugin {
        public const string UID = "com.ashnel3.ValhiemHotkeys";
        public const string NAME = "Valhiem Hotkeys";
        public const string VERSION = "0.1.0";
        private static HotkeysPlugin instance = null;
        private static Harmony harmony = new Harmony(UID);

        private class KeyboardShortcutHandler {
            public Action CallBack;
            public ConfigEntry<KeyboardShortcut> Entry;
        };

        private readonly List<KeyboardShortcutHandler> ConfigShortcuts;

        public HotkeysPlugin() {
            ConfigShortcuts = new List<KeyboardShortcutHandler> {
                // disconnect hotkey
                new KeyboardShortcutHandler {
                    CallBack = () => Game.instance.Logout(),
                    Entry = Config.Bind("General", "Disconnect", new KeyboardShortcut(KeyCode.End), "Disconnect Hotkey"),
                },
                // quit hotkey
                new KeyboardShortcutHandler {
                    CallBack = () => Application.Quit(),
                    Entry = Config.Bind("General", "Quit", new KeyboardShortcut(KeyCode.Delete), "Quit Hotkey"),
                },
            };
            // emote hotkeys
            foreach (Emotes i in Emotes.GetValues(typeof(Emotes))) {
                var name = Enum.GetName(typeof(Emotes), i);
                ConfigShortcuts.Add(new KeyboardShortcutHandler {
                    CallBack = () => Emote.DoEmote(i),
                    Entry = Config.Bind("General", "Emote " + name, new KeyboardShortcut()),
                });
            }
            // reconnect hotkeys
            for (int i = 0; i <= 9; i++) {
                var Index = i;
                ConfigShortcuts.Add(new KeyboardShortcutHandler {
                    CallBack = () => Reconnect(Index),
                    Entry = Config.Bind("General", "Reconnect " + i, new KeyboardShortcut(KeyCode.Keypad0 + i)),
                });
            }
        }

        public void Reconnect(int index) {
            var list = new List<ServerJoinData>();
            if (!ServerList.LoadServerListFromDisk(ServerListType.recent, ref list)) {
                Logger.LogError("failed to load server list!");
                return;
            }
            var data = list.ElementAtOrDefault(index);
            if (data != null) {
                FejdStartup.instance.SetServerToJoin(new ServerStatus(list[index]));
                FejdStartup.instance.JoinServer();
            } else {
                Logger.LogError("failed to find recent server '" + index + "'!");
                return;
            }
        }

        public void Awake() {
            instance = this;
            harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), UID);
        }

        public void OnDestroy() {
            instance = null;
            harmony?.UnpatchSelf();
        }

        public void Update() {
            foreach (var Shortcut in ConfigShortcuts) {
                if (Shortcut.Entry.Value.IsPressed()) {
                    Shortcut.CallBack();
                }
            }
        }
    }
}
