using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DetectBasketScore : MonoBehaviour
{
    public Canvas scoreCanvas;
    public Text scoreText;

    [Space(10)]
    public AudioClip scoreClip;
    public AudioClip failClip;

    [Header("Runtime Watcher")]
    [SerializeField]
    [Disable]
    protected int score = 0;
    [SerializeField]
    [Disable]
    protected bool hasEnteredInBasket = false;

    public virtual void Start()
    {
        scoreText.text = "Score: 0";
        scoreCanvas.enabled = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.name.Contains("BasketBall")) return;

        if (other.transform.GetComponent<Rigidbody>().linearVelocity.y < 0)
        {
            StopCoroutine("_PlayFailClip");
            StopCoroutine("_DisableScore");
            hasEnteredInBasket = true;
            scoreCanvas.enabled = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (!hasEnteredInBasket) return;
        if (!other.name.Contains("BasketBall")) return;

        hasEnteredInBasket = false;
        StartCoroutine("_DisableScore");
        
        if (other.transform.GetComponent<Rigidbody>().linearVelocity.y < 0)
        {
            score += 2;
            scoreText.text = "Score: " + score.ToString();
            SoundManager.instance.GetSoundPlayer().PlayLocatedClip(scoreClip, transform.position);
        }
        else
        {
            StartCoroutine("_PlayFailClip");
        }

    }

    protected virtual IEnumerator _PlayFailClip()
    {
        yield return new WaitForSeconds(1.5f);
        SoundManager.instance.GetSoundPlayer().PlayLocatedClip(failClip, transform.position);
    }

    protected virtual IEnumerator _DisableScore()
    {
        yield return new WaitForSeconds(10f);
        scoreCanvas.enabled = false;
    }

}
