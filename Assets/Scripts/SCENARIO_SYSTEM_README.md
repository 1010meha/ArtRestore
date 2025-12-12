# Restoration Scenario System Setup Guide

This system allows you to easily create and manage multiple restoration scenarios (clients with paintings to restore).

## Quick Start

### 1. Create a Scenario ScriptableObject

1. In Unity, right-click in the Project window
2. Go to **Create > Art Restore > Scenario**
3. Name it (e.g., "FirstClient")
4. Configure the scenario in the Inspector:
   - **Client Name**: Name of the client
   - **Dialogue Start Node**: YarnSpinner node to start (e.g., "Start")
   - **Painting Sprites**: Assign the painting sprites
   - **Restoration Steps**: Check which steps are required:
     - Varnish Removal (with required concentration %)
     - Jigsaw Puzzle
     - Gesso Application
     - Repainting
   - **Completion Dialogue Node**: YarnSpinner node when done (e.g., "Completion")

### 2. Set Up YarnSpinner Dialogue

Create a `.yarn` file in `Assets/YarnSpinny/` with nodes matching your scenario:
- Start node (matches "Dialogue Start Node")
- Completion node (matches "Completion Dialogue Node")

Example:
```yarn
title: Start
---
Client: Hello! I'd like this painting restored!
Player: Of course! Let me take a look at it.
<<jump WaitForCompletion>>
===

title: Completion
---
Client: Thanks!
<<stop>>
===
```

### 3. Set Up Scene Components

Add these components to your scene:

1. **ScenarioManager** (on an empty GameObject):
   - Assign references to:
     - DialogueRunner (YarnSpinner)
     - ScenarioProgressTracker
     - Painting Display (SpriteRenderer)
     - Canvas Managers
     - SwitchTools
     - BeakerManager
     - PuzzleManager (if using puzzles)
     - Completion Button GameObject
   - Optionally: Add scenarios to "Scenario Queue" to play them in order

2. **ScenarioProgressTracker** (on an empty GameObject):
   - No configuration needed, just add the component

3. **RestorationStepTracker** (on an empty GameObject):
   - Assign references to all the tools and managers

4. **CompletionButton** (on a UI Button):
   - Assign ScenarioManager reference
   - This button will appear when restoration is complete

### 4. Load a Scenario

**Option A: Through Code/Inspector**
- Use `ScenarioSetupHelper` component
- Assign scenario in Inspector
- Right-click component > "Load Scenario"

**Option B: Through YarnSpinner**
- Use command: `<<loadscenario ScenarioName>>`

**Option C: Through Scenario Queue**
- Add scenarios to ScenarioManager's "Scenario Queue"
- They'll play automatically in order

## How It Works

1. **ScenarioManager** loads a scenario and sets up the painting
2. **RestorationStepTracker** monitors tool usage and updates progress
3. When all required steps are complete, **CompletionButton** appears
4. Player clicks button → Completion dialogue plays → Next scenario loads

## Creating Multiple Scenarios

1. Create multiple Scenario ScriptableObjects
2. Set up each with different:
   - Paintings (sprites)
   - Required restoration steps
   - Dialogue nodes
3. Chain them together using:
   - Scenario Queue in ScenarioManager, OR
   - `nextScenario` field in each scenario

## Example: First Client Scenario

1. Create Scenario: "FirstClient"
   - Requires: Varnish Removal (50% concentration)
   - Dialogue: "Start" → "Completion"
2. Create Yarn file: `FirstClient.yarn`
   - Node "Start": Client greeting
   - Node "Completion": Client thanks
3. **IMPORTANT**: Make sure your DialogueRunner has the YarnProject assigned!
   - Select the DialogueRunner GameObject
   - In Inspector, assign the YarnProject (e.g., `ArtRestoreYarnSpin`)
   - The YarnProject should include your `.yarn` files
4. Add to ScenarioManager's queue or load manually

The system will automatically:
- Load the painting
- Track varnish removal progress
- Show completion button when done
- Play completion dialogue
- Load next scenario

## Troubleshooting

### "Cannot load node Start: No nodes have been loaded"

This means YarnSpinner hasn't loaded the dialogue files. Fix:

1. **Check DialogueRunner Setup:**
   - Select the GameObject with DialogueRunner component
   - In Inspector, make sure "Yarn Project" field is assigned
   - It should point to your YarnProject asset (e.g., `ArtRestoreYarnSpin`)

2. **Check YarnProject:**
   - Select the YarnProject asset in Project window
   - In Inspector, check "Source Files" - your `.yarn` files should be listed
   - If not, click "Add" and select your `.yarn` files

3. **Check for Compilation Errors:**
   - Look in Unity Console for YarnSpinner compilation errors
   - Fix any syntax errors in your `.yarn` files

4. **Use Setup Checker:**
   - Add `YarnSpinnerSetupChecker` component to a GameObject
   - Right-click component → "Check YarnSpinner Setup"
   - This will tell you what's missing

### Dialogue doesn't start

- Check that the node name in your Scenario matches exactly (case-sensitive)
- Use `YarnSpinnerSetupChecker` → "List All Available Nodes" to see valid node names
- Make sure DialogueUI is assigned to DialogueRunner

