using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FireSword: MonoBehaviour
{
    Model_Player _player;
    public ParticleSystem fireWaveParticle;
    [Header("Fire Sword Level and Exp:")]
    public int fireSwordLevel;
    public float currentExp;

    public List<float> expForEachLevel = new List<float>();
    public List<ClassEnemy> allEnemies = new List<ClassEnemy>();
    public Dictionary<int, Action> LevelUpdates = new Dictionary<int, Action>(); 

    [Header("Fire Sword Variables:")]
    public float fireSwordTime;
    public float fireSwordBurnDamage;
    public float fireSwordBurnTimeOnEnemy;
    public float energyToUseFireSword;

    [Header("Fire Sword LevelUpdates:")]
    public float fireSwordBurnTimeOnEnemyLV2;
    public float fireSwordTimeLV2;
    public float timeForDead;
    public float timeForDeadLV2;
    public float plusEnergy;
    public float fireWaveExpansion;
    public float fireWaveDamage;
    public float fireWaveExpansionLV2;
    public bool pushEnemies;

    [Header("Enemy Exp:")]
    public float warriorExp;
    public float shieldExp;
    public float spearExp;
    public float mageExp;

    Action GetEnergy;
    float _fireGizmo;

    private void Awake()
    {
        GetEnergy += () => {};
        _player = GetComponent<Model_Player>();
        allEnemies.AddRange(FindObjectsOfType<ClassEnemy>());

        Action Lv1 = () => { };

        Action Lv2 = () => { fireSwordBurnTimeOnEnemy = fireSwordBurnTimeOnEnemyLV2; };

        Action Lv3 = () => { _player.FireSwordEvent += FireWave; };

        Action Lv4 = () => { foreach (var item in allEnemies) item.DieEvent += MoreTimeFireSword; };
  
        Action Lv5 = () => { fireSwordTime = fireSwordTimeLV2; };

        Action Lv6 = () => { timeForDead = timeForDeadLV2; };

        Action Lv7 = () => { fireWaveExpansion = fireWaveExpansionLV2; };

        Action Lv8 = () => { timeForDead = timeForDeadLV2; };

        Action Lv9 = () => { pushEnemies = true; };

        Action Lv10 = () => { GetEnergy += () => { currentExp += plusEnergy; }; };

        LevelUpdates.Add(0, Lv1);
        LevelUpdates.Add(1, Lv2);
        LevelUpdates.Add(2, Lv3);
        LevelUpdates.Add(3, Lv4);
        LevelUpdates.Add(4, Lv5);
        LevelUpdates.Add(5, Lv6);
        LevelUpdates.Add(6, Lv7);
        LevelUpdates.Add(7, Lv8);
        LevelUpdates.Add(8, Lv9);
        LevelUpdates.Add(9, Lv10);

        for (int i = 0; i < fireSwordLevel; i++)
        {
            LevelUpdates[i+1]();
        }
     

    }

    public void SwordExp(float exp)
    {
        GetEnergy();
        currentExp += exp;
        if (currentExp >= expForEachLevel[fireSwordLevel] && fireSwordLevel <9)
        {
            fireSwordLevel++;
            currentExp = 0;
            LevelUpdates[fireSwordLevel]();
        }
    }

    public void FireWave()
    {
        var e = Physics.OverlapSphere(transform.position, fireWaveExpansion).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>());
        foreach (var item in e)
        {
            item.StartBurning();
            item.GetDamage(fireWaveDamage, Model_Player.DamageType.Heavy);
            if (pushEnemies) item.PushKnocked();
        }
        StartCoroutine(FireWaveExpansion());
    }

    IEnumerator FireWaveExpansion()
    {
        fireWaveParticle.Play();
        yield return new WaitForSeconds(2);
        fireWaveParticle.Stop();
    }
        
    public void MoreTimeFireSword()
    {
        _player.fireSwordCurrentTime -= timeForDead;
    }

    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(transform.position, fireGizmo);
    }
}
