using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Player 
{
    Model_Player _model;
    Viewer_Player _viewer;
    PlayerCamera _camera;

    float auxTimer;

    bool W;
    bool S;
    bool D;
    bool A;

    public Controller_Player(Model_Player m , Viewer_Player v)
    {
        _model = m;
        _viewer = v;
        _camera = _model.GetPlayerCam();
       

        _model.idleEvent += _viewer.IdleAnim;
        _model.WalkEvent += _viewer.WalkAnim;
        _model.WalkEvent += _model.GetIA_CombatManager().UpdateEnemyAggressive;
        _model.RunEvent += _viewer.RunAnim;
        _model.RunEvent += _model.GetIA_CombatManager().UpdateEnemyAggressive;
        _model.TakeSwordEvent += _viewer.TakeSwordAnim;
        _model.SaveSwordEvent += _viewer.SaveSwordAnim;
        _model.CombatStateEvent += _viewer.CombatStateAnimator;
        _model.DodgeEvent += _viewer.DodgeAnims;       
        _model.LockedOnEvent += _camera.LockOnCam;
        _model.LockedOnEvent += _viewer.SetLockOnParticle;
        _model.LockedOffEvent += _camera.LockOffCam;           
        _model.LockedOffEvent += _viewer.SetOffLockOnParticle;           
        _model.CombatStateEvent += _camera.SetCameraState;
        _model.DefenceEvent += _viewer.DefenceAnim;
        _model.DefenceEvent += _viewer.ChangeLayer;
       // _model.ChargeAttackEvent += _viewer.SlowTinme;

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

                else
                {
                    if (!_model.onLock) _model.CombatMovement(_camera.transform.forward, false, false);

                    else _model.CombatMovement(_model.transform.forward, false, false);
                }
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

                else
                {
                   if(!_model.onLock) _model.CombatMovement(-_camera.transform.forward, false, false);

                   else _model.CombatMovement(-_model.transform.forward, false, false);
                }
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

                else
                {
                   if(!_model.onLock) _model.CombatMovement(_camera.transform.right, false, false);

                   else _model.CombatMovement(_model.transform.right, false, false);
                }
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

                else
                {
                   if(!_model.onLock) _model.CombatMovement(-_camera.transform.right, false, false);

                   else _model.CombatMovement(-_model.transform.right, false, false);
                }
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
            if ((W && !S && !A && !D) || (W && !S && A && !D) || (W && !S && !A && D)) _model.Dodge(Model_Player.DogeDirecctions.Roll);

            if ((!W && S && !A && !D) || (!W && S && A && !D) || (!W && S && !A && D)) _model.Dodge(Model_Player.DogeDirecctions.Back);

            if ((!W && !S && !A && D)) _model.Dodge(Model_Player.DogeDirecctions.Right); 

            if ((!W && !S && A && !D)) _model.Dodge(Model_Player.DogeDirecctions.Left); 

        }

        if (Input.GetKeyUp(KeyCode.W)) W = false;
        if (Input.GetKeyUp(KeyCode.S)) S = false;
        if (Input.GetKeyUp(KeyCode.D)) D = false;
        if (Input.GetKeyUp(KeyCode.A)) A = false;

       
        if (Input.GetKeyDown(KeyCode.LeftShift)) _model.run = true;

        if (!W && !S && !D && !A) _model.idleEvent();

        if (Input.GetKeyUp(KeyCode.LeftShift)) _model.run = false;

        //-------------------------ATTACKS------------------------
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (_model.chargeAttackAmount >= 0.2f) _model.ChangeActionState(true);
            _model.chargeAttackAmount += Time.deltaTime;
            if (_model.chargeAttackAmount >= _model.chargeAttackTime - 0.1f) _viewer.SlowTinme();
            if (_model.chargeAttackAmount >= _model.chargeAttackTime) _model.ChargeAttack(_model.chargeAttackAmount); 
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) auxTimer = Time.time;
     
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _model.chargeAttackAmount = 0;

            if (Time.time - auxTimer < 0.2f && !_model.onDodge)
            {

                if (!_model.onDodge && !W && !S && !A && !D) _model.SwordAttack(_model.transform.forward);
                if (!_model.onDodge && W && !S && !A && !D) _model.SwordAttack(_camera.transform.forward);

                if (!_model.onDodge && W && !S && A && !D)
                {
                    var dir = (_camera.transform.forward - _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }

                if (!_model.onDodge && W && !S && !A && D)
                {
                    var dir = (_camera.transform.forward + _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }


                if (!_model.onDodge && !W && S && !A && !D) _model.SwordAttack(-_camera.transform.forward);

                if (!_model.onDodge && !W && S && A && !D)
                {
                    var dir = (-_camera.transform.forward - _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }

                if (!_model.onDodge && !W && S && !A && D)
                {
                    var dir = (-_camera.transform.forward + _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }

                if (!_model.onDodge && !W && !S && A && !D) _model.SwordAttack(-_camera.transform.right);

                if (!_model.onDodge && !W && S && A && !D)
                {
                    var dir = (-_camera.transform.forward - _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }

                if (!_model.onDodge && W && !S && A && !D)
                {
                    var dir = (_camera.transform.forward - _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }

                if (!_model.onDodge && !W && !S && !A && D) _model.SwordAttack(_camera.transform.right);

                if ( !_model.onDodge && !W && S && !A && D)
                {
                    var dir = (-_camera.transform.forward + _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }

                if (!_model.onDodge && W && !S && !A && D)
                {
                    var dir = (_camera.transform.forward + _camera.transform.right) / 2;
                    _model.SwordAttack(dir.normalized);
                }
            }

            _model.ChangeActionState(false);
            
            _model.ChargeAttack(Time.time - auxTimer);
            

            auxTimer = 0;
        }
        //-------------------------------------------------------
       
        if (Input.GetKey(KeyCode.Mouse1) && !_model.onDodge) _model.Defence();

        if (Input.GetKeyUp(KeyCode.Mouse1) && !_model.onDodge) _model.DefenceOff();

        if (Input.GetKeyDown(KeyCode.E)) _model.LockEnemies();

        if (Input.GetKeyDown(KeyCode.Tab)) _model.ChangeTarget();

    }

}
