using System;
using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class CrackCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI crackDisplay = null;

        private int maxCracks;
        private int avoidedCracks;

        public void SetMaxCracks(int maxCracks)
        {
            this.maxCracks = maxCracks;

            UpdateUI();
        }

        public void SetAvoidedCracks(int avoidedCracks)
        {
            this.avoidedCracks = Math.Min(avoidedCracks, maxCracks);

            UpdateUI();
        }

        private void UpdateUI()
        {
            crackDisplay.text = new string('0', avoidedCracks) + new string('O', maxCracks - avoidedCracks);
        }
    } 
}
