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

    class Screen
    {
    public:
        Screen();

        int Loop(IView^ view);

        void Quit(int exitcode);
        void Invalidate();

        void GotoXY(int x, int y);
        void EnableCursor(bool enable);
        Size GetSize();

    private:
        int m_exitcode = -1;
        bool m_quit = false;
        bool m_invalidated = false;
    };
}
