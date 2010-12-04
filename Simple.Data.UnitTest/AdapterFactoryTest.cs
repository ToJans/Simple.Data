using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Simple.Data.UnitTest
{
    [TestFixture]
    public class AdapterFactoryTest
    {
        [Test]
        public void TestObjectConstructor()
        {
            var mefHelper = new Mock<IMefHelper>();
            new AdapterFactory(mefHelper.Object).Create(new {ConnectionString = "foo"});
            mefHelper.Verify(mh => mh.Compose<Adapter>("Ado"));
        }
    }
}
