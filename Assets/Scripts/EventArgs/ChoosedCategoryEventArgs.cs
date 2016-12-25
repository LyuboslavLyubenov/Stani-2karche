using System;

namespace Assets.Scripts.EventArgs
{

    using EventArgs = System.EventArgs;

    public class ChoosedCategoryEventArgs : EventArgs
    {
        public string Name
        {
            get;
            private set;
        }

        public ChoosedCategoryEventArgs(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this.Name = name;
        }
    }

}