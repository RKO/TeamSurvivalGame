using UnityEngine;
using System.Collections;

public class DestroyWhenFinished : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ParticleSystem particle = GetComponent<ParticleSystem>();

        StartCoroutine(DelayedDestroy(particle.duration));
    }

    private IEnumerator DelayedDestroy(float time) {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }
}
