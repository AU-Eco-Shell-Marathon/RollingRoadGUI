using System.IO;
using System.Text;
using NUnit.Framework;
using RollingRoad.Infrastructure.DataAccess;

namespace RollingRoad.Test.Unit.DataAccess
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class SP4MCInterpreterTests
    {
        private MemoryStream _ms;
        private SP4MCInterpreter _interpreter;

        [SetUp]
        public void SetUp()
        {
            _ms = new MemoryStream();
            _interpreter = new SP4MCInterpreter(_ms);
        }

        [TearDown]
        public void TearDown()
        {
            _ms.Dispose();
        }


        [TestCase(1)]
        [TestCase(123)]
        [TestCase(34)]
        public void SetCruiseSpeed_PositiveValue_ValueSent(int value)
        {
            _interpreter.CruiseSpeed = value;

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Does.Contain(value.ToString()));
        }

        [TestCase(1)]
        [TestCase(123)]
        [TestCase(34)]
        public void SetMaxSpeed_PositiveValue_ValueSent(int value)
        {
            _interpreter.MaxSpeed = value;

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Does.Contain(value.ToString()));
        }

        [Test]
        public void SetCruiseSpeed_PositiveValue_CorrectId()
        {
            _interpreter.CruiseSpeed = 1;

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Does.StartWith("2"));
        }

        [Test]
        public void SetMaxSpeed_PositiveValue_CorrectId()
        {
            _interpreter.MaxSpeed = 1;

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Does.StartWith("1"));
        }
    }
}
