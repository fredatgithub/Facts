using NUnit.Framework;
using Theraot.Collections;
using Theraot.Facts;
using Theraot.Threading.Needles;

namespace Tests
{
    [TestFixture]
    public class SimpleTests
    {
        [Test]
        public void SimpleFactUse()
        {
            var friend = new Fact<string, string>();
            friend.Add("peter", "paul");
            foreach (var tuple in friend.Read(arg2: "paul"))
            {
                Assert.AreEqual(tuple.Item1, "peter");
            }
            friend.Remove(null as Needle<string>, "paul");
            Assert.IsTrue(friend.Read(arg2: "paul").IsEmpty());
            Assert.IsTrue(friend.Read("peter").IsEmpty());
            friend.Add("peter", "paul");
            foreach (var tuple in friend.Read("peter"))
            {
                Assert.AreEqual(tuple.Item2, "paul");
            }
            friend.Remove("peter");
            Assert.IsTrue(friend.Read(arg2: "paul").IsEmpty());
            Assert.IsTrue(friend.Read("peter").IsEmpty());
        }
    }
}