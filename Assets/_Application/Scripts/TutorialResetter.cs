using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020
{
    [RequireComponent(typeof(TutorialSystem))]
    public class TutorialResetter : MonoBehaviour
    {
        private TutorialSystem tutorialSystem;

        private void Awake()
        {
            tutorialSystem = GetComponent<TutorialSystem>();
        }
    } 
}
