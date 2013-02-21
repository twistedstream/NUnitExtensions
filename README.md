#NUnitExtensions
Contains handy base test classes and other extensions useful for TDD using NUnit.

##Getting Started

**COMING SOON:** NuGet package integration
For now, peform a build (see below) and copy the following binaries into a lib folder in your project:

    src\NUnitExtensions\bin\Release\TS.NUnitExtensions.dll
    src\NUnitExtensions\bin\Release\TS.NUnitExtensions.pdb
    src\NUnitExtensions\bin\Release\TS.NUnitExtensions.xml

##To build:

    .\build.cmd

**NOTE:** You should perform an initial build before compiling for the first time in Visual Studio as some non-source-controled code files need to be generated.

##Current Base Classes

Name | Description 
--- | --- 
TestBase | Base class for all test classes.  Provides simple overrides for TestFixtureSetUp, TestFixtureTearDown, SetUp, and TearDown attributes.
ComponentTestBase&lt;TComponent&gt; | A TestBase for test classes that need to test a component, which is a class that uses constructor-based dependency injection.
ComponentWithInterfaceTestBase&lt;TComponent, TInterface&gt; | A ComponentTestBase&lt;TComponent&gt; for test classes that need to test a component that implements an interface.
DependencyContainerTestBase&lt;TDependencyContainer&gt; | A ComponentTestBase&lt;TComponent&gt; for test classes that need to test a component that is a dependency container, which is a simple class that exposes all the parameters to its constructor via properties of the same name.
    
##Current NUnit Constraints

Name | Description
--- | ---
ContainsStateConstraint | An NUnit Constraint that determines if the actual object contains the state of the expected object.  Very useful for state-based testing.