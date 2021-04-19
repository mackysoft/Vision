using NUnit.Framework;

namespace MackySoft.Vision.Editor.Tests {
	public class CullingGroupKeyTest {

		[Test]
		public void EqualsAndEqualityOperatorMustReturnsSameResult () {
			var a = new CullingGroupKey(1);
			var b = new CullingGroupKey(1);
			Assert.AreEqual(a.Equals(b),a == b,"Equals and equality operator must return the same result.");
			Assert.AreEqual(!a.Equals(b),a != b,"!Equals and not equality operator must return the same result.");
		}

		[Test]
		public void NoneMustBeNegativeOne () {
			Assert.AreEqual((int)CullingGroupKey.None,-1,$"{nameof(CullingGroupKey)}.{nameof(CullingGroupKey.None)} must be equivalent to -1.");
		}

	}
}