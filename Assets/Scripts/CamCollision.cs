using UnityEngine;

namespace Cinemachine
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that post-processes
    /// the final position of the virtual camera. Pushes the camera out of intersecting colliders.
    /// </summary>
    public class CamCollision : CinemachineExtension
    {
        /// <summary>The Unity layer mask against which the collider will raycast.</summary>
        [Tooltip("The Unity layer mask against which the collider will raycast")]
        public LayerMask m_CollideAgainst = 1;

        /// <summary>Callcack to to the collision resolution and shot evaluation</summary>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            // Move the body before the Aim is calculated
            if (stage == CinemachineCore.Stage.Body)
            {
                //Vector3 displacement = RespectCameraRadius(state.RawPosition);
                //state.PositionCorrection += displacement;

                state.PositionCorrection += RespectCameraTargetRay(state.RawPosition, vcam.LookAt.position);
            }
        }

        private Ray mRay;
        private RaycastHit mHit;

        private Vector3 RespectCameraTargetRay(Vector3 cameraPos, Vector3 targetPos)
        {
            mRay.origin = targetPos;
            mRay.direction = cameraPos - targetPos;

            if (Physics.Raycast(mRay, out mHit, (cameraPos - targetPos).magnitude, m_CollideAgainst.value, QueryTriggerInteraction.Ignore))
            {
                return mHit.point - cameraPos;
            }
            else
                return Vector3.zero;

        }
    }
}