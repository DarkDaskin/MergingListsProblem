namespace MergingListsProblem;

public class ListMerger
{
	public static List<VersionedChunk<T>> Merge<T>(Dictionary<int, List<T>> input) where T : IEquatable<T>
	{
		throw new NotImplementedException();
	}
}

public record VersionRange(int? MinVersion, int? MaxVersion);

public record VersionedChunk<T>(List<T> Data, List<VersionRange> Versions) where T : IEquatable<T>;