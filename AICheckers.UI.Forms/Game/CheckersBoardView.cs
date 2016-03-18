using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace IACheckers.UI.Forms.Game
{
    public class CheckersBoardView : Control
    {
        private static readonly int PIECE_SIZE = 0;
        private static readonly int KING_SIZE = 3;
        private readonly CheckersBoardGameEngine _boardGameEngine;
        private Stack _boardStack;
        private int _cellWidth;
        private readonly CheckersMinimax _checkersMinimax;
        private readonly JavaList<object> _selectedPositions;
        private int _cornerStartX;
        private int _cornerStartY;

        public CheckersBoardView()
        {
            _selectedPositions = new JavaList<object>();
            _boardGameEngine = new CheckersBoardGameEngine();
            _checkersMinimax = new CheckersMinimax(_boardGameEngine);
            ResetBoard();
        }

        public void ResetBoard()
        {
            _boardStack = new Stack();
        }

        private bool GameOver()
        {
            bool result;

            if (_boardGameEngine.IsGameOver())
            {
                if (_boardGameEngine.GetWinner() == CheckersBoardGameEngine.BlackPiece)
                    MessageBox.Show("YOU WIN");
                else
                    MessageBox.Show("YOU LOSE");
                result = true;
            }
            else
                result = false;

            return result;
        }

        protected override void OnPaint(PaintEventArgs ev)
        {
            var g = ev.Graphics;
            var d = ClientSize;
            int marginX;
            int marginY;
            int incValue;

            if (d.Width < d.Height)
            {
                marginX = 0;
                marginY = (d.Height - d.Width)/2;

                incValue = d.Width/8;
            }
            else
            {
                marginX = (d.Width - d.Height)/2;
                marginY = 0;

                incValue = d.Height/8;
            }

            _cornerStartX = marginX;
            _cornerStartY = marginY;
            _cellWidth = incValue;

            DrawCheckersBoard(g, marginX, marginY, incValue);
            DrawCheckersPieces(g, marginX, marginY, incValue);
        }

        private void DrawCheckersBoard(Graphics g, int marginX, int marginY, int incValue)
        {
            for (var y = 0; y < 8; y++)
                for (var x = 0; x < 8; x++)
                {
                    Brush cellColor = (x + y)%2 == 0 ? new SolidBrush(Color.Ivory) : new SolidBrush(Color.DarkSlateGray);

                    g.FillRectangle(cellColor, marginX + x*incValue, marginY + y*incValue, incValue - 1, incValue - 1);
                }
        }

        private void DrawCheckersPieces(Graphics g, int marginX, int marginY, int incValue)
        {
            for (var i = 0; i < 32; i++)
                try
                {
                    if (_boardGameEngine.GetPieceAtPosition(i) != CheckersBoardGameEngine.EmptyPiece)
                    {
                        Brush pieceColor;
                        if (_boardGameEngine.GetPieceAtPosition(i) == CheckersBoardGameEngine.BlackPiece ||
                            _boardGameEngine.GetPieceAtPosition(i) == CheckersBoardGameEngine.BlackKing)
                        {
                            pieceColor = new SolidBrush(Color.Brown);
                            if (_selectedPositions.Contains(i))
                            {
                                pieceColor = new SolidBrush(Color.DarkRed);
                            }
                        }
                        else
                            pieceColor = new SolidBrush(Color.LightGray);

                        var y = i/4;
                        var x = i%4*2 + (y%2 == 0 ? 1 : 0);
                        g.FillEllipse(pieceColor, PIECE_SIZE + marginX + x*incValue, PIECE_SIZE + marginY + y*incValue,
                            incValue - 1 - 2*PIECE_SIZE, incValue - 1 - 2*PIECE_SIZE);

                        if (_boardGameEngine.GetPieceAtPosition(i) == CheckersBoardGameEngine.WhiteKingPiece)
                        {
                            pieceColor = new SolidBrush(Color.Black);
                            g.DrawEllipse(new Pen(pieceColor), KING_SIZE + marginX + x*incValue,
                                KING_SIZE + marginY + y*incValue,
                                incValue - 1 - 2*KING_SIZE, incValue - 1 - 2*KING_SIZE);
                        }
                        else if (_boardGameEngine.GetPieceAtPosition(i) == CheckersBoardGameEngine.BlackKing)
                        {
                            pieceColor = new SolidBrush(Color.White);
                            g.DrawEllipse(new Pen(pieceColor), KING_SIZE + marginX + x*incValue,
                                KING_SIZE + marginY + y*incValue,
                                incValue - 1 - 2*KING_SIZE, incValue - 1 - 2*KING_SIZE);
                        }
                    }
                }
                catch (InvalidBoardCoordinateException bad)
                {
                    Debug.WriteLine(bad.StackTrace);
                    Application.Exit();
                }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var pos = GetBoardPiecePos(e.X, e.Y);
            if (pos != -1)
                try
                {
                    int piece = _boardGameEngine.GetPieceAtPosition(pos);

                    if (piece != CheckersBoardGameEngine.EmptyPiece &&
                        (((piece == CheckersBoardGameEngine.WhitePiece || piece == CheckersBoardGameEngine.WhiteKingPiece) &&
                          _boardGameEngine.GetCurrentPlayer() == CheckersBoardGameEngine.WhitePiece) ||
                         ((piece == CheckersBoardGameEngine.BlackPiece || piece == CheckersBoardGameEngine.BlackKing) &&
                          _boardGameEngine.GetCurrentPlayer() == CheckersBoardGameEngine.BlackPiece)))
                    {
                        if (_selectedPositions.IsEmpty())
                        {
                            _selectedPositions.push_back(pos);
                        }
                        else
                        {
                            var temp = (int) _selectedPositions.peek_tail();
                            if (temp == pos) { 
                                _selectedPositions.pop_back();
                            }
                            else
                            {
                                MessageBox.Show("WAT ARE U TRYIN' TO DO?");
                            }
                        }
                        Invalidate();
                        Update();
                    }
                    else
                    {
                        var isValidMove = false;
                        if (!_selectedPositions.IsEmpty())
                        {
                            CheckersBoardGameEngine tempBoardGameEngine;
                            if (_boardStack.Count == 0)
                            {
                                tempBoardGameEngine = _boardGameEngine.Clone();
                                _boardStack.Push(tempBoardGameEngine);
                            }
                            else
                            {
                                tempBoardGameEngine = (CheckersBoardGameEngine) _boardStack.Peek();
                            }
                            var from = (int) _selectedPositions.peek_tail();
                            if (tempBoardGameEngine.IsValidMove(from, pos))
                            {
                                tempBoardGameEngine = tempBoardGameEngine.Clone();
                                var isAttacking = tempBoardGameEngine.CanCurrentPlayerAttack();
                                tempBoardGameEngine.ApplyMove(from, pos);
                                if (isAttacking && tempBoardGameEngine.CanAttackPosition(pos))
                                {
                                    _selectedPositions.push_back(pos);
                                    _boardStack.Push(tempBoardGameEngine);
                                }
                                else
                                {
                                    _selectedPositions.push_back(pos);
                                    ApplyPlayerMove(_selectedPositions, _boardGameEngine);
                                    _boardStack = new Stack();
                                }
                                isValidMove = true;
                            }
                            else if (from == pos)
                            {
                                _selectedPositions.pop_back();
                                _boardStack.Pop();
                                isValidMove = true;
                            }
                        }
                        if (!isValidMove)
                        {
                            MessageBox.Show("CAN'T DO THAT BRAH");
                        }
                        else
                        {
                            Invalidate();
                            Update();
                        }
                    }
                }
                catch (InvalidBoardCoordinateException bad)
                {
                    Debug.WriteLine(bad.StackTrace);
                    Application.Exit();
                }
                catch (InvalidPlayerMoveException bad)
                {
                    Debug.WriteLine(bad.StackTrace);
                    Application.Exit();
                }
        }

        private void ApplyPlayerMove(JavaList<object> moves, CheckersBoardGameEngine boardGameEngine)
        {
            var moveList = new JavaList<object>();

            var @from = (int) moves.pop_front();
            while (!moves.IsEmpty())
            {
                var to = (int) moves.pop_front();
                moveList.push_back(new PlayerMove(from, to));
                from = to;
            }

            boardGameEngine.ApplyMove(moveList);
            Invalidate();
            Update();
            _selectedPositions.Clear();
            ResetBoard();

            if (!GameOver())
            {
                Thread.Sleep(1000);
                _checkersMinimax.ComputerPlay();
                Invalidate();
                Update();
            }
        }

        private int GetBoardPiecePos(int currentX, int currentY)
        {
            for (var i = 0; i < 32; i++)
            {
                var y = i/4;
                var x = i%4*2 + (y%2 == 0 ? 1 : 0);
                if (_cornerStartX + x*_cellWidth < currentX &&
                    currentX < _cornerStartX + (x + 1)*_cellWidth &&
                    _cornerStartY + y*_cellWidth < currentY &&
                    currentY < _cornerStartY + (y + 1)*_cellWidth)
                    return i;
            }

            return -1;
        }

    }
}