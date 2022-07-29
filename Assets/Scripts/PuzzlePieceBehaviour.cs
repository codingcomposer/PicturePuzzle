using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePieceBehaviour : MonoBehaviour, IPointerDownHandler
{
    public LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;
    public int currentIndex;
    public int originalIndex;
    public Material fixedMaterial;
    private IPieceClickHandler pieceClickHandler;

    private void Start()
    {
        SetLine(false);
        gameObject.AddComponent<BoxCollider2D>();
    }

    public void SetHandler(IPieceClickHandler _pieceClickHandler)
    {
        pieceClickHandler = _pieceClickHandler;
    }



    public void SetLine(bool enabled)
    {
        SetLineRendererPositions();
        lineRenderer.enabled = enabled;
    }


    private void SetLineRendererPositions()
    {
        lineRenderer.positionCount = 5;
        lineRenderer.SetPosition(0, spriteRenderer.bounds.min);
        lineRenderer.SetPosition(1, new Vector3(spriteRenderer.bounds.max.x, spriteRenderer.bounds.min.y, 0f));
        lineRenderer.SetPosition(2, spriteRenderer.bounds.max);
        lineRenderer.SetPosition(3, new Vector3(spriteRenderer.bounds.min.x, spriteRenderer.bounds.max.y, 0f));
        lineRenderer.SetPosition(4, lineRenderer.GetPosition(0));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!spriteRenderer.material.name.Contains("Original"))
        {
            pieceClickHandler.OnClick(this);
        }
    }
}
