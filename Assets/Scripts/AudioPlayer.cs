using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource Source;
    private Sound Sound;
    private float Timer;
    public static AudioPlayer GenerateAudioPlayer(GameObject prefab, Sound sound, Vector3 position, float volumeMult = 1f, float DistanceMult = 1f, float PitchModifier = 0f)
    {
        AudioPlayer audio = Instantiate(prefab, position,Quaternion.identity).GetComponent<AudioPlayer>();
        audio.Sound = sound;
        audio.Sound.SetSource(audio.Source);
        audio.Source.maxDistance *= DistanceMult;
        audio.Source.volume *= volumeMult;
        audio.Source.pitch += PitchModifier;
        return audio;
    }
    public void PlaySound()
    {
        Sound.Play();
    }
    void Start()
    {
        PlaySound();
    }
    void Update()
    {
        if(Sound.Clip == null)
        {
            Destroy(gameObject);
        }
        else
        {
            Timer += Time.deltaTime;
            if (Timer > Sound.Clip.length)
            {
                Destroy(gameObject);
            }
        }
    }
}
