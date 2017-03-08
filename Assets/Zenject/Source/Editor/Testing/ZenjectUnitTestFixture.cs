﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zenject;
using NUnit.Framework;

using Assert=Assets.Zenject.Source.Internal.Assert;

namespace Zenject
{

    using Assets.Zenject.Source.Main;

    // Inherit from this and mark you class with [TestFixture] attribute to do some unit tests
    // For anything more complicated than this, such as tests involving interaction between
    // several classes, or if you want to use interfaces such as IInitializable or IDisposable,
    // then I recommend using ZenjectIntegrationTestFixture instead
    // See documentation for details
    public abstract class ZenjectUnitTestFixture
    {
        DiContainer _container;

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        [SetUp]
        public virtual void Setup()
        {
            _container = new DiContainer();
        }
    }
}
