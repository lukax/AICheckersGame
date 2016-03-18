using System.Diagnostics;
using System.Windows.Forms;

namespace IACheckers.UI.Forms.Game
{
    public class CheckersMinimax
    {
        private readonly int _computerPieceColor;
        private readonly CheckersBoardGameEngine _currentBoardGameEngine;
        public int DefaultMinimaxTreeDepth { get; set; } = 1;

        private readonly int[] _boardTableWeights =
        {
            4, 4, 4, 4,
            4, 3, 3, 3,
            3, 2, 2, 4,
            4, 2, 1, 3,
            3, 1, 2, 4,
            4, 2, 2, 3,
            3, 3, 3, 4,
            4, 4, 4, 4
        };

        public CheckersMinimax(CheckersBoardGameEngine gameBoardGameEngine)
        {
            _currentBoardGameEngine = gameBoardGameEngine;
            _computerPieceColor = CheckersBoardGameEngine.BlackPiece;
        }

        public void ComputerPlay()
        {
            try
            {
                var moves = GetComputerMinimaxMovements(_currentBoardGameEngine);

                if (!moves.IsEmpty())
                    _currentBoardGameEngine.ApplyMove(moves);
            }
            catch (InvalidPlayerMoveException ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Application.Exit();
            }
        }
        
        private bool IsValidMove(JavaList<object> moves)
        {
            return !moves.IsEmpty() && !((JavaList<object>) moves.peek_head()).IsEmpty();
        }
        
        private JavaList<object> GetComputerMinimaxMovements(CheckersBoardGameEngine boardGameEngine)
        {
            JavaList<object> bestMove = null;
            int maxValue = int.MinValue;

            var sucessors = boardGameEngine.FindAllLegalMovesForCurrentPlayer();
            while (IsValidMove(sucessors))
            {
                var move = (JavaList<object>) sucessors.pop_front();
                var nextBoard = boardGameEngine.Clone();

                nextBoard.ApplyMove(move);
                var value = PlayerMinMove(nextBoard, 1, maxValue, int.MaxValue);

                if (value > maxValue)
                {
                    Debug.WriteLine("Max value : " + value + " at depth : 0");
                    maxValue = value;
                    bestMove = move;
                }
            }

            Debug.WriteLine("Move value selected : " + maxValue + " at depth : 0");

            return bestMove;
        }
        
        private int PlayerMaxMove(CheckersBoardGameEngine boardGameEngine, int minimaxTreeDepth, int alphaCutOff, int betaCutOff)
        {
            if (TreeCutOffTest(boardGameEngine, minimaxTreeDepth))
                return GetCurrentPlayerStrength(boardGameEngine);

            Debug.WriteLine("Max node at depth : " + minimaxTreeDepth + " with alpha : " + alphaCutOff +
                            " beta : " + betaCutOff);

            var sucessors = boardGameEngine.FindAllLegalMovesForCurrentPlayer();
            while (IsValidMove(sucessors))
            {
                var move = (JavaList<object>) sucessors.pop_front();
                var nextBoard = boardGameEngine.Clone();
                nextBoard.ApplyMove(move);
                var value = PlayerMinMove(nextBoard, minimaxTreeDepth + 1, alphaCutOff, betaCutOff);

                if (value > alphaCutOff)
                {
                    alphaCutOff = value;
                    Debug.WriteLine("Max value : " + value + " at depth : " + minimaxTreeDepth);
                }

                if (alphaCutOff > betaCutOff)
                {
                    Debug.WriteLine("Max value with prunning : " + betaCutOff + " at depth : " + minimaxTreeDepth);
                    Debug.WriteLine(sucessors.Count + " sucessors left");
                    return betaCutOff;
                }
            }

            Debug.WriteLine("Max value selected : " + alphaCutOff + " at depth : " + minimaxTreeDepth);
            return alphaCutOff;
        }
        
        private int PlayerMinMove(CheckersBoardGameEngine boardGameEngine, int minimaxTreeDepth, int alphaCutoff, int betaCutOff)
        {
            if (TreeCutOffTest(boardGameEngine, minimaxTreeDepth))
                return GetCurrentPlayerStrength(boardGameEngine);

            Debug.WriteLine("Min node at depth : " + minimaxTreeDepth + " with alpha : " + alphaCutoff +
                            " beta : " + betaCutOff);

            var sucessors = boardGameEngine.FindAllLegalMovesForCurrentPlayer();
            while (IsValidMove(sucessors))
            {
                var move = (JavaList<object>) sucessors.pop_front();
                var nextBoard = boardGameEngine.Clone();
                nextBoard.ApplyMove(move);
                var value = PlayerMaxMove(nextBoard, minimaxTreeDepth + 1, alphaCutoff, betaCutOff);

                if (value < betaCutOff)
                {
                    betaCutOff = value;
                    Debug.WriteLine("Min value : " + value + " at depth : " + minimaxTreeDepth);
                }

                if (betaCutOff < alphaCutoff)
                {
                    Debug.WriteLine("Min value with prunning : " + alphaCutoff + " at depth : " + minimaxTreeDepth);
                    Debug.WriteLine(sucessors.Count + " sucessors left");
                    return alphaCutoff;
                }
            }

            Debug.WriteLine("Min value selected : " + betaCutOff + " at depth : " + minimaxTreeDepth);
            return betaCutOff;
        }
        
        private int GetCurrentPlayerStrength(CheckersBoardGameEngine boardGameEngine)
        {
            var colorForce = 0;
            var enemyForce = 0;

            int colorKing = _computerPieceColor == CheckersBoardGameEngine.WhitePiece ? CheckersBoardGameEngine.WhiteKingPiece : CheckersBoardGameEngine.BlackKing;

            try
            {
                for (var i = 0; i < 32; i++)
                {
                    int piece = boardGameEngine.GetPieceAtPosition(i);

                    if (piece != CheckersBoardGameEngine.EmptyPiece)
                        if (piece == _computerPieceColor || piece == colorKing)
                            colorForce += GetPieceStrength(piece, i);
                        else
                            enemyForce += GetPieceStrength(piece, i);
                }
            }
            catch (InvalidBoardCoordinateException ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Application.Exit();
            }

            return colorForce - enemyForce;
        }
        
        private int GetPieceStrength(int pieceColor, int pos)
        {
            int value;

            if (pieceColor == CheckersBoardGameEngine.WhitePiece) 
                if (pos >= 4 && pos <= 7)
                    value = 7;
                else
                    value = 5;
            else if (pieceColor != CheckersBoardGameEngine.BlackPiece) 
                if (pos >= 24 && pos <= 27)
                    value = 7;
                else
                    value = 5;
            else // KING
                value = 10;

            return value*_boardTableWeights[pos];
        }

        private bool TreeCutOffTest(CheckersBoardGameEngine boardGameEngine, int minimaxTreeDepth)
        {
            return minimaxTreeDepth > DefaultMinimaxTreeDepth || boardGameEngine.IsGameOver();
        }
    }
}