using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurveAnimation : MonoBehaviour
{
    public AnimationCurve curveOffset = new AnimationCurve();
    public bool isWorldOffset = false;
    public Vector3 offset = Vector3.zero;
    private Vector3 _positionBackup;

    public bool enableRotation = true;
    public bool isWorldRotation = false;
    public AnimationCurve curveRotation = new AnimationCurve();
    public Vector3 toRotation = Vector3.zero;
    private Quaternion _rotationBackup;

    public bool enableScale = true;
    public AnimationCurve curveScale = new AnimationCurve();
    public Vector3 toScale = Vector3.one;
    private Vector3 _scaleBackup;

    public float lifeTime = 5.0f;
    public float delayTime = 0f;
    public int loopTime = 1;
    public float loopIntervalTime = 0f;
    private int _curLoopTime = 1;

    private float _startTime = 0.0f;
    private Transform _transform;
    private Material _mat;

    public bool enableColor = true;
    public AnimationCurve curveColor = new AnimationCurve();
    public Color fromColor = Color.white;
    public Color toColor = Color.white;

    public bool EditorStarted { get { return _editorStarted; } }
    private bool _editorStarted = false;

    public bool enableUvOffset = true;
    private float _lastPercent = 0.0f;
    public Vector2 mainUvST = Vector2.zero;
    public Vector2 maskUvST = Vector2.zero;
    // Use this for initialization
    void Start ()
    {
        if (Application.isPlaying)
        {
            _startTime = Time.timeSinceLevelLoad + delayTime;
            _transform = GetComponent<Transform>();
            _mat = GetComponent<Renderer>().material;
            _mat.SetColor("_ExternColor", new Color(0, 0, 0, 0));

            _positionBackup = _transform.position;
            //if (isWorldRotation)
            //{
            //    _rotationBackup = _transform.rotation;
            //    _transform.rotation = Quaternion.Euler(fromRotation);
            //}
            //else
            {
                _rotationBackup = _transform.localRotation ;
            }

            _scaleBackup = _transform.localScale;
        }
	}

    public void EditorStart()
    {
        _startTime = Time.timeSinceLevelLoad + delayTime;
        _transform = GetComponent<Transform>();
        _mat = GetComponent<Renderer>().sharedMaterial;
        _mat.SetColor("_ExternColor", new Color(0, 0, 0, 0));
        _curLoopTime = 1;

        _positionBackup = _transform.position;
        //if (isWorldRotation)
        //{
        //    _rotationBackup = _transform.rotation;
        //    _transform.rotation = Quaternion.Euler(fromRotation);
        //}
        //else
        {
            _rotationBackup = _transform.localRotation;
        }
        _scaleBackup = _transform.localScale;

        _editorStarted = true;
    }

    public void EditorStop()
    {
        _transform.localScale = _scaleBackup;
        _transform.localRotation = _rotationBackup;
        _transform.position = _positionBackup;
        _mat.SetColor("_ExternColor", new Color(1, 1, 1, 1));
        _mat.SetTextureOffset("_MainTex", Vector2.zero);
        _editorStarted = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!Application.isPlaying && !_editorStarted)
            return;
        if (_curLoopTime > loopTime)
            return;

        float percent = (Time.timeSinceLevelLoad - _startTime) / lifeTime;
        if (percent < 0)
            return;

        if (percent > 1)
        {
            if (_curLoopTime < loopTime)
            {
                _mat.SetColor("_ExternColor", new Color(0, 0, 0, 0));
                _transform.localScale = _scaleBackup;
                //if (isWorldRotation)
                //    _transform.rotation = _rotationBackup;
                //else
                _transform.localRotation = _rotationBackup;
                _transform.position = _positionBackup;

                ++_curLoopTime;
                _startTime = Time.timeSinceLevelLoad + loopIntervalTime;
            }
            else if (Application.isPlaying)
            {
                Renderer renderer = GetComponent<Renderer>();
                if (renderer)
                    renderer.enabled = false;

                ParticleSystem pSystem = GetComponent<ParticleSystem>();
                if (pSystem)
                {
                    renderer = pSystem.shape.meshRenderer;
                    if (renderer)
                        renderer.enabled = false;
                    renderer = pSystem.shape.skinnedMeshRenderer;
                    if (renderer)
                        renderer.enabled = false;
                }
            }
            else
                EditorStop();

            return;
        }

        //顺序必须是 缩放->旋转->位移
        if (enableScale)
        {
            Vector3 scale = Vector3.Lerp(Vector3.one, toScale, curveScale.Evaluate(percent));
            _transform.localScale = new Vector3(_scaleBackup.x * scale.x, _scaleBackup.y * scale.y, _scaleBackup.z * scale.z);
        }
        if (enableRotation)
        {
            if (isWorldRotation)
            {
                //Vector3 angle = Vector3.Lerp(Vector3.zero, toRotation, curveRotation.Evaluate(percent));
                _transform.localEulerAngles += toRotation * (percent - _lastPercent);//angle + _rotationBackup.eulerAngles;
            }
            else
            {
              //  Vector3 angle = Vector3.Lerp(Vector3.zero, toRotation, curveRotation.Evaluate(percent));
                Quaternion q = new Quaternion();
                q.eulerAngles = toRotation * (percent - _lastPercent);
                _transform.localRotation *= q;
            }
        }
        if (isWorldOffset)
            _transform.position += offset * (percent - _lastPercent);//_positionBackup + Vector3.Lerp(Vector3.zero, offset, curveOffset.Evaluate(percent));
        else
        {
            Vector3 Localoffset = offset.x * _transform.right + offset.y * _transform.up + offset.z * _transform.forward;
            _transform.position += offset * (percent - _lastPercent);//_positionBackup + Localoffset;// Vector3.Lerp(Vector3.zero, offset, curveOffset.Evaluate(percent));
        }

        if (enableColor)
            _mat.SetColor("_ExternColor", Color.Lerp(fromColor, toColor, curveColor.Evaluate(percent)));
        else
            _mat.SetColor("_ExternColor", new Color(1, 1, 1, 1));

        if (enableUvOffset)
        {
            if(_mat.HasProperty("_MainTex"))
                _mat.SetTextureOffset("_MainTex", mainUvST * lifeTime * percent);
            if (_mat.HasProperty("_MaskTex"))
                _mat.SetTextureOffset("_MaskTex", maskUvST * lifeTime * percent);
        }
        _lastPercent = percent;
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        EditorStop();
#endif
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        EditorStop();
#endif
    }

    void OnEnable()
    {
        Start();
        if (Application.isPlaying)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer)
                renderer.enabled = true;

            ParticleSystem pSystem = GetComponent<ParticleSystem>();
            if (pSystem)
            {
                renderer = pSystem.shape.meshRenderer;
                if (renderer)
                    renderer.enabled = true;
                renderer = pSystem.shape.skinnedMeshRenderer;
                if (renderer)
                    renderer.enabled = true;
            }
        }
    }
}
