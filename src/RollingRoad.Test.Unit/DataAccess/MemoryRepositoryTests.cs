using System.Linq;
using NUnit.Framework;
using RollingRoad.Infrastructure.DataAccess;

namespace RollingRoad.Test.Unit.DataAccess
{
    internal class MemoryRepositoryTestClass
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    [TestFixture]
    public class MemoryRepositoryTests
    {

        private MemoryRepository<MemoryRepositoryTestClass> _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new MemoryRepository<MemoryRepositoryTestClass>();
        }

        [Test]
        public void Insert_OneInsert_OneValuePresent()
        {
            _repository.Insert(new MemoryRepositoryTestClass() {Id = 1, Value = "TestValue"});

            Assert.That(_repository.Get().ToList().Count, Is.EqualTo(1));
        }
    }
}
