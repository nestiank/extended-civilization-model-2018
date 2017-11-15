#pragma once

namespace FakeView
{
    interface class IView
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

        int Loop(IView^ view);

        void Quit(int exitcode);

        auto& GetBuffer()
        {
            return m_buffer;
        }
        auto& GetChar(int x, int y)
        {
            auto sz = GetSize();
            return m_buffer.at(y * sz.width + x);
        }

        void GotoXY(int x, int y);
        void EnableCursor(bool enable);
        Size GetSize();

    private:
        void DrawBuffer();
        void ClearBuffer();

        int m_exitcode = -1;
        bool m_quit = false;

        std::array<Character, 120 * 36> m_buffer;
    };
}
