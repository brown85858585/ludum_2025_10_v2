using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class SliderFader : MonoBehaviour
{
    private float speed = 1;

	private bool fadeIn = false;
    private bool fadeOut = false;

	private float barOrigAlpha = 1.0f;
	private float behindBarOrigAlpha = 1.0f;

	private Image fillImage;
	private Image behindBar;

	private Color _currentBarColor;
	private Color _currentFillColor;

    void Awake()
    {
        behindBar = transform.GetChild(0).GetComponent<Image>();
        fillImage = transform.GetChild(1).GetComponentInChildren<Image>();
		if (fillImage == null || behindBar == null){
            Debug.LogWarning("Something its' wrong with this slider. Not getting the fill & background bars.");
			return;
		}

        if (fillImage != null)
            barOrigAlpha = fillImage.color.a;
		
        if (behindBar != null)
            behindBarOrigAlpha = behindBar.color.a;

		_currentBarColor = behindBar.color;
		_currentFillColor = fillImage.color;
    }

    void Update()
    {
        if (fadeIn)
        {
			_currentBarColor = behindBar.color;
			_currentFillColor = fillImage.color;

			if (Mathf.Abs(fillImage.color.a - barOrigAlpha) >= 0.05f){
				_currentFillColor.a = Mathf.Lerp(fillImage.color.a, barOrigAlpha, speed * Time.deltaTime);
				fillImage.color = _currentFillColor;

	            if (behindBar)
	            {
					_currentBarColor.a = Mathf.Lerp(behindBar.color.a, behindBarOrigAlpha, speed * Time.deltaTime);
					behindBar.color = _currentBarColor;
	            }
			}
            else if (Mathf.Abs(fillImage.color.a - barOrigAlpha) < 0.05f)
            {
				_currentFillColor.a = barOrigAlpha;
				fillImage.color = _currentFillColor;

                if (behindBar)
                {
					_currentBarColor.a = behindBarOrigAlpha;
					behindBar.color = _currentBarColor;
                }

                fadeIn = false;
            }
        }

        if (fadeOut)
        {
			_currentBarColor = behindBar.color;
			_currentFillColor = fillImage.color;

			if (fillImage.color.a >= 0.05f)
			{
				_currentFillColor.a = Mathf.Lerp(fillImage.color.a, 0, speed * Time.deltaTime);
				fillImage.color = _currentFillColor;

				if (behindBar)
				{
					_currentBarColor.a = Mathf.Lerp(behindBar.color.a, 0, speed * Time.deltaTime);
					behindBar.color = _currentBarColor;
				}
			}
            else if (fillImage.color.a < 0.05f)
            {
				_currentFillColor.a = 0;
				fillImage.color = _currentFillColor;

				if (behindBar)
				{
					_currentBarColor.a = 0;
					behindBar.color = _currentBarColor;
				}

                fadeOut = false;
            }
        }
    }

    public void StartFadeIn(float _speed)
    {
        speed = _speed;
        fadeIn = true;
        fadeOut = false;
    }

    public IEnumerator StartFadeOut(float _speed, float _timeToWait)
    {
        speed = _speed;
        fadeIn = false;
        yield return new WaitForSeconds(_timeToWait);
        fadeOut = true;
    }

}