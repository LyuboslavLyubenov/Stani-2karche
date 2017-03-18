namespace Controllers
{

    using UnityEngine;
    using UnityEngine.UI;

    public class EndGameUIController : MonoBehaviour
    {
        public void SetMark(int mark)
        {
            var endScreenMark = this.transform.GetChild(0).GetChild(1).GetComponent<Text>();
            endScreenMark.text = mark.ToString();
        }
    }

}
