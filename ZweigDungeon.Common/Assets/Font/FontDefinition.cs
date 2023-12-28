namespace ZweigDungeon.Common.Assets.Font;

public class FontDefinition
{
	public string                     FamilyName  { get; internal set; } = string.Empty;
	public int                        Size        { get; internal set; }
	public bool                       IsBold      { get; internal set; }
	public bool                       IsItalic    { get; internal set; }
	public int                        LineHeight  { get; internal set; }
	public string                     ImageName   { get; internal set; } = string.Empty;
	public int                        ImageWidth  { get; internal set; }
	public int                        ImageHeight { get; internal set; }
	public IEnumerable<FontCharacter> Characters  { get; internal set; } = Enumerable.Empty<FontCharacter>();
	public IEnumerable<FontKerning>   Kernings    { get; internal set; } = Enumerable.Empty<FontKerning>();
}