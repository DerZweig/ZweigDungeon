﻿using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Common.Assets.Font;

public class FontCharacter
{
	public VideoRect ImageRect  { get; init; }
	public int       OffsetLeft { get; init; }
	public int       OffsetTop  { get; init; }
	public int       Advance    { get; init; }
}