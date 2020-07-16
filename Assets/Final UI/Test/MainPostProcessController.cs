using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MainPostProcessController : MonoBehaviour
{
    public Material mat;
    Model_Player player;

    void Start()
    {
        player = FindObjectOfType<Model_Player>();
        mat.SetFloat("_HitFXIntensity", 0);
        mat.SetFloat("_FocusIntensity", 0);
    }

    public void LowLifeEffect()
    {
        StartCoroutine(LowLifeEffectC());
    }

    IEnumerator LowLifeEffectC()
    {
        while (player.life <= 50 && player.life > 0)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.deltaTime;
                mat.SetFloat("_HitFXIntensity", t * (1 - (player.life / 50)));
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void HitEffect()
    {
        StartCoroutine(HitEffectC());
    }

    IEnumerator HitEffectC()
    {
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            mat.SetFloat("_HitFXIntensity", t);
            yield return new WaitForEndOfFrame();
        }
    }

    public void HeavyAttackEffect(bool state)
    {
        StartCoroutine(HeavyAttackEffectC(state));
    }

    IEnumerator HeavyAttackEffectC(bool state)
    {
        if (state)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 20;
                mat.SetFloat("_FocusIntensity", t);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.deltaTime * 4;
                mat.SetFloat("_FocusIntensity", t);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void HeavyAttackFail()
    {
        StartCoroutine(HeavyAttackFailC());
    }

    IEnumerator HeavyAttackFailC()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            mat.SetFloat("_FocusIntensity", t);
            yield return new WaitForEndOfFrame();
        }

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * 4;
            mat.SetFloat("_FocusIntensity", t);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
