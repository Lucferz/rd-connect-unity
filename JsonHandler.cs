using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


public class JsonHandler
{

    
    public static string ConvertDictionaryToJson(Dictionary<string, object> data)
    {
        // Serialize the dictionary to JSON using JSON.NET
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        return json;
    }

    public static Dictionary<string, object> ConvertStringJsonToDictionary(string json)
    {
        Dictionary<string, object> parsedJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        return parsedJson;
    }


}
