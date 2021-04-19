# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.3] - 2021-04-20
### Added
- Added an unit tests.

### Changed
- Every time there is a change in the repository, unit tests and build tests on each platform are now performed.
    - This ensures a certain level of package quality.

## [1.1.2] - 2021-04-18
### Fixed
- Fixed `FilePathAttribute` is inaccessible due to its protection level, which was occurring in Unity 2019.

## [1.1.1] - 2021-03-29
### Added
- Added the `ColorSwitcher`.

### Removed
- Removed the `MaterialSwitcher`.
- Removed the material's from example assets.
- Removed the `Resources/VisionSettings`.

### Changed
- The automatic generation of `VisionSettings` can now be avoided by defining the `VISION_DISABLE_GENERATE_SETTINGS` definition symbol.
    - This is a measure to avoid including auto-generated VisionSettings in the package. They will not be used by the user.

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