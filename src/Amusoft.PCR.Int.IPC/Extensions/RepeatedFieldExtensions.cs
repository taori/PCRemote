using Google.Protobuf.Collections;

namespace Amusoft.PCR.Int.IPC.Extensions;

public static class RepeatedFieldExtensions
{
	public static RepeatedField<T> ToRepeatedField<T>(this IEnumerable<T> source)
	{
		var result = new RepeatedField<T>();
		result.AddRange(source);
		return result;
	}
}