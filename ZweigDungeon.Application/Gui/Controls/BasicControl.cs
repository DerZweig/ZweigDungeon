using ZweigDungeon.Application.Gui.Constants;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigDungeon.Application.Gui.Controls;

public abstract class BasicControl
{
	public VideoRect           LayoutRect          { get; private set; }
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

	public void UpdateLayoutRect(in VideoRect viewport)
	{
		var marginLeft    = MarginLeft ?? 0;
		var marginRight   = MarginRight ?? 0;
		var marginTop     = MarginTop ?? 0;
		var marginBottom  = MarginBottom ?? 0;
		var minimumWidth  = MinimumWidth ?? 0;
		var minimumHeight = MinimumHeight ?? 0;
		var maximumWidth  = MaximumWidth ?? int.MaxValue;
		var maximumHeight = MaximumHeight ?? int.MaxValue;

		var width  = Math.Clamp(viewport.Width - marginLeft - marginRight, minimumWidth, maximumWidth);
		var height = Math.Clamp(viewport.Height - marginTop - marginBottom, minimumHeight, maximumHeight);

		var left = HorizontalAlignment switch
		           {
			           HorizontalAlignment.Left => 0,
			           HorizontalAlignment.Center => viewport.Width / 2 - width / 2,
			           HorizontalAlignment.Right => viewport.Width - width - marginRight,
			           _ => throw new UnhandledEnumException<HorizontalAlignment>(HorizontalAlignment)
		           };
		
		var top = VerticalAlignment switch
		          {
			          VerticalAlignment.Top => 0,
			          VerticalAlignment.Center => viewport.Height / 2 - height / 2,
			          VerticalAlignment.Bottom => viewport.Height - height - marginBottom,
			          _ => throw new UnhandledEnumException<VerticalAlignment>(VerticalAlignment)
		          };

		left   = Math.Max(left, marginLeft);
		top    = Math.Max(top, marginTop);
		width  = Math.Min(left + width, viewport.Width) - left;
		height = Math.Min(top + height, viewport.Height) - top;

		LayoutRect = new VideoRect
		{
			Left   = left + viewport.Left,
			Top    = top + viewport.Top,
			Width  = width,
			Height = height
		};
	}
}