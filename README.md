# State Machine Framework (SFM) for Unity

## What I'm currently working on in this repo

## Overview

This Simple State Machine is a starting point to allow you to focus more on implementation rather than recreating a new State Machine each time. It works with States as components, providing ease of use and better modification of behavior directly within the Unity Editor. This repo Includes:

- **Base Classes**: Abstract classes that define the core functionality of states and state machines.

* Editor Window Tool: A Unity Editor tool (SFMWindow) that automates the creation of state machines and states.
* **Example Implementations**: Sample classes demonstrating how to extend the base classes.

## Table of Contents

- [Getting Started](#getting-started)

  - [Installation](#installation)

- [Usage](#usage)

  - [Creating a State Machine Manually](#creating-a-state-machine-manually)
    - [Step 1: Create a Custom Base State](#step-1-create-a-custom-base-state)
    - [Step 2: Create States](#step-2-create-states)
    - [Step 3: Create a State Machine](#step-3-create-a-state-machine)
  - [Using the SFMWindow Editor Tool](#using-the-sfmwindow-editor-tool)
    - [Creating a State Machine with SFMWindow](#creating-a-state-machine-with-sfmwindow)
    - [Adding States to an Existing State Machine](#adding-states-to-an-existing-state-machine)
   
- [Why Choose This Approach?](#Why-Choose-This-Approach)
## Getting Started

### Installation

Option 1:

1. **Download the Scripts**: Clone or download the repository containing the SFM scripts, or download the Unity package.
2. **Import into Unity**: Place the scripts or import the Unity package into your Unity project's `Assets` folder.

Option 2:

1. **Download the Unity Package**.
2. **Import the Unity package** into your Unity project by double-clicking it or using Assets > Import Package > Custom Package.



## Usage

### Creating a State Machine Manually

#### Step 1: Create a Custom Base State

Create a new abstract class that inherits from `BaseState`. 

```csharp
using UnityEngine;
using StateMachine;

public abstract class MyBaseState : BaseState
{
    protected MyStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<MyStateMachine>();
    }
}
```

#### Step 2: Create States

Create concrete state classes that inherit from your custom base state.

```csharp
using UnityEngine;
using StateMachine;

public class IdleState : MyBaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Initialization
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        // Behavior

        //Example of switching state
        if (Input.GetKeyDown(KeyCode.W))
        {
            stateMachine.SwitchState(stateMachine.WalkingState);
        }
        
    }

    public override void OnExit()
    {
        base.OnExit();
        // Cleanup
    }
}
```

Repeat this step for each state you need (e.g., `WalkingState`, `RunningState`).

#### Step 3: Create a State Machine

Create a class that inherits from `BaseStateMachine` to manage your states.

```csharp
using UnityEngine;
using StateMachine;

[RequireComponent(typeof(IdleState))]
[RequireComponent(typeof(WalkingState))]
public class MyStateMachine : BaseStateMachine
{
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkingState walkingState;

    private void Awake()
    {
        idleState = GetComponent<IdleState>();
        walkingState = GetComponent<WalkingState>();
    }

    public void Start() { SwitchState(idleState); }

    public void SwitchState(MyBaseState state)
    {
        if (state is MyBaseState)
        {
            base.SwitchState(state);
        }
        else
        {
            Debug.LogError("State must be of type MyBaseState");
        }
    }
}
```

### Using the SFMWindow Editor Tool

The `SFMWindow` editor tool automates the creation of state machines and states.

#### Accessing SFMWindow

- In Unity, go to the menu bar and select **SFM > State Machine Tools**.

#### Creating a State Machine with SFMWindow
![image](https://github.com/user-attachments/assets/11c39cce-a435-4a75-9a55-0a285b9b0715)

1. **Open SFMWindow**: Navigate to **SFM > State Machine Tools**.
2. **Select "Create State Machine" Tab**.
3. **Enter State Machine Name**: Provide a name for your state machine.
4. **Add States**:
   - Click **Add State** to add a new state field.
   - Enter a name for each state.
5. **Generate Scripts**:
   - Click **Generate State Machine**.
   - Choose the directory folder to save the scripts.
6. **Scripts Generated**:
   - The tool will generate the base state, state machine, and state scripts.

#### Adding States to an Existing State Machine

1. **Select "Add States" Tab**.
2. **Select Base State Script**:
   - Click the object field to select your existing base state script.
3. **Add New States**:
   - Click **Add State** to add new state fields.
   - Enter names for the new states.
4. **Add States**:
   - Click **Add States**.
   - Choose the directory to save the new state scripts.
5. **Scripts Generated**:
   - The tool will generate the new state scripts and update your state machine if necessary.

## Contributing
Contributions are welcome! If you find bugs or want to add features

## Why Choose This Approach
This is my personal preferred approach due to segmenting the states into other components, this primarily allows you to from inspector modify the variables on each state component
so you underestand better what you are modifying.
