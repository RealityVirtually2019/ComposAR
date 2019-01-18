// Teleportal SDK
// Copyright 2018 WiTag Inc

using UnityEngine;
using System;

/// <summary>
/// Helper functions for JSON file parsing and object creation.
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// From a JSON array to an object array.
    /// </summary>
    /// <param name="json">JSON array string.</param>
    /// <returns>An object array of the specified type T.</returns>
    public static T[] FromJson<T>(string json)
    {
        // The JSON needs to be prefixed with an "Inventory" object
        // before it is parsed from the server.
        json = prefixJson(json);

        // Parse.
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Inventory;
    }

    /// <summary>
    /// From an object array to a JSON array string.
    /// </summary>
    /// <param name="array">An object array of the specified type T.</param>
    /// <returns>JSON array string.</returns>
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Inventory = array;
        return JsonUtility.ToJson(wrapper);
    }

    /// <summary>
    /// "Pretty print" version of ToJson()
    /// </summary>
    /// <param name="array">An object array of the specified type T.</param>
    /// <param name="prettyPrint">Whether to "pretty print" the result.</param>
    /// <returns></returns>
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Inventory = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    /// <summary>
    /// Wrapper for a serializable item array of type T.
    /// </summary>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Inventory;
    }

    /// <summary>
    /// Encapsulates a JSON string array for transfer.
    /// </summary>
    /// <param name="str">String array to encapsulate.</param>
    /// <returns>Encapsulated string array.</returns>
    public static string prefixJson(string str)
    {
        str = "{\"Inventory\":" + str + "}";
        return str;
    }
}
