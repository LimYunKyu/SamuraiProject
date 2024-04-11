using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraHolderController : MonoBehaviour
{


    Vector3 _angle = Vector3.zero;
    Vector3 _preMousePos = Vector3.zero;
    Vector3 _currentMousePos = Vector3.zero;
    Vector3 _forwardVec = Vector3.zero;

    float _clampDown = -89f;

    float _clapmUP = 89f;
    Transform _parent;
    PlayerController _playerController;
    void Start()
    {
        Init();
    }


    void Update()
    {


    }

    void Init()
    {
        if (transform.parent)
        {
            _parent = transform.parent;
            transform.rotation = Quaternion.LookRotation(_parent.forward);
        }
        if (_parent.GetComponent<PlayerController>() != null)
        {

            _playerController = _parent.GetComponent<PlayerController>();
        }

        _currentMousePos = Input.mousePosition;
        _preMousePos = Input.mousePosition;

    }

    void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(CalcAngleUsedMousePos());
        transform.rotation = rotation;
        _forwardVec = transform.forward;

        ApplyHolderForward();
       
    }
    void ApplyHolderForward()
    {

        transform.rotation = Quaternion.LookRotation(_forwardVec);
    }

    Vector3 CalcAngleUsedMousePos()
    {
        _currentMousePos = Input.mousePosition;
        Vector3 offset = _currentMousePos - _preMousePos;
        _preMousePos = _currentMousePos;

        _angle.x -= offset.y * 30 * Time.deltaTime;
        _angle.y += offset.x * 50 * Time.deltaTime;

        _angle.x = Mathf.Clamp(_angle.x, _clampDown, _clapmUP);

        return _angle;
    }


}
