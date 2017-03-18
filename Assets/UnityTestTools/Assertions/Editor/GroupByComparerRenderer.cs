namespace UnityTestTools.Assertions.Editor
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GroupByComparerRenderer : AssertionListRenderer<Type>
    {
        protected override IEnumerable<IGrouping<Type, AssertionComponent>> GroupResult(IEnumerable<AssertionComponent> assertionComponents)
        {
            return assertionComponents.GroupBy(c => c.Action.GetType());
        }

        protected override string GetStringKey(Type key)
        {
            return key.Name;
        }
    }
}
