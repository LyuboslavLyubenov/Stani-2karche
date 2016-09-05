using UnityEngine;
using System.Collections;

public class TESTSERVERLOADQUESTIONS : MonoBehaviour
{
    public LocalGameData GameData;

    void Start()
    {
        GameData.LoadDataAsync();
    }
}
