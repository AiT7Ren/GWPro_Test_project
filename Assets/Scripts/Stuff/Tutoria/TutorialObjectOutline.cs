using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectOutline : MonoBehaviour
{
    [Header("TimeToChange")] [field: SerializeField] [field: Range(0f, 10f)]
    private float _timeToChangeSpeed;

    #region MaterialPhaseClass
    [Serializable]
    public class MaterialPhase
    {
        public Color PhaseBaseColor;
        public float PhaseOutlinePower;
        public float PhaseOutlineEmmision;
        public MaterialPhase(Color phaseBaseColor, float phaseOutlinePower, float phaseOutlineEmmision)
        {
            PhaseBaseColor = phaseBaseColor;
            PhaseOutlinePower = phaseOutlinePower;
            PhaseOutlineEmmision = phaseOutlineEmmision;
        }
    }
    #endregion

    [SerializeField] private List<Transform> _multyObject;
    [Header("MaterialPhase")] public List<MaterialPhase> MaterialPhases;
    private Coroutine _buttonAnimationCoroutine;
    private Material _material;
    private Material[] _multyMaterials;

    private void OnEnable()
    {
        if (_multyObject.Count>0)
        {
            _multyMaterials=new Material[_multyObject.Count];
            for (int i = 0; i < _multyObject.Count; i++)
            {
                _multyMaterials[i] = _multyObject[i].gameObject.GetComponent<Renderer>().material;
                if(_multyMaterials[i]==null)throw new Exception($"no multi Material  found in{gameObject.name}");
            }
            _buttonAnimationCoroutine = StartCoroutine(ButtonAnimation());
            return;
        }
        if (_material == null)
            if (TryGetComponent(out MeshRenderer buttonRenderer))
                _material = buttonRenderer.material;
            else throw new Exception($"no Material found in{gameObject.name}");
        if (MaterialPhases.Count < 2 || MaterialPhases == null) BuildMaterialPhases();
        if (_timeToChangeSpeed == 0 || _material == null) return;
        _buttonAnimationCoroutine = StartCoroutine(ButtonAnimation());
    }

    private void BuildMaterialPhases()
    {
        MaterialPhases=new List<MaterialPhase>();
        MaterialPhases.Add(new MaterialPhase(_material.color, 0.04f, 10));
        MaterialPhases.Add(new MaterialPhase(Color.red, 0f, 0f));
        
    }
    
    private IEnumerator ButtonAnimation()
    {
        var timer = 0f;
        var toHighlight = true;

        var (startColor, startPower, startEmmissin) = SetMaterial(0);
        var (endColor, endPower, endEmmissin) = SetMaterial(1);
        while (true)
        {
            timer += Time.deltaTime;
            var progress = timer / _timeToChangeSpeed;
            if (progress >= 1f)
            {
                toHighlight = !toHighlight;
                timer = 0f;
                progress = 0f;
                (startColor, startPower, startEmmissin) = toHighlight ? SetMaterial(1) : SetMaterial(0);
                (endColor, endPower, endEmmissin) = toHighlight ? SetMaterial(0) : SetMaterial(1);
            }

            var smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            var currentColor = Color.Lerp(startColor, endColor, smoothProgress);
            var outlinePower = Mathf.Lerp(startPower, endPower, smoothProgress);
            var outlineEmmision = Mathf.Lerp(startEmmissin, endEmmissin, smoothProgress);

            if(_multyMaterials==null)SetColor(_material,currentColor, outlinePower, outlineEmmision);
            else
            {
                for (int i = 0; i < _multyMaterials.Length; i++)
                {
                    SetColor(_multyMaterials[i],currentColor, outlinePower, outlineEmmision);
                }
            }
            yield return null;
        }
    }

    private void OnDisable()
    {
        StopCoroutine(_buttonAnimationCoroutine);
    }

    [ContextMenu("Check Phase One")]
    public void CheckPhaseOne()
    {
        CheckPhase(0);
    }

    [ContextMenu("Check Phase Two")]
    public void CheckPhaseTwo()
    {
        CheckPhase(1);
    }
    private void CheckPhase(int i)
    {
        var (color, power, emmissin) = SetMaterial(i);
        SetColor(_material,color, power, emmissin);
    }
    
    private void SetColor(Material material,Color color, float outlineEmmision, float outlinePower)
    {
        if (material == null) throw new Exception("Material is null");
        material.SetColor("_Color", color);
        material.SetFloat("_OutlineIntensity", outlineEmmision);
        material.SetFloat("_OutlineWidth", outlinePower);
    }
    

    private (Color outlineColor, float outlineEmmision, float outlinePower) SetMaterial(int i)
    {
        var phase = MaterialPhases[i];
        return (phase.PhaseBaseColor, phase.PhaseOutlineEmmision, phase.PhaseOutlinePower);
    }
}