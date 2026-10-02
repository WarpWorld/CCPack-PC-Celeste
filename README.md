# Celeste

Crowd Control PC effect-pack definition for the game.

## Connector and layout

- `Celeste.cs` registers the effects through `SimpleTCPPack<SimpleTCPServerConnector>`.
- `PC-Celeste/` contains the mod project and `everest.yaml` manifest.
- `Celeste.cs.old` is a retained older definition.

## Development

Use the project in `PC-Celeste/` for game-side changes and update `Celeste.cs`; keep the legacy file separate from the active definition.
