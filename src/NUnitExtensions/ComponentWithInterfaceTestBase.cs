﻿using NUnit.Framework;

namespace TS.NUnitExtensions
{
    /// <summary>
    /// A <see cref="ComponentTestBase{TComponent}"/> for test classes that need to test a component 
    /// that implements an interface.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The type of the component being tested by the concrete test class.
    /// </typeparam>
    /// <typeparam name="TInterface">
    /// The type of the interface that the component implements.
    /// </typeparam>
    public abstract class ComponentWithInterfaceTestBase<TComponent, TInterface>
        : ComponentTestBase<TComponent>
        where TComponent : class, TInterface
    {
        /// <summary>
        /// Allows tests to create instances of the component with all of its injected dependencies.
        /// </summary>
        protected new TInterface CreateTarget()
        {
            return base.CreateTarget();
        }

        /// <summary>
        /// Test that asserts that the <typeparamref name="TComponent"/> component class under test 
        /// implements the specified <typeparamref name="TInterface" /> interface.
        /// </summary>
        [Test]
        public void Class_should_implement_the_component_interface()
        {
            Assert.That(typeof(TInterface).IsAssignableFrom(typeof(TComponent)));
        }
    }
}