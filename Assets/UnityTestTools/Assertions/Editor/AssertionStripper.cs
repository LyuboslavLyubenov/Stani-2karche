namespace UnityTestTools.Assertions.Editor
{

    using UnityEditor.Callbacks;

    using UnityEngine;

    public class AssertionStripper
    {
        [PostProcessScene]
        public static void OnPostprocessScene()
        {
            if (Debug.isDebugBuild) return;
            RemoveAssertionsFromGameObjects();
        }

        private static void RemoveAssertionsFromGameObjects()
        {
            var allAssertions = Resources.FindObjectsOfTypeAll(typeof(AssertionComponent)) as AssertionComponent[];
            foreach (var assertion in allAssertions)
            {
                Object.DestroyImmediate(assertion);
            }
        }
    }

}
