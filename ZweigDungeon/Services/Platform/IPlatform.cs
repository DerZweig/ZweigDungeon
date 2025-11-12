namespace ZweigDungeon.Services.Platform;

internal interface IPlatform
{
    bool Initialize();
    bool ProcessInput();
    void DisplayScreen();
}