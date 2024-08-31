using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ClientData
{
    public static Dictionary<string, object> Dict;
    public static SaveData<float> MouseSensitivity;
    public static SaveData<float> ControllerSensitivity;
    public static SaveData<float> SoundVolume;
    public static SaveData<float> MusicVolume;
    public static SaveData<float> ParticleMultiplier;

    //World settings might need to be saveable as Preset in the future, in case people have certain settings they like more than others
    public static SaveData<float> WorldSize;
    public static SaveData<int> WorldType;
    public static SaveData<int> WorldGenUCI;
    public static SaveData<int> WorldGenChaos;
    public static SaveData<int> WorldGenBorder;
    public static SaveData<int> WorldGenPadding;
    public static SaveData<int> WorldApoc;
    public static void Initialize()
    {
        MouseSensitivity = new SaveData<float>(1, "MouseSens", "Mouse Sensitivity");
        ControllerSensitivity = new SaveData<float>(1, "GamepadSens", "Gamepad Sensitivity");
        SoundVolume = new SaveData<float>(1, "Sound", "Sound Volume");
        MusicVolume = new SaveData<float>(1, "Music", "Music Volume");
        ParticleMultiplier = new SaveData<float>(1, "ParticleMult", "Particle Multiplier");
        WorldSize = new SaveData<float>(World.DefaultChunkRadius, "WorldSize", "World Size (Experimental)");
        WorldType = new SaveData<int>(0, "WorldType", "World Type");
        WorldGenUCI = new SaveData<int>(0, "WorldGenUCI", "Generate UCI");
        WorldGenChaos = new SaveData<int>(0, "WorldGenChaos", "CHAOS?!");
        WorldGenBorder = new SaveData<int>(0, "WorldGenBorder", "World Border");
        WorldGenPadding = new SaveData<int>(0, "WorldGenPadding", "Empty Outer Chunks");
        WorldApoc = new SaveData<int>(0, "WorldApoc", "Apocalypse");
        ReadValues();

        Dict = new Dictionary<string, object>
        {
            { MouseSensitivity.Key, MouseSensitivity },
            { ControllerSensitivity.Key, ControllerSensitivity },
            { SoundVolume.Key, SoundVolume },
            { MusicVolume.Key, MusicVolume },
            { ParticleMultiplier.Key, ParticleMultiplier },
            { WorldSize.Key, WorldSize },
            { WorldType.Key, WorldType },
            { WorldGenUCI.Key, WorldGenUCI },
            { WorldGenChaos.Key, WorldGenChaos },
            { WorldGenBorder.Key, WorldGenBorder },
            { WorldGenPadding.Key, WorldGenPadding },
            { WorldApoc.Key, WorldApoc },
        };
    }
    /// <summary>
    /// Retrieve save data from player pref keys, that way we can save settings accross play sessions.
    /// </summary>
    public static void ReadValues()
    {
        MouseSensitivity.ReadValue();
        ControllerSensitivity.ReadValue();
        SoundVolume.ReadValue();
        MusicVolume.ReadValue();
        ParticleMultiplier.ReadValue();
        WorldSize.ReadValue();
        WorldType.ReadValue();
        WorldGenUCI.ReadValue();
        WorldGenChaos.ReadValue();
        WorldGenBorder.ReadValue();
        WorldGenPadding.ReadValue();
        WorldApoc.ReadValue();
    }
}
public class SaveData<T>
{
    public T Value { get; private set; }
    public T DefaultValue;
    public string Key { get; private set; }
    public string DisplayName { get; private set; }
    public SaveData(T defaultValue, string key, string displayName)
    {
        Value = defaultValue;
        DefaultValue = Value;
        Key = key;
        DisplayName = displayName;
    }
    public SaveData(string key, string displayName)
    {
        if (Value.GetType() == typeof(int))
        {
            Value = (T)(object)1;
            DefaultValue = Value;
        }
        else if (Value.GetType() == typeof(float))
        {
            Value = (T)(object)1f;
            DefaultValue = Value;
        }
        else
        {
            Value = default;
            DefaultValue = Value;
        }
        Key = key;
        DisplayName = displayName;
    }
    public void ReadValue()
    {
        if (Value.GetType() == typeof(string))
        {
            Value = (T)(object)PlayerPrefs.GetString(Key, (string)(object)DefaultValue);
        }
        else if (Value.GetType() == typeof(int))
        {
            Value = (T)(object)PlayerPrefs.GetInt(Key, (int)(object)DefaultValue);
        }
        else if (Value.GetType() == typeof(float))
        {
            Value = (T)(object)PlayerPrefs.GetFloat(Key, (float)(object)DefaultValue);
        }
        else
            throw new NotImplementedException("Cannot retrieve save data that is not a string, float, or int. The current system uses Unity Player Prefs");
    }
    public void WriteValue(T value)
    {
        Value = value;
        if (Value.GetType() == typeof(string))
        {
            PlayerPrefs.SetString(Key, (string)(object)value);
        }
        else if (Value.GetType() == typeof(int))
        {
            PlayerPrefs.SetInt(Key, (int)(object)value);
        }
        else if (Value.GetType() == typeof(float))
        {
            PlayerPrefs.SetFloat(Key, (float)(object)value);
        }
        else
            throw new NotImplementedException("Cannot save data that is not a string, float, or int. The current system uses Unity Player Prefs");
        PlayerPrefs.Save();
    }
}
