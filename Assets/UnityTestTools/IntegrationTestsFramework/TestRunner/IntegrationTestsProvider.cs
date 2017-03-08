namespace Assets.UnityTestTools.IntegrationTestsFramework.TestRunner
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    class IntegrationTestsProvider
    {
        internal Dictionary<ITestComponent, HashSet<ITestComponent>> testCollection = new Dictionary<ITestComponent, HashSet<ITestComponent>>();
        internal ITestComponent currentTestGroup;
        internal IEnumerable<ITestComponent> testToRun;

        public IntegrationTestsProvider(IEnumerable<ITestComponent> tests)
        {
            this.testToRun = tests;
            foreach (var test in tests.OrderBy(component => component))
            {
                if (test.IsTestGroup())
                {
                    throw new Exception(test.Name + " is test a group");
                }
                this.AddTestToList(test);
            }
            if (this.currentTestGroup == null)
            {
                this.currentTestGroup = this.FindInnerTestGroup(TestComponent.NullTestComponent);
            }
        }

        private void AddTestToList(ITestComponent test)
        {
            var group = test.GetTestGroup();
            if (!this.testCollection.ContainsKey(group))
                this.testCollection.Add(group, new HashSet<ITestComponent>());
            this.testCollection[group].Add(test);
            if (group == TestComponent.NullTestComponent) return;
            this.AddTestToList(group);
        }

        public ITestComponent GetNextTest()
        {
            var test = this.testCollection[this.currentTestGroup].First();
            this.testCollection[this.currentTestGroup].Remove(test);
            test.EnableTest(true);
            return test;
        }

        public void FinishTest(ITestComponent test)
        {
            try
            {
                test.EnableTest(false);
                this.currentTestGroup = this.FindNextTestGroup(this.currentTestGroup);
            }
            catch (MissingReferenceException e)
            {
                Debug.LogException(e);
            }
        }

        private ITestComponent FindNextTestGroup(ITestComponent testGroup)
        {
            if (testGroup == null) 
                throw new Exception ("No test left");

            if (this.testCollection[testGroup].Any())
            {
                testGroup.EnableTest(true);
                return this.FindInnerTestGroup(testGroup);
            }
            this.testCollection.Remove(testGroup);
            testGroup.EnableTest(false);

            var parentTestGroup = testGroup.GetTestGroup();
            if (parentTestGroup == null) return null;

            this.testCollection[parentTestGroup].Remove(testGroup);
            return this.FindNextTestGroup(parentTestGroup);
        }

        private ITestComponent FindInnerTestGroup(ITestComponent group)
        {
            var innerGroups = this.testCollection[group];
            foreach (var innerGroup in innerGroups)
            {
                if (!innerGroup.IsTestGroup()) continue;
                innerGroup.EnableTest(true);
                return this.FindInnerTestGroup(innerGroup);
            }
            return group;
        }

        public bool AnyTestsLeft()
        {
            return this.testCollection.Count != 0;
        }

        public List<ITestComponent> GetRemainingTests()
        {
            var remainingTests = new List<ITestComponent>();
            foreach (var test in this.testCollection)
            {
                remainingTests.AddRange(test.Value);
            }
            return remainingTests;
        }
    }
}
