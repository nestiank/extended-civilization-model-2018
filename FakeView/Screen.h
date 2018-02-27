#pragma once

namespace FakeView
{
    interface class IScreenClient
    {
        void Render();
        void OnKeyStroke(int ch);
        void OnTick();
    };

    struct Size
    {
        int width;
        int height;
    };

    struct Character
    {
        char ch;
        unsigned char color;
    };

    class Screen
    {
    public:
        Screen();

        int Loop(IScreenClient^ view);
        void Invoke(System::Action^ fn);

        void Quit(int exitcode);

        Character& GetChar(int x, int y);
        Character* TryGetChar(int x, int y);

        Size GetSize();

        void PrintString(int x, int y, unsigned char color, const std::string& str);
        void PrintStringEx(int x, int y, unsigned char color, const std::string& str);

    private:
        void GotoXY(int x, int y);
        void EnableCursor(bool enable);

        void DrawBuffer();
        void ClearBuffer();

        int m_exitcode = -1;
        bool m_quit = false;

        CRITICAL_SECTION m_crit;
        std::deque<gcroot<System::Action^>> m_invokee;

        std::array<Character, 150 * 45> m_buffer;
    };
}
