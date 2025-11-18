namespace ZweigEngine.Platform.SDL;

internal interface ISDLEventListener
{
    public void InputBegin();
    public void InputMessage(SDL3.SDL.Event ev);
    public void InputFinish();
}