using UnityEngine;
using UnityEngine.SceneManagement;

public class TESTServerDiscoverySceneChanger : MonoBehaviour
{
    public void OpenServer()
    {
        SceneManager.LoadScene("TESTServerDiscoveryServer");
    }

    public void OpenClient()
    {
        SceneManager.LoadScene("TESTServerDiscoveryClient");
    }
}
