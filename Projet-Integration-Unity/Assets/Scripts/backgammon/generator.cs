abstract public class generator
{
  public BoardState Pos;
  public abstract void generate();
}
public abstract class generatorPlayer : generator
{
  public static readonly uint[] bearoff_mask = { 0, 0b01000000000000000000000000, 0b01100000000000000000000000, 0b01110000000000000000000000, 0b01111000000000000000000000, 0b01111100000000000000000000, 0b01111110000000000000000000 };
}
public abstract class generatorAI : generator
{
  public static readonly uint[] bearoff_mask = { 0, 0b10, 0b110, 0b1110, 0b11110, 0b111110, 0b1111110 };
}