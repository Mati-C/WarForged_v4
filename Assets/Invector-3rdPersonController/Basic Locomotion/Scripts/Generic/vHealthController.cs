using System.Collections;
using UnityEngine;

namespace Invector
{
    [vClassHeader("HealthController",iconName ="HealthControllerIcon")]
    public class vHealthController : vMonoBehaviour, vIHealthController
    {
        [vEditorToolbar("Health", order = 0)]
        [SerializeField] [vReadOnly] protected bool _isDead;
        [vBarDisplay("maxHealth", true)] [SerializeField] protected float _currentHealth;

        public int maxHealth = 100;

        public int MaxHealth
        {
            get
            {
                return maxHealth;
            }
            protected set
            {
                maxHealth = value;
            }
        }
        public float currentHealth
        {
            get
            {
                return _currentHealth;
            }
            protected set
            {
                _currentHealth = value;
                if (!_isDead && _currentHealth <= 0)
                {
                    _isDead = true;
                    onDead.Invoke(gameObject);
                }
            }
        }

        public bool isDead
        {
            get
            {
                if (!_isDead && currentHealth <= 0)
                {
                    _isDead = true;
                    onDead.Invoke(gameObject);
                }
                return _isDead;
            }
            set
            {
                _isDead = value;
            }
        }

        public float healthRecovery = 0f;
        public float healthRecoveryDelay = 0f;
        [HideInInspector]
        public float currentHealthRecoveryDelay;
        [vEditorToolbar("Events", order = 100)]
        [SerializeField] protected OnReceiveDamage _onReceiveDamage = new OnReceiveDamage();
        [SerializeField] protected OnDead _onDead = new OnDead();
        public OnReceiveDamage onReceiveDamage { get { return _onReceiveDamage; }protected set { _onReceiveDamage = value; } }
        public OnDead onDead { get { return _onDead; } protected set { _onDead = value; } }
        bool inHealthRecovery;

        protected virtual void Start()
        {
            currentHealth = maxHealth;
            currentHealthRecoveryDelay = healthRecoveryDelay;
        }

        protected virtual bool canRecoverHealth
        {
            get
            {
                return !(currentHealth <= 0 || (healthRecovery == 0 && currentHealth < maxHealth));
            }
        }

        protected virtual IEnumerator RecoverHealth()
        {
            inHealthRecovery = true;
            while (canRecoverHealth)
            {
                HealthRecovery();
                yield return null;
            }
            inHealthRecovery = false;
        }

        protected virtual void HealthRecovery()
        {
            if (!canRecoverHealth) return;
            if (currentHealthRecoveryDelay > 0)
                currentHealthRecoveryDelay -= Time.deltaTime;
            else
            {
                if (currentHealth > maxHealth)
                    currentHealth = maxHealth;
                if (currentHealth < maxHealth)
                    currentHealth += healthRecovery * Time.deltaTime;
            }
        }

        /// <summary>
        /// Change the currentHealth of Character
        /// </summary>
        /// <param name="value"></param>
        public virtual void ChangeHealth(int value)
        {            
            currentHealth += value;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (!isDead && currentHealth <= 0)
            {
                isDead = true;
                onDead.Invoke(gameObject);
            }
        }

        /// <summary>
        /// Change the MaxHealth of Character
        /// </summary>
        /// <param name="value"></param>
        public virtual void ChangeMaxHealth(int value)
        {
            maxHealth += value;
            if (maxHealth < 0)
                maxHealth = 0;
        }

        /// <summary>
        /// Apply Damage to Current Health
        /// </summary>
        /// <param name="damage">damage</param>
        public virtual void TakeDamage(vDamage damage)
        {
            if (damage != null)
            {
                currentHealthRecoveryDelay = currentHealth <= 0 ? 0 : healthRecoveryDelay;
                if (damage.damageValue > 0 && !inHealthRecovery)
                {
                    StartCoroutine(RecoverHealth());
                }                    
                
                if (currentHealth > 0)
                {
                    currentHealth -= damage.damageValue;
                }
                onReceiveDamage.Invoke(damage);
            }
        }
    }
}

