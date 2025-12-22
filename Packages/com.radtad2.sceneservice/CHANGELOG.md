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