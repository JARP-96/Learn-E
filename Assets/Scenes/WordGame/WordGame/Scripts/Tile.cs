using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public char letter;
    private Vector2 startingPosition;
    private List<Transform> touchingTiles;
    private Transform parent;
    private AudioSource audioSource;

    private void Start()
    {
        startingPosition = transform.position;
        touchingTiles = new List<Transform>();
        parent = transform.parent;
        audioSource = gameObject.GetComponent<AudioSource>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    public void PickUp()
    {
        transform.localScale = new Vector3(1.1f,1.1f,1.1f);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    public void Drop()
    {
        transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        Vector2 newPosition;
        if (touchingTiles.Count == 0)
        {
            transform.position = startingPosition;
            transform.parent = parent;
            return;
        }

        var currentCell = touchingTiles[0];
        if (touchingTiles.Count == 1)
        {
            newPosition = currentCell.position;
        }
        else
        {
            var distance = Vector2.Distance(transform.position, touchingTiles[0].position);

            foreach (Transform cell in touchingTiles)
            {
                if (Vector2.Distance(transform.position, cell.position) < distance)
                {
                    currentCell = cell;
                    distance = Vector2.Distance(transform.position, cell.position);
                }
            }
            newPosition = currentCell.position;
        }
        if (currentCell.childCount != 0)
        {
            transform.position = startingPosition;
            transform.parent = parent;
            return;
        }
        else
        {
            transform.parent = currentCell;
            StartCoroutine(SlotIntoPlace(transform.position, newPosition));
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Cell") return;
        if (!touchingTiles.Contains(other.transform))
        {
            touchingTiles.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Cell") return;
        if (touchingTiles.Contains(other.transform))
        {
            touchingTiles.Remove(other.transform);
        }
    }

    IEnumerator SlotIntoPlace(Vector2 startingPos, Vector2 endingPos)
    {
        float duration = 0.1f;
        float elapsedTime = 0;
        audioSource.Play();
        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startingPos, endingPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endingPos;
    }
}
