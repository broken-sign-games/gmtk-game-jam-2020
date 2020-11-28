using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    [Serializable]
    public class Tutorial
    {
        public TutorialID ID;

        [TextArea]
        public string Message;
        public bool ShowDismissButton;
        public List<Tool> InteractableTools;
        public bool PlaybackButtonAvailable;
        public List<GridRect> InteractableRects;
    }
}