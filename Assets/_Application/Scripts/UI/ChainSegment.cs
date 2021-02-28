using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class ChainSegment : MonoBehaviour
    {
        [SerializeField] private Image wideLink = null;
        [SerializeField] private Sprite wideLinkShadowedSprite = null;

        public void AddShadow()
        {
            wideLink.sprite = wideLinkShadowedSprite;
        }
    } 
}
