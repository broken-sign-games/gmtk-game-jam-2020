using DG.Tweening;
using UnityEngine;

namespace GMTK2020
{
    public class DOTweenInitializer : MonoBehaviour
    {
        private void Start()
        {
            DOTween.Init();
            Destroy(gameObject);
        }
    }
}