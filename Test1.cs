namespace MergingListsProblem
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void WhenNoVersions_ReturnEmptyList()
        {
            Dictionary<int, List<string>> input = [];

            var output = ListMerger.Merge(input);

            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void WhenSingleVersion_ReturnSingleNonVersionedChunk()
        {
            Dictionary<int, List<string>> input = new() { [1] = ["A", "B", "C"] };

            var output = ListMerger.Merge(input);

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(3, output[0].Data.Count);
            Assert.IsTrue(output[0].Data.SequenceEqual(["A", "B", "C"]));
            Assert.AreEqual(0, output[0].Versions.Count);
        }

        [TestMethod]
        public void WhenAllVersionsAreSame_ReturnSingleNonVersionedChunk()
        {
            Dictionary<int, List<string>> input = new() 
            { 
                [1] = ["A", "B", "C"],
                [2] = ["A", "B", "C"],
            };

            var output = ListMerger.Merge(input);

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(3, output[0].Data.Count);
            Assert.IsTrue(output[0].Data.SequenceEqual(["A", "B", "C"]));
            Assert.AreEqual(0, output[0].Versions.Count);
        }

        [TestMethod]
        public void WhenElementIsAdded_ProduceVersionedChunk()
        {
            Dictionary<int, List<string>> input = new() 
            { 
                [1] = ["A", "B", "C"],
                [2] = ["A", "B", "B2", "C"],
            };

            var output = ListMerger.Merge(input);

            Assert.AreEqual(3, output.Count);

            Assert.AreEqual(2, output[0].Data.Count);
            Assert.IsTrue(output[0].Data.SequenceEqual(["A", "B"]));
            Assert.AreEqual(0, output[0].Versions.Count);

            Assert.AreEqual(1, output[1].Data.Count);
            Assert.IsTrue(output[1].Data.SequenceEqual(["B2"]));
            Assert.AreEqual(1, output[1].Versions.Count);
            Assert.AreEqual(2, output[1].Versions[0].MinVersion);
            Assert.IsNull(output[1].Versions[0].MaxVersion);

            Assert.AreEqual(1, output[2].Data.Count);
            Assert.IsTrue(output[2].Data.SequenceEqual(["C"]));
            Assert.AreEqual(0, output[2].Versions.Count);
        }

        [TestMethod]
        public void WhenElementIsRemoved_ProduceVersionedChunk()
        {
            Dictionary<int, List<string>> input = new() 
            { 
                [1] = ["A", "B", "C"],
                [2] = ["A", "C"],
            };

            var output = ListMerger.Merge(input);

            Assert.AreEqual(3, output.Count);

            Assert.AreEqual(1, output[0].Data.Count);
            Assert.IsTrue(output[0].Data.SequenceEqual(["A"]));
            Assert.AreEqual(0, output[0].Versions.Count);

            Assert.AreEqual(1, output[1].Data.Count);
            Assert.IsTrue(output[1].Data.SequenceEqual(["B"]));
            Assert.AreEqual(1, output[1].Versions.Count);
            Assert.IsNull(output[1].Versions[0].MinVersion);
            Assert.AreEqual(1, output[1].Versions[0].MaxVersion);

            Assert.AreEqual(1, output[2].Data.Count);
            Assert.IsTrue(output[2].Data.SequenceEqual(["C"]));
            Assert.AreEqual(0, output[2].Versions.Count);
        }

        [TestMethod]
        public void WhenElementIsRemovedAndAddedAgain_ProduceVersionedChunk()
        {
            Dictionary<int, List<string>> input = new() 
            { 
                [1] = ["A", "B", "C"],
                [2] = ["A", "C"],
                [3] = ["A", "B", "C"],
            };

            var output = ListMerger.Merge(input);

            Assert.AreEqual(3, output.Count);

            Assert.AreEqual(1, output[0].Data.Count);
            Assert.IsTrue(output[0].Data.SequenceEqual(["A"]));
            Assert.AreEqual(0, output[0].Versions.Count);

            Assert.AreEqual(1, output[1].Data.Count);
            Assert.IsTrue(output[1].Data.SequenceEqual(["B"]));
            Assert.AreEqual(2, output[1].Versions.Count);
            Assert.IsNull(output[1].Versions[0].MinVersion);
            Assert.AreEqual(1, output[1].Versions[0].MaxVersion);
            Assert.AreEqual(3, output[1].Versions[1].MinVersion);
            Assert.IsNull(output[1].Versions[1].MaxVersion);

            Assert.AreEqual(1, output[2].Data.Count);
            Assert.IsTrue(output[2].Data.SequenceEqual(["C"]));
            Assert.AreEqual(0, output[2].Versions.Count);
        }

        [TestMethod]
        public void WhenElementIsAddedAndRemovedAgain_ProduceVersionedChunk()
        {
            Dictionary<int, List<string>> input = new() 
            { 
                [1] = ["A", "B"],
                [2] = ["A", "B", "C"],
                [3] = ["A", "B"],
            };

            var output = ListMerger.Merge(input);

            Assert.AreEqual(2, output.Count);

            Assert.AreEqual(2, output[0].Data.Count);
            Assert.IsTrue(output[0].Data.SequenceEqual(["A", "B"]));
            Assert.AreEqual(0, output[0].Versions.Count);

            Assert.AreEqual(1, output[1].Data.Count);
            Assert.IsTrue(output[1].Data.SequenceEqual(["C"]));
            Assert.AreEqual(1, output[1].Versions.Count);
            Assert.AreEqual(2, output[1].Versions[0].MinVersion);
            Assert.AreEqual(2, output[1].Versions[0].MaxVersion);
        }

        [TestMethod]
        public void WhenElementsAreAddedAndRemoved_ProduceVersionedChunks()
        {
            Dictionary<int, List<string>> input = new() 
            { 
                [1] = ["A", "B", "C", "D", "E"],
                [2] = ["A", "B", "B2", "B3", "C", "D", "E"],
                [3] = ["A", "B", "B2", "B3", "D", "E"],
                [11] = ["A", "B", "B2", "B3", "C", "D", "E"],
            };

            var output = ListMerger.Merge(input);

            Assert.AreEqual(4, output.Count);

            Assert.AreEqual(2, output[0].Data.Count);
            Assert.IsTrue(output[0].Data.SequenceEqual(["A", "B"]));
            Assert.AreEqual(0, output[0].Versions.Count);

            Assert.AreEqual(2, output[1].Data.Count);
            Assert.IsTrue(output[1].Data.SequenceEqual(["B2", "B3"]));
            Assert.AreEqual(1, output[1].Versions.Count);
            Assert.AreEqual(2, output[1].Versions[0].MinVersion);
            Assert.IsNull(output[1].Versions[0].MaxVersion);

            Assert.AreEqual(1, output[2].Data.Count);
            Assert.IsTrue(output[2].Data.SequenceEqual(["C"]));
            Assert.AreEqual(2, output[2].Versions.Count);
            Assert.IsNull(output[2].Versions[0].MinVersion);
            Assert.AreEqual(2, output[2].Versions[0].MaxVersion);
            Assert.AreEqual(11, output[2].Versions[1].MinVersion);
            Assert.IsNull(output[2].Versions[1].MaxVersion);

            Assert.AreEqual(2, output[3].Data.Count);
            Assert.IsTrue(output[3].Data.SequenceEqual(["D", "E"]));
            Assert.AreEqual(0, output[3].Versions.Count);
        }
    }
}
