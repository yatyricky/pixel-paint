using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LerpZoom : MonoBehaviour
{
    public float TargetSize = 40f;
    public float TransitionTime = 3;

    private float _currentSize;
    private Camera _camera;

    void Start()
    {
        //could also use Camera.main if apropriate
        _camera = GetComponent<Camera>();
        _currentSize = _camera.orthographicSize;
    }

    void Update()
    {
        if (Math.Abs(_currentSize - TargetSize) > float.Epsilon)
        {
            //_lerpTime += Time.deltaTime;
            //float t = _lerpTime / TransitionTime;

            ////Different ways of interpolation if you comment them all it is just a linear lerp
            //t = Mathf.SmoothStep(0, 1, t); //Mathf.SmoothStep() can be used just like Lerp, here it is used to calc t so it works with the other examples.

            //_currentSize = Mathf.Lerp(_currentSize, TargetSize, t);
            _camera.orthographicSize = TargetSize;
        }
    }

    private float SmootherStep(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}