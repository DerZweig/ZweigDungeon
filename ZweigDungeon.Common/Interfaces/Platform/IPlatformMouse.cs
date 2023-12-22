﻿using ZweigDungeon.Common.Constants;

namespace ZweigDungeon.Common.Interfaces.Platform;

public interface IPlatformMouse
{
	int  GetPositionLeft();
	int  GetPositionTop();
	bool IsButtonPressed(MouseButton button);
	bool IsButtonReleased(MouseButton button);
}