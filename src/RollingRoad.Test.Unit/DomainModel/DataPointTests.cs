using NUnit.Framework;
using RollingRoad.Core.DomainModel;

namespace RollingRoad.Test.Unit.DomainModel
{
    [TestFixture]
    public class DatapointTests
    {
        [Test]
        public void Ctor_ValueSet_CorrectValue()
        {
            DataPoint entry = new DataPoint(0);
            Assert.That(entry.Value, Is.EqualTo(0));
        }


        [TestCase(5)]
        [TestCase(10)]
        public void Id_SetValue_CorrectId(int id)
        {
            DataPoint entry = new DataPoint(0) {Id = id};
            Assert.That(entry.Id, Is.EqualTo(id));
        }

        [Test]
        public void Ctor_Default_IndexNegative1()
        {
            Assert.That(new DataPoint().Index, Is.EqualTo(-1));
        }

        [Test]
        public void Ctor_Default_ValueZero()
        {
            Assert.That(new DataPoint().Value, Is.Zero);
        }

        [Test]
        public void DataList_SetAndGet_CorrectValue()
        {
            DataList list = new DataList();
            DataPoint point = new DataPoint(0) {DataList = list};

            Assert.That(point.DataList, Is.EqualTo(list));
        }
    }
}
