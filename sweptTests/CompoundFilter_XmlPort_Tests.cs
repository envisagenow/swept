using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class CompoundFilter_XmlPort_Tests
    {
        private XmlPort port;

        [SetUp]
        public void CanCreate()
        {
            port = new XmlPort();
        }

        [Test]
        public void CompoundFilter_ToXmlText_attributes()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "thing",
                Description = "for my test"
            };

            string serializedFilter = port.ToText( filter );
            string expectedFilter =
@"    <Filter ID='thing' Description='for my test'>
    </Filter>
";
            Assert.AreEqual( expectedFilter, serializedFilter );
        }

        [Test]
        public void CompoundFilter_ToXmlText_children()
        {
            CompoundFilter filter = new CompoundFilter{
                ID = "parent",
                Description = "outer"
            };
            filter.Children.Add( new CompoundFilter { ID = "child1" } );
            filter.Children.Add( new CompoundFilter { ID = "child2", Operator = FilterOperator.Or } );

            string serializedFilter = port.ToText( filter );
            string expectedFilter =
@"    <Filter ID='parent' Description='outer'>
        <Filter ID='child1'>
        </Filter>
        <Or ID='child2'>
        </Or>
    </Filter>
";
            Assert.AreEqual( expectedFilter, serializedFilter );
        }

    }
}
