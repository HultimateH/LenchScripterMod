using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lench.Scripter.Internal;
using Lench.Scripter.Resources;
using Lench.Scripter.UI;
using UnityEngine;
using Configuration = Lench.Scripter.Internal.Configuration;
using MachineData = Lench.Scripter.Internal.MachineData;
using Object = UnityEngine.Object;
using Modding;
using PluginManager.Plugin;
// ReSharper disable UnusedMember.Local

namespace Lench.Scripter
{
    /// <summary>
    ///     Mod class loaded by the Mod Loader.
    /// </summary>
    [OnGameInit]
    public class Mod :MonoBehaviour
    {
        /// <summary>
        ///     Is LenchScripterMod Block API loaded.
        /// </summary>
        public static bool LoadedAPI { get; internal set; }

        /// <summary>
        ///     Is LenchScripterMod full scripter loaded.
        /// </summary>
        public static bool LoadedScripter { get; internal set; }

        /// <summary>
        ///     Parent GameObject of all mod components.
        /// </summary>
        public static GameObject Controller { get; internal set; }

        internal static IdentifierDisplayWindow IdentifierDisplayWindow;
        internal static ScriptOptionsWindow ScriptOptionsWindow;
        internal static WatchlistWindow WatchlistWindow;
        internal static Toolbar Toolbar;
        //internal static SettingsButton EnableScriptButton;
        //internal static OptionsButton PythonVersion2Button;
        //internal static OptionsButton PythonVersion3Button;

        /// <summary>
        ///     Instantiates the mod and it's components.
        ///     Looks for and loads assemblies.
        /// </summary>
        public /*override*/ void /*OnLoad()*/Start()
        {
            Events.OnSimulationToggle += Block.OnSimulationToggle;
            Events.OnSimulationToggle += Script.OnSimulationToggle;
            Events.OnBlockPlaced += block => Block.FlagForIDRebuild();
            //Events.OnBlockRemoved += Block.FlagForIDRebuild;
            Block.OnInitialisation += Script.Start;

            XmlSaver.OnSave += MachineData.Save;
            XmlLoader.OnLoad += MachineData.Load;

            //Commands.RegisterCommand("lsm", ConfigurationCommand,
            //    "Scripter Mod configuration command.");
            //Commands.RegisterCommand("py", PythonCommand,
            //    "Executes Python expression.");
            //Commands.RegisterCommand("python", PythonCommand,
            //    "Executes Python expression.");

            Controller = new GameObject("LenchScripterMod") { hideFlags = HideFlags.DontSave };
            Controller.AddComponent<ModController>();
            Object.DontDestroyOnLoad(Controller);

            IdentifierDisplayWindow = new IdentifierDisplayWindow();
            ScriptOptionsWindow = new ScriptOptionsWindow();
            WatchlistWindow = new WatchlistWindow();
            Toolbar = new Toolbar
            {
                Texture = Images.IconPython,
                Visible = Script.Enabled,
                Buttons =
                {
                    new Toolbar.Button
                    {
                        Style = new GUIStyle
                        {
                            normal = { background = Images.ButtonKeyNormal },
                            focused = { background = Images.ButtonKeyFocus },
                            hover = { background = Images.ButtonKeyHover },
                            active = { background = Images.ButtonKeyActive },
                            fixedWidth = 32,
                            fixedHeight = 32
                        },
                        Text="",
                        OnClick = OpenIdentifier
                    },
                    new Toolbar.Button
                    {
                        Style = new GUIStyle
                        {
                            normal = { background = Images.ButtonListNormal },
                            focused = { background = Images.ButtonListFocus },
                            hover = { background = Images.ButtonListHover },
                            active = { background = Images.ButtonListActive },
                            fixedWidth = 32,
                            fixedHeight = 32
                        },
                        Text="",
                        OnClick = OpenWatchlist
                    },
                    new Toolbar.Button
                    {
                        Style = new GUIStyle
                        {
                            normal = { background = Images.ButtonScriptNormal },
                            focused = { background = Images.ButtonScriptFocus },
                            hover = { background = Images.ButtonScriptHover },
                            active = { background = Images.ButtonScriptActive },
                            fixedWidth = 32,
                            fixedHeight = 32
                        },
                        Text="",
                        OnClick = OpenScript
                    },
                    new Toolbar.Button
                    {
                        Style = new GUIStyle()
                        {
                            normal = { background = Images.ButtonSettingsNormal },
                            focused = { background = Images.ButtonSettingsFocus },
                            hover = { background = Images.ButtonSettingsHover },
                            active = { background = Images.ButtonSettingsActive },
                            fixedWidth = 32,
                            fixedHeight = 32
                        },
                        Text="",
                        OnClick = OpenSettings
                    }
                }
            };

            //Object.DontDestroyOnLoad(DependencyInstaller.Instance);

            LoadedAPI = true;

            Configuration.Load();

            //EnableScriptButton = new SettingsButton
            //{
            //    Text = "SCRIPT",
            //    Value = Script.Enabled,
            //    OnToggle = enabled =>
            //    {
            //        Script.Enabled = enabled;
            //        Toolbar.Visible = enabled;
            //    }
            //};
            //EnableScriptButton.Create();

            //PythonVersion2Button = new OptionsButton
            //{
            //    Text = "Python 2.7",
            //    Value = PythonEnvironment.Version == "ironpython2.7",
            //    OnToggle = enabled =>
            //    {
            //        if (enabled)
            //        {
            //            if (PythonEnvironment.Version != "ironpython3.0") return;
            //            PythonVersion3Button.Value = false;
            //            Script.SetVersionAndReload("ironpython2.7");
            //        }
            //        else
            //        {
            //            PythonVersion2Button.Value = true;
            //        }
            //    }
            //};
            //PythonVersion2Button.Create();

            //PythonVersion3Button = new OptionsButton
            //{
            //    Text = "Python 3.0",
            //    Value = PythonEnvironment.Version == "ironpython3.0",
            //    OnToggle = enabled =>
            //    {
            //        if (enabled)
            //        {
            //            if (PythonEnvironment.Version != "ironpython2.7") return;
            //            PythonVersion2Button.Value = false;
            //            Script.SetVersionAndReload("ironpython3.0");
            //        }
            //        else
            //        {
            //            PythonVersion3Button.Value = true;
            //        }
            //    }
            //};
            //PythonVersion3Button.Create();
        }

        private static void OpenIdentifier()
        {
            IdentifierDisplayWindow.Visible = true;
        }

        private static void OpenWatchlist()
        {
            WatchlistWindow.Visible = true;
        }

        private static void OpenScript()
        {
            if (File.Exists(Script.FilePath))
            {
#if DEBUG
                Debug.Log($"Opening file {Script.FilePath} ...");
#endif
                Application.OpenURL(Script.FilePath);
            }
            else if (!string.IsNullOrEmpty(Script.EmbeddedCode))
            {
#if DEBUG
                Debug.Log($"Exporting code ...");
#endif
                Application.OpenURL(Script.Export());
            }
        }

        private static void OpenSettings()
        {
            ScriptOptionsWindow.Visible = true;
        }

        internal class ModController : MonoBehaviour
        {
            private void Start()
            {
                Debug.Log("script mod");
                
                if (Script.LoadEngine(true)) return;

                Debug.Log("[LenchScripterMod]: Additional assets required.\n" +
                          "\tFiles will be placed in Mods/Resources/LenchScripter.\n" +
                          "\tType `lsm python 2.7` or `lsm python 3.0` to download them.");
                //DependencyInstaller.Visible = true;
            }

            private void Update()
            {
                if (Toolbar != null)
                    Toolbar.Visible = !StatMaster.inMenu &&
                                      !Game.IsSimulating &&
                                      AddPiece.Instance != null;
            }
        }

        /// <summary>
        ///     Called on python console command.
        /// </summary>
        public static string PythonCommand(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length == 0)
                return "Executes a Python expression.";

            if (Script.Python == null)
                return "Python engine not initialized.";

            var expression = args.Aggregate("", (current, t) => current + (t + " "));

            try
            {
                var result = Script.Python.Execute(expression);
                return result?.ToString() ?? "";
            }
            catch (Exception e)
            {
                if (e.InnerException != null) e = e.InnerException;
                Debug.Log("<b><color=#FF0000>Python error: " + e.Message + "</color></b>\n" +
                          PythonEnvironment.FormatException(e));
                return "";
            }
        }

        /// <summary>
        ///     Called on lsm console command.
        /// </summary>
        public static string ConfigurationCommand(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length <= 0)
                return "Available commands:\n" +
                       "  lsm modupdate check  \t Checks for mod update.\n" +
                       "  lsm modupdate enable \t Enables update checker.\n" +
                       "  lsm modupdate disable\t Disables update checker.\n" +
                       "  lsm python version   \t Current Python version.\n" +
                       "  lsm python 2.7       \t Switches to IronPython 2.7.\n" +
                       "  lsm python 3.0       \t Switches to IronPython 3.0.\n";
            switch (args[0].ToLower())
            {
                case "python":
                    if (args.Length > 1)
                        switch (args[1].ToLower())
                        {
                            case "version":
                                return (string)Script.Python.Execute("sys.version");
                            case "2.7":
                                PythonEnvironment.Version = "ironpython2.7";
                                return null;
                            case "3.0":
                                PythonEnvironment.Version = "ironpython3.0";
                                return null;
                            default:
                                return "Invalid argument [version/2.7/3.0]. Enter 'lsm' for all available commands.";
                        }
                    return "Missing argument [version/2.7/3.0]. Enter 'lsm' for all available commands.";
                default:
                    return "Invalid command. Enter 'lsm' for all available commands.";
            }
        }
    }
}