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
        _model.TakeSwordEvent += _viewer.TakeSwordAnim;
        _model.SaveSwordEvent += _viewer.SaveSwordAnim;
        _model.CombatStateEvent += _viewer.CombatStateAnimator;
        _model.DodgeEvent += _viewer.DodgeAnims;

        _camera = _model.GetPlayerCam();

        _model.CombatStateEvent += _camera.SetCameraState;

       Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
 
    public void ControllerUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if(!S) W = true;

            if (!A && !D)
            {
                if(!_model.isInCombat) _model.Movement(_camera.transform.forward);

                else _model.CombatMovement(_camera.transform.forward, true, false);
            }

            if (A && !D)
            {
                var dir = (_camera.transform.forward - _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, false);
            }

            if (!A && D)
            {
                var dir = (_camera.transform.forward + _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, false);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (!W) S = true;

            if (!A && !D)
            {
                if (!_model.isInCombat) _model.Movement(-_camera.transform.forward);

                else _model.CombatMovement(-_camera.transform.forward, false, false);
            }

            if (A && !D)
            {
                var dir = (-_camera.transform.forward - _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, true);
            }

            if (!A && D)
            {
                var dir = (-_camera.transform.forward + _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, true);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (!A) D = true;

            if (!W && !S)
            {
                if (!_model.isInCombat) _model.Movement(_camera.transform.right);

                else _model.CombatMovement(_camera.transform.right, false, false);
            }

            if (W && !S)
            {
                var dir = (_camera.transform.forward + _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, false);
            }

            if (!W && S)
            {
                var dir = (-_camera.transform.forward + _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, true);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (!D) A = true;

            if (!W && !S)
            {
                if (!_model.isInCombat) _model.Movement(-_camera.transform.right);

                else _model.CombatMovement(-_camera.transform.right, false, false);
            }

            if (W && !S)
            {
                var dir = (_camera.transform.forward - _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, false);
            }

            if (!W && S)
            {
                var dir = (-_camera.transform.forward - _camera.transform.right) / 2;

                if (!_model.isInCombat) _model.Movement(dir.normalized);

                else _model.CombatMovement(dir.normalized, true, true);
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && !_model.onDodge)
        {
            if ((W && !S && !A && !D) || (W && !S && A && !D) || (W && !S && !A && D) || (!W && !S && !A && D)) _model.Dodge(Model_Player.DogeDirecctions.Roll);

            if ((!W && S && !A && !D) || (!W && S && A && !D) || (!W && S && !A && D)) _model.Dodge(Model_Player.DogeDirecctions.Back);

            if ((!W && !S && !A && D)) _model.Dodge(Model_Player.DogeDirecctions.Right); 

            if ((!W && !S && A && !D)) _model.Dodge(Model_Player.DogeDirecctions.Left); 

        }

        if (Input.GetKeyUp(KeyCode.W)) W = false;
        if (Input.GetKeyUp(KeyCode.S)) S = false;
        if (Input.GetKeyUp(KeyCode.D)) D = false;
        if (Input.GetKeyUp(KeyCode.A)) A = false;

        if (!W && !S && !D && !A && !_model.run) _model.idleEvent();

        if (Input.GetKeyDown(KeyCode.LeftShift)) _model.run = true;

        if (Input.GetKeyUp(KeyCode.LeftShift)) _model.run = false;

        if (Input.GetKeyDown(KeyCode.Mouse0)) _model.SwordAttack();

    }

}
