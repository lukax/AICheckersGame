namespace IACheckers.UI.Forms.Game
{
    public class PlayerMove
    {
        private readonly int _from;
        private readonly int _to;

        public PlayerMove(int moveFrom, int moveTo)
        {
            _from = moveFrom;
            _to = moveTo;
        }
        
        public int GetFrom()
        {
            return _from;
        }

        public int GetTo()
        {
            return _to;
        }

        public override string ToString()
        {
            return "(" + _from + "," + _to + ")";
        }
    }
}