namespace Assets.Scripts.Utils
{
    using System;

    public class TimerUtils
    {
        TimerUtils()
        {
            
        }
        
        public void ExecuteAfter(float seconds, Action method)
        {
            new Timer_ExecuteMethodAfterTime(seconds * 1000)
            {
                Method = method,
                AutoDispose = true
            };
        }

        public void ExecuteEvery(float seconds, Action method)
        {
            new Timer_ExecuteMethodEverySeconds(seconds * 1000)
            {
                Method = method
            };
        }
    }
}