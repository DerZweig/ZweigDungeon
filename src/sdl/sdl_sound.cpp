#include "sdl_local.h"

/**************************************************
 * SDLAudio Dummy
 **************************************************/
SDLSound::~SDLSound() noexcept
{
        if (!m_init)
        {
                return;
        }

        m_init = false;
        Common_LogInfo("SDL::Sound", "Shutdown");
}

void SDLSound::Initialize()
{
        if (m_init)
        {
                return;
        }

        m_init = true;
        Common_LogInfo("SDL::Sound", "Initializing...");

        if (!SDL_WasInit(SDL_INIT_AUDIO) && !SDL_Init(SDL_INIT_AUDIO))
        {
                Common_LogInfo("SDL::Sound", "Failed to initialize subsystem");
        }
}
