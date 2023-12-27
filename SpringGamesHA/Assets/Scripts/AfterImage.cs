using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class AfterImage : MonoBehaviour
{
    [SerializeField] private float _activateTime = 2f;
    [SerializeField] private float _meshRefreshRate = 0.1f;
    [SerializeField] private Material _material;
    [SerializeField] private float _meshDestroyTimer = 1f;
    [SerializeField] private string _shaderVarRef;
    [SerializeField] private float _shaderVarRate = 0.1f;
    [SerializeField] private float _shaderVarRefreshRate = 0.5f;
    [SerializeField] private VolumeProfile _profile;
    [SerializeField] private ChromaticAberration _chromaticAberration;
    
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _cameraEffectDuration = 1f;
    [SerializeField] private float _startingFOV = 60f;
    [SerializeField] private float _dashFOV = 90f;
    [SerializeField] private float _chromaticMax = 1f;
    [SerializeField] private AnimationCurve _cameraFOVCurve;
    
    private MeshRenderer[] _meshRenderers;

    private void Awake()
    {
        _virtualCamera.m_Lens.FieldOfView = _startingFOV;
    }

    private void Start()
    {
        _profile.TryGet(out _chromaticAberration);
        _chromaticAberration.intensity.Override(0);
    }


    public void StartAfterImageEffect()
    {
        DOTween.To(() => _virtualCamera.m_Lens.FieldOfView, x => _virtualCamera.m_Lens.FieldOfView = x, 
            _dashFOV, _cameraEffectDuration).SetEase(_cameraFOVCurve);
        DOTween.To(() => _chromaticAberration.intensity.value, x => _chromaticAberration.intensity.value = x,
            _chromaticMax, _cameraEffectDuration).SetEase(_cameraFOVCurve);
        
        StartCoroutine(ActivateAfterImage(_activateTime));
    }

    IEnumerator ActivateAfterImage(float timeActivated)
    {
        while (timeActivated > 0)
        {
            
            timeActivated -= _meshRefreshRate;
            if (_meshRenderers == null)
            {
                _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            }

            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                GameObject newObject = new GameObject();
                MeshRenderer newMeshRenderer = newObject.AddComponent<MeshRenderer>();
                MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
                newMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                newMeshFilter.mesh = _meshRenderers[i]?.GetComponent<MeshFilter>()?.mesh;

                newMeshRenderer.sharedMaterial = _material;
                newObject.transform.SetPositionAndRotation(transform.position, _meshRenderers[i].transform.rotation);

                StartCoroutine(AnimateMaterialFloat(newMeshRenderer.material, 0, _shaderVarRate, _shaderVarRefreshRate));
                Destroy(newObject, _meshDestroyTimer);
            }
            
            yield return new WaitForSeconds(_meshRefreshRate);
        }
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(_shaderVarRef);
        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(_shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
