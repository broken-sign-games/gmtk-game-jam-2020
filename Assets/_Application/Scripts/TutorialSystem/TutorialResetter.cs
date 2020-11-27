using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.TutorialSystem
{
    [RequireComponent(typeof(TutorialManager))]
    public class TutorialResetter : MonoBehaviour
    {
        private TutorialManager tutorialManager;

        private void Awake()
        {
            tutorialManager = GetComponent<TutorialManager>();
        }
    } 
}
