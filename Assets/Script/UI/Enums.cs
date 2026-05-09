using System;

public enum ButtonType
{
    None,
    ChangePanel,
    OpenPopup,
    ClosePopup,
    GoScene,
    Quit
}

[Flags]
public enum UIAnimaType
{
    None = 0,
    FadeInOut = 1 << 0,
    ScaleInOut = 1 << 1,
    Moving = 1 << 2
}