using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;

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
        Move bestMove = moves[0];
        
        int bestMoveUnsafePawn = 9;
        // to find the moves possible for pawns use Move.StartSquare and associate it with Piece.Square
        // whenever create dictionary we should update the target rank variable

        // loop through pawns if their position is behind target rank then move it or if en passant possible
        // use target rank - Piece.Square.File % 2 to decide if push pawn

        foreach (KeyValuePair<Piece, List<Move>> pawn in pawns)
        {
            if(pawn.Value.Count == 0)
            {
                continue;
            }

            // Move finalMove;
            // keep this
            foreach(Move move in pawn.Value)
            {
                if(move.IsEnPassant)
                {
                    return move;
                }
                
                int unSafePawns = 0;
                board.MakeMove(move);
                
                foreach(Move oppMove in board.GetLegalMoves(true))
                {
                    if (oppMove.CapturePieceType == PieceType.Pawn)
                    {
                        board.MakeMove(oppMove);
                        foreach(Move ourMove in board.GetLegalMoves(true))
                        {
                            if (oppMove.TargetSquare != ourMove.TargetSquare)
                            {
                                unSafePawns++;
                            }
                            
                            
                        }
                        board.UndoMove(oppMove);
                    }
                }
                board.UndoMove(move);
                Debug.WriteLine(unSafePawns);
                if (unSafePawns < bestMoveUnsafePawn) 
                {
                    bestMove = move;
                    bestMoveUnsafePawn = unSafePawns;

                }
            }

            // check if we did the possible move (may be multiple) would pawns be in danger - makemove
            // if currently pawns are in danger and moving another only puts more/keeps it in danger then we use other piece to protect - tryskipmove
            
            // priority is if we can find a pawn move where no pawns are unprotected then use that move
            

        }

        //if(pawn.Key.Square.Rank < targetRank - ((pawn.Key.Square.File % 2)*2))
        //{
        /*            return pawn.Value[0];      
                }*/

        // then we check for current position and we iterate all the moves and see if there is a situation in which all pawns are protected or more than the best scenario for the pawn moves

        // instead we need to decide if we need to protect, recapture, or move in different order
        // check the skip turn followup moves by black and the planned move and see if it's safe
        return bestMoveUnsafePawn != 9 ? bestMove : moves[0];

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