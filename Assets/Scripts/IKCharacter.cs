using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKCharacter : MonoBehaviour
{
    Animator anim;
    Model_Player _Player;
    float _currentTime = 0;

    [Header("LayerMask")]
    public LayerMask _layerMask;

    [Header("Distance To Ground")]
    [Range(0, 1)]
    public float DistanceToGround;

    [Header("Ik target:")]
    public float timeToIkHand = 0.5f;
    public Transform IkLeftHand;
    public float _ikWeight = 1;


    void Start()
    {
        anim = GetComponent<Animator>();
        _Player = GetComponent<Model_Player>();
    }

    public void Update()
    {
        if (_Player.isInCombat)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            _currentTime = 0;
        }

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
            //foot
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IkLeftFootWheight"));
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IkLeftFootWheight"));
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IkRightFootWheight"));
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IkRightFootWheight"));

            RaycastHit hit;
            //LeftFoot
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up , Vector3.down);
            if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, _layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
    
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));


                }
            }


            //RightFoot
            ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, _layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                   
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));


                }
            }
            //hand
            if (_Player.isInCombat)
            {
                if (_currentTime >= timeToIkHand)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
                    anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, _ikWeight);

                    anim.SetIKPosition(AvatarIKGoal.LeftHand, IkLeftHand.position);
                    anim.SetIKRotation(AvatarIKGoal.LeftHand, IkLeftHand.rotation);
                }
            }
        }
    }

}
