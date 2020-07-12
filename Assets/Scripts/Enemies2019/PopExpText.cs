using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopExpText : MonoBehaviour
{
    public Text damageText;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    public void SetExp(float dmg)
    {
        damageText.text = dmg.ToString() + "-Exp";
    }
}
