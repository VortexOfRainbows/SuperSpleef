using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    private AudioSource source;

    [Range(0f,1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;
    [Range(0f, 150f)]
    public float maxDistance = 100f;
    [Range(0f, 1f)]
    public float spacialBlend = 1f;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.spatialBlend = spacialBlend;
        source.maxDistance = maxDistance;
        source.volume = volume;
        source.pitch = pitch;
    }

    public void Play()
    {
        
        source.Play();
    }

}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    [SerializeField]
    Sound[] sounds;

    [SerializeField]
    private GameObject audioPlayer;

    private void Awake()
    {
            instance = this;        
    }

    private void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string _name, Vector3 position)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].name == _name)
            {
                AudioPlayer.GenerateAudioPlayer(audioPlayer, sounds[i], position);
                return;
            }
        }

        Debug.LogWarning("AudioManager: Sound not found in list:" + _name);
    }
   
}