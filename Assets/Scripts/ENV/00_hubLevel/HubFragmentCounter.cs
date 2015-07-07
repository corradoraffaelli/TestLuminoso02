using UnityEngine;
using System.Collections;

public class HubFragmentCounter : MonoBehaviour {

	public int fragmentTotalNumber = 0;

	void Start () {
		CountFragment();
	}

	void CountFragment()
	{
		InformativeSection[] sections = GeneralFinder.informativeManager.sections;
		for (int i = 0; i < sections.Length; i++)
		{
			if (sections[i] != null && sections[i].contentType == infoContentType.Fragments)
			{
				for (int j = 0; j < sections[i].contents.Length; j++)
				{
					if (sections[i].contents[j] != null)
					{
						if (!sections[i].contents[j].locked)
							fragmentTotalNumber ++;
					}
				}
			}
		}
	}
}
