using System.Net;
using System.Text.Json;
using Amusoft.PCR.AM.UI.Converters;
using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.UnitTests;

[UsesVerify]
public class SettingsSerializableTests
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true, Converters = { new SettingsEndpointDictionaryConverter() } };

	[Fact]
	public async Task VerifySerialize_EndpointAccountIdByEndpoint()
	{
		var settings = new Settings();
		settings.EndpointAccountIdByEndpoint[IPEndPoint.Parse("1.1.1.1:80")] = Guid.Parse("60A871A6-6EF6-4570-BBD6-472277438232");
		settings.EndpointAccountIdByEndpoint[IPEndPoint.Parse("1.1.1.1:81")] = Guid.Parse("BD89D839-F585-4E1F-AF9D-2BE2BC54BD86");

		var serialized = JsonSerializer.Serialize(settings, JsonSerializerOptions);
		await Verify(serialized).DontScrubGuids();
	}

	[Fact]
	public async Task VerifyDeserialize_EndpointAccountIdByEndpoint()
	{
		await using var reader = File.OpenRead("Snapshots/SettingsSerializableTests.VerifySerialize_EndpointAccountIdByEndpoint.verified.txt");
		var deserialized = await JsonSerializer.DeserializeAsync<Settings>(reader, JsonSerializerOptions);

		await Verify(deserialized).DontScrubGuids();
	}
}