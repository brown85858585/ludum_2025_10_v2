// Changing the button Icons depending on the buttons status (on/off)
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MobileButton))]
public class MobileChangeButtonIcon : MonoBehaviour
{
    public Color offColor = new Color(73, 62, 68);
    public Sprite offSprite;
    [Space(5)]
    public Color onColor = new Color(88, 128, 168);
    public Sprite onSprite;
    

    private MobileButton myMobileButton;
    private Image myImage;

    private bool isButtonOn = false;

    void Start ()
    {
        myMobileButton = GetComponent<MobileButton>();
        myImage = transform.GetChild(0).GetComponent<Image>();
        myImage.sprite = offSprite;
        myImage.color = offColor;
        isButtonOn = false;
    }
	
	void Update ()
    {
		if(isButtonOn != myMobileButton.IsButtonOn())
        {
            isButtonOn = myMobileButton.IsButtonOn();
            myImage.sprite = isButtonOn ? onSprite : offSprite;
            myImage.color = isButtonOn ? onColor : offColor;
        }
	}
}
