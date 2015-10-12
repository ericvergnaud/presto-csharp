using NUnit.Framework;
using prompto.parser;

namespace prompto.translate.oso
{

	[TestFixture]
	public class TestSelf : BaseOParserTest
	{

		[Test]
		public void testSelfAsParameter()
		{
			compareResourceOSO("self/selfAsParameter.poc");
		}

		[Test]
		public void testSelfMember()
		{
			compareResourceOSO("self/selfMember.poc");
		}

	}
}

