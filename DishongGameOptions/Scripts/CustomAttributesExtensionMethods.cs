using System.Collections.Generic;

public static class CustomAttributesExtensionMethods
{
    public static bool GetBoolAttribute(this Dictionary<string, string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            var output = false;
            bool.TryParse(dictionary[key], out output);
            return output;
        }

        return false;
    }

    public static string GetStringAttribute(this Dictionary<string, string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        return string.Empty;
    }

    public static int GetIntAttribute(this Dictionary<string, string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            var output = 0;
            int.TryParse(dictionary[key], out output);
            return output;
        }

        return 0;
    }

    public static void SetAttribute(this Dictionary<string, string> dictionary, string key, object value)
    {
        if (dictionary.ContainsKey(key))
            dictionary[key] = value.ToString();
        else
            dictionary.Add(key, value.ToString());
    }
}