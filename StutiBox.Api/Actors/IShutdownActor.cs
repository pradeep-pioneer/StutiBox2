using System;
namespace StutiBox.Api.Actors
{
    public enum ShutdownType
    {
        RestartImmediate = 0,
        ShutdownImmediate = 1,
    }
    public interface IShutdownActor
    {
        void Initiate(ShutdownType shutdownType);
    }
}
