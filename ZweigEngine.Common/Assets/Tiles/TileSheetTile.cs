namespace ZweigEngine.Common.Assets.Tiles;

public class TileSheetTile
{
	public bool Repeat       { get; set; }
	public int  ImageLeft    { get; set; }
	public int  ImageTop     { get; set; }
	public int  ImageWidth   { get; set; }
	public int  ImageHeight  { get; set; }
	public int  BorderLeft   { get; set; }
	public int  BorderTop    { get; set; }
	public int  BorderRight  { get; set; }
	public int  BorderBottom { get; set; }
}