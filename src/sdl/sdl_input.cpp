#include "sdl_local.h"

/**************************************************
 * SDLInput Shutdown
 **************************************************/
SDLInput::~SDLInput() noexcept
{
        Common_LogInfo("SDL::Input", "Shutdown");
}

/**************************************************
 * SDLInput Initialize
 **************************************************/
void SDLInput::Initialize()
{
        Common_LogInfo("SDL::Input", "Initialize");
        if (!SDL_WasInit(SDL_INIT_EVENTS) && !SDL_Init(SDL_INIT_EVENTS))
        {
                App_Error("SDL::Input", "Failed to initialize subsystem");
        }

        m_event = {};
}

/**************************************************
 * SDLInput Poll
 **************************************************/
void SDLInput::Poll()
{
        while (SDL_PollEvent(&m_event))
        {
                switch (m_event.type)
                {
                case SDL_EVENT_QUIT:
                        App_Quit();
                default:
                        break;
                }
        }
}
