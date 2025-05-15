using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MatchFinder : MonoBehaviour
{
    public HashSet<Vector2Int> FindMatches(DiamondType[,] boardData)
    {
        HashSet<Vector2Int> _allMatches = new HashSet<Vector2Int>();
        DiamondType firstDiamond, secondDiamond, thirdDiamond;
        //Check for row
        for (int y = 0; y < boardData.GetLength(0); y++)
        {
            for (int x = 0; x < boardData.GetLength(1) - 2; x++)
            {
                firstDiamond = boardData[y, x];
                secondDiamond = boardData[y, x + 1];
                thirdDiamond = boardData[y, x + 2];
                if (firstDiamond == DiamondType.None || secondDiamond == DiamondType.None || thirdDiamond == DiamondType.None)
                {
                    continue;
                }
                if (firstDiamond == secondDiamond && secondDiamond == thirdDiamond)
                {
                    _allMatches.Add(new Vector2Int(x, y));
                    _allMatches.Add(new Vector2Int(x + 1, y));
                    _allMatches.Add(new Vector2Int(x + 2, y));
                }
            }
        }
        //Check for col
        for (int y = 0; y < boardData.GetLength(0) - 2; y++)
        {
            for (int x = 0; x < boardData.GetLength(1); x++)
            {
                firstDiamond = boardData[y, x];
                secondDiamond = boardData[y + 1, x];
                thirdDiamond = boardData[y + 2, x];
                if (firstDiamond == DiamondType.None || secondDiamond == DiamondType.None || thirdDiamond == DiamondType.None)
                {
                    continue;
                }
                if (firstDiamond == secondDiamond && secondDiamond == thirdDiamond)
                {
                    _allMatches.Add(new Vector2Int(x, y));
                    _allMatches.Add(new Vector2Int(x, y + 1));
                    _allMatches.Add(new Vector2Int(x, y + 2));
                }
            }
        }
        return _allMatches;
    }


}
