using UnityEngine;

namespace Invector.vCharacterController
{
    using vEventSystems;
    [vClassHeader("DAMAGE RECEIVER", "You can add damage multiplier for example causing twice damage on Headshots",  openClose = false)]
    public partial class vDamageReceiver : vMonoBehaviour, vIAttackReceiver
    {
        
        private vIHealthController healthController;

        public void OnReceiveAttack(vDamage damage, vIMeleeFighter attacker)
        {            
            if (overrideReactionID)
                damage.reaction_id = reactionID;

            if (ragdoll && !ragdoll.iChar.isDead)
            {
                var _damage = new vDamage(damage);
                var value = (float)_damage.damageValue;
                _damage.damageValue = (int)(value * damageMultiplier);
                ragdoll.gameObject.ApplyDamage(_damage, attacker);
                onReceiveDamage.Invoke(_damage);
            }
            else
            {
                if (healthController == null)
                    healthController = GetComponentInParent<vIHealthController>();

                if (healthController != null)
                {
                    var _damage = new vDamage(damage);
                    var value = (float)_damage.damageValue;
                    _damage.damageValue = (int)(value * damageMultiplier);
                    try
                    {
                        healthController.gameObject.ApplyDamage(_damage, attacker);
                        onReceiveDamage.Invoke(_damage);
                    }
                    catch
                    {
                        this.enabled = false;
                    }
                }
            }
        }
    }
}