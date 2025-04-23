using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    public HashSet<Diamond> FindMatches(int width, int height, Diamond[,] board)
    {
        HashSet<Diamond> _allMatches = new HashSet<Diamond>();
        Diamond firstDiamond, secondDiamond, thirdDiamond;
        //Check for row
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width - 2; x++)
            {
                if(board[y, x] == null)
                {
                    return null;
                }
                firstDiamond = board[y, x];
                secondDiamond = board[y, x+1];
                thirdDiamond = board[y, x+2];
                if(firstDiamond.DiamondType == secondDiamond.DiamondType && secondDiamond.DiamondType == thirdDiamond.DiamondType)
                {
                    _allMatches.Add(firstDiamond);
                    _allMatches.Add(secondDiamond);
                    _allMatches.Add(thirdDiamond);
                }
            }
        }
        //Check for col
        for(int y = 0; y < height - 2; y++)
        {
            for(int x = 0; x < width; x++)
            {
                firstDiamond = board[y, x];
                secondDiamond = board[y+1, x];
                thirdDiamond = board[y+2, x];
                if(firstDiamond.DiamondType == secondDiamond.DiamondType && secondDiamond.DiamondType == thirdDiamond.DiamondType)
                {
                    _allMatches.Add(firstDiamond);
                    _allMatches.Add(secondDiamond);
                    _allMatches.Add(thirdDiamond);
                }
            }
        }
        return _allMatches;
    }


}
