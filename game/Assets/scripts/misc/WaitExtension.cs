using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//Extention to wait for common purposes
public static class WaitExtension
{
    public static void Wait(this MonoBehaviour mono, float delay, UnityAction action) 
    {
        _ = mono.StartCoroutine(ExecuteAction(delay, action));
    }

    private static IEnumerator ExecuteAction(float delay, UnityAction action) 
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
        yield break;
    }
}
