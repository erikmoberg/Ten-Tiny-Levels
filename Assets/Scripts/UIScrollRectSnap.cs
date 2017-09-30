using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScrollRectSnap : MonoBehaviour {
    public GridLayoutGroup gridLayoutGroup;
    private float minSpeed = 200.0f;
    private float snapSpeed = 8.0f;

    private ScrollRect scrollRect;
    private Vector2 target = Vector2.zero;
    private bool isLerping;
    private Vector2 ContentStartPosition;

    [HideInInspector]
    public string currentOption;

    void Start() 
    {
        this.scrollRect = this.GetComponent<ScrollRect>(); // Cache the scroll rect
        this.ContentStartPosition = this.scrollRect.content.position;
        this.target = this.ContentStartPosition;
        this.currentOption = this.gridLayoutGroup.transform.GetChild(0).name;
    }

    void Update() 
    {
        if (this.scrollRect.velocity.magnitude <= this.minSpeed)
        {
            if (this.scrollRect.content.position.x != this.target.x)
            {
                this.isLerping = true;
                this.scrollRect.content.position = Vector2.Lerp(this.scrollRect.content.position, this.target, this.snapSpeed * Time.deltaTime);
            }
            else
            {
                this.isLerping = false;
            }
        }
        else
        {
            this.isLerping = false;
        }
    }

    // The size for a single cell (element)
    Vector2 SingleCell 
    {
        get 
        {
            return new Vector2(
                this.gridLayoutGroup.cellSize.x + this.gridLayoutGroup.spacing.x, 
                this.gridLayoutGroup.cellSize.y + this.gridLayoutGroup.spacing.y);
        }
    }

    public void OnValueChanged(Vector2 normalized) 
    {
        if (!this.isLerping && this.scrollRect.horizontal == true) {

            // Find the closest target to the current normalization
            var smallestDistance = float.MaxValue;

            var closestIndex = 0;
            for (int i = 0; i < this.gridLayoutGroup.transform.childCount; i++) {
                var testDistance = Mathf.Abs(this.ContentStartPosition.x - this.SingleCell.x * i - this.scrollRect.content.position.x);
                if (testDistance < smallestDistance) 
                {
                    smallestDistance = testDistance;
                    closestIndex = i;
                }
            }

            this.currentOption = this.gridLayoutGroup.transform.GetChild(closestIndex).name;
            target.x = this.ContentStartPosition.x - this.SingleCell.x * closestIndex;
        }
    }

    public Transform GetChild(int index)
    {
        return this.gridLayoutGroup.transform.GetChild(index);
    }

    public int GetChildCount()
    {
        return this.gridLayoutGroup.transform.childCount;
    }
}