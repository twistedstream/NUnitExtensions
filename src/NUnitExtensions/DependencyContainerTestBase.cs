using System;
using System.Linq;
using NUnit.Framework;

namespace TS.NUnitExtensions
{
    /// <summary>
    /// A <see cref="ComponentTestBase{TComponent}"/> for test classes that need to test a component 
    /// that is a dependency container, which is a simple class that exposes all the parameters to its 
    /// constructor via properties of the same name.
    /// </summary>
    /// <typeparam name="TDependencyContainer">
    /// The type of the dependency container being tested by the concrete test class.
    /// </typeparam>
    public abstract class DependencyContainerTestBase<TDependencyContainer>
        : ComponentTestBase<TDependencyContainer>
        where TDependencyContainer : class
    {
        /// <summary>
        /// A test that asserts that the instance under test exposes all of its dependencies 
        /// (specified by the test class by overriding 
        /// <see cref="ComponentTestBase{TComponent}.GetDependencies"/>) as properties.
        /// </summary>
        [Test]
        public void Instances_should_expose_their_dependencies_as_properties()
        {
            var dependencies = GetDependencyParameters();

            if (!dependencies.Any())
                throw new NotSupportedException("A dependency container must have at least one parameter.");

            var type = typeof (TDependencyContainer);

            // create an instance of the dependency container
            var args = GetDependencies().ToArray();
            var target = Activator.CreateInstance(type, args);

            // get list of public property names
            var propertiesByName = type.GetProperties().ToDictionary(x => x.Name.ToLower());

            // make sure a public property exists for each dependency parameter (ignoring case)
            foreach (var dependency in dependencies)
            {
                var parameterName = dependency.Key.ToLower();
                if (!propertiesByName.ContainsKey(parameterName))
                    Assert.Fail("No matching property found for dependency parameter '{0}'.",
                                dependency.Key);

                var property = propertiesByName[parameterName];

                // make sure value returned by property is the same as the dependency parameter
                var propertyValue = property.GetValue(target, null);
                if (!propertyValue.Equals(dependency.Value))
                    Assert.Fail("Property '{0}' did not return the same value as dependency parameter '{1}'.",
                                property.Name,
                                dependency.Key);
            }
        }
    }
}