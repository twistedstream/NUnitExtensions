#NUnitExtensions
Handy base test classes and other extensions useful for TDD using NUnit.

##Getting Started

Run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console):
```
PM> Install-Package NUnitExtensions
```

##To Build

```
git submodule update --init
.\build.cmd
```

**NOTE:** You should perform an initial build before compiling for the first time in Visual Studio as some non-source-controled code files need to be generated.

##Current Base Classes

**TestBase**

Base class for all test classes.  Provides simple overrides for TestFixtureSetUp, TestFixtureTearDown, SetUp, and TearDown attributes.

**ComponentTestBase<TComponent>**

A TestBase for test classes that need to test a component, which is a class that uses constructor-based dependency injection.

```cs
// Assume: Foo class has IBar and IBaz dependencies, injected through the constructor
[TestFixture]
public class FooTests : ComponentTestBase<Foo>
{
    private IBar _bar;
    private IBaz _baz;

    // override Setup from TestBase to create stub instances of your dependencies
    public override void Setup()
    {
        base.Setup();

        _bar = MockRepository.GenerateStub<IBar>();
        _baz = MockRepository.GenerateStub<IBaz>();
    }

    // override GetDependencies to provide base class with instances of your dependencies
    protected override IEnumerable<object> GetDependencies()
    {
        yield return _bar;
        yield return _baz;
    }

    [Test]
    public void TestMethod1()
    {
        // use ComponentTestBase.CreateTarget to create an instance of the class under test with mocked dependencies
        var target = CreateTarget();
        
        // perform test
    }
    
    // with ComponentTestBase, you also inherit test: Instances_should_require_their_dependencies
    // (see source code for more details)
}
```

**ComponentWithInterfaceTestBase<TComponent, TInterface>**

A ComponentTestBase<TComponent> for test classes that need to test a component that implements an interface.

**DependencyContainerTestBase<TDependencyContainer>**

A ComponentTestBase<TComponent> for test classes that need to test a component that is a dependency container, which is a simple class that exposes all the parameters to its constructor via properties of the same name.
    
##Current NUnit Constraints

**ContainsStateConstraint**

An NUnit Constraint that determines if the actual object contains the state of the expected object.  Very useful for state-based testing.  Easily accessed using the ContainsState.With() static method:

```cs
[Test]
public void TestMethod1()
{
    var target = CreateTarget();
    var result = target.MethodBeingTested("tron");

    Assert.That(result, ContainsState.With(
        // use anonymous type to test whatever desired subset of state
        new
            {
                FirstName = "Kevin",
                LastName = "Flynn"
            }));
}
```
