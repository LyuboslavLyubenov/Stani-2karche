using UnityEngine.UI;

public class DialogController : ExtendedMonoBehaviour
{
    Text text;

    bool initialized = false;

    void Start()
    {
        CoroutineUtils.WaitForSeconds(1, Initialize);
    }

    void Initialize()
    {
        text = GetComponentInChildren<Text>();
        initialized = true;
    }

    public void SetMessage(string message)
    {
        CoroutineUtils.WaitUntil(() => initialized, () => text.text = message);
    }
}
