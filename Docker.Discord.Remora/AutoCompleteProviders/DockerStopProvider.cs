using Docker.DotNet;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace Docker.Discord.Remora.AutoCompleteProviders;

public class DockerStopProvider : IAutocompleteProvider
{
    public string Identity => "docker::stop";
    
    private readonly IDockerClient _client;
    
    public DockerStopProvider(IDockerClient client) => _client = client;

    public async ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
        GetSuggestionsAsync(IReadOnlyList<IApplicationCommandInteractionDataOption> options, string userInput, CancellationToken ct = default)
    {
        var containers = await _client.Containers.ListContainersAsync(new(), ct);
        
        return containers.Where(c => c.Status.Contains("Up")).Select(c => new ApplicationCommandOptionChoice($"{c.ID[..12]} - {c.Image}", c.ID)).ToArray();
    }
   
}