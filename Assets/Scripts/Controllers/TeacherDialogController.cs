using UnityEngine.UI;

public class TeacherDialogController : ExtendedMonoBehaviour
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
        CoroutineUtils.WaitForFrames(0, () => _SetMessage(message));
    }

    void _SetMessage(string message)
    {
        CoroutineUtils.WaitUntil(() => initialized, () => text.text = message);
    }
}
