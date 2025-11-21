#include "common.h"

/********************************************************************
 * Character Traits
 ********************************************************************/
static constexpr uint8_t CHARACTER_TYPE_LOWER       = 0x01;
static constexpr uint8_t CHARACTER_TYPE_UPPER       = 0x02;
static constexpr uint8_t CHARACTER_TYPE_CONTROL     = 0x04;
static constexpr uint8_t CHARACTER_TYPE_SPACE       = 0x08;
static constexpr uint8_t CHARACTER_TYPE_WHITESPACE  = 0x10;
static constexpr uint8_t CHARACTER_TYPE_PUNCTUATION = 0x20;
static constexpr uint8_t CHARACTER_TYPE_DIGIT       = 0x40;
static constexpr uint8_t CHARACTER_TYPE_HEX         = 0x80;
static constexpr uint8_t CHARACTER_TRAITS[256]
{
        CHARACTER_TYPE_CONTROL, // (NUL)
        CHARACTER_TYPE_CONTROL, // (SOH)
        CHARACTER_TYPE_CONTROL, // (STX)
        CHARACTER_TYPE_CONTROL, // (ETX)
        CHARACTER_TYPE_CONTROL, // (EOT)
        CHARACTER_TYPE_CONTROL, // (ENQ)
        CHARACTER_TYPE_CONTROL, // (ACK)
        CHARACTER_TYPE_CONTROL, // (BEL)
        CHARACTER_TYPE_CONTROL, // (BS)
        CHARACTER_TYPE_SPACE | CHARACTER_TYPE_CONTROL | CHARACTER_TYPE_WHITESPACE, // (HT)
        CHARACTER_TYPE_SPACE | CHARACTER_TYPE_CONTROL, // (LF)
        CHARACTER_TYPE_SPACE | CHARACTER_TYPE_CONTROL, // (VT)
        CHARACTER_TYPE_SPACE | CHARACTER_TYPE_CONTROL, // (FF)
        CHARACTER_TYPE_SPACE | CHARACTER_TYPE_CONTROL, // (CR)
        CHARACTER_TYPE_CONTROL, // (SI)
        CHARACTER_TYPE_CONTROL, // (SO)
        CHARACTER_TYPE_CONTROL, // (DLE)
        CHARACTER_TYPE_CONTROL, // (DC1)
        CHARACTER_TYPE_CONTROL, // (DC2)
        CHARACTER_TYPE_CONTROL, // (DC3)
        CHARACTER_TYPE_CONTROL, // (DC4)
        CHARACTER_TYPE_CONTROL, // (NAK)
        CHARACTER_TYPE_CONTROL, // (SYN)
        CHARACTER_TYPE_CONTROL, // (ETB)
        CHARACTER_TYPE_CONTROL, // (CAN)
        CHARACTER_TYPE_CONTROL, // (EM)
        CHARACTER_TYPE_CONTROL, // (SUB)
        CHARACTER_TYPE_CONTROL, // (ESC)
        CHARACTER_TYPE_CONTROL, // (FS)
        CHARACTER_TYPE_CONTROL, // (GS)
        CHARACTER_TYPE_CONTROL, // (RS)
        CHARACTER_TYPE_CONTROL, // (US)
        CHARACTER_TYPE_SPACE | CHARACTER_TYPE_WHITESPACE, // SPACE
        CHARACTER_TYPE_PUNCTUATION, // !
        CHARACTER_TYPE_PUNCTUATION, // "
        CHARACTER_TYPE_PUNCTUATION, // #
        CHARACTER_TYPE_PUNCTUATION, // $
        CHARACTER_TYPE_PUNCTUATION, // %
        CHARACTER_TYPE_PUNCTUATION, // &
        CHARACTER_TYPE_PUNCTUATION, // '
        CHARACTER_TYPE_PUNCTUATION, // (
        CHARACTER_TYPE_PUNCTUATION, // )
        CHARACTER_TYPE_PUNCTUATION, // *
        CHARACTER_TYPE_PUNCTUATION, // +
        CHARACTER_TYPE_PUNCTUATION, // ,
        CHARACTER_TYPE_PUNCTUATION, // -
        CHARACTER_TYPE_PUNCTUATION, // .
        CHARACTER_TYPE_PUNCTUATION, // /
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 0
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 1
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 2
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 3
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 4
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 5
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 6
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 7
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 8
        CHARACTER_TYPE_DIGIT | CHARACTER_TYPE_HEX, // 9
        CHARACTER_TYPE_PUNCTUATION, // :
        CHARACTER_TYPE_PUNCTUATION, // ;
        CHARACTER_TYPE_PUNCTUATION, // <
        CHARACTER_TYPE_PUNCTUATION, // =
        CHARACTER_TYPE_PUNCTUATION, // >
        CHARACTER_TYPE_PUNCTUATION, // ?
        CHARACTER_TYPE_PUNCTUATION, // @
        CHARACTER_TYPE_UPPER | CHARACTER_TYPE_HEX, // A
        CHARACTER_TYPE_UPPER | CHARACTER_TYPE_HEX, // B
        CHARACTER_TYPE_UPPER | CHARACTER_TYPE_HEX, // C
        CHARACTER_TYPE_UPPER | CHARACTER_TYPE_HEX, // D
        CHARACTER_TYPE_UPPER | CHARACTER_TYPE_HEX, // E
        CHARACTER_TYPE_UPPER | CHARACTER_TYPE_HEX, // F
        CHARACTER_TYPE_UPPER, // G
        CHARACTER_TYPE_UPPER, // H
        CHARACTER_TYPE_UPPER, // I
        CHARACTER_TYPE_UPPER, // J
        CHARACTER_TYPE_UPPER, // K
        CHARACTER_TYPE_UPPER, // L
        CHARACTER_TYPE_UPPER, // M
        CHARACTER_TYPE_UPPER, // N
        CHARACTER_TYPE_UPPER, // O
        CHARACTER_TYPE_UPPER, // P
        CHARACTER_TYPE_UPPER, // Q
        CHARACTER_TYPE_UPPER, // R
        CHARACTER_TYPE_UPPER, // S
        CHARACTER_TYPE_UPPER, // T
        CHARACTER_TYPE_UPPER, // U
        CHARACTER_TYPE_UPPER, // V
        CHARACTER_TYPE_UPPER, // W
        CHARACTER_TYPE_UPPER, // X
        CHARACTER_TYPE_UPPER, // Y
        CHARACTER_TYPE_UPPER, // Z
        CHARACTER_TYPE_PUNCTUATION, // [
        CHARACTER_TYPE_PUNCTUATION, // "\"
        CHARACTER_TYPE_PUNCTUATION, // ]
        CHARACTER_TYPE_PUNCTUATION, // ^
        CHARACTER_TYPE_PUNCTUATION, // _
        CHARACTER_TYPE_PUNCTUATION, // `
        CHARACTER_TYPE_LOWER | CHARACTER_TYPE_HEX, // a
        CHARACTER_TYPE_LOWER | CHARACTER_TYPE_HEX, // b
        CHARACTER_TYPE_LOWER | CHARACTER_TYPE_HEX, // c
        CHARACTER_TYPE_LOWER | CHARACTER_TYPE_HEX, // d
        CHARACTER_TYPE_LOWER | CHARACTER_TYPE_HEX, // e
        CHARACTER_TYPE_LOWER | CHARACTER_TYPE_HEX, // f
        CHARACTER_TYPE_LOWER, // g
        CHARACTER_TYPE_LOWER, // h
        CHARACTER_TYPE_LOWER, // i
        CHARACTER_TYPE_LOWER, // j
        CHARACTER_TYPE_LOWER, // k
        CHARACTER_TYPE_LOWER, // l
        CHARACTER_TYPE_LOWER, // m
        CHARACTER_TYPE_LOWER, // n
        CHARACTER_TYPE_LOWER, // o
        CHARACTER_TYPE_LOWER, // p
        CHARACTER_TYPE_LOWER, // q
        CHARACTER_TYPE_LOWER, // r
        CHARACTER_TYPE_LOWER, // s
        CHARACTER_TYPE_LOWER, // t
        CHARACTER_TYPE_LOWER, // u
        CHARACTER_TYPE_LOWER, // v
        CHARACTER_TYPE_LOWER, // w
        CHARACTER_TYPE_LOWER, // x
        CHARACTER_TYPE_LOWER, // y
        CHARACTER_TYPE_LOWER, // z
        CHARACTER_TYPE_PUNCTUATION, // {
        CHARACTER_TYPE_PUNCTUATION, // |
        CHARACTER_TYPE_PUNCTUATION, // }
        CHARACTER_TYPE_PUNCTUATION, // ~
        CHARACTER_TYPE_CONTROL, // (DEL)
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
};

bool Character_IsLower(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_LOWER;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

bool Character_IsUpper(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_UPPER;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

bool Character_IsSpace(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_SPACE;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

bool Character_IsDigit(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_DIGIT;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

bool Character_IsHexDigit(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_HEX;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

bool Character_IsPrint(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_UPPER |
                CHARACTER_TYPE_LOWER |
                CHARACTER_TYPE_DIGIT |
                CHARACTER_TYPE_WHITESPACE |
                CHARACTER_TYPE_PUNCTUATION;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

bool Character_IsLetter(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_UPPER | CHARACTER_TYPE_LOWER;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

bool Character_IsControl(char character) noexcept
{
        constexpr auto flags = CHARACTER_TYPE_CONTROL;

        auto const index = static_cast<unsigned char>(character);
        return (CHARACTER_TRAITS[index] & flags) == flags;
}

char Character_ToLower(char character) noexcept
{
        if (character >= 'A' && character <= 'Z')
        {
                character += 'a' - 'A';
        }
        return character;
}

char Character_ToUpper(char character) noexcept
{
        if (character >= 'a' && character <= 'z')
        {
                character -= 'a' - 'A';
        }
        return character;
}

/********************************************************************
 * String Library
 ********************************************************************/
