using System.Collections;
using Utils;
using TMPro;
using UnityEngine;

public class FadingOutText : MonoBehaviour
{
    private struct FadingOutRoutineInfo
    {
        public string textToShow;
        public float delayDuration;
        public float queryTime;

        public FadingOutRoutineInfo(string textToShow, float delayDuration, float queryTime)
        {
            this.textToShow = textToShow;
            this.delayDuration = delayDuration;
            this.queryTime = queryTime;
        }
    }

    public string TextToShow { get; set; } = "";
    private TextMeshProUGUI TMPro;

    private const float alphaDecrease = 0.05f;
    private readonly WaitForSeconds alphaDecreaseCooldown = new WaitForSeconds(0.1f);
    private Coroutine currentFadingOutRoutine;
    private PriorityQueue<FadingOutRoutineInfo, float> fadingOutRoutineQueue = new PriorityQueue<FadingOutRoutineInfo, float>();

    private const float timeBeforeFadingOut = 3f;

    private void Awake()
    {
        TMPro = GetComponent<TextMeshProUGUI>();
    }

    public void ShowHint(float delayDuration)
    {
        if (fadingOutRoutineQueue.Count == 0)
        {
            fadingOutRoutineQueue.Enqueue(new FadingOutRoutineInfo(TextToShow, delayDuration, Time.time), delayDuration);
            currentFadingOutRoutine = StartCoroutine(FadingOutRoutine(delayDuration));
        }  
        else
        {
            FadingOutRoutineInfo oldInfo = fadingOutRoutineQueue.Peek();
            // if new delayDuration less than time before old hint appearance then interrupt old routine
            if (delayDuration < oldInfo.delayDuration - (Time.time - oldInfo.queryTime))
            {
                StopCoroutine(currentFadingOutRoutine);
                fadingOutRoutineQueue.Dequeue();
                fadingOutRoutineQueue.Enqueue(new FadingOutRoutineInfo(TextToShow, delayDuration, Time.time), delayDuration);
                currentFadingOutRoutine = StartCoroutine(FadingOutRoutine(delayDuration));
                fadingOutRoutineQueue.Enqueue(oldInfo, delayDuration + 1f); // make old routine second after new
            }
            else if (delayDuration < oldInfo.delayDuration)
            {
                fadingOutRoutineQueue.Dequeue(); // dequeue current routine info to make new routine second after it
                fadingOutRoutineQueue.Enqueue(new FadingOutRoutineInfo(TextToShow, delayDuration, Time.time), delayDuration);
            }
            else
            {
                fadingOutRoutineQueue.Enqueue(new FadingOutRoutineInfo(TextToShow, delayDuration, Time.time), delayDuration);
            }
        } 
    }


    private IEnumerator FadingOutRoutine(float delayDuration)
    {
        FadingOutRoutineInfo info = fadingOutRoutineQueue.Peek();
        if (delayDuration > 0)
            yield return new WaitForSeconds(delayDuration);
        
        TMPro.text = info.textToShow;
        yield return new WaitForSeconds(timeBeforeFadingOut);

        while (TMPro.alpha > 0f)
        {
            TMPro.alpha -= alphaDecrease;
            yield return alphaDecreaseCooldown;
        }
        TMPro.alpha = 1f;
        TMPro.text = "";
        if (info.textToShow == fadingOutRoutineQueue.Peek().textToShow)
            fadingOutRoutineQueue.Dequeue();

        if (fadingOutRoutineQueue.Count > 0)
        {
            FadingOutRoutineInfo newInfo = fadingOutRoutineQueue.Peek();
            currentFadingOutRoutine = StartCoroutine(FadingOutRoutine(newInfo.delayDuration - (Time.time - newInfo.queryTime)));
        }    
    }
}
