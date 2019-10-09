<p align="center">
	<img src="https://i.imgur.com/4JGmPas.png" />
	</br>
	<a href="https://discord.gg/ZJaVXK8">
		<img src="https://img.shields.io/badge/Discord-Support-%237289DA.svg?logo=discord&style=for-the-badge&logoWidth=20&labelColor=0d0d0d" />
	</a>
  <a href="http://buymeacoff.ee/Yucked">
		<img src="https://img.shields.io/badge/Buy%20Me%20A-Coffee-%23FF813F.svg?logo=buy-me-a-coffee&style=for-the-badge&logoWidth=20&labelColor=0d0d0d" />
	</a>  
	<p align="center">
	     🎶 - The best Discord's voice wrapper out there. Outperforms Discord.NET, DSharpPlus and any other .NET Discord library. Aiming to be plug and play.
  </p>
</p>

---

## `⚗️ USAGE:`

~~Even I don't know how to use this thing.~~ The current usage is something like so:
```cs
var vcClient = new VoiceGatewayClient(new ConnectionPacket {
    Endpoint = VOICE_SERVER_ENDPOINT,
    UserId = BOT_USER_ID,
    GuildId = VOICE_CHANNEL_GUILD_ID,
    Token = VOICE_CONNECTION_TOKEN,
    SessionId = VOICE_SESSION_ID
});

vcClient.OnLog += logMessage => {
    Console.WriteLine(logMessage);
    return Task.CompletedTask;
};

// FOR SENDING AUDIO

await vcClient.SendAudioAsync(someAudioStreamObject)
  .ConfigureAwait(false);

// FOR RECEIVING AUDIO
// ???????
```
