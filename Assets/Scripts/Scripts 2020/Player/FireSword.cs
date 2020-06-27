using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSword: MonoBehaviour
{
    [Header("Fire Sword Level and Exp:")]
    public int fireSwordLevel;
    public float currentExp;

    public List<float> expForEachLevel = new List<float>();

    [Header("Fire Sword Variables:")]
    public float fireSwordTime;
    public float fireSwordBurnDamage;
    public float fireSwordBurnTimeOnEnemy;
    public float energyToUseFireSword;
 
    public void SwordExp(float exp)
    {
        currentExp += exp;
        if (currentExp >= expForEachLevel[fireSwordLevel])
        {
            fireSwordLevel++;
            currentExp = 0;
        }
    }
}
