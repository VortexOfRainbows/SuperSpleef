using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;

    private Sound sound;

    private float timer;

    public static AudioPlayer GenerateAudioPlayer(GameObject prefab, Sound sound, Vector3 position)
    {
        AudioPlayer something = Instantiate(prefab, position,Quaternion.identity).GetComponent<AudioPlayer>();
        something.sound = sound;
        something.sound.SetSource(something.source);
        return something;
    }

    public void PlaySound()
    {
        sound.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlaySound();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > sound.clip.length)
        {
            Destroy(gameObject);
        }
    }
}
