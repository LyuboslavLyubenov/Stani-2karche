namespace UnityTestTools.Assertions.Comparers
{

    public class ValueDoesNotChange : ActionBase
    {
        private object m_Value;

        protected override bool Compare(object a)
        {
            if (this.m_Value == null)
                this.m_Value = a;
            if (!this.m_Value.Equals(a))
                return false;
            return true;
        }
    }
}
