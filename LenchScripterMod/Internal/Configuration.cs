using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lench.Scripter.Internal
{
    internal /*static*/ class Configuration
    {
        private static Configuration configuration = FormatXDataToConfig(); 

        internal static void Load()
        {
            //Script.Enabled = GetBool("script-enabled", true);
            Script.Enabled = configuration.GetValue<bool>("script-enabled");

            //Mod.WatchlistWindow.Position = new Vector2
            //{
            //    x = GetFloat("WatchlistXPos", -380),
            //    y = GetFloat("WatchlistYPos", 200)
            //};
            Mod.WatchlistWindow.Position = new Vector2(configuration.GetValue<float>("WatchlistXPos"), configuration.GetValue<float>("WatchlistYPos"));

            //Mod.IdentifierDisplayWindow.Position = new Vector2
            //{
            //    x = GetFloat("IdentifierDisplayXPos", 900),
            //    y = GetFloat("IdentifierDisplayYPos", -240)
            //};
            Mod.IdentifierDisplayWindow.Position = new Vector2(configuration.GetValue<float>("IdentifierDisplayXPos"), configuration.GetValue<float>("IdentifierDisplayYPos"));

            //Mod.ScriptOptionsWindow.Position = new Vector2
            //{
            //    x = GetFloat("ScriptOptionsXPos", -380),
            //    y = GetFloat("ScriptOptionsYPos", -400)
            //};
            Mod.ScriptOptionsWindow.Position = new Vector2(configuration.GetValue<float>("ScriptOptionsXPos"), configuration.GetValue<float>("ScriptOptionsYPos"));

            //Mod.Toolbar.Position = GetFloat("ToolbarPos", 400);
            Mod.Toolbar.Position = configuration.GetValue<float>("ToolbarPos");

            //PythonEnvironment.Version = GetString("PythonVersion", "ironpython2.7");
            PythonEnvironment.Version = configuration.GetValue<string>("PythonVersion");
        }
        
        internal static void Save()
        {
            //SetBool("mod-updater-enabled", Mod.UpdateCheckerEnabled);
            //SetBool("script-enabled", Script.Enabled);
            configuration.SetValue("script-enabled", Script.Enabled);

            //SetFloat("WatchlistXPos", Mod.WatchlistWindow.Position.x);
            //SetFloat("WatchlistYPos", Mod.WatchlistWindow.Position.y);
            configuration.SetValue("WatchlistXPos", Mod.WatchlistWindow.Position.x);
            configuration.SetValue("WatchlistYPos", Mod.WatchlistWindow.Position.y);

            //SetFloat("IdentifierDisplayXPos", Mod.IdentifierDisplayWindow.Position.x);
            //SetFloat("IdentifierDisplayYPos", Mod.IdentifierDisplayWindow.Position.y);
            configuration.SetValue("IdentifierDisplayXPos", Mod.IdentifierDisplayWindow.Position.x);
            configuration.SetValue("IdentifierDisplayYPos", Mod.IdentifierDisplayWindow.Position.y);

            //SetFloat("ScriptOptionsXPos", Mod.ScriptOptionsWindow.Position.x);
            //SetFloat("ScriptOptionsYPos", Mod.ScriptOptionsWindow.Position.y);
            configuration.SetValue("ScriptOptionsXPos", Mod.ScriptOptionsWindow.Position.x);
            configuration.SetValue("ScriptOptionsYPos", Mod.ScriptOptionsWindow.Position.y);

            //SetFloat("ToolbarPos", Mod.Toolbar.Position);
            configuration.SetValue("ToolbarPos", Mod.Toolbar.Position);

            //SetString("PythonVersion", PythonEnvironment.Version);
            configuration.SetValue("PythonVersion", PythonEnvironment.Version);

            Modding.Configuration.Save();
        }

        internal static ArrayList Propertises { get; private set; } = new ArrayList()
        {
            new Propertise<bool>("script-enabled",  true ),
            new Propertise<float>("WatchlistXPos",-380f ),
            new Propertise<float>("WatchlistXPos",  200f ),
            new Propertise<float>("IdentifierDisplayXPos",900f ),
            new Propertise<float>("IdentifierDisplayYPos", -240f ),
            new Propertise<float>("ScriptOptionsXPos", -380f ),
            new Propertise<float>("ScriptOptionsYPos",  -400f),
            new Propertise<float>("ToolbarPos",  400f),
            new Propertise<string>("PythonVersion","ironpython2.7"),
        };

        public class Propertise<T>
        {
            public string Key = "";
            public T Value = default;

            public Propertise(string key, T value) { Key = key; Value = value; }
            public override string ToString()
            {
                return Key + " - " + Value.ToString();
            }
        }

        public T GetValue<T>(string key)
        {
            T value = default;

            foreach (var pro in Propertises)
            {
                if (pro is Propertise<T>)
                {
                    var _pro = pro as Propertise<T>;
                    if (_pro.Key == key)
                    {
                        value = _pro.Value;
                        break;
                    }
                }
            }
            return value;
        }

        public void SetValue<T>(string key, T value)
        {
            var exist = false;

            foreach (var pro in Propertises)
            {
                if (pro is Propertise<T>)
                {
                    var _pro = pro as Propertise<T>;
                    if (_pro.Key == key)
                    {
                        _pro.Value = value;
                        exist = true;
                        break;
                    }
                }
            }

            if (!exist)
            {
                Propertises.Add(new Propertise<T>(key, value));
            }

            Modding.Configuration.GetData().Write(key, value);
        }

        ~Configuration()
        {
            Modding.Configuration.Save();
        }

        public static Configuration FormatXDataToConfig(Configuration config = null)
        {
            XDataHolder xDataHolder = Modding.Configuration.GetData();
            bool reWrite = true;
            bool needWrite = false;

            if (config == null)
            {
                reWrite = false;
                needWrite = true;
                config = new Configuration();
            }

            for (int i = 0; i < Propertises.Count; i++)
            {
                var value = Propertises[i];

                if (value is Propertise<int>)
                {
                    value = getValue(value as Propertise<int>);
                }
                else if (value is Propertise<bool>)
                {
                    value = getValue(value as Propertise<bool>);
                }
                else if (value is Propertise<float>)
                {
                    value = getValue(value as Propertise<float>);
                }
                else if (value is Propertise<string>)
                {
                    value = getValue(value as Propertise<string>);
                }
                else if (value is Propertise<Vector3>)
                {
                    value = getValue(value as Propertise<Vector3>);
                }
                Propertises[i] = value;
            }

            if (needWrite) Modding.Configuration.Save();

            return config;

            Propertise<T> getValue<T>(Propertise<T> propertise)
            {
                var key = propertise.Key;
                var defaultValue = propertise.Value;

                if (xDataHolder.HasKey(key) && !reWrite)
                {
                    defaultValue = (T)Convert.ChangeType(typeSpecialAction[typeof(T)](xDataHolder, key), typeof(T));
                }
                else
                {
                    xDataHolder.Write(key, defaultValue);
                    needWrite = true;
                }

                return new Propertise<T>(key, defaultValue);
            }
        }
        private static Dictionary<Type, Func<XDataHolder, string, object>> typeSpecialAction = new Dictionary<Type, Func<XDataHolder, string, object>>
        {
            { typeof(int), (xDataHolder,key)=>xDataHolder.ReadInt(key)},
            { typeof(bool), (xDataHolder,key)=>xDataHolder.ReadBool(key)},
            { typeof(float), (xDataHolder,key)=>xDataHolder.ReadFloat(key)},
            { typeof(string), (xDataHolder,key)=>xDataHolder.ReadString(key)},
            { typeof(Vector3), (xDataHolder,key)=>xDataHolder.ReadVector3(key)},
        };
    }
}