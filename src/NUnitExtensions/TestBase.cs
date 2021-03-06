﻿using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Impl;

namespace TS.NUnitExtensions
{
    /// <summary>
    /// Base class for all test classes.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Method called once before all the tests in a given test fixture are executed.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            //write out Rhino logs for each test
            RhinoMocks.Logger = new TextWriterExpectationLogger(Console.Out);
        }

        /// <summary>
        /// Method called once after all of the tests in a given test fixture are executed.
        /// </summary>
        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
        }

        /// <summary>
        /// Method called before each test in a text fixture is executed.
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
        }

        /// <summary>
        /// Method called after each test in a text fixture is executed.
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
        }

        /// <summary>
        /// Helper method that asserts that an <see cref="ArgumentException"/> gets thrown 
        /// with the specified parameter name
        /// when the specified code is executed.
        /// </summary>
        protected TArgumentException AssertThrowsArgumentException<TArgumentException>(TestDelegate code, string expectedParamName)
            where TArgumentException : ArgumentException
        {
            var ex = Assert.Throws<TArgumentException>(code);
            Assert.That(ex.ParamName, Is.EqualTo(expectedParamName));
            return ex;
        }
    }
}
