using ZweigDungeon.Application.Screen.Constants;
using ZweigDungeon.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Screen;

public struct Layout
{
	private HorizontalAlignment m_horizontal_alignment;
	private VerticalAlignment   m_vertical_alignment;
	private int                 m_minimum_width;
	private int                 m_minimum_height;
	private int                 m_maximum_width;
	private int                 m_maximum_height;
	private int                 m_margin_left;
	private int                 m_margin_top;
	private int                 m_margin_right;
	private int                 m_margin_bottom;

	public void CalculateLayout(in VideoRect viewport, out VideoRect result)
	{
		var viewWidth  = (int)viewport.Width;
		var viewHeight = (int)viewport.Height;
		var sizeWidth  = viewWidth - m_margin_left - m_margin_right;
		var sizeHeight = viewHeight - m_margin_top - m_margin_bottom;

		sizeWidth  = Math.Clamp(sizeWidth, m_minimum_width, m_maximum_width);
		sizeHeight = Math.Clamp(sizeHeight, m_minimum_height, m_maximum_height);

		var positionLeft = m_horizontal_alignment switch
		                   {
			                   HorizontalAlignment.Left => viewWidth - sizeWidth - m_margin_right,
			                   HorizontalAlignment.Center => viewWidth / 2 - sizeWidth / 2,
			                   HorizontalAlignment.Right => 0,
			                   _ => throw new Exception("Unexpected horizontal alignment value.")
		                   };

		var positionTop = m_vertical_alignment switch
		                  {
			                  VerticalAlignment.Top => 0,
			                  VerticalAlignment.Center => 0,
			                  VerticalAlignment.Bottom => 0,
			                  _ => throw new Exception("Unexpected vertical alignment value.")
		                  };

		positionLeft  = Math.Max(positionLeft, m_margin_left);
		positionTop   = Math.Max(positionTop, m_margin_top);
		sizeWidth     = Math.Min(positionLeft + sizeWidth, viewWidth) - positionLeft;
		sizeHeight    = Math.Min(positionTop + sizeHeight, viewHeight) - positionTop;
		result.Left   = (uint)positionLeft;
		result.Top    = (uint)positionTop;
		result.Width  = (uint)sizeWidth;
		result.Height = (uint)sizeHeight;
	}

	public void Reset()
	{
		m_horizontal_alignment = HorizontalAlignment.Left;
		m_vertical_alignment   = VerticalAlignment.Top;
		m_minimum_width        = 0;
		m_minimum_height       = 0;
		m_maximum_width        = int.MaxValue;
		m_maximum_height       = int.MaxValue;
		m_margin_left          = 0;
		m_margin_top           = 0;
		m_margin_right         = 0;
		m_margin_bottom        = 0;
	}

	public void SetHorizontalAlignment(HorizontalAlignment value)
	{
		m_horizontal_alignment = value;
	}

	public void SetVerticalAlignment(VerticalAlignment value)
	{
		m_vertical_alignment = value;
	}

	public void SetMinimumWidth(int value)  => m_minimum_width = Math.Max(value, 0);
	public void SetMinimumHeight(int value) => m_minimum_height = Math.Max(value, 0);
	public void SetMaximumWidth(int value)  => m_maximum_width = Math.Max(value, 0);
	public void SetMaximumHeight(int value) => m_maximum_height = Math.Max(value, 0);
	public void SetMarginLeft(int value)    => m_margin_left = Math.Max(value, 0);
	public void SetMarginTop(int value)     => m_margin_top = Math.Max(value, 0);
	public void SetMarginRight(int value)   => m_margin_right = Math.Max(value, 0);
	public void SetMarginBottom(int value)  => m_margin_bottom = Math.Max(value, 0);
	public void RemoveMinimumWidth()        => m_minimum_width = 0;
	public void RemoveMinimumHeight()       => m_minimum_height = 0;
	public void RemoveMaximumWidth()        => m_maximum_width = int.MaxValue;
	public void RemoveMaximumHeight()       => m_maximum_height = int.MaxValue;
	public void RemoveMarginLeft()          => m_margin_left = 0;
	public void RemoveMarginTop()           => m_margin_top = 0;
	public void RemoveMarginRight()         => m_margin_right = 0;
	public void RemoveMarginBottom()        => m_margin_bottom = 0;

	public void SetMinimumSize(int width, int height)
	{
		m_minimum_width  = Math.Max(width, 0);
		m_minimum_height = Math.Max(height, 0);
	}

	public void SetMaximumSize(int width, int height)
	{
		m_maximum_width  = Math.Max(width, 0);
		m_maximum_height = Math.Max(height, 0);
	}

	public void RemoveMinimumSize()
	{
		m_minimum_width  = 0;
		m_minimum_height = 0;
	}

	public void RemoveMaximumSize()
	{
		m_maximum_width  = int.MaxValue;
		m_maximum_height = int.MaxValue;
	}

	public void SetMargin(int value)
	{
		value           = Math.Max(value, 0);
		m_margin_left   = value;
		m_margin_top    = value;
		m_margin_right  = value;
		m_margin_bottom = value;
	}

	public void SetMargin(int leftRight, int topBottom)
	{
		leftRight       = Math.Max(leftRight, 0);
		topBottom       = Math.Max(topBottom, 0);
		m_margin_left   = leftRight;
		m_margin_right  = leftRight;
		m_margin_top    = topBottom;
		m_margin_bottom = topBottom;
	}

	public void SetMargin(int left, int top, int right, int bottom)
	{
		m_margin_left   = Math.Max(left, 0);
		m_margin_top    = Math.Max(top, 0);
		m_margin_right  = Math.Max(right, 0);
		m_margin_bottom = Math.Max(bottom, 0);
	}

	public void RemoveMargin()
	{
		m_margin_left   = 0;
		m_margin_top    = 0;
		m_margin_right  = 0;
		m_margin_bottom = 0;
	}
}