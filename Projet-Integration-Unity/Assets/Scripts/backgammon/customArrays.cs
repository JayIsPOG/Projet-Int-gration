
abstract public class customArray
{
	public int index;
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
		index = 0;
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
		index = 0;
	}
	public void push_back(uint move) {
		moves[index++] = move;
	}
};