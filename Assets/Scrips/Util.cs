using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {

	public static bool AreNeighbors(Hex h1, Hex h2)
    {
        return (h1.Col == h2.Col || h1.Row == h2.Row) && Mathf.Abs(h1.Col - h2.Col) <= 1 && Mathf.Abs(h1.Row - h2.Row) <= 1;
    }

    public static IEnumerator LerpToPoint(GameObject go, Vector3 end, float duration)
    {
        float startTime = Time.time;
        float frac;
        while(Mathf.Abs(Vector3.Distance(go.transform.position, end)) > 0.02f)
        {
            float covered = (Time.time - startTime) * Constants.MovaAnimationSpeed;
            frac = covered / duration;
            go.transform.position = Vector3.Lerp(go.transform.position, end, frac);
            yield return new WaitForEndOfFrame();
        }
        go.transform.position = end;
    }

}
