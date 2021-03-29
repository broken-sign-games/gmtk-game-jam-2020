using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool HighlightSpikeBalls;
        public List<GridRect> InteractableRects;

        public Tutorial()
        {
        }

        public Tutorial(Tutorial other)
        {
            ID = other.ID;
            Message = other.Message;
            ShowDismissButton = other.ShowDismissButton;
            InteractableTools = other.InteractableTools.ToList();
            PlaybackButtonAvailable = other.PlaybackButtonAvailable;
            HighlightSpikeBalls = other.HighlightSpikeBalls;
            InteractableRects = other.InteractableRects.ToList();
        }
    }
}