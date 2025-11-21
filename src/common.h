#ifndef ZE_COMMON_H
#define ZE_COMMON_H

#include <cinttypes>

/********************************************************************
 * Array View Templates
 ********************************************************************/
template <typename TElem>
struct array_view final
{
        array_view() noexcept                             = default;
        array_view(array_view&&) noexcept                 = default;
        array_view(const array_view&) noexcept            = default;
        array_view& operator=(array_view&&) noexcept      = default;
        array_view& operator=(const array_view&) noexcept = default;
        ~array_view() noexcept                            = default;

        template <size_t N>
        array_view(TElem (&arr)[N]) noexcept :
                m_begin(static_cast<TElem*>(arr)),
                m_end(static_cast<TElem*>(arr) + N)
        {
        }

        array_view(TElem* first, TElem* last) noexcept :
                m_begin(first),
                m_end(last)
        {
        }

        [[nodiscard]] size_t size() const noexcept
        {
                return m_end - m_begin;
        }

        TElem* begin() const noexcept { return m_begin; }
        TElem* end() const noexcept { return m_end; }

private:
        TElem* m_begin = nullptr;
        TElem* m_end   = nullptr;
};

template <typename TElem>
struct array_view<const TElem> final
{
        array_view() noexcept                             = default;
        array_view(array_view&&) noexcept                 = default;
        array_view(const array_view&) noexcept            = default;
        array_view& operator=(array_view&&) noexcept      = default;
        array_view& operator=(const array_view&) noexcept = default;
        ~array_view() noexcept                            = default;

        template <size_t N>
        array_view(const TElem (&arr)[N]) noexcept :
                m_begin(static_cast<TElem*>(arr)),
                m_end(static_cast<TElem*>(arr) + N)
        {
        }

        array_view(const TElem* first, const TElem* last) noexcept :
                m_begin(first),
                m_end(last)
        {
        }

        [[nodiscard]] size_t size() const noexcept
        {
                return m_end - m_begin;
        }

        const TElem* begin() const noexcept { return m_begin; }
        const TElem* end() const noexcept { return m_end; }

private:
        const TElem* m_begin = nullptr;
        const TElem* m_end   = nullptr;
};


/********************************************************************
 * Character Traits
 ********************************************************************/
bool Character_IsLower(char character) noexcept;
bool Character_IsUpper(char character) noexcept;
bool Character_IsSpace(char character) noexcept;
bool Character_IsDigit(char character) noexcept;
bool Character_IsHexDigit(char character) noexcept;
bool Character_IsPrint(char character) noexcept;
bool Character_IsLetter(char character) noexcept;
bool Character_IsControl(char character) noexcept;
char Character_ToLower(char character) noexcept;
char Character_ToUpper(char character) noexcept;

/********************************************************************
 * String Library
 ********************************************************************/


#endif //ZE_COMMON_H
