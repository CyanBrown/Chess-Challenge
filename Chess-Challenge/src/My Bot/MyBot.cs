using ChessChallenge.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

public class MyBot : IChessBot
{
    // target rank variable is the furthest pushed pawn
    public int targetRank = 2;

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        PieceList pieces = board.GetPieceList(PieceType.Pawn, true);
        Dictionary<Piece, List<Move>> pawns = GetPawns(moves, pieces);
        // get all pawns and their associated Piece object Dictionary<Piece, List<Move>>
        // to find the moves possible for pawns use Move.StartSquare and associate it with Piece.Square
        // whenever create dictionary we should update the target rank variable

        // loop through pawns if their position is behind target rank then move it or if en passant possible
        // use target rank - Piece.Square.File % 2 to decide if push pawn

        foreach(KeyValuePair<Piece, List<Move>> pawn in pawns)
        {
            if(pawn.Value.Count == 0)
            {
                continue;
            }
            if(pawn.Key.Square.Rank < targetRank - ((pawn.Key.Square.File % 2)*2))
            {
                // Move finalMove;
                foreach(Move move in pawn.Value)
                {
                    if(move.IsEnPassant)
                    {
                        return move;
                    }
                }
                return pawn.Value[0];

            }
        }

        return moves[0];

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