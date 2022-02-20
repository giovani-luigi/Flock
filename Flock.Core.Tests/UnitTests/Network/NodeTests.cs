using Flock.Core.Identification;
using Flock.Core.Network;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Network {

    [TestFixture]
    public class NodeTests {

        [Test]
        public void Test_Node_Does_Not_Allow_Default_UniqueID() {
            
            Assert.That(new Node(UniqueIdFactory.Default), Throws.ArgumentException);

        }
    }
}
