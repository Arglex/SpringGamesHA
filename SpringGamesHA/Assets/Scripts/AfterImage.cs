using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    
    private MeshRenderer[] _meshRenderers;
    private bool _isDashing = false;

    private void Start()
    {
        _profile.TryGet(out _chromaticAberration);
        _chromaticAberration.intensity.Override(0);
    }


    public void StartAfterImageEffect()
    {
        DOVirtual.Float(0, 360, _activateTime * 2, angle => {
            _chromaticAberration.intensity.Override(Mathf.Sin(angle * Mathf.Deg2Rad));
        });
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
