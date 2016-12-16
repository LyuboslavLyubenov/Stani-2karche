using System;

public interface IJokerRouter
{
    EventHandler OnActivated { get; set; }

    EventHandler OnFinished { get; set; }

    EventHandler<UnhandledExceptionEventArgs> OnError { get; set; }
}