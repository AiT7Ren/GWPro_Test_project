using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitsDate", menuName = "Date/Hits")]
public class СontextHintsDate : ScriptableObject
{
    [System.Serializable]
    public class Hits
    {
        public IteractibleType type;
        [TextArea(2, 5)]
        public string description;
    }
   public List<Hits> hits;
}