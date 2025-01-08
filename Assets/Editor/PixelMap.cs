using Common;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteImporter
{
    public class PixelMap : ScriptableObject
    {
        public SerializedDictionary<Color32, Vector2Int> lookup = new();
        //public Dictionary<Color32, Vector2Int> lookup = new();
        public Color32[] data;
    }
}