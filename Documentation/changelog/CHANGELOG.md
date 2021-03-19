# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2021-03-20
### Added
- Added the example scenes.

### Changed
- When the `CullingGroupProxy` is selected in hierarchy during playmode, the `TargetCamera` frustum gizmo and the culling target handle's are now also displayed.
- `CullingTargetRenderers` now perform a null check on `Rendrerers`.

## [1.0.2] - 2021-03-16
### Added
- Added the `LinkOnEnableOrStart` property and Link method to `CullingGroupTargetSetter`.

### Removed
- Removed the constructor of `CullingGroupKeyDefinition`.

### Changed
- Renamed `CameraSourceMode` to `CameraReferenceMode`.
- Updated some documents.

## [1.0.1] - 2021-03-15
### Added
- Added the `VisionSettingsInitializer`.
    - `VisionSettings` will now be generated automatically.

## [1.0.0] - 2021-03-14
First release