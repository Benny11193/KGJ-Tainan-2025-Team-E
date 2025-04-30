using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanmakuBuilder : MonoBehaviour
{
    public RectTransform parent;
    public GameObject dmk_r2l;
    public GameObject dmk_l2r;

    [SerializeField] List<string> quotes_right;

    private string[] quotes_left;

    public void StartBuild()
    {
        StartCoroutine(builder());   
    }

    void build_right(){
        GameObject dmk = Instantiate(dmk_r2l, Vector3.zero, Quaternion.identity, parent);
        dmk.GetComponent<Text>().text = "";
    }

    void build_left(){
        GameObject dmk = Instantiate(dmk_r2l, Vector3.zero, Quaternion.identity, parent);
        dmk.GetComponent<Text>().text = "";
    }
    

    IEnumerator builder(){
        while(true){
            yield return new WaitForSeconds(1);
        }
    }
}
