using UnityEngine;

namespace Assets.Scripts.Utils
{

    public abstract class ExtendedMonoBehaviour : MonoBehaviour
    {
        CoroutineUtils coroutineUtils;

        public CoroutineUtils CoroutineUtils
        {
            get
            {
                if (this.coroutineUtils == null)
                {
                    this.coroutineUtils = new CoroutineUtils(this);
                }

                return this.coroutineUtils;
            }
        }
    }

}
