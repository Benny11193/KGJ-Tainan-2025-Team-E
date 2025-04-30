using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hito : MonoBehaviour
{
    public List<HitoTypes> myTypes = new List<HitoTypes>();
    public static Hito NoFearHito;
    static float MaxBadTemper = 10f;
    float badTemper;
    static float angerTime = 4f;
    
    [SerializeField] List<string> ReactStringList = new List<string>();
    [SerializeField] SpriteRenderer HitoSpriteRenderer;
    [SerializeField] Sprite[] HitoSprites = new Sprite[4];
    [SerializeField] TextMeshProUGUI React;
    RoomManager roomManager {get => RoomManager.Instance;}
    Coroutine angerCoroutine;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if(NoFearHito == null) 
        {
            int i = Random.Range(0, 2);
            if (i == 1) NoFearHito = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        roomManager.Crowd.Add(this);
        badTemper = 0f;
        React.text = "";
        React.fontSize = 0.4f;
        React.color = Color.white;
        myTypes = GetRandomHitoTypes(1, 2);
        HitoSprites = GetHitoSprites(myTypes[0]);
        HitoSpriteRenderer.sprite = HitoSprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(badTemper >= MaxBadTemper)
        {
            if (angerCoroutine == null) angerCoroutine = StartCoroutine(Anger_enum());

            if(React.text == "") React.text = ReactStringList[Random.Range(0, ReactStringList.Count)];
            animator.Play("InAnger");
        }
        else
        {
            if(React.text != "") React.text = "";
        }

        SetFace();
    }

    public void GetBadTemper(float value)
    {
        badTemper += value;
    }

    IEnumerator Anger_enum()
    {
        float duration = (NoFearHito != this) ? angerTime : angerTime * 3;
        float endTime = Time.time + duration;
        while(Time.time < endTime)
        {
            if(NoFearHito != this)
            {
                if(badTemper > MaxBadTemper + 3)
                {
                    Manager.Instance.money += 100;
                    animator.Play("Pay");
                    badTemper = 0f;
                    React.text = "";
                    angerCoroutine = null;
                    yield break;
                }
            }
            else
            {
                if(Time.time > endTime - 0.5f)
                {
                    React.text = "你是在大聲什麼啦";
                    React.fontSize = 0.6f;
                    if (ColorUtility.TryParseHtmlString("#EC4D4D", out Color newColor))
                    {
                        React.color = newColor;
                    }
                }
            }

            yield return null;
        }

        if(NoFearHito != this)
        {
            animator.Play("Idle");
            badTemper = 0f;
            React.text = "";
            angerCoroutine = null;
        }
        else
        {
            RoomManager.ToBattleScene();
            Manager.Instance.fighting_sp = HitoSprites[3];
        }
    }

    List<HitoTypes> GetRandomHitoTypes(int minCount, int maxCount)
    {
        List<HitoTypes> copy = new List<HitoTypes>((HitoTypes[])System.Enum.GetValues(typeof(HitoTypes)));
        List<HitoTypes> result = new List<HitoTypes>();

        int pickCount = Random.Range(minCount, maxCount + 1);
        pickCount = Mathf.Min(pickCount, copy.Count);

        for (int i = 0; i < pickCount; i++)
        {
            int randomIndex = Random.Range(0, copy.Count);
            result.Add(copy[randomIndex]);
            copy.RemoveAt(randomIndex);
        }

        return result;
    }

    Sprite[] GetHitoSprites(HitoTypes hitoType)
    {
        Sprite[] sprites = new Sprite[4];

        switch(hitoType)
        {
            case HitoTypes.家裡蹲:
                sprites[0] = Resources.Load<Sprite>("Characters/k1-1");
                sprites[1] = Resources.Load<Sprite>("Characters/k1-2");
                sprites[2] = Resources.Load<Sprite>("Characters/k1-3");
                sprites[3] = Resources.Load<Sprite>("Characters/k1-sp");
                break;
            case HitoTypes.上班族:
                sprites[0] = Resources.Load<Sprite>("Characters/k2-1");
                sprites[1] = Resources.Load<Sprite>("Characters/k2-2");
                sprites[2] = Resources.Load<Sprite>("Characters/k2-3");
                sprites[3] = Resources.Load<Sprite>("Characters/k2-sp");
                break;
            case HitoTypes.胖妞:
                sprites[0] = Resources.Load<Sprite>("Characters/k3-1");
                sprites[1] = Resources.Load<Sprite>("Characters/k3-2");
                sprites[2] = Resources.Load<Sprite>("Characters/k3-3");
                sprites[3] = Resources.Load<Sprite>("Characters/k3-sp");
                break;
            case HitoTypes.跨性別:
                sprites[0] = Resources.Load<Sprite>("Characters/k4-1");
                sprites[1] = Resources.Load<Sprite>("Characters/k4-2");
                sprites[2] = Resources.Load<Sprite>("Characters/k4-3");
                sprites[3] = Resources.Load<Sprite>("Characters/k4-sp");
                break;
            case HitoTypes.網紅:
                sprites[0] = Resources.Load<Sprite>("Characters/k5-1");
                sprites[1] = Resources.Load<Sprite>("Characters/k5-2");
                sprites[2] = Resources.Load<Sprite>("Characters/k5-3");
                sprites[3] = Resources.Load<Sprite>("Characters/k5-sp");
                break;
            case HitoTypes.肥宅:
                sprites[0] = Resources.Load<Sprite>("Characters/k6-1");
                sprites[1] = Resources.Load<Sprite>("Characters/k6-2");
                sprites[2] = Resources.Load<Sprite>("Characters/k6-3");
                sprites[3] = Resources.Load<Sprite>("Characters/k6-sp");
                break;
            case HitoTypes.普通人:
                sprites[0] = Resources.Load<Sprite>("Characters/k7-1");
                sprites[1] = Resources.Load<Sprite>("Characters/k7-2");
                sprites[2] = Resources.Load<Sprite>("Characters/k7-3");
                sprites[3] = Resources.Load<Sprite>("Characters/k7-sp");
                break;
            default:
                break;
        }

        return sprites;
    }

    void SetFace()
    {
        switch (badTemper)
        {
            case var temp when temp < 4:
                HitoSpriteRenderer.sprite = HitoSprites[0];
                break;

            case var temp when temp < 9:
                HitoSpriteRenderer.sprite = HitoSprites[1];
                break;

            default:
                HitoSpriteRenderer.sprite = HitoSprites[2];
                break;
        }
    }
}
