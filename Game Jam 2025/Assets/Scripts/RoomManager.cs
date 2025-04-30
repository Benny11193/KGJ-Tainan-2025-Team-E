using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Playables;

public enum HitoTypes
{
    家裡蹲,
    上班族,
    胖妞,
    跨性別,
    網紅,
    肥宅,
    普通人
}

[System.Serializable]
public class Statement
{
    public string FullStatement = "";
    public List<HitoTypes> AffectHitoType = new List<HitoTypes>();

    public Statement(string fullStatement, List<HitoTypes> affectHitoType)
    {
        FullStatement = fullStatement;
        AffectHitoType = affectHitoType;
    }
}

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public List<Hito> Crowd = new List<Hito>();

    public static float maxTimeLimit = 4f;
    float timeLimit;
    [SerializeField] Image TimeLimit_img;
    [SerializeField] GameObject MaskOnButtons;

    float blameValue = 3f;
    Coroutine blameCoroutine;

    Dictionary<string, Statement> BlameStringDict = new Dictionary<string, Statement>();
    [SerializeField] TextMeshProUGUI BlameText_1;
    [SerializeField] TextMeshProUGUI BlameText_2;
    [SerializeField] TextMeshProUGUI BlameText_3;
    [SerializeField] TextMeshProUGUI BlameText_4;

    public int Money {get => Manager.Instance.money;} 
    [SerializeField] TextMeshProUGUI MoneyText;

    [SerializeField] PlayableDirector director;
    [SerializeField] TextMeshProUGUI BigBlameText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Manager.Instance.RunTimer();
        LoadStatements();
        SetRandomBlameTexts();
        timeLimit = maxTimeLimit;
        // Manager.Instance.money = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(blameCoroutine == null)
        {
            timeLimit -= Time.deltaTime;
            if(timeLimit <= 0)
            {
                SetRandomBlameTexts();
                timeLimit = maxTimeLimit;
            }
            else
            {
                timeLimit = Mathf.Clamp(timeLimit, 0, maxTimeLimit);
            }
        }

        TimeLimit_img.fillAmount = timeLimit/maxTimeLimit;
        MoneyText.text = "$" + Money;
    }

    // Blame Button
    public void Blame(int index)
    {
        if (blameCoroutine != null) return;

        string blameKey = "";
        switch (index)
        {
            case 1:
                blameKey = BlameText_1.text;
                break;
            case 2:
                blameKey = BlameText_2.text;
                break;
            case 3:
                blameKey = BlameText_3.text;
                break;
            case 4:
                blameKey = BlameText_4.text;
                break;
            default:
                break;
        }

        timeLimit = maxTimeLimit;

        if (BlameStringDict.TryGetValue(blameKey, out Statement statement))
        {
            blameCoroutine = StartCoroutine(Blame_enum(statement));
        }
        else
        {
            Debug.LogWarning($"Blame Key '{blameKey}' not found in BlameStringDict!");
        }

        director.Play();
        BigBlameText.text = blameKey;

        int i = Random.Range(0, 2);
        if(i == 0)
        {
            var rt = BigBlameText.rectTransform;
            Vector2 pos = rt.anchoredPosition;
            pos.x = 240f;
            rt.anchoredPosition = pos;

            Vector3 rotation = rt.localEulerAngles;
            rotation.z = -10f;
            rt.localEulerAngles = rotation;
        }
        else
        {
            var rt = BigBlameText.rectTransform;
            Vector2 pos = rt.anchoredPosition;
            pos.x = -240f;
            rt.anchoredPosition = pos;

             Vector3 rotation = rt.localEulerAngles;
            rotation.z = 7f;
            rt.localEulerAngles = rotation;
        }
    }

    IEnumerator Blame_enum(Statement statement)
    {
        MaskOnButtons.SetActive(true);
        List<Hito> matchedHitos = new List<Hito>();

        // Find all Hito who match at least one of the statement's HitoTypes
        foreach (Hito hito in Crowd)
        {
            foreach (HitoTypes affectType in statement.AffectHitoType)
            {
                if (hito.myTypes.Contains(affectType))
                {
                    matchedHitos.Add(hito);
                    break;
                }
            }
        }

        while (matchedHitos.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);
            int randomIndex = Random.Range(0, matchedHitos.Count);
            matchedHitos[randomIndex].GetBadTemper(Random.Range(blameValue-1, blameValue+1));
            matchedHitos.RemoveAt(randomIndex);
        }

        MaskOnButtons.SetActive(false);
        blameCoroutine = null;
        SetRandomBlameTexts();
    }

    public List<T> GetRandomElements<T>(List<T> sourceList, int count)
    {
        if (count > sourceList.Count) count = sourceList.Count;

        List<T> tempList = new List<T>(sourceList);
        List<T> resultList = new List<T>();

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, tempList.Count);
            resultList.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        return resultList;
    }

    public void SetRandomBlameTexts()
    {
        List<string> allShortContents = new List<string>(BlameStringDict.Keys);

        if (allShortContents.Count < 4)
        {
            Debug.LogWarning("Not enough blame contents to fill 4 texts.");
            return;
        }

        // Randomly pick 4 different short contents
        List<string> randomPicks = GetRandomElements(allShortContents, 4);

        BlameText_1.text = randomPicks[0];
        BlameText_2.text = randomPicks[1];
        BlameText_3.text = randomPicks[2];
        BlameText_4.text = randomPicks[3];
    }

    void LoadStatements()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("批判內容");
        if (csvFile == null)
        {
            Debug.LogError("CSV not found");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line.Split(',');

            if (columns.Length < 4) continue;

            string shortStatement = columns[1].Trim();   // 批判內容（簡略版）
            string fullStatement = columns[2].Trim();    // 批判內容（詳細版）
            string hitoRaw = columns[3].Trim();           // 受影響的人物（對應特性）

            List<HitoTypes> hitoTypes = ParseHitoTypes(hitoRaw);

            Statement statement = new Statement(fullStatement, hitoTypes);

            if (!BlameStringDict.ContainsKey(shortStatement))
            {
                BlameStringDict.Add(shortStatement, statement);
            }
            else
            {
                Debug.LogWarning($"Duplicate short statement key: {shortStatement}");
            }
        }
    }

    List<HitoTypes> ParseHitoTypes(string raw)
    {
        List<HitoTypes> results = new List<HitoTypes>();

        MatchCollection matches = Regex.Matches(raw, "（(.*?)）");

        foreach (Match match in matches)
        {
            string trait = match.Groups[1].Value.Trim();

            if (System.Enum.TryParse<HitoTypes>(trait, out var hito))
            {
                results.Add(hito);
            }
            else
            {
                Debug.LogWarning($"Unknown HitoType: {trait}");
            }
        }

        return results;
    }

    public static void ToBattleScene(){
        SceneManager.LoadScene("scene_2", LoadSceneMode.Single);
    }
}