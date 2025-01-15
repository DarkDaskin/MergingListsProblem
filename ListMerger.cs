namespace MergingListsProblem;

public class ListMerger
{
	public static List<VersionedChunk<T>> Merge<T>(Dictionary<int, List<T>> input) where T : class, IEquatable<T>
	{
		var output = new List<VersionedChunk<T>>();
		var versions = input.Keys.Order().ToArray();
		if (versions.Length == 0)
			return output;
		var comparer = EqualityComparer<T>.Default;
		T? current = null;
		VersionedChunk<T>? currentChunk = null;
		while (input.Values.Any(list => list.Count > 0))
		{
			var versionsWhereItemIsMissing = new List<int>();
			foreach (var version in versions)
			{
				var list = input[version];

				if (list.Count == 0)
				{
					versionsWhereItemIsMissing.Add(version);
					continue;
				}

				if (current == null || comparer.Equals(current, list[0]))
				{
					current ??= list[0];
					list.RemoveAt(0);
				}
				else
				{
					versionsWhereItemIsMissing.Add(version);
				}
			}

			if (current == null) continue;

			var ranges = BuildVersionRanges(versionsWhereItemIsMissing, versions);

			if (currentChunk == null || !currentChunk.Versions.SequenceEqual(ranges))
			{
				if (currentChunk != null)
					output.Add(currentChunk);

				currentChunk = new VersionedChunk<T>([], ranges);
			}

			currentChunk.Data.Add(current);

			current = null;
		}

		if (currentChunk != null)
			output.Add(currentChunk);

		return output;
	}

	private static List<VersionRange> BuildVersionRanges(IReadOnlyCollection<int> versionsWhereItemIsMissing, IReadOnlyCollection<int> versions)
	{
		var ranges = new List<VersionRange>();
		if (versionsWhereItemIsMissing.Count == 0)
			return ranges;
		var minVersion = versions.Min();
		var maxVersion = versions.Max();
		var versionsWhereItemIsPresent = versions.Except(versionsWhereItemIsMissing).ToArray();
		var last = versionsWhereItemIsPresent.First();
		var first = last;
		foreach (var version in versionsWhereItemIsPresent.Skip(1))
		{
			if (last + 1 != version)
			{
				ranges.Add(new VersionRange(first == minVersion ? null : first, last == maxVersion ? null : last));
				first = version;
			}
			last = version;
		}
		if (first != minVersion || last != maxVersion)
			ranges.Add(new VersionRange(first == minVersion ? null : first, last == maxVersion ? null : last));
		return ranges;
	}
}

public record VersionRange(int? MinVersion, int? MaxVersion);

public record VersionedChunk<T>(List<T> Data, List<VersionRange> Versions) where T : IEquatable<T>;