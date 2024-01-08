namespace ZweigDungeon.Application.Entities.Assets.Controls;

public class ImageControl : BasicControl
{
	public string Path           { get; set; } = string.Empty;
	public int?   SubImageLeft   { get; set; }
	public int?   SubImageTop    { get; set; }
	public int?   SubImageWidth  { get; set; }
	public int?   SubImageHeight { get; set; }
}