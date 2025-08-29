using UnityEngine;
using UnityEngine.Events;

public class Keypad : MonoBehaviour
{
    public string password = "1234";
    public AudioClip clickSound;
    public AudioClip openSound; // ������
    public AudioClip noSound;   // ������

    public UnityEvent OnEntryAllowed;
    public UnityEvent onCorrect;
    public UnityEvent onIncorrect;

    public GameObject Obstacle;

    AudioSource audioSource;
    string userInput = "";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        userInput = string.Empty;
    }

    public void ButtonClicked(string number)
    {
        // Ű ���� �� Ŭ����
        if (audioSource && clickSound) audioSource.PlayOneShot(clickSound);

        // �ʰ� �Է� ����
        if (userInput.Length >= password.Length) return;

        userInput += number;

        // ���̰� ��й�ȣ ���̿� �������� ���� �����Ѵ�
        if (userInput.Length == password.Length)
        {
            bool ok = string.Equals(userInput, password, System.StringComparison.Ordinal);
            if (ok)
            {
                Debug.Log("Entry Allowed");
                if (audioSource && openSound) audioSource.PlayOneShot(openSound);
                onCorrect?.Invoke();
                ClearPassword();

            }
            else
            {
                Debug.Log("Not this time");
                if (audioSource && noSound) audioSource.PlayOneShot(noSound);
                onIncorrect?.Invoke();
            }

            userInput = string.Empty; // ���� �Է��� ���� �ʱ�ȭ
        }
    }

    void ClearPassword()
    {
        Destroy(Obstacle);
    }
   
}
