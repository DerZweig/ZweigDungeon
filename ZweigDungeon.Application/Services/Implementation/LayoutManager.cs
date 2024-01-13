using ZweigDungeon.Application.Entities;
using ZweigDungeon.Application.Entities.Menu;
using ZweigDungeon.Application.Entities.Menu.Constants;
using ZweigDungeon.Application.Entities.Menu.Controls;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Repositories;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigDungeon.Application.Services.Implementation;

public class LayoutManager : ILayoutManager
{
	private readonly CurrentScene m_scene;
	private readonly IMenuAssets  m_assets;

	public LayoutManager(CurrentScene scene, IMenuAssets assets)
	{
		m_scene  = scene;
		m_assets = assets;
	}

	public void Update(in VideoRect viewport)
	{
		var halfWidth = viewport.Width / 2;
		var leftView  = viewport with { Width = halfWidth };
		var rightView = viewport with { Left = viewport.Left + halfWidth, Width = halfWidth };

		UpdatePanel(m_scene.MenuPanel, viewport);
		UpdatePanel(m_scene.MessagePanel, viewport);
		UpdatePanel(m_scene.LeftPanel, leftView);
		UpdatePanel(m_scene.RightPanel, rightView);
	}

	private void UpdatePanel(ControlPanel? panel, in VideoRect viewport)
	{
		if (panel == null)
		{
			return;
		}

		foreach (var control in panel.Children)
		{
			UpdateControlLayoutRect(control, viewport);
		}
	}

	private static void UpdateControlLayoutRect(BasicControl controlEntry, in VideoRect viewport)
	{
		var marginLeft    = controlEntry.MarginLeft ?? 0;
		var marginRight   = controlEntry.MarginRight ?? 0;
		var marginTop     = controlEntry.MarginTop ?? 0;
		var marginBottom  = controlEntry.MarginBottom ?? 0;
		var minimumWidth  = controlEntry.MinimumWidth ?? 0;
		var minimumHeight = controlEntry.MinimumHeight ?? 0;
		var maximumWidth  = controlEntry.MaximumWidth ?? int.MaxValue;
		var maximumHeight = controlEntry.MaximumHeight ?? int.MaxValue;

		var width  = Math.Clamp(viewport.Width - marginLeft - marginRight, minimumWidth, maximumWidth);
		var height = Math.Clamp(viewport.Height - marginTop - marginBottom, minimumHeight, maximumHeight);

		var left = controlEntry.HorizontalAlignment switch
		           {
			           HorizontalAlignment.Left => 0,
			           HorizontalAlignment.Center => viewport.Width / 2 - width / 2,
			           HorizontalAlignment.Right => viewport.Width - width - marginRight,
			           _ => throw new UnhandledEnumException<HorizontalAlignment>(controlEntry.HorizontalAlignment)
		           };
		var top = controlEntry.VerticalAlignment switch
		          {
			          VerticalAlignment.Top => 0,
			          VerticalAlignment.Center => viewport.Height / 2 - height / 2,
			          VerticalAlignment.Bottom => viewport.Height - height - marginBottom,
			          _ => throw new UnhandledEnumException<VerticalAlignment>(controlEntry.VerticalAlignment)
		          };

		left   = Math.Max(left, marginLeft);
		top    = Math.Max(top, marginTop);
		width  = Math.Min(left + width, viewport.Width) - left;
		height = Math.Min(top + height, viewport.Height) - top;

		controlEntry.LayoutRect = new VideoRect
		{
			Left   = left + viewport.Left,
			Top    = top + viewport.Top,
			Width  = width,
			Height = height
		};
	}
}