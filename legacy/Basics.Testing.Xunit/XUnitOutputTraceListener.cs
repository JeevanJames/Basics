using System.Diagnostics;

using Xunit.Abstractions;

namespace Basics.Testing.Xunit
{
    public sealed class XUnitOutputTraceListener : TraceListener
    {
        private readonly ITestOutputHelper _output;

        public XUnitOutputTraceListener(ITestOutputHelper output)
        {
            _output = output;
        }

        public XUnitOutputTraceListener(string name, ITestOutputHelper output) : base(name)
        {
            _output = output;
        }

        /// <summary>
        ///     When overridden in a derived class, writes the specified message to the listener you create in the derived
        ///     class.
        /// </summary>
        /// <param name="message">A message to write. </param>
        public override void Write(string message)
        {
            _output.WriteLine(message);
        }

        /// <summary>
        ///     When overridden in a derived class, writes a message to the listener you create in the derived class, followed
        ///     by a line terminator.
        /// </summary>
        /// <param name="message">A message to write. </param>
        public override void WriteLine(string message)
        {
            _output.WriteLine(message);
        }
    }
}
