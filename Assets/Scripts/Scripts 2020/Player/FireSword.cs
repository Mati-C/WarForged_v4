using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public class FireSword : MonoBehaviour
{
    Model_Player _player;
    Viewer_Player _viewer;
    DataManager _dataManager;
    public ParticleSystem fireWaveParticle;
    [Header("Fire Sword Level and Exp:")]
    public int fireSwordLevel;
    public float currentExp;
    public float expEarned;
    
    public List<float> expForEachLevel = new List<float>();
    public List<ClassEnemy> allEnemies = new List<ClassEnemy>();
    public List<String> LevelsInfo = new List<String>();
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
    public float BossOgre1Exp;
    public float warriorExp;
    public float shieldExp;
    public float spearExp;
    public float mageExp;

    Action GetEnergy;
    float _fireGizmo;
    public bool onExpUpdate;

    private void Awake()
    {
        _dataManager = FindObjectOfType<DataManager>();
        GetEnergy += () => {};
        _player = GetComponent<Model_Player>();
        _viewer = GetComponent<Viewer_Player>();
        allEnemies.AddRange(FindObjectsOfType<ClassEnemy>());

        LevelsInfo.Add("More burning damage per second.");
        LevelsInfo.Add("Unlock Fire Wave.");
        LevelsInfo.Add("Longer duration of Fire Sword. Slightly recharges skill when killing enemies");
        LevelsInfo.Add("Longer duration of Fire Sword");
        LevelsInfo.Add("More damage per second. Recharges skill when killing enemies");
        LevelsInfo.Add("Fire Wave whith more range");
        LevelsInfo.Add("Longer Fire Wave damage duration. Recharges skill faster when killing enemies");
        LevelsInfo.Add("Push enemies");
        LevelsInfo.Add("Skill recharges faster");

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

        _dataManager.Load();
        fireSwordLevel = _dataManager.data.swordLevel;
        currentExp = _dataManager.data.currentExp;
        expEarned = _dataManager.data.expEarned;

        for (int i = 0; i < fireSwordLevel; i++)
        {
            LevelUpdates[i+1]();
        }
     
    }

    public void SwordExp(float exp)
    {
        GetEnergy();
        currentExp += exp;
        _dataManager.data.currentExp = currentExp;
        _dataManager.Save();
    }

    public void UpdateSword()
    {
       if(!onExpUpdate)StartCoroutine(FireSwordExpUpdate());
    }

    IEnumerator FireSwordExpUpdate()
    {
        if (currentExp > 0 && !onExpUpdate)
        {
            onExpUpdate = true;
            float newExp = 0;

            if (currentExp >= expForEachLevel[fireSwordLevel])
            {
                newExp = expForEachLevel[fireSwordLevel];
            }

            else newExp = currentExp;

            float t = 2;
            while (t > 0)
            {
                expEarned += Time.deltaTime * (newExp / 2);
                currentExp -= Time.deltaTime * (newExp / 2);
                int c = (int)currentExp;
                _viewer.timertAlphaSwordExp = 2;
                _viewer.swordExp.text = c + "Exp";
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            int e = (int)expEarned;
            expEarned = e;


            if (expEarned >= expForEachLevel[fireSwordLevel] && fireSwordLevel < 9)
            {
                expEarned -= expForEachLevel[fireSwordLevel];
                fireSwordLevel++;              
                int level = fireSwordLevel + 1;
                _viewer.swordLevel.text = "Level-" + level;
                SoundManager.instance.Play(Player.LEVEL_UP, transform.position, true);
                LevelUpdates[fireSwordLevel]();
            }

            if (currentExp > 0)
            {
                onExpUpdate = false;
                StartCoroutine(FireSwordExpUpdate());
            }

            if (currentExp <= 0)
            {
                currentExp = 0;
                onExpUpdate = false;
            }

            _dataManager.data.swordLevel = fireSwordLevel;
            _dataManager.data.currentExp = currentExp;
            _dataManager.data.expEarned = expEarned;
            _dataManager.Save();
        }
    }

    public void FireWave()
    {
        var e = Physics.OverlapSphere(transform.position, fireWaveExpansion).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>());
        var destructibles = Physics.OverlapSphere(transform.position, fireWaveExpansion).Where(x => x.GetComponent<DestructibleOBJ>()).Select(x => x.GetComponent<DestructibleOBJ>());
        var roots = Physics.OverlapSphere(transform.position, fireWaveExpansion).Where(x => x.GetComponent<Roots>()).Select(x => x.GetComponent<Roots>());

        foreach (var item in destructibles) item.Break();

        foreach (var item in e)
        {
            item.StartBurning();
            item.GetDamage(fireWaveDamage, Model_Player.DamageType.Heavy);
            if (pushEnemies) item.PushKnocked();
        }

        foreach (var item in roots) item.StartDissolve();

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
