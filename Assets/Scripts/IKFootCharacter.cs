using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootCharacter : MonoBehaviour
{
    Animator anim;


    [Header("LayerMask")]
    public LayerMask _layerMask;

    [Header("Distance To Ground")]
    [Range(0, 1)]
    public float DistanceToGround;


    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
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
                    Debug.Log("foot IK");
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
                    Debug.Log("foot IK");
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround;
                    anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));


                }
            }
        }
    }
}
