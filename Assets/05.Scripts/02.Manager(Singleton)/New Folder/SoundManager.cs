using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField][Range(0f, 1f)] private float soundEffectVolume; // ȿ���� ����
    [SerializeField][Range(0f, 1f)] private float soundEffectPitchVariance; // ȿ���� ��ġ ������
    [SerializeField][Range(0f, 1f)] private float musicVolume; // ��� ���� ����

    private AudioSource musicAudioSource; // ��� ���ǿ� AudioSource
    public AudioClip musicClip; // �⺻ ��� ���� Ŭ��

    private void Awake()
    {
        instance = this;

        // ����� ����� AudioSource ����
        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.volume = musicVolume;
        musicAudioSource.loop = true;
    }

    private void Start()
    {
        // �⺻ ��� ���� ����
        ChangeBackGroundMusic(musicClip);
    }

    // ��� ������ �ٸ� Ŭ������ ��ü�ϴ� �Լ�
    public void ChangeBackGroundMusic(AudioClip clip)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }
}