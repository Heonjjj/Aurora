using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField][Range(0f, 1f)] private float soundEffectVolume; // 효과음 볼륨
    [SerializeField][Range(0f, 1f)] private float soundEffectPitchVariance; // 효과음 피치 랜덤성
    [SerializeField][Range(0f, 1f)] private float musicVolume; // 배경 음악 볼륨

    private AudioSource musicAudioSource; // 배경 음악용 AudioSource
    public AudioClip musicClip; // 기본 배경 음악 클립

    private void Awake()
    {
        instance = this;

        // 배경음 재생용 AudioSource 설정
        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.volume = musicVolume;
        musicAudioSource.loop = true;
    }

    private void Start()
    {
        // 기본 배경 음악 시작
        ChangeBackGroundMusic(musicClip);
    }

    // 배경 음악을 다른 클립으로 교체하는 함수
    public void ChangeBackGroundMusic(AudioClip clip)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }
}