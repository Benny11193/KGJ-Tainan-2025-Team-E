using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public Sprite fighting_sp;
    static float Timer = 180f;
    [SerializeField] RectTransform TimeUI;

    public int money;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
        money = Load();
    }

    void Save(){
        int higt_score =  PlayerPrefs.GetInt("HighScore", 0);
        if(money > higt_score){
            PlayerPrefs.SetInt("HighScore", money);
        }
    }

    public int Load(){
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    public void RunTimer()
    {
        StartCoroutine(Time_enum());
    }

    IEnumerator Time_enum()
    {
        float endTime = Time.time + Timer;
        Vector3 rotation = TimeUI.localEulerAngles;
        float zAngle = 0f;

        while (endTime > Time.time)
        {
            zAngle -= Time.deltaTime;
            rotation.z = zAngle;
            TimeUI.localEulerAngles = rotation;
            yield return null;
        }

        Save();
        SceneManager.LoadScene("menu", LoadSceneMode.Single);
    }

}
