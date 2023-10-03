using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using HarmonyLib;
using System.Xml.Linq;
using System.Xml.XPath;

public static class WMMGameOptions
{
    private static bool ConfigsLoaded = false;
    private static string SaveDataPath = "";
    private static string ModPath = "";
    private static string GameOptionsFilename = "DishongGameOptions.xml";
    public static Dictionary<string, string> StoredOptions;
    public static GameRandom Random;
    public static bool QualityReworkModExists = false;

    static WMMGameOptions()
    {
        StoredOptions = new Dictionary<string, string>();
        var mod = ModManager.GetMod("DishongGameOptions", true);
        ModPath = mod.Path;
        SaveDataPath = mod.Path + "/SaveData";
        Directory.CreateDirectory(SaveDataPath);
        Log.Out("DishongGameOptions DataConfigPath = " + SaveDataPath);
        LoadConfigs();

        if (Random == null)
        {
            Random = GameRandomManager.Instance.CreateGameRandom();
        }

        QualityReworkModExists = ModManager.GetMod("WMMQualityRework", true) != null;
    }

    public static void LoadConfigs()
    {
        if (!ConfigsLoaded)
        {
            ConfigsLoaded = true;
            LoadGameOptions();
            Log.Out("Dishong Game Options Loaded.");
        }
    }

    private static void LoadGameOptions()
    {
        // For SP/MP games the UI guides the player through the menus and a SaveGameOptions() is called when the game starts (see Harmony patches). This in turn creates the custom game xml file and hence is available to load further down the method. 
        // But for dedicated servers we dont run this and hence we need to load the default values from the xui xml!
        if (GameManager.IsDedicatedServer)
        {
            var path = ModPath + "/Config/XUi_Menu/";
            if (File.Exists(path + "windows.xml"))
            {
                var xmlFile = new XmlFile(path, "windows.xml");

                var root = xmlFile.XmlDoc.Root;
                if (root == null || !root.HasElements)
                {
                    throw new Exception("No element <customgameoptions> found!");
                }

                var xelements = root.XPathSelectElements("/windows/insertAfter/rect[@tab_key='xuiCustomGameOptions']/grid/gameoption");

                foreach (var xelement in xelements)
                {
                    var name = xelement.GetAttribute("name");
                    var defaultValue = xelement.GetAttribute("defaultValue");

                    if (!string.IsNullOrEmpty(name) &&
                        !string.IsNullOrEmpty(defaultValue))
                    {
                        StoredOptions[name] = defaultValue;
                    }
                }
            }

            Log.Out("Custom Game Options Default Values Loaded.");
        }

        // Check for a saved game file and load from there now!
        // This is covers SP/MP games from UI and Dedicated if there is a Savegame xml file.
        if (!string.IsNullOrEmpty(SaveDataPath))
        {
            var path = SaveDataPath + "/" + GameOptionsFilename;
            if (File.Exists(path))
            {
                var xmlFile = new XmlFile(SaveDataPath, GameOptionsFilename);
                var root = xmlFile.XmlDoc.Root;
                if (root == null || !root.HasElements)
                {
                    throw new Exception("No element <customgameoptions> found!");
                }

                foreach (XElement xelement in root.Elements())
                {
                    if (xelement.Name.LocalName.Equals("config"))
                    {
                        if (!xelement.HasAttribute("name"))
                        {
                            throw new Exception("Attribute 'name' missing on item");
                        }

                        if (!xelement.HasAttribute("value"))
                        {
                            throw new Exception("Attribute 'value' missing on item");
                        }

                        var value = xelement.GetAttribute("value");
                        var name = xelement.GetAttribute("name");

                        StoredOptions[name] = value;
                    }
                }
            }
        }

        if (GameManager.IsDedicatedServer)
        {
            SaveGameOptions();
        }
    }

    public static void SaveGameOptions()
    {
        XmlDocument xmlDocument = new XmlDocument();
        XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDocument.InsertBefore(newChild, xmlDocument.DocumentElement);
        XmlNode parent = xmlDocument.AppendChild(xmlDocument.CreateElement("customgameoptions"));

        foreach (var key in StoredOptions.Keys)
        {
            parent.AppendChild(CreateXMLGameOptionChild(xmlDocument, key, StoredOptions[key].ToString()));
        }

        Log.Out(SerializeToString(xmlDocument));

        // Make sure folder is there before saving
        Directory.CreateDirectory(SaveDataPath);
        var path = SaveDataPath + "/" + GameOptionsFilename;
        File.WriteAllText(path, SerializeToString(xmlDocument), Encoding.UTF8);
    }

    private static XmlElement CreateXMLGameOptionChild(XmlDocument xmlDocument, string name, string value)
    {
        XmlElement xmlElement = xmlDocument.CreateElement("config");
        XmlAttribute xmlAttributeName = xmlDocument.CreateAttribute("name");
        xmlAttributeName.Value = name;
        xmlElement.Attributes.Append(xmlAttributeName);
        XmlAttribute xmlAttributeValue = xmlDocument.CreateAttribute("value");
        xmlAttributeValue.Value = value.ToLower();
        xmlElement.Attributes.Append(xmlAttributeValue);

        return xmlElement;
    }

    private static string SerializeToString(XmlDocument xmlDocument)
    {
        string result;
        using (StringWriter stringWriter = new StringWriter())
        {
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
            {
                xmlDocument.WriteTo(xmlTextWriter);
                result = stringWriter.ToString();
            }
        }
        return result;
    }

    public static bool HasValue(string name)
    {
        return StoredOptions.ContainsKey(name);
    }

    public static void SetValue(string name, object value)
    {
        StoredOptions[name] = value.ToString();
    }

    public static int GetInt(string name, int defaultValue = 0)
    {
        if (!StoredOptions.ContainsKey(name))
        {
            return defaultValue;
        }

        return int.TryParse(StoredOptions[name], out var output) ? output : defaultValue;
    }

    public static string GetString(string name, string defaultValue = "")
    {
        if (!StoredOptions.ContainsKey(name))
        {
            return defaultValue;
        }

        return StoredOptions[name].ToString();
    }

    public static bool GetBool(string name, bool defaultValue = false)
    {
        if (!StoredOptions.ContainsKey(name))
        {
            return defaultValue;
        }

        return bool.TryParse(StoredOptions[name], out var output) ? output : defaultValue;
    }
}
