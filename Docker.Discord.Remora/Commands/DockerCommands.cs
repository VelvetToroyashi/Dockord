using System.ComponentModel;
using System.Drawing;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using Humanizer;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Docker.Discord.Remora;

[Group("docker")]
[Ephemeral(true)]
public class DockerCommands : CommandGroup
{
    public DockerCommands(IDockerClient dockerClient, InteractionContext context, IDiscordRestInteractionAPI interactions)
    {
        _docker = dockerClient;
        _context = context;
        _interactions = interactions;
    }
    
    private readonly IDockerClient _docker;
    private readonly InteractionContext _context;
    private readonly IDiscordRestInteractionAPI _interactions;
    
    [Command("ps")]
    [Description("List all running containers")]
    public async Task<IResult> ListContainersAsync()
    {
        var containers = _docker.Containers.ListContainersAsync(new() { All = true });
        
        var sb = new StringBuilder();
        sb.AppendLine("```");
        sb.AppendLine("CONTAINER ID\tIMAGE\t\t\t  CREATED\t\t STATUS\t\t  PORTS");
        
        foreach (var container in await containers)
           sb.AppendLine($"{container.ID[..12]}\t{container.Image,-15}" +
                         $"\t{container.Created.Humanize(true, DateTime.UtcNow),-12}\t{container.Status,-12}" +
                         $"\t{string.Join(", ", container.Ports.Select(p => $"{p.IP}:{p.PublicPort}->{p.PrivatePort}"))}");
        
        sb.AppendLine("```");

        return await _interactions.EditOriginalInteractionResponseAsync(_context.ApplicationID, _context.Token, sb.ToString());
    }

    [Command("start")]
    [Description("Start a container")]
    public async Task<IResult> StartContainerAsync
    (
        [AutocompleteProvider("docker::start")]
        [Description("The ID of the coontainer to start")]
        string containerID
    )
    {
        containerID = containerID[..12];
        
        var containerStarted = await _docker.Containers.StartContainerAsync(containerID, new());
        
        var message = containerStarted ? $"{Emojis.ConfirmEmoji} Started container {containerID}" :
                                         $"{Emojis.DeclineEmoji} Failed to start container {containerID}";

        return await _interactions.EditOriginalInteractionResponseAsync(_context.ApplicationID, _context.Token, message);
    }
    
    [Command("stop")]
    [Description("Stop a container")]
    public async Task<IResult> StopContainerAsync
    (
        [AutocompleteProvider("docker::stop")]
        [Description("The ID of the coontainer to stop")]
        string containerID
    )
    {
        containerID = containerID[..12];
        
        var containerStopped = await _docker.Containers.StopContainerAsync(containerID, new());
        
        var message = containerStopped ? $"{Emojis.ConfirmEmoji} Stopped container {containerID}" :
                                         $"{Emojis.DeclineEmoji} Failed to stop container {containerID}";

        return await _interactions.EditOriginalInteractionResponseAsync(_context.ApplicationID, _context.Token, message);
    }
    
    [Command("remove")]
    [Description("Remove a container")]
    public async Task<IResult> RemoveContainerAsync
    (
        [AutocompleteProvider("docker::remove")]
        [Description("The ID of the coontainer to remove")]
        string containerID,
        
        [Description("Remove the volumes associated with the container")]
        bool volumes = false,
        
        [Description("Force the container to stop before removing (SIGKILL)")]
        bool force = false
    )
    {
        containerID = containerID[..12];
        
        await _docker.Containers.RemoveContainerAsync(containerID, new() { Force = force, RemoveVolumes = volumes });
        
        var message = $"{Emojis.ConfirmEmoji} Removed container {containerID}";

        return await _interactions.EditOriginalInteractionResponseAsync(_context.ApplicationID, _context.Token, message);
    }
    
    
}