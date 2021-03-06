﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TS.NUnitExtensions
{
    /// <summary>
    /// Extension methods used by the <see cref="ContainsStateConstraint"/>.
    /// </summary>
    public static class ContainsStateExtensions
    {
        /// <summary>
        /// Determines if the actual object contains the state of the expected object.
        /// </summary>
        /// <param name="actual">
        /// The object whose state is being examined.
        /// </param>
        /// <param name="expected">
        /// The state to confirm within the <paramref name="actual"/> object.  
        /// This parameter can be any object; however, a useful application is to use an 
        /// anonymous type.
        /// </param>
        /// <remarks>
        /// This method is useful for state-based unit tests where you need to assert 
        /// the state of objects with complext structures.
        /// </remarks>
        public static ContainsStateResult ContainsState(this object actual, object expected)
        {
            return actual.ContainsState(expected, LocationDelimiter);
        }

        private const string LocationDelimiter = "/";

        private static HashSet<Type> _simpleTypes;
        private static HashSet<Type> SimpleTypes
        {
            get
            {
                if (_simpleTypes == null)
                {
                    var types = new[]
                                    {
                                        // primatives
                                        typeof (bool),
                                        typeof (byte),
                                        typeof (sbyte),
                                        typeof (short),
                                        typeof (UInt16),
                                        typeof (int),
                                        typeof (UInt32),
                                        typeof (long),
                                        typeof (UInt64),
                                        typeof (IntPtr),
                                        typeof (UIntPtr),
                                        typeof (char),
                                        typeof (double),
                                        typeof (float),

                                        // other
                                        typeof (string),
                                        typeof (Guid),
                                        typeof (DateTime),
                                        typeof (TimeSpan)
                                    };

                    _simpleTypes = new HashSet<Type>(
                        // combine types above
                        types
                            // with the Nullables of all the value types
                            .Concat(types
                                        .Where(t => t.IsValueType)
                                        .Select(t => typeof (Nullable<>).MakeGenericType(t))));
                }
                return _simpleTypes;
            }
        }

        private static ContainsStateResult ContainsState(this object actual, object expected, string location, Type actualType = null)
        {
            // check for object equality
            if (Equals(actual, expected))
                return new ContainsStateResult();

            // only continue more complex examination if both values are not null
            if (actual != null && expected != null)
            {
                if (actualType == null)
                    actualType = actual.GetType();
                var expectedType = expected.GetType();

                // only continue more complex examination if object is not one of the simple types
                if (!SimpleTypes.Contains(expectedType))
                {

                    // check for collection equality
                    if (!(actual is string) && !(expected is string))
                    {
                        var actualEnumerable = actual as IEnumerable;
                        var expectedEnumerable = expected as IEnumerable;
                        if (actualEnumerable != null && expectedEnumerable != null)
                        {
                            var actualEnumerator = actualEnumerable.GetEnumerator();
                            var expectedEnumerator = expectedEnumerable.GetEnumerator();
                            var index = 0;

                            while (expectedEnumerator.MoveNext())
                            {
                                // actual collection not big enough
                                if (!actualEnumerator.MoveNext())
                                    return new ContainsStateResult(location,
                                                                   "Actual collection (size = {0}) is smaller than expected collection.",
                                                                   index);

                                // compare collection items
                                var result = actualEnumerator.Current.ContainsState(expectedEnumerator.Current,
                                                                                    location.Append(index));
                                if (!result.Success)
                                    return result;

                                index++;
                            }

                            // make sure actual collection doesn't contain any more items
                            if (actualEnumerator.MoveNext())
                                return new ContainsStateResult(location,
                                                               "Actual collection is larger than expected collection (size = {0}).",
                                                               index);

                            // collections were equal
                            return new ContainsStateResult();
                        }
                    }

                    // check for property equality
                    var actualProperties = actualType.GetProperties().ToDictionary(p => p.Name);
                    var expectedProperties = expectedType.GetProperties();

                    foreach (var expectedProperty in expectedProperties)
                    {
                        // actual property doesn't exist
                        if (!actualProperties.ContainsKey(expectedProperty.Name))
                            return new ContainsStateResult(location,
                                                           "Expected property '{0}' is missing in actual object.",
                                                           expectedProperty.Name);

                        // compare property values
                        var actualProperty = actualProperties[expectedProperty.Name];
                        var actualValue = actualProperty.GetValue(actual, null);
                        var expectedValue = expectedProperty.GetValue(expected, null);

                        var result = actualValue.ContainsState(expectedValue,
                                                               location.Append(expectedProperty.Name),
                                                               actualProperty.PropertyType);
                        if (!result.Success)
                            return result;
                    }

                    // properties were equal
                    return new ContainsStateResult();
                }
            }

            // expected is not contained within actual
            return new ContainsStateResult(location,
                                           "Actual value is not equal to expected value.")
                       {
                           Actual = actual,
                           Expected = expected
                       };
        }

        private static string Append(this string location, object value)
        {
            return string.Concat(location,
                                 location.EndsWith(LocationDelimiter) ? string.Empty : LocationDelimiter,
                                 value);
        }
    }
}
