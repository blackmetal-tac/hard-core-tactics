using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class ProgressExtension
{
    public static void Progress(this MonoBehaviour mono, float delay, UnityAction action)
    {
        _ = mono.StartCoroutine(Load(delay, action));
    }

    private static IEnumerator Load(float duration, UnityAction action)
    {
        float time = 0.0f;        
        while (time < duration)
        {
            action.Invoke();
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
