using System;
using System.Collections.Generic;
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
    public static SaveData<int> WorldGenUCI;
    public static void Initialize()
    {
        MouseSensitivity = new SaveData<float>("MouseSens", "Mouse Sensitivity");
        ControllerSensitivity = new SaveData<float>("GamepadSens", "Gamepad Sensitivity");
        SoundVolume = new SaveData<float>("Sound", "Sound Volume");
        MusicVolume = new SaveData<float>("Music", "Music Volume");
        ParticleMultiplier = new SaveData<float>("ParticleMult", "Particle Multiplier");
        WorldSize = new SaveData<float>("WorldSize", "World Size (Experimental)");
        WorldGenUCI = new SaveData<int>("WorldGenUCI", "Generate UCI");
        ReadValues();

        Dict = new Dictionary<string, object>
        {
            { MouseSensitivity.Key, MouseSensitivity },
            { ControllerSensitivity.Key, ControllerSensitivity },
            { SoundVolume.Key, SoundVolume },
            { MusicVolume.Key, MusicVolume },
            { ParticleMultiplier.Key, ParticleMultiplier },
            { WorldSize.Key, WorldSize },
            { WorldGenUCI.Key, WorldGenUCI },
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
        WorldGenUCI.ReadValue();
    }
}
public class SaveData<T>
{
    public T Value { get; private set; }
    public string Key { get; private set; }
    public string DisplayName { get; private set; }
    public SaveData(T defaultValue, string key, string displayName)
    {
        Value = defaultValue;
        Key = key;
        DisplayName = displayName;
    }
    public SaveData(string key, string displayName)
    {
        if (Value.GetType() == typeof(int))
        {
            Value = (T)(object)1;
        }
        else if (Value.GetType() == typeof(float))
        {
            Value = (T)(object)1f;
        }
        else
        {
            Value = default;
        }
        Key = key;
        DisplayName = displayName;
    }
    public void ReadValue()
    {
        if (Value.GetType() == typeof(string))
        {
            Value = (T)(object)PlayerPrefs.GetString(Key, "Unnamed");
        }
        else if (Value.GetType() == typeof(int))
        {
            Value = (T)(object)PlayerPrefs.GetInt(Key, 1);
        }
        else if (Value.GetType() == typeof(float))
        {
            Value = (T)(object)PlayerPrefs.GetFloat(Key, 1);
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
