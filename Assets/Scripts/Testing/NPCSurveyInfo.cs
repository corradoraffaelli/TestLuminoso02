using UnityEngine;
using System.Collections;

public class NPCSurveyInfo : MonoBehaviour {

	int contentIndex = -1;
	int sectionIndex = -1;

	public void c_setContentInt(int contentIndexInput)
	{
		contentIndex = contentIndexInput;
	}

	public void c_setSectionInt(int sectionIndexInput)
	{
		sectionIndex = sectionIndexInput;
	}

	public void wrongAnswer()
	{
		if (contentIndex != -1 && sectionIndex != -1)
			GeneralFinder.informativeManager.c_addWrongTrial (sectionIndex, contentIndex);
		else
			Debug.Log ("CONTENT E SECTION DEL QUESTIONARIO NON ASSEGNATE CORRETTAMENTE");
	}

	public void correctAnswer()
	{
		if (contentIndex != -1 && sectionIndex != -1)
			GeneralFinder.informativeManager.c_setQuestionAnswered(sectionIndex,contentIndex);
		else
			Debug.Log ("CONTENT E SECTION DEL QUESTIONARIO NON ASSEGNATE CORRETTAMENTE");
	}
}
