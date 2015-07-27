using NUnit.Framework;
using Theraot.Collections;
using Theraot.Facts;

namespace Tests
{
    [TestFixture]
    public class SimpleTests
    {
        [Test]
        public void QueryAll()
        {
            var friend = new Fact<string, string>();
            friend.Add("peter", "paul");
            foreach (var tuple in Fact.Query(friend.Item1)) // We need to fix this syntax
            {
                Assert.AreEqual(tuple.Item1, "peter");
                Assert.AreEqual(tuple.Item2, "paul");
            }
        }

        [Test]
        public void QueryFact()
        {
            var friend = new Fact<string, string>();
            Assert.IsTrue(Fact.Query(friend.Item2 == "paul").IsEmpty());
            Assert.IsTrue(Fact.Query(friend.Item1 == "peter").IsEmpty());
            friend.Add("peter", "paul");
            foreach (var tuple in Fact.Query(friend.Item2 == "paul"))
            {
                Assert.AreEqual(tuple.Item1, "peter");
            }
        }

        [Test]
        public void NegativeQueryFact()
        {
            var friend = new Fact<string, string>();
            Assert.IsTrue(Fact.Query(friend.Item2 == "paul").IsEmpty());
            Assert.IsTrue(Fact.Query(friend.Item1 == "peter").IsEmpty());
            friend.Add("peter", "paul");
            Assert.IsTrue(Fact.Query(friend.Item2 != "paul").IsEmpty());
            Assert.IsTrue(Fact.Query(friend.Item1 != "peter").IsEmpty());
            friend.Add("yesenia", "jackson");
            Assert.IsFalse(Fact.Query(friend.Item2 != "paul").IsEmpty());
            Assert.IsFalse(Fact.Query(friend.Item1 != "peter").IsEmpty());
        }

        [Test]
        public void SimpleFactUse()
        {
            var friend = new Fact<string, string>();
            friend.Add("peter", "paul");
            foreach (var tuple in Fact.Query(friend.Item2 == "paul"))
            {
                Assert.AreEqual(tuple.Item1, "peter");
            }
            Fact.Remove(friend.Item2 == "paul");
            Assert.IsTrue(Fact.Query(friend.Item2 == "paul").IsEmpty());
            Assert.IsTrue(Fact.Query(friend.Item1 == "peter").IsEmpty());
            friend.Add("peter", "paul");
            foreach (var tuple in Fact.Query(friend.Item1 == "peter"))
            {
                Assert.AreEqual(tuple.Item2, "paul");
            }
            Fact.Remove(friend.Item1 == "peter");
            Assert.IsTrue(Fact.Query(friend.Item2 == "paul").IsEmpty());
            Assert.IsTrue(Fact.Query(friend.Item1 == "peter").IsEmpty());
        }
    }
}