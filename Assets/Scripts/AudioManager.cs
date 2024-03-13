using UnityEngine;
using UnityEngine.SceneManagement;

///Team members that contributed to this script: Ian Bunnell, David Bu
public static class SoundID
{
    public const int Grass = 0;
    public const int Stone = 1;
    public const int Wood = 2;
    public const int Weapon = 3;
    public const int BattleMusic = 4;
    public const int MenuMusic = 5;
    public const int Dirt = 6;
}

[System.Serializable]
public struct Sound
{
    public AudioClip Clip => clip;
    [SerializeField]
    private AudioClip clip;
    [SerializeField, Range(0f, 2f)]
    private float volume;
    [SerializeField, Range(-1f, 1f)]
    private float pitch;
    [SerializeField, Range(0f, 150f)]
    private float maxDistance;
    [SerializeField, Range(0f, 1f)]
    private float spacialBlend;
    [SerializeField]
    private bool loop;
    [SerializeField]
    private bool music;
    private AudioSource source;
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.rolloffMode = AudioRolloffMode.Linear;
        if (GameStateManager.LocalMultiplayer)
            source.spatialBlend = 0f;
        else
            source.spatialBlend = spacialBlend;
        source.maxDistance = maxDistance;
        source.volume = volume * (music ? GameStateManager.MusicMultiplier : GameStateManager.VolumeMultiplier);
        source.pitch = pitch;
        source.loop = loop;
    }
    public float GetVolume()
    {
        return volume;
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
    public static AudioPlayer PlaySound(int SoundType, Vector3 position, float volumeMult = 1f, float distanceMult = 1f, float pitchModifier = 0f)
    {
        return AudioPlayer.GenerateAudioPlayer(Instance.audioPlayer, Instance.sounds[SoundType], position, volumeMult, distanceMult, pitchModifier);
    }
    private AudioPlayer MusicPlayer = null;
    private int CurrentMusicType = -1;
    private void Update()
    {
        if(MusicPlayer == null)
        {
            SwapMenuMusic(SoundID.MenuMusic);
        }
        else
        {
            MusicPlayer.SetVolume(sounds[CurrentMusicType].GetVolume() * GameStateManager.MusicMultiplier);
        }
        if(SceneManager.GetActiveScene().name == GameStateManager.TitleScreen || SceneManager.GetActiveScene().name == GameStateManager.MultiplayerGameLobby)
        {
            ///If we are in a menu
            if (CurrentMusicType != SoundID.MenuMusic)
                SwapMenuMusic(SoundID.MenuMusic);
        }
        else
        {
            ///If we are in a battle scene
            if(CurrentMusicType != SoundID.BattleMusic)
                SwapMenuMusic(SoundID.BattleMusic);
        }
    }
    private void SwapMenuMusic(int type)
    {
        if (MusicPlayer != null)
            Destroy(MusicPlayer.gameObject);
        CurrentMusicType = type;
        MusicPlayer = PlaySound(type, Vector3.zero);
        DontDestroyOnLoad(MusicPlayer);
    }
}