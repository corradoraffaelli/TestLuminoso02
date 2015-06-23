using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum cellArea {
	WholeSpace,
	Fixed,
}



public class GridHandler : MonoBehaviour {

	GridLayoutGroup myGrid;
	GridLayoutGroup []grids;
	RectTransform myRectTransform;

	public CellDisposition myGridCell;


	// Use this for initialization
	void Start () {

		myGrid = GetComponent<GridLayoutGroup> ();

		grids = GetComponentsInChildren<GridLayoutGroup> ();

		myRectTransform = GetComponent<RectTransform> ();

		//myRectTransform.rect.width

		setGridsWidth ();

	}

	void setGridsWidth() {

		if (myGrid != null) {

			float horizSize;
			float horizSpace;
			float vertSize;
			float vertSpace;

			if(myGridCell.verticalArea== cellArea.WholeSpace) {
				vertSize = (myRectTransform.rect.height / transform.childCount) * 0.8f;
				vertSpace = (myRectTransform.rect.height / transform.childCount) * 0.2f;
			}
			else {
				vertSize = myRectTransform.rect.height * myGridCell.verticalSizePerc;
				vertSpace = myRectTransform.rect.height * myGridCell.verticalSpacePerc;

			}

			if(myGridCell.horizontalArea== cellArea.WholeSpace) {
				horizSize = myRectTransform.rect.width * 0.9f;
				horizSpace = myRectTransform.rect.width * 0.1f;
			}
			else {
				horizSize = myRectTransform.rect.width * myGridCell.horizontalSizePerc;
				horizSpace = myRectTransform.rect.width * myGridCell.horizontalSpacePerc;
				
			}

			myGrid.cellSize = new Vector2 (myRectTransform.rect.width * 0.9f, vertSize);
			
			myGrid.spacing = new Vector2 (myRectTransform.rect.width * 0.1f, vertSpace);

		}

		if (grids != null) {

			foreach (GridLayoutGroup grid in grids) {

				grid.cellSize = new Vector2 (myRectTransform.rect.width * 0.8f, grid.cellSize.y);

			}

		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

[System.Serializable]
public class CellDisposition {

	public cellArea verticalArea;
	public float verticalSizePerc;
	public float verticalSpacePerc;
	
	public cellArea horizontalArea;
	public float horizontalSizePerc;
	public float horizontalSpacePerc;


}
