using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Utils
{

    public class LoadScene : MonoBehaviour
    {
        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }
    }

}
