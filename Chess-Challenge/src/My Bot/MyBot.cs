using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MyBot : IChessBot
{
    // target rank variable is the furthest pushed pawn
    public int targetRank = 2;
    Dictionary<String, Move> tradeMap = new Dictionary<String, Move>();
    int bestMoveUnsafePawn = 9;

    public Move Think(Board board, Timer timer)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (true)
        {
            //some other processing to do possible
            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                break;
            }
        }

        Move[] moves = board.GetLegalMoves();
        PieceList pieces = board.GetPieceList(PieceType.Pawn, true);
        Dictionary<Piece, List<Move>> pawns = GetPawns(moves, pieces);
        // get all pawns and their associated Piece object Dictionary<Piece, List<Move>>
        Move bestMove = moves[0];

        String currentFen = board.GetFenString();
        foreach (KeyValuePair<String, Move> retakeMove in tradeMap)
        {
            if(currentFen == retakeMove.Key)
            {
                foreach (Move nextMove in moves)
                {
                    if(nextMove.Equals(retakeMove.Value))
                    {
                        return nextMove;
                    }
                }
            }
        }

        foreach (KeyValuePair<Piece, List<Move>> pawn in pawns)
        {
            if(pawn.Value.Count == 0)
            {
                continue;
            }

            foreach(Move move in pawn.Value)
            {
                if(move.IsEnPassant)
                {
                    return move;
                }

                if (IsBestMove(board, move))
                {
                    bestMove = move;
                }
            }
        }


        foreach (Move move in moves)
        {
            if(move.MovePieceType == PieceType.Pawn)
            {
                continue;
            }

            if(IsBestMove(board, move))
            {
                bestMove = move;
            }
        }

        if(bestMoveUnsafePawn == 9)
        {
            return moves[0];
        }

        bestMoveUnsafePawn = 9;
        return bestMove;

    }

    private bool IsBestMove(Board board, Move move)
    {
        int unSafePawns = 0;
        board.MakeMove(move);
        foreach (Move oppMove in board.GetLegalMoves(true))
        {
            if (oppMove.CapturePieceType == PieceType.Pawn)
            {
                board.MakeMove(oppMove);
                bool canRetake = false;
                foreach (Move ourMove in board.GetLegalMoves(true))
                {
                    if (oppMove.TargetSquare.Equals(ourMove.TargetSquare))
                    {
                        if (!tradeMap.ContainsKey(board.GetFenString()))
                            tradeMap.Add(board.GetFenString(), ourMove);
                        canRetake = true;
                    }
                }
                if (!canRetake)
                    unSafePawns++;

                board.UndoMove(oppMove);
            }
        }
        board.UndoMove(move);

        if (unSafePawns < bestMoveUnsafePawn)
        {
            bestMoveUnsafePawn = unSafePawns;
            return true;
        }

        return false;
    }

    private Dictionary<Piece, List<Move>> GetPawns(Move[] moves, PieceList pawns)
    {
        Dictionary<Piece, List<Move>> pawnsDict = new Dictionary<Piece, List<Move>>();
        foreach (Move move in moves)
        {
            if(move.MovePieceType == PieceType.Pawn)
            {
                for(int i = 0; i < pawns.Count; i++)
                {
                    Piece pawn = pawns.GetPiece(i);

                    if(targetRank < pawn.Square.Rank+1)
                    {
                        targetRank = pawn.Square.Rank+1;
                    }

                    if (pawn.Square.Equals(move.StartSquare))
                    {
                        if (!pawnsDict.ContainsKey(pawn))
                        {
                            pawnsDict.Add(pawn, new List<Move> { move });
                        } else
                        {
                            pawnsDict[pawn].Add(move);
                        }
                    
                    }
                }
            }
        }

        return pawnsDict;
    }
}