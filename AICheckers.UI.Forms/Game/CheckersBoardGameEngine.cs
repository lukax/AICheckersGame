namespace IACheckers.UI.Forms.Game
{
    public class CheckersBoardGameEngine
    {
        public const byte EmptyPiece = 0;
        public const byte WhitePiece = 2;
        public const byte WhiteKingPiece = 3;
        public const byte BlackPiece = 4;
        public const byte BlackKing = 5;
        private const byte King = 1;

        private const int LastDirectionNone = 0;
        private const int LastDirectionLeftBelow = 1; 
        private const int LastDirectionLeftAbove = 2;
        private const int LastDirectionRightBelow = 3;
        private const int LastDirectionRightAbove = 4;

        private int _currentPlayer;
        private readonly byte[] _pieces;

        public int WhitePieces { get; set; }
        public int BlackPieces  { get; set; }

        public CheckersBoardGameEngine()
        {
            _pieces = new byte[32];
            SetupBoardForNewGame();
        }

        public bool IsValidMove(int fromBoardPos, int toBoardPos)
        {
            // If the arguments are invalid so is the game movement
            if (fromBoardPos < 0 || fromBoardPos > 32 || toBoardPos < 0 || toBoardPos > 32)
                return false;

            // If the from or the to houses aren't empty the game move is invalid
            if (_pieces[fromBoardPos] == EmptyPiece || _pieces[toBoardPos] != EmptyPiece)
                return false;

            // Are we trying to move a pience from the current player ?
            if ((_pieces[fromBoardPos] & ~King) != _currentPlayer)
                return false;


            int enemy;
            var color = _pieces[fromBoardPos] & ~King;
            if (color == WhitePiece)
                enemy = BlackPiece;
            else
                enemy = WhitePiece;


            var fromLine = PosToBoardLine(fromBoardPos);
            var fromCol = PosToCol(fromBoardPos);
            var toLine = PosToBoardLine(toBoardPos);
            var toCol = PosToCol(toBoardPos);

            int incX, incY;

            // Set the increments
            if (fromCol > toCol)
                incX = -1;
            else
                incX = 1;


            if (fromLine > toLine)
                incY = -1;
            else
                incY = 1;

            var x = fromCol + incX;
            var y = fromLine + incY;


            if ((_pieces[fromBoardPos] & King) == 0)
            {
                // Simple piece
                bool goodDir;

                if ((incY == -1 && color == WhitePiece) || (incY == 1 && color == BlackPiece))
                    goodDir = true;
                else
                    goodDir = false;

                if (x == toCol && y == toLine) // Simple move
                    return goodDir && !CanCurrentPlayerAttack();


                // If it wasn't a simple move it can only be an attack move
                return goodDir && x + incX == toCol && y + incY == toLine &&
                       (_pieces[ColLineToPos(x, y)] & ~King) == enemy;
            }
            // Is a king piece
            while (x != toCol && y != toLine && _pieces[ColLineToPos(x, y)] == EmptyPiece)
            {
                x += incX;
                y += incY;
            }

            // Simple move with a king piece
            if (x == toCol && y == toLine)
                return !CanCurrentPlayerAttack();

            if ((_pieces[ColLineToPos(x, y)] & ~King) == enemy)
            {
                x += incX;
                y += incY;

                while (x != toCol && y != toLine && _pieces[ColLineToPos(x, y)] == EmptyPiece)
                {
                    x += incX;
                    y += incY;
                }

                if (x == toCol && y == toLine)
                    return true;
            }


            return false;
        }

        public bool CanCurrentPlayerAttack()
        {
            for (var i = 0; i < 32; i++)
                if ((_pieces[i] & ~King) == _currentPlayer && CanAttackPosition(i))
                    return true;

            return false;
        }

        public bool CanAttackPosition(int pos)
        {
            if (_pieces[pos] == EmptyPiece)
                return false;

            int enemy;

            var color = _pieces[pos] & ~King;
            if (color == WhitePiece)
                enemy = BlackPiece;
            else
                enemy = WhitePiece;

            var x = PosToCol(pos);
            var y = PosToBoardLine(pos);

            if ((_pieces[pos] & King) == 0)
            {
                var i = color == WhitePiece ? -1 : 1;

                // Diagonal UPR e DNR
                if (x < 6 && y + i > 0 && y + i < 7 && (_pieces[ColLineToPos(x + 1, y + i)] & ~King) == enemy &&
                    _pieces[ColLineToPos(x + 2, y + 2 * i)] == EmptyPiece)
                    return true;

                // Diagonal UPL e DNL
                if (x > 1 && y + i > 0 && y + i < 7 && (_pieces[ColLineToPos(x - 1, y + i)] & ~King) == enemy &&
                    _pieces[ColLineToPos(x - 2, y + 2 * i)] == EmptyPiece)
                    return true;
            }
            else // KING
            {
                // Diagonal DNR
                var i = x + 1;
                var j = y + 1;
                while (i < 6 && j < 6 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                {
                    i++;
                    j++;
                }

                if (i < 7 && j < 7 && (_pieces[ColLineToPos(i, j)] & ~King) == enemy)
                {
                    i++;
                    j++;

                    if (i <= 7 && j <= 7 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        return true;
                }

                // Diagonal UPL
                i = x - 1;
                j = y - 1;
                while (i > 1 && j > 1 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                {
                    i--;
                    j--;
                }

                if (i > 0 && j > 0 && (_pieces[ColLineToPos(i, j)] & ~King) == enemy)
                {
                    i--;
                    j--;

                    if (i >= 0 && j >= 0 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        return true;
                }

                // Diagonal UPR
                i = x + 1;
                j = y - 1;
                while (i < 6 && j > 1 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                {
                    i++;
                    j--;
                }

                if (i < 7 && j > 0 && (_pieces[ColLineToPos(i, j)] & ~King) == enemy)
                {
                    i++;
                    j--;

                    if (i <= 7 && j >= 0 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        return true;
                }

                // Diagonal DNL
                i = x - 1;
                j = y + 1;
                while (i > 1 && j < 6 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                {
                    i--;
                    j++;
                }

                if (i > 0 && j < 7 && (_pieces[ColLineToPos(i, j)] & ~King) == enemy)
                {
                    i--;
                    j++;

                    if (i >= 0 && j <= 7 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        return true;
                }
            }


            return false;
        }

        public void ApplyMove(int startPos, int endPos)
        {
            var haveToAttack = CanCurrentPlayerAttack();

            InternalApplyMove(startPos, endPos);

            if (!haveToAttack)
                SwitchCurrentPlayer();
            else if (!CanAttackPosition(endPos))
                SwitchCurrentPlayer();
        }

        public void ApplyMove(JavaList<object> moves)
        {
            var iter = moves.GetIterarator();

            while (iter.HasMoreElements())
            {
                var move = (PlayerMove)iter.NextElement();
                InternalApplyMove(move.GetFrom(), move.GetTo());
            }

            SwitchCurrentPlayer();
        }

        private void InternalApplyMove(int fromPos, int toPos)
        {
            if (!IsValidMove(fromPos, toPos))
                throw new InvalidPlayerMoveException();

            RemovePieceFromBoardBetween(fromPos, toPos);
            // performs the movement
            if (toPos < 4 && _pieces[fromPos] == WhitePiece)
                _pieces[toPos] = WhiteKingPiece;
            else if (toPos > 27 && _pieces[fromPos] == BlackPiece)
                _pieces[toPos] = BlackKing;
            else
                _pieces[toPos] = _pieces[fromPos];

            _pieces[fromPos] = EmptyPiece;
        }

        private void SetupBoardForNewGame()
        {
            int i;

            WhitePieces = 12;
            BlackPieces = 12;

            _currentPlayer = BlackPiece;

            for (i = 0; i < 12; i++)
                _pieces[i] = BlackPiece;

            for (i = 12; i < 20; i++)
                _pieces[i] = EmptyPiece;

            for (i = 20; i < 32; i++)
                _pieces[i] = WhitePiece;
        }

        public int GetCurrentPlayer()
        {
            return _currentPlayer;
        }
        
        public CheckersBoardGameEngine Clone()
        {
            var board = new CheckersBoardGameEngine
            {
                _currentPlayer = _currentPlayer,
                WhitePieces = WhitePieces,
                BlackPieces = BlackPieces
            };

            for (var i = 0; i < 32; i++)
                board._pieces[i] = _pieces[i];

            return board;
        }
        
        public JavaList<object> FindAllLegalMovesForCurrentPlayer()
        {
            var color = _currentPlayer;
            int enemy = color == WhitePiece ? BlackPiece : WhitePiece;
            return CanCurrentPlayerAttack() ? FindAllLegalAttackMovesForCurrentPlayer(color, enemy) : FindAllNonAttackMoves(color);
        }
        
        private JavaList<object> FindAllLegalAttackMovesForCurrentPlayer(int currentPlayerColor, int enemyColor)
        {
            var moves = new JavaList<object>();
            for (var k = 0; k < 32; k++)
                if ((_pieces[k] & ~King) == _currentPlayer)
                {
                    JavaList<object> tempMoves;
                    if ((_pieces[k] & King) == 0)
                    {
                        tempMoves = FindAllLegalSimpleAttackMoves(k, currentPlayerColor, enemyColor);
                    }
                    else // KING
                    {
                        var lastPos = new JavaList<object>();
                        lastPos.push_back(k);
                        tempMoves = FindAllLegalKingAttackMoves(lastPos, k, LastDirectionNone, currentPlayerColor, enemyColor);
                    }
                    if (Any(tempMoves))
                    {
                        moves.AppendToTail(tempMoves);
                    }
                }
            return moves;
        }
        
        private JavaList<object> FindAllLegalSimpleAttackMoves(int currentPiecePos, int currentPlayerColor, int enemyColor)
        {
            var x = PosToCol(currentPiecePos);
            var y = PosToBoardLine(currentPiecePos);
            var moves = new JavaList<object>();
            JavaList<object> tempMoves;
            int enemyPos, nextPos;

            var i = currentPlayerColor == WhitePiece ? -1 : 1;
            
            // Diagonals UPR e DNR
            if (x < 6 && y + i > 0 && y + i < 7)
            {
                enemyPos = ColLineToPos(x + 1, y + i);
                nextPos = ColLineToPos(x + 2, y + 2*i);

                if ((_pieces[enemyPos] & ~King) == enemyColor && _pieces[nextPos] == EmptyPiece)
                {
                    tempMoves = FindAllLegalSimpleAttackMoves(nextPos, currentPlayerColor, enemyColor);
                    moves.AppendToTail(AddNewGameMovement(new PlayerMove(currentPiecePos, nextPos), tempMoves));
                }
            }

            // Diagonals DNR e UPL
            if (x > 1 && y + i > 0 && y + i < 7)
            {
                enemyPos = ColLineToPos(x - 1, y + i);
                nextPos = ColLineToPos(x - 2, y + 2*i);

                if ((_pieces[enemyPos] & ~King) == enemyColor && _pieces[nextPos] == EmptyPiece)
                {
                    tempMoves = FindAllLegalSimpleAttackMoves(nextPos, currentPlayerColor, enemyColor);
                    moves.AppendToTail(AddNewGameMovement(new PlayerMove(currentPiecePos, nextPos), tempMoves));
                }
            }

            if (moves.IsEmpty())
                moves.push_back(new JavaList<object>());

            return moves;
        }
        
        private JavaList<object> FindAllLegalKingAttackMoves(JavaList<object> lastPos, int currentPos, int lastDirection, int playerColor, int enemyColor)
        {
            JavaList<object> tempMoves, moves = new JavaList<object>();

            if (lastDirection != LastDirectionRightBelow)
            {
                tempMoves = FindAllLegalKingDiagonalAttackMoves(lastPos, currentPos, playerColor, enemyColor, 1, 1);

                if (Any(tempMoves))
                    moves.AppendToTail(tempMoves);
            }

            if (lastDirection != LastDirectionLeftAbove)
            {
                tempMoves = FindAllLegalKingDiagonalAttackMoves(lastPos, currentPos, playerColor, enemyColor, -1, -1);

                if (Any(tempMoves))
                    moves.AppendToTail(tempMoves);
            }


            if (lastDirection != LastDirectionRightAbove)
            {
                tempMoves = FindAllLegalKingDiagonalAttackMoves(lastPos, currentPos, playerColor, enemyColor, 1, -1);

                if (Any(tempMoves))
                    moves.AppendToTail(tempMoves);
            }

            if (lastDirection != LastDirectionLeftBelow)
            {
                tempMoves = FindAllLegalKingDiagonalAttackMoves(lastPos, currentPos, playerColor, enemyColor, -1, 1);

                if (Any(tempMoves))
                    moves.AppendToTail(tempMoves);
            }


            return moves;
        }
        
        private JavaList<object> FindAllLegalKingDiagonalAttackMoves(JavaList<object> lastPos, int currentPos, int playerColor, int enemyColor, int incX, int incY)
        {
            var x = PosToCol(currentPos);
            var y = PosToBoardLine(currentPos);
            var moves = new JavaList<object>();


            var startPos = (int) lastPos.peek_head();

            var i = x + incX;
            var j = y + incY;

            // Find enemy
            while (i > 0 && i < 7 && j > 0 && j < 7 &&
                   (_pieces[ColLineToPos(i, j)] == EmptyPiece || ColLineToPos(i, j) == startPos))
            {
                i += incX;
                j += incY;
            }

            if (i > 0 && i < 7 && j > 0 && j < 7 && (_pieces[ColLineToPos(i, j)] & ~King) == enemyColor &&
                !lastPos.Contains(ColLineToPos(i, j)))
            {
                lastPos.push_back(ColLineToPos(i, j));

                i += incX;
                j += incY;

                var saveI = i;
                var saveJ = j;
                JavaList<object> tempMoves;
                while (i >= 0 && i <= 7 && j >= 0 && j <= 7 &&
                       (_pieces[ColLineToPos(i, j)] == EmptyPiece || ColLineToPos(i, j) == startPos))
                {
                    int dir;

                    if (incX == 1 && incY == 1)
                        dir = LastDirectionLeftAbove;
                    else if (incX == -1 && incY == -1)
                        dir = LastDirectionRightBelow;
                    else if (incX == -1 && incY == 1)
                        dir = LastDirectionRightAbove;
                    else
                        dir = LastDirectionLeftBelow;


                    var tempPos = lastPos.Clone();
                    tempMoves = FindAllLegalKingAttackMoves(tempPos, ColLineToPos(i, j), dir, playerColor, enemyColor);

                    if (Any(tempMoves))
                        moves.AppendToTail(AddNewGameMovement(new PlayerMove(currentPos, ColLineToPos(i, j)), tempMoves));

                    i += incX;
                    j += incY;
                }

                lastPos.pop_back();

                if (moves.IsEmpty())
                {
                    i = saveI;
                    j = saveJ;

                    while (i >= 0 && i <= 7 && j >= 0 && j <= 7 &&
                           (_pieces[ColLineToPos(i, j)] == EmptyPiece || ColLineToPos(i, j) == startPos))
                    {
                        tempMoves = new JavaList<object>();
                        tempMoves.push_back(new PlayerMove(currentPos, ColLineToPos(i, j)));
                        moves.push_back(tempMoves);

                        i += incX;
                        j += incY;
                    }
                }
            }

            return moves;
        }
        
        private bool Any(JavaList<object> moves)
        {
            return !moves.IsEmpty() && !((JavaList<object>) moves.peek_head()).IsEmpty();
        }
        
        private JavaList<object> AddNewGameMovement(PlayerMove playerMove, JavaList<object> moves)
        {
            if (playerMove == null)
                return moves;

            JavaList<object> temp = new JavaList<object>();
            while (!moves.IsEmpty())
            {
                var current = (JavaList<object>) moves.pop_front();
                current.push_front(playerMove);
                temp.push_back(current);
            }

            return temp;
        }

        private JavaList<object> FindAllNonAttackMoves(int currentPlayerColor)
        {
            var moves = new JavaList<object>();

            for (var k = 0; k < 32; k++)
                if ((_pieces[k] & ~King) == _currentPlayer)
                {
                    var x = PosToCol(k);
                    var y = PosToBoardLine(k);
                    int i, j;

                    JavaList<object> tempMove;
                    if ((_pieces[k] & King) == 0)
                    {
                        // Simple piece
                        i = currentPlayerColor == WhitePiece ? -1 : 1;

                        // Diagonal UPR e DNR
                        if (x < 7 && y + i >= 0 && y + i <= 7 &&
                            _pieces[ColLineToPos(x + 1, y + i)] == EmptyPiece)
                        {
                            tempMove = new JavaList<object>();
                            tempMove.push_back(new PlayerMove(k, ColLineToPos(x + 1, y + i)));
                            moves.push_back(tempMove);
                        }


                        // Diagonal UPL e DNL
                        if (x > 0 && y + i >= 0 && y + i <= 7 &&
                            _pieces[ColLineToPos(x - 1, y + i)] == EmptyPiece)
                        {
                            tempMove = new JavaList<object>();
                            tempMove.push_back(new PlayerMove(k, ColLineToPos(x - 1, y + i)));
                            moves.push_back(tempMove);
                        }
                    }
                    else // KING
                    {
                        // Diagonal DNR
                        i = x + 1;
                        j = y + 1;

                        while (i <= 7 && j <= 7 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        {
                            tempMove = new JavaList<object>();
                            tempMove.push_back(new PlayerMove(k, ColLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i++;
                            j++;
                        }


                        // Diagonal UPL
                        i = x - 1;
                        j = y - 1;
                        while (i >= 0 && j >= 0 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        {
                            tempMove = new JavaList<object>();
                            tempMove.push_back(new PlayerMove(k, ColLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i--;
                            j--;
                        }

                        // Diagonal UPR
                        i = x + 1;
                        j = y - 1;
                        while (i <= 7 && j >= 0 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        {
                            tempMove = new JavaList<object>();
                            tempMove.push_back(new PlayerMove(k, ColLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i++;
                            j--;
                        }

                        // Diagonal DNL
                        i = x - 1;
                        j = y + 1;
                        while (i >= 0 && j <= 7 && _pieces[ColLineToPos(i, j)] == EmptyPiece)
                        {
                            tempMove = new JavaList<object>();
                            tempMove.push_back(new PlayerMove(k, ColLineToPos(i, j)));
                            moves.push_back(tempMove);

                            i--;
                            j++;
                        }
                    }
                }

            return moves;
        }

        private void SwitchCurrentPlayer()
        {
            if (_currentPlayer == WhitePiece)
                _currentPlayer = BlackPiece;
            else
                _currentPlayer = WhitePiece;
        }

        public byte GetPieceAtPosition(int position)
        {
            if (position < 0 || position > 32)
                throw new InvalidBoardCoordinateException();

            return _pieces[position];
        }
        
        public bool IsGameOver()
        {
            return WhitePieces == 0 || BlackPieces == 0 || !Any(FindAllLegalMovesForCurrentPlayer());
        }

        public int GetWinner()
        {
            if (_currentPlayer == WhitePiece)
                if (Any(FindAllLegalMovesForCurrentPlayer()))
                    return WhitePiece;
                else
                    return BlackPiece;
            if (Any(FindAllLegalMovesForCurrentPlayer()))
                return BlackPiece;
            return WhitePiece;
        }

        private void RemovePieceFromBoardBetween(int fromPos, int toPos)
        {
            var fromLine = PosToBoardLine(fromPos);
            var fromCol = PosToCol(fromPos);
            var toLine = PosToBoardLine(toPos);
            var toCol = PosToCol(toPos);

            int i, j;

            if (fromCol > toCol)
                i = -1;
            else
                i = 1;


            if (fromLine > toLine)
                j = -1;
            else
                j = 1;


            fromCol += i;
            fromLine += j;

            while (fromLine != toLine && fromCol != toCol)
            {
                var pos = ColLineToPos(fromCol, fromLine);
                int piece = _pieces[pos];

                if ((piece & ~King) == WhitePiece)
                    WhitePieces--;
                else if ((piece & ~King) == BlackPiece)
                    BlackPieces--;

                _pieces[pos] = EmptyPiece;
                fromCol += i;
                fromLine += j;
            }
        }
        
        private int ColLineToPos(int boardColumn, int boardLine)
        {
            if (boardLine % 2 ==0)
                return boardLine*4 + (boardColumn - 1)/2;
            return boardLine*4 + boardColumn/2;
        }
        
        private int PosToBoardLine(int boardPos)
        {
            return boardPos/4;
        }
        
        private int PosToCol(int boardPos)
        {
            return boardPos%4*2 + (boardPos/4%2 == 0 ? 1 : 0);
        }
    }
}