using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ClientData
{
    public static SaveData<float> MouseSensitivity;
    public static SaveData<float> ControllerSensitivity;
    public static SaveData<float> SoundVolume;
    public static SaveData<float> MusicVolume;
    public static SaveData<float> ParticleMultiplier;
    public static void Initialize()
    {
        MouseSensitivity = new SaveData<float>("MouseSens", "Mouse Sensitivity");
        ControllerSensitivity = new SaveData<float>("GamepadSens", "Gamepad Sensitivity");
        SoundVolume = new SaveData<float>("Sound", "Sound Volume");
        MusicVolume = new SaveData<float>("Music", "Music Volume");
        ParticleMultiplier = new SaveData<float>("ParticleMult", "Particle Multiplier");
        ReadValues();
    }
    public static void ReadValues()
    {
        MouseSensitivity.ReadValue();
        ControllerSensitivity.ReadValue();
        SoundVolume.ReadValue();
        MusicVolume.ReadValue();
        ParticleMultiplier.ReadValue();
    }
}
public class SaveData<T>
{
    public T Value;
    public string Key;
    public string DisplayName;
    public SaveData(T defaultValue, string key, string displayName)
    {
        Value = defaultValue;
        Key = key;
        DisplayName = displayName;
    }
    public SaveData(string key, string displayName)
    {
        Value = default;
        Key = key;
        DisplayName = displayName;
    }
    public void ReadValue()
    {
        if (Value.GetType() == typeof(string))
        {
            Value = (T)(object)PlayerPrefs.GetString(Key);
        }
        else if (Value.GetType() == typeof(int))
        {
            Value = (T)(object)PlayerPrefs.GetInt(Key);
        }
        else if (Value.GetType() == typeof(float))
        {
            Value = (T)(object)PlayerPrefs.GetFloat(Key);
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
