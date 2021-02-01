using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GMTK2020.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TwitterLink : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string twitterHandle = "";

        public void OnPointerClick(PointerEventData eventData)
        {
            Application.OpenURL($"https://twitter.com/{twitterHandle}");
        }

        private void Awake()
        {
            UpdateLabel();
        }

        private void OnValidate()
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var text = GetComponent<TextMeshProUGUI>();
            text.text = $"@{twitterHandle}";
        }
    }
}
