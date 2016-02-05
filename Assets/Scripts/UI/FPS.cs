using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPS : MonoBehaviour {
	public float frequency = 0.5f;
	public int FramesPerSec { get; protected set; }
	// Use this for initialization
	void Start () {
		StartCoroutine(calc());
	}

	private IEnumerator calc() {
		for(;;){
			// Capture frame-per-second
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(frequency);
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;

			// Display it
			FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
			gameObject.GetComponent<Text>().text = string.Format("FPS: {0}", FramesPerSec);
		}
	}
}
