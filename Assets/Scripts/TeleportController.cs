using UnityEngine;
using System.Collections;

public class TeleportController : MonoBehaviour {

    public void OnTriggerEnter2D (Collider2D col)
    {
        var rigidBody = col.gameObject.GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            return;
        }

        var verticalOffset = 0f;
        var collider = col.gameObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            verticalOffset = collider.bounds.size.y;
        }

        if (gameObject.tag == TagNames.BottomTeleport && rigidBody.velocity.y < 0)
        {
            this.MoveToObjectWithTag(TagNames.TopTeleport, -verticalOffset, col.gameObject);
        }
        else if (gameObject.tag == TagNames.TopTeleport && rigidBody.velocity.y > 0)
        {
            this.MoveToObjectWithTag(TagNames.BottomTeleport, verticalOffset, col.gameObject);
        }
    }

    private void MoveToObjectWithTag(string objectTag, float verticalOffset, GameObject objectToMove)
    {
        var otherObject = GameObject.FindGameObjectWithTag(objectTag);

        if (otherObject != null)
        {
            // send to other object position
            objectToMove.transform.position = new Vector2(this.gameObject.transform.position.x, otherObject.transform.position.y + verticalOffset);
        }
    }
}
