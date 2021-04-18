using NUnit.Framework;

namespace MackySoft.Vision.Editor.Tests {
	public class CullingGroupKeyTests {

		[Test]
		public void NoneMustBeNegativeOne () {
			Assert.AreEqual((int)CullingGroupKey.None,-1,$"{nameof(CullingGroupKey)}.{nameof(CullingGroupKey.None)} must be equivalent to -1.");
		}

	}
}