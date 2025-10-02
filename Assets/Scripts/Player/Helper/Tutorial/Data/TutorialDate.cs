using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TutorialDate", menuName = "Date/Tutorial")]
public class TutorialDate : ScriptableObject
{
    
    [TextArea(2, 5)]
    public List<string> tutorialStep;
}