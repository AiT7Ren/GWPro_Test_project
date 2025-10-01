using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitsDate", menuName = "Date/Hits")]
public class СontextHintsData : ScriptableObject
{
    [System.Serializable]
    public class Hits
    {
        public IteractibleType type;
        public string description;
    }
   public List<Hits> hits;
}