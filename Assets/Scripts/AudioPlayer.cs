using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;
    private Sound sound;
    private float timer;
    public static AudioPlayer GenerateAudioPlayer(GameObject prefab, Sound sound, Vector3 position, float volumeMult = 1f, float DistanceMult = 1f, float PitchModifier = 0f)
    {
        AudioPlayer audio = Instantiate(prefab, position,Quaternion.identity).GetComponent<AudioPlayer>();
        audio.sound = sound;
        audio.sound.SetSource(audio.source);
        audio.source.maxDistance *= DistanceMult;
        audio.source.volume *= volumeMult;
        audio.source.pitch += PitchModifier;
        return audio;
    }
    public void PlaySound()
    {
        sound.Play();
    }
    void Start()
    {
        PlaySound();
    }
    void Update()
    {
        if(sound == null || sound.clip == null)
        {
            Destroy(gameObject);
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > sound.clip.length)
            {
                Destroy(gameObject);
            }
        }
    }
}
