﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace TS.Testing
{
    /// <summary>
    /// A <see cref="TestBase"/> for test classes that need to test a component, 
    /// which is a class that uses constructor-based dependency injection.
    /// </summary>
    /// <typeparam name="TComponent">
    /// The type of the component being tested by the concrete test class.
    /// </typeparam>
    public abstract class ComponentTestBase<TComponent>
        : TestBase
        where TComponent : class 
    {
        /// <summary>
        /// Gets the set of dependency values (in the correct order) that get injected into the component's constructor
        /// during the <see cref="CreateTarget"/> method.
        /// </summary>
        protected abstract IEnumerable<object> GetDependencies();

        /// <summary>
        /// Allows tests to create instances of the component with all of its injected dependencies.
        /// </summary>
        protected TComponent CreateTarget()
        {
            var values = GetDependencies().ToArray();

            var target = (TComponent)Activator.CreateInstance(
                typeof(TComponent),
                values);
            return target;
        }

        /// <summary>
        /// Allow subclasses to get access to the dependency parameters passed into the constructor.
        /// </summary>
        /// <returns>
        /// An dictionary containing all dependency instances by name.
        /// </returns>
        protected IDictionary<string, object> GetDependencyParameters()
        {
            // Get constructor
            var type = typeof (TComponent);
            var constructors = type.GetConstructors();

            // Only support a single constructor
            if (constructors.Length != 1)
                throw new NotSupportedException("Component class can only have one constructor.");

            var constructor = constructors.First();

            // Get constructor's parameters
            var constructorParameters = constructor.GetParameters();

            // Get valid instances for each parameter
            var values = GetDependencies().ToList();

            // Make sure both collections are the same size
            if (values.Count != constructorParameters.Length)
                throw new NotSupportedException(
                    string.Format("The number of values returned by GetParameters ({0}) does not match the number of constructor parameters on the component ({1}).",
                                  values.Count,
                                  constructorParameters.Length));

            // Build dictionary of parmeter names to dependency values
            var tuples = new List<Tuple<string, object>>();
            for (var index = 0; index < constructorParameters.Length; index++)
            {
                var parameter = constructorParameters[index];
                var value = values[index];

                // Make sure value is not null
                if (value == null)
                    throw new InvalidOperationException(
                        string.Format("Dependency value for parameter '{0}' cannot be null.",
                                      parameter.Name));

                var tuple = Tuple.Create(parameter.Name, value);
                tuples.Add(tuple);
            }
            return tuples.ToDictionary(s => s.Item1, s => s.Item2);
        }

        /// <summary>
        /// A test that asserts that the instance under test requires all dependencies 
        /// (specified by the test class by overriding <see cref="GetDependencies"/>) by 
        /// injecting them through the constructor.
        /// </summary>
        [Test]
        public void Instances_should_require_their_dependencies()
        {
            // get dependency parameters
            var parameters = GetDependencyParameters();

            // Short-circuit pass if class only has a default (parameter-less) constructor
            if (!parameters.Any()) return;

            // Enumerate through each reference type parameter and create component with a null value
            foreach (var key in parameters.Keys)
            {
                // skip parameter if it is not a reference type
                if (parameters[key].GetType().IsValueType) continue;

                var name = key;

                // Build parameters that all are all dependencies except for the current parameter, which is null
                var args = (from p in parameters.Keys
                            select p == name
                                       ? null
                                       : parameters[p]).ToArray();

                // Create class instance and test for expected exception
                var expectedExceptionThrown = false;
                try
                {
                    Activator.CreateInstance(typeof(TComponent), args);
                }
                catch (TargetInvocationException ex)
                {
                    var inner = ex.InnerException as ArgumentNullException;
                    expectedExceptionThrown = inner != null;
                }

                if (expectedExceptionThrown) continue;

                Assert.Fail(
                    "Component class has constructor parameter ('{0}') that does not throw an {1} when passed a null value.",
                    name,
                    typeof (ArgumentNullException));
            }
        }
    }
}