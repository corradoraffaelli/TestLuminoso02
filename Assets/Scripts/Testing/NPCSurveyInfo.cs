using UnityEngine;
using System.Collections;

public class NPCSurveyInfo : MonoBehaviour {

	int contentIndex = 0;
	int sectionIndex = 0;

	public void c_setContentInt(int contentIndexInput)
	{
		contentIndex = contentIndexInput;
	}

	public void c_setSectionInt(int sectionIndexInput)
	{
		sectionIndex = sectionIndexInput;
	}

	public void wrongAnswer(int section, int content)
	{
		GeneralFinder.informativeManager.c_addWrongTrial(sectionIndex,contentIndex);
	}

	public void correctAnswer(int section, int content)
	{
		GeneralFinder.informativeManager.c_setQuestionAnswered(sectionIndex,contentIndex);
	}
}
