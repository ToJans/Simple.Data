using System;
using NUnit.Framework;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.UnitTest
{
    public class ForeignKeyCollectionTests
    {
        [Test]
        public void CanAddSingleKey()
        {
            var collection = new ForeignKeyCollection();
            var key = new ForeignKey(new ObjectName("", "Table1"), new[] {"Field1"}, new ObjectName("", "Table2"),
                                     new[] {"Field2"});
            collection.Add(key);

            Assert.That(collection, Contains.Item(key));
        }

        [Test]
        public void CanAddMultipleForeignKeys()
        {
            var collection = new ForeignKeyCollection();
            var key1 = new ForeignKey(new ObjectName("", "DetailTable1"), new[] { "Field1" }, new ObjectName("", "MasterTable1"),new[] { "Field1" });
            var key2 = new ForeignKey(new ObjectName("", "DetailTable1"), new[] { "Field2" }, new ObjectName("", "MasterTable2"),new[] { "Field2" });
            
            collection.Add(key1);
            collection.Add(key2);

            Assert.That(collection,Contains.Item(key1));
            Assert.That(collection,Contains.Item(key2));

            
        }
        [Test]
        public void CanNotAddMultipleForeignKeysToTheSameTable()
        {
            var collection = new ForeignKeyCollection();
            var key1 = new ForeignKey(new ObjectName("", "DetailTable1"), new[] { "Field1" }, new ObjectName("", "MasterTable1"),new[] { "Field1" });
            var key2 = new ForeignKey(new ObjectName("", "DetailTable1"), new[] { "Field2" }, new ObjectName("", "MasterTable1"),new[] { "Field2" });
            
            collection.Add(key1);
            Assert.Throws<ArgumentException>(() => collection.Add(key2));
            
        }
    }
}