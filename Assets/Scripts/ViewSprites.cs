using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSprites : MonoBehaviour, IPieceClickHandler
{
    public Sprite sprite;
    public int splitX;
    public int splitY;
    public float xPadding;
    public float yPadding;
    public Vector3 startingPosition;
    public GameObject piecePrefab;
    public static UnityEngine.Events.UnityEvent clear = new UnityEngine.Events.UnityEvent();
    private PuzzlePieceBehaviour firstSelected;
    private List<PuzzlePieceBehaviour> puzzlePieceBehaviours = new List<PuzzlePieceBehaviour>();
    private bool moving = false;
    private bool isClear = false;

    private void Start()
    {
        Show(sprite);
    }
    public void Show(Sprite sprite)
    {
        Vector2Int tileSize = new Vector2Int(sprite.texture.width / splitX, sprite.texture.height / splitY);
        Sprite[] sprites = SplitSprite.SplitTextureToSprites(sprite.texture, tileSize);
        List<GameObject> pieceInstances = new List<GameObject>();
        for (int i = 0; i < sprites.Length; i++)
        {
            pieceInstances.Add(Instantiate(piecePrefab, transform));
        }
        if (sprites.Length > 0)
        {
            float cofX = (float)(sprites[0].rect.width) / sprites[0].pixelsPerUnit;
            float cofY = (float)(sprites[0].rect.height) / sprites[0].pixelsPerUnit;
            for (int i = 0; i < sprites.Length; i++)
            {
                SpriteRenderer spriteRenderer = pieceInstances[i].GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprites[i];
                pieceInstances[i].transform.position = new Vector3(startingPosition.x + (i % splitX) * (cofX + xPadding), startingPosition.y + (int)(i / splitY) * (cofY + yPadding), 0);
                PuzzlePieceBehaviour puzzlePieceBehaviour = pieceInstances[i].GetComponent<PuzzlePieceBehaviour>();
                puzzlePieceBehaviour.currentIndex = i;
                puzzlePieceBehaviour.originalIndex = i;
                puzzlePieceBehaviour.SetHandler(this);
                puzzlePieceBehaviours.Add(puzzlePieceBehaviour);
            }
            // Shuffle
            for (int i = 0; i < puzzlePieceBehaviours.Count; i++)
            {
                Vector3 temp;
                int targetIndex = Random.Range(0, puzzlePieceBehaviours.Count);
                Transform target = puzzlePieceBehaviours[targetIndex].transform;
                temp = puzzlePieceBehaviours[i].transform.position;
                puzzlePieceBehaviours[i].transform.position = target.position;
                target.position = temp;
                SwapIndexes(puzzlePieceBehaviours[i], puzzlePieceBehaviours[targetIndex]);
            }
            ResetMaterials();
        }
    }

    private void SwapIndexes(PuzzlePieceBehaviour pieceA, PuzzlePieceBehaviour pieceB)
    {
        int temp = pieceA.currentIndex;
        pieceA.currentIndex = pieceB.currentIndex;
        pieceB.currentIndex = temp;
    }

    private IEnumerator SwapPieces(Transform pieceA, Transform pieceB, UnityEngine.Events.UnityAction action)
    {
        const float speed = 5f;
        moving = true;
        Vector3 posA, posB;
        posA = pieceA.position;
        posB = pieceB.position;
        while ((pieceA.transform.position - posB).sqrMagnitude > 0.05f)
        {
            pieceA.position = Vector3.MoveTowards(pieceA.position, posB, Time.deltaTime * speed);
            pieceB.position = Vector3.MoveTowards(pieceB.position, posA, Time.deltaTime * speed);
            yield return null;
        }
        pieceA.position = posB;
        pieceB.position = posA;
        action();
    }

    public void OnClick(PuzzlePieceBehaviour piece)
    {
        if (moving || isClear)
        {
            return;
        }
        if (firstSelected)
        {
            if (firstSelected.Equals(piece))
            {
                firstSelected = null;
                piece.SetLine(false);
            }
            else
            {
                firstSelected.SetLine(false);
                piece.SetLine(false);
                SwapIndexes(firstSelected, piece);
                StartCoroutine(SwapPieces(firstSelected.transform, piece.transform, AfterSwap));
                firstSelected = null;

            }
        }
        else
        {
            firstSelected = piece;
            firstSelected.SetLine(true);
        }
    }

    private void AfterSwap()
    {
        moving = false;
        ResetMaterials();
        CheckClear();
    }

    private void ResetMaterials()
    {
        for (int i = 0; i < puzzlePieceBehaviours.Count; i++)
        {

            if (puzzlePieceBehaviours[i].originalIndex == puzzlePieceBehaviours[i].currentIndex)
            {
                puzzlePieceBehaviours[i].spriteRenderer.material = puzzlePieceBehaviours[i].fixedMaterial;
            }
        }
    }

    private void CheckClear()
    {
        for (int i = 0; i < puzzlePieceBehaviours.Count; i++)
        {
            if (puzzlePieceBehaviours[i].originalIndex != puzzlePieceBehaviours[i].currentIndex)
            {
                return;
            }
        }
        isClear = true;
        Debug.Log("Clear");
        clear.Invoke();
    }

}
