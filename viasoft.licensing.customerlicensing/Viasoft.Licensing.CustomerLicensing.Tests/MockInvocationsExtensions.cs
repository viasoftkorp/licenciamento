using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest
{
    public static class MockInvocationsExtensions
    {
        public static void AssertCount(this IInvocationList invocationList, int expectCount, string method)
        {
            var callCount = invocationList.Count(i => i.Method.Name == method);
            Assert.Equal(expectCount, callCount);
        }
        
        public static void AssertSingle(this IInvocationList invocationList, string method)
        {
            invocationList.AssertCount(1, method);
        }

        public static void AssertArgument(this IInvocationList invocationList, int argumentIndex, string method, object expected)
        {
            invocationList.AssertArgument(0, argumentIndex, method, expected);
        }

        public static void AssertArgument(this IInvocationList invocationList, int invocationIndex, int argumentIndex, string method, object expected)
        {
            var invocations = invocationList.Where(i => i.Method.Name == method).ToList();
            var arguments = invocations[invocationIndex].Arguments;
            arguments[argumentIndex].Should().BeEquivalentTo(expected);
        }
        
        public static void AssertContains(this IInvocationList invocationList, string method, object expected)
        {
            invocationList.AssertContains(0, method, expected);
        }
        
        public static void AssertContains(this IInvocationList invocationList, int invocationIndex, string method, object expected)
        {
            var invocations = invocationList.Where(i => i.Method.Name == method).ToList();
            var arguments = invocations[invocationIndex].Arguments;
            Assert.Contains(expected, arguments);
        }
    }
}