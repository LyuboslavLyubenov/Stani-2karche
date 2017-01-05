namespace Assets.Scripts.Extensions
{

    using System.Timers;

    public static class TimerExtensions
    {
        public static void Reset(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
