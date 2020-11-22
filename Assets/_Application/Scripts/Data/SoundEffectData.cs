using GMTK2020.Audio;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class SoundEffectData : ScriptableObject
    {
        [Serializable]
        public class AudioClipArray
        {
            public AudioClip[] Clips = new AudioClip[0];
        }

        [Serializable]
        public class SoundEffectDict : SerializableDictionaryBase<SoundEffect, AudioClipArray>
        { }

        public SoundEffectDict Map;

        public SoundEffectData()
        {
            Map = new SoundEffectDict();

            foreach (var type in Utility.GetEnumValues<SoundEffect>())
                Map[type] = new AudioClipArray();
        }
    }
}