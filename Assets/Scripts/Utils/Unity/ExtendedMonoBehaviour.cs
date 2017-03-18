namespace Utils.Unity
{

    using UnityEngine;

    public abstract class ExtendedMonoBehaviour : MonoBehaviour
    {
        private CoroutineUtils coroutineUtils;

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
