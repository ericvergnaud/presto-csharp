// generated: 2015-10-05T01:03:20.465
using NUnit.Framework;
using prompto.parser;
using prompto.utils;
using prompto.runtime;

namespace prompto.library.e
{

	[TestFixture]
	public class TestCore : BaseEParserTest
	{

		[SetUp]
		public void before()
		{
			Loader.Load ();
			Out.init();
			coreContext = null;
			LoadDependency("core");
		}

		[TearDown]
		public void after()
		{
			Out.restore();
		}

		[Test]
		public void testAny()
		{
			CheckTests("core/any.pec");
		}

		[Test]
		public void testAttribute()
		{
			CheckTests("core/attribute.pec");
		}

		[Test]
		public void testError()
		{
			CheckTests("core/error.pec");
		}

	}
}

