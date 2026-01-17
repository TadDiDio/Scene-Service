## [1.6.1] - 2026-01-16
### Unload order bug fix
-  Unload active scene first when passing to external control.

## [1.6.0] - 2026-01-10
### Support external control
- You can pass control of scene flow to external systems. Useful in multiplayer when a server needs to control the scene flow for all clients directly.

## [1.5.0] - 2026-01-04
### Added tags to scene groups
- You can now add tags to scene groups to mark them with metadata.

## [1.4.0] - 2026-01-03
### Added support for MPPM
- Modded 3rd party scene reference lib to enable MPPM to work.

## [1.3.0] - 2026-01-03
### Fixed editor scene load bugs
- Fixed bug where editor scene group was incorrectly assigned and bootstrap scene not being picked up without window open.

## [1.2.0] - 2025-12-21
### Fixed awake artifacts when using bootstrapper
- Fixed bug where any loaded editor scene would call awake on some or all objects before the bootstrapper was loaded.

## [1.1.1] - 2025-12-20
### Fixed double boot bug
- Fixed bug where bootstrapper awaked twice if it was already loaded in editor.

## [1.1.0] - 2025-12-18
### Scene Events
- Added scene events to controller for load start, progressed, and completed.

## [1.0.0] - 2025-11-25
### First Release
- A generic scene transitioner that unifies the scene loading API across multiple providers like Unity and a networking library.