using UnityEngine;

namespace Game.UI
{
    public class CommonMutexComp : MonoBehaviour
    {
        public GameObject mutexRoot;

        public void CheckSetMutex(bool thisActive)
        {
            if (mutexRoot != null)
            {
                if (this.gameObject.activeSelf)
                {
                    mutexRoot.SetActive(thisActive);
                }
            }
        }
        void OnEnable()
        {
            if (mutexRoot != null)
            {
                mutexRoot.SetActive(false);
            }
        }
        void OnDisable()
        {
            if (mutexRoot != null)
            {
                mutexRoot.SetActive(true);
            }
        }
    }
}