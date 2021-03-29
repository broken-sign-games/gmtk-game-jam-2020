using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class SpikeBall : MonoBehaviour
    {
        [SerializeField] private Image mask;

        public void EnableMask()
        {
            mask.enabled = true;
        }

        public void DisableMask()
        {
            mask.enabled = false;
        }
    } 
}
