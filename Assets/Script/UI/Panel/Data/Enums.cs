using System;

/// <summary>
/// 패널 애니메이션 타입
/// </summary>
[Flags]
public enum PanelType
{
    None = 0,
    FadeInOut = 1 << 0,
    ScaleInOut = 1 << 1,
    Move = 1 << 2
}