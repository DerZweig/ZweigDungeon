using ZweigDungeon.Application.Entities.Assets.Menu.Constants;
using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Assets.Menu.Controls;

public abstract class BasicControl
{
	protected BasicControl()
	{
		HorizontalAlignment = HorizontalAlignment.Left;
		VerticalAlignment   = VerticalAlignment.Top;
	}

	public VideoRect           LayoutRect          { get; set; }
	public HorizontalAlignment HorizontalAlignment { get; set; }
	public VerticalAlignment   VerticalAlignment   { get; set; }
	public int?                MinimumWidth        { get; set; }
	public int?                MinimumHeight       { get; set; }
	public int?                MaximumWidth        { get; set; }
	public int?                MaximumHeight       { get; set; }
	public int?                MarginLeft          { get; set; }
	public int?                MarginTop           { get; set; }
	public int?                MarginRight         { get; set; }
	public int?                MarginBottom        { get; set; }
}