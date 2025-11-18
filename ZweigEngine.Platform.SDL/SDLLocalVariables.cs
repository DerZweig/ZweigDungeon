using ZweigEngine.Common.Services.Platform;

namespace ZweigEngine.Platform.SDL;

internal sealed class SDLLocalVariables
{
    internal int              WindowPositionLeft { get; set; }
    internal int              WindowPositionTop  { get; set; }
    internal int              WindowSizeWidth    { get; set; }
    internal int              WindowSizeHeight   { get; set; }
    internal int              WindowClientWidth  { get; set; }
    internal int              WindowClientHeight { get; set; }
    internal int              WindowVideoWidth   { get; set; }
    internal int              WindowVideoHeight  { get; set; }
    internal IMouseDevice?    Mouse              { get; set; }
    internal IKeyboardDevice? Keyboard           { get; set; }
    
}