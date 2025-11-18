using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerUIPopupManager : MonoBehaviour
{
    [Header("You Died Pop Up")]
    [SerializeField] GameObject youDiedPopUpObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    [SerializeField] TextMeshProUGUI youDiedBackGroundText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup; //allows setting alpha to fade over time 

    public void SendYouDiedPopUp()
    {
        //activate post process effects

        youDiedPopUpObject.SetActive(true);
        youDiedBackGroundText.characterSpacing = 0;

        StartCoroutine(StrectchPopUpTextOverTime(youDiedBackGroundText, 8, 20));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 4));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 3, 4));
    }

    public IEnumerator StrectchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
    {
        if (duration > 0f)
        {
            text.characterSpacing = 0;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if (duration > 0f)
        {
            canvas.alpha = 0;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }
        }
        canvas.alpha = 1;
        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if (duration > 0f)
        {
            while (delay > 0f)
            {
                delay -= Time.deltaTime;
                yield return null;
            }
            canvas.alpha = 1;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;
            }
        }
        canvas.alpha = 0;
        yield return null;
    }
}
