
abstract public class customArray
{
	public int index;
	public int moveDepth;
	public static short[] moveScores;
	public int size()
	{
		return index;
	}
}
class simpleMoveArray : customArray{
	public ushort[] moves;

	public simpleMoveArray()
	{
		moves = new ushort[2*15*15];
		moveScores = new short[2*15*15]; // Can be made to be only one array use for all move
		index = 0;
		moveDepth = 0;
	}
	public void push_back(ushort move) {
		moves[index++] = move;
	}
};
class doubleMoveArray : customArray{
	public uint[] moves;
	public doubleMoveArray()
	{
		moves = new uint[15 * 15 * 15 * 15]; // maybe 15*14*13*12*11 instead
		moveScores = new short[15 * 15 * 15 * 15];
		index = 0;
		moveDepth = 0;
	}
	public void push_back(uint move) {
		moves[index++] = move;
	}
};