﻿namespace ZweigEngine.Common.Services.Video.Interfaces;

public interface IVideoSurface
{
	int Width  { get; }
	int Height { get; }

	void Resize(int width, int height);
	void Present();
}