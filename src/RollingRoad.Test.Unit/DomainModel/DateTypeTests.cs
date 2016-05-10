using NUnit.Framework;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Test.Unit.DomainModel
{
    [TestFixture]
    public class DateTypeTests
    {
        [TestCase("Test1")]
        [TestCase("Test2")]
        public void Name_SetName_CorrectValue(string name)
        {
            DataType type = new DataType(){Name = name };
            Assert.That(type.Name, Is.EqualTo(name));
        }

        [TestCase("Test1")]
        [TestCase("Test2")]
        public void Unit_SetUnit_CorrectValue(string unit)
        {
            DataType type = new DataType() { Unit = unit };
            Assert.That(type.Unit, Is.EqualTo(unit));
        }

        [Test]
        public void Ctor_UnitAndName_NameCorrect()
        {
            DataType type = new DataType("Name1", "Unit1");
            Assert.That(type.Name, Is.EqualTo("Name1"));
        }

        [Test]
        public void Ctor_UnitAndName_UnitCorrect()
        {
            DataType type = new DataType("Name1", "Unit1");
            Assert.That(type.Unit, Is.EqualTo("Unit1"));
        }
    }
}
