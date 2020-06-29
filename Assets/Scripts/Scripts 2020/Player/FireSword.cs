using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireSword: MonoBehaviour
{
    Model_Player _player;
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

    Action GetEnergy;

    private void Awake()
    {
        GetEnergy += () => {};
        _player = GetComponent<Model_Player>();
        allEnemies.AddRange(FindObjectsOfType<ClassEnemy>());

        Action Lv2 = () => { fireSwordBurnTimeOnEnemy = fireSwordBurnTimeOnEnemyLV2; };

        Action Lv3 = () => { _player.FireSwordEvent += FireWave; };

        Action Lv4 = () => { foreach (var item in allEnemies) item.DieEvent += MoreTimeFireSword; };
  
        Action Lv5 = () => { fireSwordTime = fireSwordTimeLV2; };

        Action Lv6 = () => { timeForDead = timeForDeadLV2; };

        Action Lv8 = () => { timeForDead = timeForDeadLV2; };

        Action Lv10 = () => { GetEnergy += () => { currentExp += plusEnergy; }; };

        LevelUpdates.Add(1, Lv2);
        LevelUpdates.Add(2, Lv3);
        LevelUpdates.Add(3, Lv4);
        LevelUpdates.Add(4, Lv5);
        LevelUpdates.Add(5, Lv6);
        LevelUpdates.Add(7, Lv8);
        LevelUpdates.Add(9, Lv10);
    }

    public void SwordExp(float exp)
    {
        GetEnergy();
        currentExp += exp;
        if (currentExp >= expForEachLevel[fireSwordLevel])
        {
            fireSwordLevel++;
            currentExp = 0;
            LevelUpdates[fireSwordLevel]();
        }
    }

    public void FireWave()
    {

    }

    public void MoreTimeFireSword()
    {
        _player.fireSwordCurrentTime -= timeForDead;
    }
}
