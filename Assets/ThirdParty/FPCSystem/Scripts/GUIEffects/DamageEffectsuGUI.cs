using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class DamageEffectsuGUI : MonoBehaviour
{
	private float fadeTime = 2f;
    private float OrigAlpha;
    private float rate;
    private float i;
    private Image damageImg;

    void Start()
    {
        damageImg = GetComponent<Image>();
        if (damageImg.enabled == true)
            damageImg.enabled = false;

        OrigAlpha = damageImg.color.a;
        rate = 1f / fadeTime;
    }

    // Realiza el fade y desactiva el GUI cuando el alpha ya es cero (no se ve con alpha cero).
    void Update()
    {
        if (damageImg.enabled)
        {
            if (i < 1f)
            {
                i = i + (Time.deltaTime * rate);

				Color _myColor = damageImg.color;
				_myColor.a = Mathf.Lerp(OrigAlpha, 0, i);
				damageImg.color = _myColor;
            }
            else
            {
                damageImg.enabled = false;
            }
        }
    }

    public void SetDamageFadeTime(float _fadeTime)
    {
        fadeTime = _fadeTime;
        rate = 1f / fadeTime;
    }

    // Funcion que sera llamada por otros scripts que hagan danio a nuetro.
    public void ShowGUIDamage()
    {
        i = 0;

        Color _color = damageImg.color;
		_color.a = OrigAlpha;
		damageImg.color = _color;

        damageImg.enabled = true;
    }

}