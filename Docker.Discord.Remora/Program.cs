using System.Reflection;
using Docker.Discord.Remora;
using Docker.Discord.Remora.AutoCompleteProviders;
using Docker.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remora.Commands.Extensions;
using Remora.Discord.API;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;

var host = Host.CreateDefaultBuilder().UseConsoleLifetime();

host.ConfigureHostConfiguration(c => c.AddUserSecrets(Assembly.GetEntryAssembly()));

host
    .AddDiscordService(s => s.GetService<IConfiguration>().GetValue<string>("token"));

host.ConfigureServices((c, services) =>
{
    services.AddDiscordCommands(true);

    services.AddCommandTree()
        .WithCommandGroup<DockerCommands>()
        .WithCommandGroup<DockerComposeCommands>();

    services
        .AddAutocompleteProvider<DockerStartProvider>()
        .AddAutocompleteProvider<DockerStopProvider>()
        .AddAutocompleteProvider<DockerRemoveProvider>()
        ;

    services.AddSingleton<IDockerClient>(_ =>
    {
        var dockerEndpoint = OperatingSystem.IsWindows() ? "npipe://./pipe/docker_engine" : "unix:/var/run/docker.sock";

        return new DockerClientConfiguration(new Uri(dockerEndpoint)).CreateClient();
    });
});

var builder = host.Build();

var slash = builder.Services.GetRequiredService<SlashService>();

await slash.UpdateSlashCommandsAsync(DiscordSnowflake.New(721518523704410202));

await builder.RunAsync();