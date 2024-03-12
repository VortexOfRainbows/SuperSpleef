using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundID
{
    public const int Grass = 0;
    public const int Stone = 1;
    public const int Weapon = 2;
    public const int Wood = 3;
    public const int BattleMusic = 4;
    public const int MenuMusic = 5;
}

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    private AudioSource source;
    [Range(0f,2f)]
    public float volume = 0.7f;
    [Range(-1f, 1f)]
    public float pitch = 1f;
    [Range(0f, 150f)]
    public float maxDistance = 100f;
    [Range(0f, 1f)]
    public float spacialBlend = 1f;
    public bool loop = false;
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.spatialBlend = spacialBlend;
        source.maxDistance = maxDistance;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
    }
    public void Play()
    {
        source.Play();
    }
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private Sound[] sounds;

    [SerializeField]
    private GameObject audioPlayer;

    private void Awake()
    {
        Instance = this;        
    }
    private void Start()
    {
        /*for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }*/
    }
    public static void PlaySound(int SoundType, Vector3 position, float volumeMult = 1f, float distanceMult = 1f, float pitchModifier = 0f)
    {
        AudioPlayer.GenerateAudioPlayer(Instance.audioPlayer, Instance. sounds[SoundType], position, volumeMult, distanceMult, pitchModifier);
        return;
    }
   
}