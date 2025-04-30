using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Fight : MonoBehaviour
{

    public AudioClip bgm;
    private bool isFighting;
    private KeyCode targetKey;
    public Text tip;

    public Material DiagonalMask;

    private KeyCode[] targetKeys = new KeyCode[]
    {
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow
    };
    public Image chara_sp;

    public float value = 50;
    public float add_value = 10;
    public float reduce_value = 1;
    public float clear_value = 100;//過關需要的分數

    private Fight fight;

    private Coroutine ChangeTargetKey_Coroutine;



    void Awake()
    {
        fight = this.GetComponent<Fight>();
    }

    void Start()
    {
        AudioManager.Instance.PlayBGM(bgm);
        chara_sp.sprite = Manager.Instance.fighting_sp;
    }

    public void StartFight()
    {
        isFighting = true;
        RandomTargetKey();
        ChangeTargetKey_Coroutine = StartCoroutine(ChangeTargetKey());
        
    }

    void Update()
    {
        if(isFighting && Input.GetKeyDown(targetKey)){
            value += add_value;
        }

        //UI 更新
        DiagonalMask.SetFloat("_Progress", 1 - value/100);
    }

    void FixedUpdate()
    {
        if(!isFighting){
            return;
        }
        value -= reduce_value;

        if(value <= 0){
            stopFight();
        }
        if(value >= clear_value){
            stopFight();
            Manager.Instance.money += 1000;
        }
        
    }

    void stopFight(){
        fight.enabled = false;
        isFighting = false;
        StopCoroutine(ChangeTargetKey_Coroutine);
        Manager.Instance.fighting_sp = null;
        backToScene();
    }

    IEnumerator ChangeTargetKey(){
        while(isFighting){
            yield return new WaitForSeconds(3);
            RandomTargetKey();
        }
    }

    void RandomTargetKey(){
        targetKey = targetKeys[Random.Range(0, targetKeys.Length)];
            switch(targetKey){
                case KeyCode.UpArrow:
                    tip.text = "up";
                    break;
                case KeyCode.DownArrow:
                    tip.text = "down";
                    break;
                case KeyCode.LeftArrow:
                    tip.text = "left";
                    break;
                case KeyCode.RightArrow:
                    tip.text = "right";
                    break;
            }
    }


    public void backToScene(){
        SceneManager.LoadScene("scene_1");
    }
}
