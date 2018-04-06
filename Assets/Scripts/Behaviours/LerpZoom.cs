using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LerpZoom : MonoBehaviour
{
    public float FovStart = 60;
    public float FovEnd = 30;
    public float TransitionTime = 3;

    private float _currentFov;
    private float _lerpTime;
    private Camera _camera;

    void Start()
    {
        //could also use Camera.main if apropriate
        _camera = this.GetComponent<Camera>();
    }

    void Update()
    {
        ChangeFOV();
    }

    void ChangeFOV()
    {
        if (Math.Abs(_currentFov - FovEnd) > float.Epsilon)
        {
            _lerpTime += Time.deltaTime;
            var t = _lerpTime / TransitionTime;

            //Different ways of interpolation if you comment them all it is just a linear lerp
            t = Mathf.SmoothStep(0, 1, t); //Mathf.SmoothStep() can be used just like Lerp, here it is used to calc t so it works with the other examples.
                                           //t = SmootherStep(t);
                                           //t = t * t;
                                           //t = t * t * t;

            _currentFov = Mathf.Lerp(FovStart, FovEnd, t);
        }
        else if (Math.Abs(_currentFov - FovEnd) < float.Epsilon)
        {
            //Just going back where we came from. For demonstrative purpos only ...
            _lerpTime = 0;
            Debug.Log("Switch");
            var tmp = FovStart;
            FovStart = FovEnd;
            FovEnd = tmp;
        }

        _camera.fieldOfView = _currentFov;
    }

    private float SmootherStep(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}