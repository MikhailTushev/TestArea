using System;
using TestArea.Workers;

namespace TestArea;

public record SomeDto(int LineServiceId,
    int TranslationId,
    DateTimeOffset ProxyTimestamps,
    int EventCode,
    int ScoreHome,
    int ScoreAway,
    int PointHome,
    int PointAway,
    int GamePeriod) : IMessage;