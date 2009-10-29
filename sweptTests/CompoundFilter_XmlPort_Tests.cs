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
@"    <When ID='parent' Description='outer'>
        <When ID='child1'>
        </When>
        <Or ID='child2'>
        </Or>
    </When>
";
            Assert.AreEqual( expectedFilter, serializedFilter );
        }

        [Test]
        public void use_persisters_instead_of_direct_db_access()
        {
            CompoundFilter use_persisters = new CompoundFilter
            {
                ID = "Pers_1",
                Description = "Remove all direct DB access, use persisters instead"
            };

            use_persisters.Children.Add( new CompoundFilter { ContentPattern = "(XADR|Oracle|NHibernate)" } );
            use_persisters.Children.Add( new CompoundFilter { NamePattern = "(Persister|Service)", Operator = FilterOperator.NotAnd } );
            use_persisters.Children.Add( new CompoundFilter { Subpath = "(XADR|Hibernate)", Operator = FilterOperator.NotAnd } );

            CompoundFilter special_cases = new CompoundFilter { Operator = FilterOperator.Or, ID = "Special cases" };
            special_cases.Children.Add( new CompoundFilter { NamePattern = "this.cs" } );
            special_cases.Children.Add( new CompoundFilter { NamePattern = "that.cs", Operator = FilterOperator.Or } );
            special_cases.Children.Add( new CompoundFilter { NamePattern = "another.cs", Operator = FilterOperator.Or } );

            use_persisters.Children.Add( special_cases );

            string expected_text =
@"    <Change ID='Pers_1' Description='Remove all direct DB access, use persisters instead'>
        <When ContentPattern='(XADR|Oracle|NHibernate)' />
        <AndNot NamePattern='(Persister|Service)' />
        <AndNot FilePath='(XADR|Hibernate)' />
        <Or ID='Special cases'>
            <When NamePattern='this.cs' />
            <Or NamePattern='that.cs' />
            <Or NamePattern='another.cs' />
        </Or>
    </Change>";

            string current_text =
@"    <And ID='Pers_1' Description='Remove all direct DB access, use persisters instead'>
        <And ContentPattern='(XADR|Oracle|NHibernate)' />
        <AndNot NamePattern='(Persister|Service)' />
        <AndNot FilePath='(XADR|Hibernate)' />
        <Or ID='Special cases'>
            <And NamePattern='this.cs' />
            <Or NamePattern='that.cs' />
            <Or NamePattern='another.cs' />
        </Or>
    </And>";

            string serializedFilter = port.ToText( use_persisters );
            Assert.AreEqual( current_text, serializedFilter );
        }


    }
}

/*

*/
















