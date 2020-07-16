using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScreenSpace : MonoBehaviour
{
    ClassEnemy enemy;

    Canvas canvas;
    public HealthBar barPrefab;
    GameObject levelUI;

    HealthBar healthBar;
    Image tempHealthFill;
    Image healthFill;
    Camera cam;
    DepthUI depthUI;

    float timer;

    void Start()
    {
        levelUI = GameObject.Find("LEVEL UI");
        enemy = GetComponent<ClassEnemy>();
        healthBar = Instantiate(barPrefab);
        healthBar.transform.SetParent(levelUI.transform, false);
        tempHealthFill = healthBar.transform.GetChild(0).GetComponent<Image>();
        healthFill = healthBar.transform.GetChild(1).GetComponent<Image>();
        canvas = FindObjectOfType<Canvas>();

        depthUI = healthBar.GetComponent<DepthUI>();
        canvas.GetComponent<ScreenSpaceCanvas>().AddToCanvas(healthBar.gameObject);
        cam = FindObjectOfType<PlayerCamera>().GetComponent<Camera>();
        healthBar.gameObject.SetActive(false);
        timer = 0;

        healthBar.enemy = enemy;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        Vector3 worldPos = transform.position + (Vector3.up * 1.75f);
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        healthBar.transform.position = screenPos;

        float distance = Vector3.Distance(worldPos, cam.transform.position);
        depthUI.depth = -distance;

        Vector3 vp = cam.WorldToViewportPoint(worldPos);

        if (timer > 0)
        {
            if (tempHealthFill.fillAmount > 0 && (vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1 && vp.z > 0))
                healthBar.gameObject.SetActive(true);
            else
                healthBar.gameObject.SetActive(false);
        }
        else
            healthBar.gameObject.SetActive(false);
    }

    public IEnumerator UpdateLifeBar(float target)
    {
        timer = 3;
        float smoothTimer = 0;

        float current = tempHealthFill.fillAmount;
        healthFill.fillAmount = target;

        if (current - target <= 0.025f)
            tempHealthFill.fillAmount = target;

        while (smoothTimer <= 1)
        {
            smoothTimer += Time.deltaTime;
            tempHealthFill.fillAmount = Mathf.Lerp(current, target, smoothTimer);
            yield return new WaitForEndOfFrame();
        }
    }
}
