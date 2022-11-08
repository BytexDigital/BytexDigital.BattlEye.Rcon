![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BytexDigital.BattlEye.Rcon?style=for-the-badge)

# BytexDigital.BattlEye.Rcon

This library provides an easy way of communicating with a BattlEye RCON server.

## Installation

[nuget package](https://www.nuget.org/packages/BytexDigital.BattlEye.Rcon/)

```
Install-Package BytexDigital.BattlEye.Rcon
```

## Usage

#### Creating an RconClient

```cs
RconClient networkClient = new RconClient("127.0.0.1", 2310, "testPassword");
```

#### Connecting

Notice: By default, the RconClient will attempt reconnecting after a initial `Connect()` call or when the client is
disconnected.
You can adjust this behaviour by setting `ReconnectInterval` and `ReconnectOnFailure`.

```cs
// Connect
bool initialConnectSuccessful = networkClient.Connect();

// Optionally make sure we are connected
networkClient.WaitUntilConnected();
```

#### Sending a request that does not expect a response

```cs
networkClient.Send(new SendMessageCommand("This is a global message"));
```

#### Sending a request that returns something

```cs
bool requestSuccess = networkClient.Fetch(
	command: new GetPlayersRequest(),
	timeout: 5000,
	result: out List<Player> onlinePlayers);
```

#### Example: Request players and send everyone with an odd ID a personal message

```cs
bool requestSuccess = networkClient.Fetch(
	command: new GetPlayersRequest(),
	timeout: 5000,
	result: out List<Player> onlinePlayers);

foreach (var player in onlinePlayers.Where(x => x.Id % 2 != 0)) {
	networkClient.Send(new SendMessageCommand(player.Id, "You're a special person"));
}
```

#### Events

A `RconClient` offers the ability to hook up eventhandlers for specific events. The currently implemented events are
when a player connects (`PlayerConnected`), disconnects (`PlayerDisconnected`), when a message not bound to a request is
received ( for example a chat message) (`MessageReceived`) and when a player is removed due to a kick or
ban (`PlayerRemoved`).

# License

Copyright 2019 Bytex Digital UG (haftungsbeschränkt)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
