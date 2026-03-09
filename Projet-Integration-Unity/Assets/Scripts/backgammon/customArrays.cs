class simpleMoveArray { /// might add a move number variable to make the move for loop better type shit ///////////
	public ushort[] moves;
	public int index;
	public int moveDepth;
	public simpleMoveArray()
	{
		moves = new ushort[15*15];
		index = 0;
		moveDepth = 0;
	}
	public void push_back(ushort move) {
		moves[index++] = move;
	}
	public int size()
	{
		return index;
	}
};
class doubleMoveArray {
	public uint[] moves;
	public int index;
	public int moveDepth;
	public doubleMoveArray()
	{
		moves = new uint[15 * 15 * 15 * 15]; // maybe 15*14*13*12*11 instead
		index = 0;
		moveDepth = 0;
	}
	public void push_back(uint move) {
		moves[index++] = move;
	}
	public int size()
	{
		return index;
	}
};