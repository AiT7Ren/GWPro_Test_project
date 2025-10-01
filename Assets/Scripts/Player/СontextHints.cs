using TMPro;
using UnityEngine;

public class СontextHints : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI _text;
    [SerializeField]private СontextHintsData _hintsDate;
    
    public void SetNewHint(IteractibleType? key)
    {
        if (key == null)
        {
            _text.gameObject.SetActive(false);
            return;
        }
        _text.text = SetHitsInText(key.Value);
    }
    private string SetHitsInText(IteractibleType hit)
    {
        foreach (var h in _hintsDate.hits)
        {
            _text.gameObject.SetActive(true);
            if (h.type == hit) return h.description;
        }
        _text.gameObject.SetActive(false);
        return string.Empty;
    }
    
}
