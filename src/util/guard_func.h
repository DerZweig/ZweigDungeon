#ifndef ZE_GUARD_FUNC_H
#define ZE_GUARD_FUNC_H

/**************************************************
 * Guard Function
 **************************************************/
template <typename TFunc>
struct guard_func final
{
        guard_func()                             = delete;
        guard_func(guard_func&&)                 = delete;
        guard_func(const guard_func&)            = delete;
        guard_func& operator=(guard_func&&)      = delete;
        guard_func& operator=(const guard_func&) = delete;

        explicit guard_func(TFunc func) noexcept :
                m_func(func),
                m_active(true)
        {
        }

        ~guard_func()
        {
                if (!m_active)
                {
                        return;
                }

                m_active = false;
                m_func();
        }

        void discard() noexcept
        {
                m_active = false;
        }
private:
        [[no_unique_address]] TFunc m_func;
        [[no_unique_address]] bool  m_active;
};

#endif //ZE_GUARD_FUNC_H
