#include "sdl_local.h"

/**************************************************
 * SDLInput Shutdown
 **************************************************/
SDLInput::~SDLInput() noexcept
{
        if (!m_init)
        {
                return;
        }

        m_init = false;
        Common_LogInfo("SDL::Input", "Shutdown");
}

/**************************************************
 * SDLInput Initialize
 **************************************************/
void SDLInput::Initialize()
{
        if (m_init)
        {
                return;
        }

        m_init = true;
        Common_LogInfo("SDL::Input", "Initializing...");
        if (!SDL_WasInit(SDL_INIT_EVENTS) && !SDL_Init(SDL_INIT_EVENTS))
        {
                App_Error("SDL::Input", "Failed to initialize subsystem");
        }
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
