using UnityEngine;

namespace Invector.vCharacterController
{
    [vClassHeader("DAMAGE RECEIVER", "You can add damage multiplier for example causing twice damage on Headshots", openClose = false)]
    public partial class vDamageReceiver : vMonoBehaviour, vIDamageReceiver
    {
        public float damageMultiplier = 1f;
        [HideInInspector]
        public vRagdoll ragdoll;
        public bool overrideReactionID;
        [vHideInInspector("overrideReactionID")]
        public int reactionID;
        [SerializeField] protected OnReceiveDamage _onReceiveDamage = new OnReceiveDamage();
        public OnReceiveDamage onReceiveDamage { get { return _onReceiveDamage; } protected set { _onReceiveDamage = value; } }

        void Start()
        {
            ragdoll = GetComponentInParent<vRagdoll>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision != null)
            {
                if (ragdoll && ragdoll.isActive)
                {
                    ragdoll.OnRagdollCollisionEnter(new vRagdollCollision(this.gameObject, collision));
                    if (!inAddDamage)
                    {
                        float impactforce = collision.relativeVelocity.x + collision.relativeVelocity.y + collision.relativeVelocity.z;
                        if (impactforce > 10 || impactforce < -10)
                        {
                            inAddDamage = true;
                            vDamage damage = new vDamage((int)Mathf.Abs(impactforce) - 10);
                            damage.ignoreDefense = true;
                            damage.sender = collision.transform;
                            damage.hitPosition = collision.contacts[0].point;

                            Invoke("ResetAddDamage", 0.1f);
                        }
                    }
                }
            }
        }

        bool inAddDamage;

        void ResetAddDamage()
        {
            inAddDamage = false;
        }

        public void TakeDamage(vDamage damage)
        {
            if (!ragdoll) return;
            if (!ragdoll.iChar.isDead)
            {
                inAddDamage = true;
                if (overrideReactionID) damage.reaction_id = reactionID;

                var _damage = new vDamage(damage);
                var value = (float)_damage.damageValue;
                _damage.damageValue = (int)(value * damageMultiplier);

                ragdoll.ApplyDamage(damage);
                onReceiveDamage.Invoke(_damage);
                Invoke("ResetAddDamage", 0.1f);
            }
        }
    }
}