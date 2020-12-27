using GMTK2020.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GMTK2020.UI
{
    public class SliderReleased : MonoBehaviour, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData _)
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
        }
    } 
}
