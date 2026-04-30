# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity-based VFX (Visual Effects) toolset project called "Scorpio VFX Toolset". It's designed to enhance Unity's VFX workflow with additional tools and utilities for managing shaders, materials, and asset references.

## Codebase Structure

The project is organized as a Unity package with the following key components:

### Main Package
- `Packages/com.scorpio.vfxtoolset/` - The core package directory
  - `Editor/` - Editor tools and utilities
    - `ReferenceFinder/` - Asset reference finder functionality
    - `ShaderGUI/` - Shader GUI editors
    - `EffectShaderTemplates/` - Shader template processors
    - `ShaderScannerAndReplacer.cs` - Shader scanning and replacement tool
  - `Runtime/` - Runtime components (currently minimal)
  - `Shaders/` - Shader assets and libraries
    - `Effect/` - Effect-specific shaders
    - `ShaderLibrary/` - Shader libraries and utility files

## Key Features

1. **Reference Finder**: A tool to find references between assets in the Unity project, helping developers understand dependencies between assets.

2. **Shader Scanner & Replacer**: A utility to scan materials and replace legacy shaders with newer versions.

3. **Shader Templates**: Pre-built shader templates for common VFX effects.

4. **Shader GUI Editors**: Custom editors for VFX shaders with enhanced functionality.

## Development Setup

### Building and Testing

This is a Unity package that works within the Unity Editor. To develop or test:

1. Open the project in Unity Editor (version 2019.4 recommended)
2. The package is already included in the project's Packages folder
3. Use Unity's built-in editor tools for development

### Common Development Tasks

- **Reference Finder**: Access via `Assets/Find References In Project` or `Window/Reference Finder`
- **Shader Scanner & Replacer**: Access via `Scorpio/VFXToolset/Shader Scanner & Replacer`

## Package Dependencies

- Unity Editor Coroutines package (`com.unity.editorcoroutines`)
- Unity Render Pipelines Core (`com.unity.render-pipelines.core`) - version 14.0.0

## Architecture Notes

The codebase follows Unity's editor extension patterns:
- Editor tools are in the `Editor/` folder
- Uses Unity's AssetDatabase API for asset management
- Implements custom editor windows and inspectors
- Uses JsonUtility for serialization to cache data
- Implements caching system for performance optimization

The ReferenceFinder system is particularly noteworthy as it:
- Builds a dependency graph of all assets in the project
- Caches results to avoid re-scanning on every access
- Supports both dependency and reference modes
- Provides a visual tree view for exploring asset relationships

## Development Commands

### Building
- No explicit build commands required - Unity handles compilation
- Changes to C# scripts are automatically compiled when saved in Unity Editor

### Testing
- No specific test framework mentioned, but Unity's built-in testing capabilities can be used
- Manual testing through Unity Editor is the primary method

### Running Tools
- Reference Finder: `Assets/Find References In Project` or `Window/Reference Finder`
- Shader Scanner & Replacer: `Scorpio/VFXToolset/Shader Scanner & Replacer`