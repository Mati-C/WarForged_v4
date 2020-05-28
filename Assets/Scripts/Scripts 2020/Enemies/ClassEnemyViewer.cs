using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClassEnemyViewer : MonoBehaviour
{
    Camera _cam;
    public PopText prefabTextDamage;
    GameObject _levelUI;

    private void Awake()
    {
        _cam = FindObjectOfType<PlayerCamera>().GetComponent<Camera>();
        _levelUI = GameObject.Find("LEVEL UI");
    }

    void Start()
    {

    }


    void Update()
    {

    }

    public void CreatePopText(float damage)
    {

        PopText text = Instantiate(prefabTextDamage);
        StartCoroutine(FollowEnemy(text));
        text.transform.SetParent(_levelUI.transform, false);
        text.SetDamage(damage);

    }

    IEnumerator FollowEnemy(PopText text)
    {
        while (text != null)
        {
            Vector2 screenPos = _cam.WorldToScreenPoint(transform.position + (Vector3.up * 2));
            text.transform.position = screenPos;
            yield return new WaitForEndOfFrame();
        }
    }
}

   
