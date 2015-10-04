using UnityEngine;
using System.Collections;

public class CumulativeAppearTrigger : MonoBehaviour {

	public int timesAfterAppear = 5;

	int actualTimes = 0;
	float lastTouch = 0.0f;
	float timeBeforeTouch = 0.5f;

	bool needToWork = true;

	public GameObject sceneChanger;

	public GameObject[] toAppear;
	public GameObject[] toDisappear;

	void Start () {
		if (sceneChanger != null)
		{
			sceneChanger.GetComponent<sceneChanger>().toAppear = toAppear;
			sceneChanger.GetComponent<sceneChanger>().toDisappear = toDisappear;
		}

		setZeroAlphas();
	}

	void Update () {
		if (needToWork && (actualTimes>=timesAfterAppear))
		{
			needToWork = false;
			if (sceneChanger != null)
			{
				sceneChanger.SetActive(true);
				sceneChanger.GetComponent<sceneChanger>().c_manualActivation();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			if ((Time.time - lastTouch) > timeBeforeTouch)
			{
				lastTouch = Time.time;
				actualTimes ++;
			}
		}
	}

	void setZeroAlphas()
	{
		SpriteRenderer[] renderers = new SpriteRenderer[toAppear.Length];

		for (int i = 0; i < renderers.Length; i++)
		{
			if (toAppear[i] != null)
			{
				renderers[i] = toAppear[i].GetComponent<SpriteRenderer>();
				if (renderers[i] != null)
				{
					Color actColor = renderers[i].color;
					renderers[i].color = new Color(actColor.r, actColor.g, actColor.b, 0.0f);
				}
			}
		}
	}
}
