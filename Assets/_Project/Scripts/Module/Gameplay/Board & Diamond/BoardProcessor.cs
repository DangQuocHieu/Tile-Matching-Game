using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoardProcessor : MonoBehaviour
{
    [SerializeField] private Diamond[] _diamondPrefabs;
    private List<Diamond> _availableDiamond;

    [Header("Animation Config")]
    [SerializeField] private DropAnimationSO _dropAnim;
    [SerializeField] private ScaleAmimationSO _scaleAnim;
    [SerializeField] private SwapAnimationSO _swapAnim;

    public void GenerateBoard(Diamond[,] board, Transform diamondContainer)
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                _availableDiamond = _diamondPrefabs.ToList();
                if (HasMatchAtRow(board, x, y))
                {
                    _availableDiamond = _availableDiamond.Where(t => t.DiamondType != board[y, x - 1].DiamondType).ToList();
                }
                if (HasMatchAtColumn(board, x, y))
                {
                    _availableDiamond = _availableDiamond.Where(t => t.DiamondType != board[y - 1, x].DiamondType).ToList();
                }
                Diamond newDiamond = Instantiate(_availableDiamond[UnityEngine.Random.Range(0, _availableDiamond.Count)],
                    new Vector3(x, y, 0), Quaternion.identity, diamondContainer);
                board[y, x] = newDiamond;
            }
        }
    }

    private bool HasMatchAtRow(Diamond[,] board, int x, int y)
    {
        if (x >= 2)
        {
            if (board[y, x - 1].DiamondType == board[y, x - 2].DiamondType) return true;
        }
        return false;
    }

    private bool HasMatchAtColumn(Diamond[,] board, int x, int y)
    {
        if (y >= 2)
        {
            if (board[y - 1, x].DiamondType == board[y - 2, x].DiamondType) return true;
        }
        return false;
    }

    public void ClearBoard(Diamond[,] board)
    {
        if (board == null) return;
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[y, x] != null)
                {
                    Destroy(board[y, x].gameObject);
                    MessageManager.SendMessage(new Message(GameMessageType.OnDiamondDestroy, new object[] {board[y,x].DiamondType}));
                    board[y, x] = null;
                }
            }
        }
    }


    public IEnumerator RefillBoard(Diamond[,] board, Transform diamondContainer)
    {
        Sequence sequence = DOTween.Sequence();
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[y, x] == null)
                {
                    Diamond newDiamond = Instantiate(_diamondPrefabs[UnityEngine.Random.Range(0, _diamondPrefabs.Length)],
                        new Vector3(x, y + board.GetLength(0), 0), Quaternion.identity, diamondContainer);
                    sequence.Join(_dropAnim.Drop(newDiamond.gameObject, y));
                    board[y, x] = newDiamond;
                }
            }
        }

        yield return sequence.Play().WaitForCompletion();
        yield return new WaitForEndOfFrame();
    }


    public IEnumerator CollapseBoard(Diamond[,] board)
    {
        Sequence sequence = DOTween.Sequence();
        for (int x = 0; x < board.GetLength(1); x++)
        {
            for (int y = 0; y < board.GetLength(0); y++)
            {
                if (board[y, x] == null)
                {
                    for (int index = y + 1; index < board.GetLength(0); index++)
                    {
                        if (board[index, x] != null)
                        {
                            board[y, x] = board[index, x];
                            board[index, x] = null;
                            sequence.Join(_dropAnim.Drop(board[y, x].gameObject, y));
                            break;
                        }
                    }
                }
            }
        }
        yield return sequence.Play().WaitForCompletion();
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator ClearAllMatchDiamond(HashSet<Diamond> allMatches)
    {
        Sequence sequence = DOTween.Sequence();
        foreach (var diamond in allMatches)
        {
            sequence.Join(_scaleAnim.ScaleOut(diamond.gameObject, () =>
            {
                //Send Message: Diamond Destroy
                Destroy(diamond.gameObject);
                MessageManager.SendMessage(new Message(GameMessageType.OnDiamondDestroy, new object[] {diamond.DiamondType}));
            }));
        }
        yield return sequence.Play().WaitForCompletion();
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator Swap(GameObject prev, GameObject curr, TweenCallback callback)
    {
        yield return _swapAnim.Swap(prev, curr, callback);
    }
}
