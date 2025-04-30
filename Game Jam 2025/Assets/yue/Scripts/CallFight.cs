using UnityEngine;
public class CallFight : MonoBehaviour
{
    public Fight fight;
    void call_fight()
    {
        fight.StartFight();
    }
}
