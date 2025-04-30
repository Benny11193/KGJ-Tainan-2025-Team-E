using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource bgm;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        bgm = GetComponent<AudioSource>();   
    }

    public void PlayBGM(AudioClip clip){
        bgm.clip = clip;
        bgm.Play();
    }

    public void PlaySE(AudioClip clip){
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

}
