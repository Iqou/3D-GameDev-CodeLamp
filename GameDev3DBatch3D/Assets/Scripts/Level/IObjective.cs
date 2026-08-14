using System;
using System.Collections.Generic;

public interface IObjective
{
    string ObjectiveName { get; }

    bool IsComplete { get; }

    event Action<IObjective> OnCompleted;
}