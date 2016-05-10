using NUnit.Framework;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Test.Unit.DomainModel
{
    [TestFixture]
    public class DataSetTests
    {
        [Test]
        public void ToString_SetName_EqualsName()
        {
            DataSet set = new DataSet() {Name = "TestSet"};

            Assert.That(set.ToString(), Is.EqualTo("TestSet"));
        }

        [TestCase(5)]
        [TestCase(10)]
        public void Id_SetValue_CorrectId(int id)
        {
            DataSet set = new DataSet() {Id = id};
            Assert.That(set.Id, Is.EqualTo(id));
        }
    }
}
