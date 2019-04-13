﻿using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle_Quest_3.Assets.Scripts.Extensions;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        board.doForEveryDot((c, r) =>
        {
            GameObject currentDot = board.allDotsOnBoard[c, r];
            currentDot?.Also(dot =>
            {
                if (c > 0 && c < board.width - 1)
                {
                    var leftDot = board.allDotsOnBoard[c - 1, r];
                    var rightDot = board.allDotsOnBoard[c + 1, r];

                    if (leftDot != null && rightDot != null && leftDot.tag == dot.tag && rightDot.tag == dot.tag)
                    {
                        if (isAnyDotOfType(DotType.RowBomb, dot, leftDot))
                        {
                            currentMatches.Union(GetDotsForRowAndSetMatched(r));
                        }
                        if (isAnyDotOfType(DotType.ColumnBomb, dot))
                        {
                            currentMatches.Union(GetDotsForColumnAndSetMatched(c));
                        }
                        if (isAnyDotOfType(DotType.ColumnBomb, leftDot))
                        {
                            currentMatches.Union(GetDotsForColumnAndSetMatched(c - 1));
                        }
                        if (isAnyDotOfType(DotType.ColumnBomb, rightDot))
                        {
                            currentMatches.Union(GetDotsForColumnAndSetMatched(c + 1));
                        }
                        AddToListAndMatch(dot);
                        AddToListAndMatch(leftDot);
                        AddToListAndMatch(rightDot);
                    }
                }
                if (r > 0 && r < board.height - 1)
                {
                    var upDot = board.allDotsOnBoard[c, r + 1];
                    var downDot = board.allDotsOnBoard[c, r - 1];

                    if (upDot != null && downDot != null && upDot.tag == dot.tag && downDot.tag == dot.tag)
                    {
                        if (isAnyDotOfType(DotType.ColumnBomb, dot, upDot, downDot))
                        {
                            currentMatches.Union(GetDotsForColumnAndSetMatched(c));
                        }
                        if (isAnyDotOfType(DotType.RowBomb, dot))
                        {
                            currentMatches.Union(GetDotsForRowAndSetMatched(r));
                        }
                        if (isAnyDotOfType(DotType.RowBomb, upDot))
                        {
                            currentMatches.Union(GetDotsForRowAndSetMatched(r + 1));
                        }
                        if (isAnyDotOfType(DotType.RowBomb, downDot))
                        {
                            currentMatches.Union(GetDotsForRowAndSetMatched(r - 1));
                        }
                        AddToListAndMatch(dot);
                        AddToListAndMatch(upDot);
                        AddToListAndMatch(downDot);
                    }
                }
            });
        });
    }

    private List<GameObject> GetDotsForColumnAndSetMatched(int column)
    {
        var dots = new List<GameObject>();
        board.doForColumn(column, (c, r) =>
        {
            if (board.allDotsOnBoard[c, r] != null)
            {
                dots.Add(board.allDotsOnBoard[c, r]);
                board.allDotsOnBoard[c, r].GetComponent<Dot>().isMatched = true;
            }
        });
        return dots;
    }

    private List<GameObject> GetDotsForRowAndSetMatched(int row)
    {
        var dots = new List<GameObject>();
        board.doForRow(row, (c, r) =>
        {
            if (board.allDotsOnBoard[c, r] != null)
            {
                dots.Add(board.allDotsOnBoard[c, r]);
                board.allDotsOnBoard[c, r].GetComponent<Dot>().isMatched = true;
            }
        });
        return dots;
    }

    private bool isAnyDotOfType(DotType dotType, params GameObject[] dots)
    {
        return dots.Any(x => x.GetComponent<Dot>().dotType == dotType);
    }

    public void CheckBombs()
    {
        if (board.currentDot == null)
        {
            return;
        }
        if (board.currentDot.isMatched)
        {
            // unmatch and swap for a bomb
            board.currentDot.MakeBomb(DotTypeHelpers.GetRandomDirectionBomb());
        }
        else if (board.currentDot.otherDot != null)
        {
            var otherDot = board.currentDot.otherDot.GetComponent<Dot>();
            if (otherDot.isMatched)
            {
                otherDot.MakeBomb(DotTypeHelpers.GetRandomDirectionBomb());
            }
        }
    }

    public void MatchDotsOfColor(string color)
    {
        board.doForEveryDot((c, r) =>
        {
            if (board.allDotsOnBoard[c, r] != null && board.allDotsOnBoard[c, r].tag == color)
            {
                board.allDotsOnBoard[c, r].GetComponent<Dot>().isMatched = true;
            }
        });
    }

    private void AddToListAndMatch(GameObject dot){
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
            dot.GetComponent<Dot>().isMatched = true;
        }
    }
}
