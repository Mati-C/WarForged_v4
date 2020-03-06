using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Player 
{
    Model_Player _model;
    Viewer_Player _viewer;
    PlayerCamera _camera;

    bool W;
    bool S;
    bool D;
    bool A;

    public Controller_Player(Model_Player m , Viewer_Player v)
    {
        _model = m;
        _viewer = v;

        _model.idleEvent += _viewer.IdleAnim;
        _model.WalkEvent += _viewer.WalkAnim;
        _model.RunEvent += _viewer.RunAnim;

        _camera = _model.GetPlayerCam();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
 
    public void ControllerUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if(!S) W = true;

            if(!A && !D) _model.Movement(_camera.transform.forward);

            if (A && !D)
            {
                var dir = (_camera.transform.forward - _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }

            if (!A && D)
            {
                var dir = (_camera.transform.forward + _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (!W) S = true;

            if (!A && !D) _model.Movement(-_camera.transform.forward);

            if (A && !D)
            {
                var dir = (-_camera.transform.forward - _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }

            if (!A && D)
            {
                var dir = (-_camera.transform.forward + _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (!A) D = true;

            if (!W && !S) _model.Movement(_camera.transform.right);

            if (W && !S)
            {
                var dir = (_camera.transform.forward + _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }

            if (!W && S)
            {
                var dir = (-_camera.transform.forward + _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (!D) A = true;

            if (!W && !S) _model.Movement(-_camera.transform.right);

            if (W && !S)
            {
                var dir = (_camera.transform.forward - _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }

            if (!W && S)
            {
                var dir = (-_camera.transform.forward - _camera.transform.right) / 2;
                _model.Movement(dir.normalized);
            }
        }

        if (Input.GetKeyUp(KeyCode.W)) W = false;
        if (Input.GetKeyUp(KeyCode.S)) S = false;
        if (Input.GetKeyUp(KeyCode.D)) D = false;
        if (Input.GetKeyUp(KeyCode.A)) A = false;

        if (!W && !S && !D && !A) _model.idleEvent();

        if (Input.GetKeyDown(KeyCode.LeftShift)) _model.run = true;

        if (Input.GetKeyUp(KeyCode.LeftShift)) _model.run = false;
    }

}
