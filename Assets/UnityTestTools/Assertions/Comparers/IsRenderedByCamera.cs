namespace Assets.UnityTestTools.Assertions.Comparers
{

    using System;

    using UnityEngine;

    public class IsRenderedByCamera : ComparerBaseGeneric<Renderer, Camera>
    {
        public enum CompareType
        {
            IsVisible,
            IsNotVisible,
        };

        public CompareType compareType;

        protected override bool Compare(Renderer renderer, Camera camera)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            var isVisible = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
            switch (this.compareType)
            {
                case CompareType.IsVisible:
                    return isVisible;
                case CompareType.IsNotVisible:
                    return !isVisible;
            }
            throw new Exception();
        }
    }
}
