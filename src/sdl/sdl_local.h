#ifndef ZE_SDL_LOCAL_H
#define ZE_SDL_LOCAL_H

#include "../com_shared.h"
#include "../sys_shared.h"
#include "../util/guard_func.h"
#include <SDL3/SDL.h>

/**************************************************
 * SDL Input
 **************************************************/
struct SDLInput final
{
        SDLInput()                           = default;
        SDLInput(SDLInput&&)                 = delete;
        SDLInput(const SDLInput&)            = delete;
        SDLInput& operator=(SDLInput&&)      = delete;
        SDLInput& operator=(const SDLInput&) = delete;
        ~SDLInput() noexcept;

        void Initialize();
        void Poll();

private:
        bool      m_init;
        SDL_Event m_event;
};

/**************************************************
 * SDL Display
 **************************************************/
struct SDLDisplay final
{
        SDLDisplay()                             = default;
        SDLDisplay(SDLDisplay&&)                 = delete;
        SDLDisplay(const SDLDisplay&)            = delete;
        SDLDisplay& operator=(SDLDisplay&&)      = delete;
        SDLDisplay& operator=(const SDLDisplay&) = delete;
        ~SDLDisplay() noexcept;

        [[nodiscard]] uint16_t GetBufferViewportWidth() const noexcept
        {
                return m_scaled_width;
        }

        [[nodiscard]] uint16_t GetBufferViewportHeight() const noexcept
        {
                return m_scaled_height;
        }

        void Initialize();
        void UpdateProperties();
        void PresentBuffer() const;
        void ResizeBuffer(uint16_t width, uint16_t height);
        void CopyToBuffer(const void* ptr, uint32_t pitch, uint32_t rows) const;

private:
        bool          m_init;
        SDL_Window*   m_window;
        SDL_Renderer* m_renderer;
        SDL_Texture*  m_target;
        SDL_Rect      m_window_position;
        SDL_Rect      m_window_viewport;
        uint16_t      m_target_width;
        uint16_t      m_target_height;
        uint16_t      m_scaled_width;
        uint16_t      m_scaled_height;
};

/**************************************************
 * SDL Audio
 **************************************************/
struct SDLSound final
{
        SDLSound()                           = default;
        SDLSound(SDLSound&&)                 = delete;
        SDLSound(const SDLSound&)            = delete;
        SDLSound& operator=(SDLSound&&)      = delete;
        SDLSound& operator=(const SDLSound&) = delete;
        ~SDLSound() noexcept;

        void Initialize();

private:
        bool m_init;
};

/**************************************************
 * SDL Instance
 **************************************************/
struct SDLInstance final
{
        SDLInput   input   = {};
        SDLDisplay display = {};
        SDLSound   sound   = {};
};

#endif //ZE_SDL_LOCAL_H
