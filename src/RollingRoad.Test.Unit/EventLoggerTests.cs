using NUnit.Framework;
using RollingRoad.Core.ApplicationServices;

namespace RollingRoad.Test.Unit
{
    [TestFixture]
    public class EventLoggerTests
    {
        [Test]
        public void WriteLine_SendString_EventCalled()
        {
            Logger eventLogger = new Logger();
            int eventCallCount = 0;

            eventLogger.OnLog += (sender, args) => eventCallCount++;

            eventLogger.WriteLine("Test line");

            Assert.That(eventCallCount, Is.EqualTo(1));
        }

        [TestCase("Test one")]
        [TestCase("Two tests")]
        public void WriteLine_SendSingleString_CorrectStringInEvent(string str)
        {
            Logger eventLogger = new Logger();
            string eventCallValue = "";

            eventLogger.OnLog += (sender, args) => eventCallValue = args;

            eventLogger.WriteLine(str);

            Assert.That(eventCallValue, Is.EqualTo(str));
        }

        [TestCase("Test", 4)]
        [TestCase("All the strings", 10)]
        public void WriteLine_SendMultipleStrings_CorrectStringAtEnd(string prefix, int number)
        {
            Logger eventLogger = new Logger();
            string eventCallValue = "";

            eventLogger.OnLog += (sender, args) => eventCallValue = args;
            
            int i;
            for (i = 0; i < number; i++)
            {
                eventLogger.WriteLine(prefix + i);
            }


            Assert.That(eventCallValue, Is.EqualTo(prefix + (number - 1)));
        }
    }
}
