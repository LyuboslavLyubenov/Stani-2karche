namespace Assets.UITween.Prefabs.Click_Effect_Pref
{

    using System.Collections;
    using System.Collections.Generic;

    using Assets.UITween.Scripts;

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ClickedWaveAnimation : MonoBehaviour {

        public GameObject WaveObject;
        public GameObject CanvasMain;

        public int PoolSize;

        private Pool poolClass;

        void Start()
        {
            this.poolClass = this.gameObject.AddComponent<Pool>();
            this.poolClass.CreatePool(this.WaveObject, this.PoolSize);
        }

        void Update () 
        {
            if (Input.GetMouseButtonDown(0) 
#if UNITY_EDITOR
                || Input.GetMouseButtonDown(1) 
#endif
                )
            {
                GameObject hittedUIButton = this.UiHitted();

                if (hittedUIButton)
                {
                    this.CreateWave(hittedUIButton.transform);
                }
            }
        }

        void CreateWave(Transform Parent)
        {
            GameObject wave = this.poolClass.GetObject();

            if (wave)
            {
                wave.transform.SetParent( this.CanvasMain.transform );
                wave.GetComponent<MaskableGraphic>().color = Parent.GetComponent<MaskableGraphic>().color - new Color(.1f, .1f, .1f);

                Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

                mousePos.x = mousePos.x * Screen.width - Screen.width / 2f;
                mousePos.y = mousePos.y * Screen.height - Screen.height / 2f;
                mousePos.z = 0f;

                wave.GetComponent<RectTransform>().localPosition = mousePos / this.CanvasMain.transform.localScale.x;
                wave.transform.SetParent( Parent );
                wave.GetComponent<EasyTween>().OpenCloseObjectAnimation();
            }
        }

        public GameObject UiHitted()
        {
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position =  Input.mousePosition;
		
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll( pe, hits );

            for (int i = 0; i < hits.Count ; i++)
            {
                if (hits[i].gameObject.GetComponent<Button>() && hits[i].gameObject.GetComponent<Mask>())
                {
                    return hits[i].gameObject;
                }
            }

            return null;
        }
    }

    public class Pool : MonoBehaviour {

        private GameObject[] ObjectPool;
        private GameObject ObjectToPool;

        public void CreatePool(GameObject ObjectToPool, int numberOfObjects)
        {
            this.ObjectPool = new GameObject[numberOfObjects];
            this.ObjectToPool = ObjectToPool;

            for (int i = 0; i < this.ObjectPool.Length; i++)
            {
                this.ObjectPool[i] = Instantiate(ObjectToPool) as GameObject;
                this.ObjectPool[i].SetActive(false);
            }
        }

        public GameObject GetObject()
        {
            for (int i = 0; i < this.ObjectPool.Length; i++)
            {
                if (this.ObjectPool[i])
                {
                    if (!this.ObjectPool[i].activeSelf)
                    {
                        this.ObjectPool[i].SetActive(true);
                        return this.ObjectPool[i];
                    }
                }
                else
                {
                    this.ObjectPool[i] = Instantiate(this.ObjectToPool) as GameObject;
                    this.ObjectPool[i].SetActive(false);
                }
            }

            return null;
        }
    }

}