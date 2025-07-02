# Servable

[![GitHub stars](https://img.shields.io/github/stars/zenikode/Servable?style=social)](https://github.com/zenikode/Servable/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/zenikode/Servable?style=social)](https://github.com/zenikode/Servable/network/members)
[![GitHub issues](https://img.shields.io/github/issues/zenikode/Servable)](https://github.com/zenikode/Servable/issues)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)
![Unity](https://img.shields.io/badge/Unity-2021.3+-green?logo=unity)
![Maintained](https://img.shields.io/maintenance/yes/2025)

---

## 🎯 Features

- 🔌 Bind any observable property in Unity (MonoBehaviour or ScriptableObject)
- 🖱️ Editor-first: Link properties visually with custom inspectors and drawers
- ♻️ Works for runtime data, UI, and hybrid approaches
- 💡 Supports Observable data, commands, and auto listeners
- 🛠️ Minimal dependencies (only [Newtonsoft.Json](https://www.newtonsoft.com/json))
- 🏷️ Hierarchy tags and visual helpers in the Editor

---

## 🛠️ Installation

Add Servable to your Unity project by editing your `manifest.json`:

```json
"com.zeni.servable": "https://github.com/zenikode/Servable.git?path=Assets"
```

- Minimum Unity version: **2021.3**
- Requires: [Newtonsoft.Json](https://www.newtonsoft.com/json)

---

## 🚀 Quickstart: Health Bar, Health Percent, and Damage Feedback Chain

This example demonstrates a linear chain of observable data and commands in `PlayerModel`, using generic binding names.

---

### 1️⃣ Health Label

**Model:**
```csharp
using Servable.Runtime;
using Servable.Runtime.ObservableProperty;
using UnityEngine;

public class PlayerModel : ModelBehaviour
{
    public ObservableData<int> MaxHealth { get; } = new(100);
    public ObservableData<int> CurrentHealth { get; } = new(100);
    // ... (further properties and commands below)
}
```

**Binding:**
```csharp
using Servable.Runtime.Bindings;
using UnityEngine;
using UnityEngine.UI;

public class BindLabelInt : ABindingData<int>
{
    Text _label;
    Text label => _label ??= GetComponent<Text>();

    public override void OnValue(int value)
    {
        if (label)
            label.text = $"Health: {value}";
    }
}
```

**Setup:**
- Add `PlayerModel` to your scene.
- Add a `Text` UI component and place `BindLabelInt` on the same GameObject.
- In the Inspector, bind `BindLabelInt.data` to `PlayerModel.CurrentHealth`.

---

### 2️⃣ Health Percent Calculation and Animator Float

**Model Additions:**
```csharp
public ObservableData<float> HealthPercent { get; } = new(1.0f);

private void Awake()
{
    MaxHealth.AddListener(_ => UpdatePercent());
    CurrentHealth.AddListener(_ => UpdatePercent());
    UpdatePercent();
}

private void UpdatePercent()
{
    float percent = MaxHealth.Value > 0
        ? Mathf.Clamp01((float)CurrentHealth.Value / MaxHealth.Value)
        : 0f;
    HealthPercent.Value = percent;
}
```

**Binding:**
```csharp
using Servable.Runtime.Bindings;
using UnityEngine;

public class BindAnimatorFloat : ABindingData<float>
{
    Animator _animator;
    Animator animator => _animator ??= GetComponent<Animator>();

    public string parameterName = "HealthPercent";

    public override void OnValue(float value)
    {
        if (animator)
            animator.SetFloat(parameterName, value);
    }
}
```

**Setup and Usage:**
- Add an `Animator` component and place `BindAnimatorFloat` on the same GameObject.
- Bind `BindAnimatorFloat.data` to `PlayerModel.HealthPercent`.
- Set `parameterName` to your desired float parameter (e.g., `"HealthPercent"`).

**How to use:**
- In your Unity Animator Controller, use the `"HealthPercent"` parameter to blend between different walk (or movement) animations.
- For example, when `HealthPercent` is near 1.0, use the standard walk/run animation. As `HealthPercent` drops (player gets hurt), blend to a more wounded or limping walk animation.
- This makes the avatar visibly appear more wounded as health decreases, providing a more immersive and reactive character.

---

### 3️⃣ Damage Command and Hit Feedback

**Model Additions:**
```csharp
using Servable.Runtime.ObservableCommand;

public class IncomingDamageCommand { public int Amount; }

public ObservableCommand<IncomingDamageCommand> TakeDamage { get; } = new();
public ObservableCommand HitFx { get; } = new();

private void Awake()
{
    // ... previous listeners
    TakeDamage.AddListener(OnTakeDamage);
}

private void OnTakeDamage(IncomingDamageCommand cmd)
{
    CurrentHealth.Value = Mathf.Max(0, CurrentHealth.Value - cmd.Amount);
    HitFx.Emit(); // Emit hit FX command
}
```

**Animator Trigger Binding:**
```csharp
using Servable.Runtime.Bindings;
using UnityEngine;

public class BindAnimatorTrigger : ABindingCommand
{
    Animator _animator;
    Animator animator => _animator ??= GetComponent<Animator>();

    public string triggerName = "Hit";

    public override void OnCommand()
    {
        if (animator)
            animator.SetTrigger(triggerName);
    }
}
```

**Setup:**
- Place `BindAnimatorTrigger` on a GameObject with an `Animator` component.
- Bind `BindAnimatorTrigger.command` to `PlayerModel.HitFx`.
- Set `triggerName` to your desired trigger parameter (e.g., `"Hit"`).

**To deal damage in your game logic:**
```csharp
playerModel.TakeDamage.Emit(new IncomingDamageCommand { Amount = 25 });
```

**The chain:**
- Damage command emitted → HP reduced → Health percent recalculated → UI and animator updated → Hit FX command emitted → Animator "Hit" trigger is set.

---

### 🏁 Final PlayerModel Example

```csharp
using Servable.Runtime;
using Servable.Runtime.ObservableProperty;
using Servable.Runtime.ObservableCommand;
using UnityEngine;

public class IncomingDamageCommand { public int Amount; }

public class PlayerModel : ModelBehaviour
{
    public ObservableData<int> MaxHealth { get; } = new(100);
    public ObservableData<int> CurrentHealth { get; } = new(100);
    public ObservableData<float> HealthPercent { get; } = new(1.0f);

    public ObservableCommand<IncomingDamageCommand> TakeDamage { get; } = new();
    public ObservableCommand HitFx { get; } = new();

    private void Awake()
    {
        MaxHealth.AddListener(_ => UpdatePercent());
        CurrentHealth.AddListener(_ => UpdatePercent());
        TakeDamage.AddListener(OnTakeDamage);
        UpdatePercent();
    }

    private void UpdatePercent()
    {
        float percent = MaxHealth.Value > 0
            ? Mathf.Clamp01((float)CurrentHealth.Value / MaxHealth.Value)
            : 0f;
        HealthPercent.Value = percent;
    }

    private void OnTakeDamage(IncomingDamageCommand cmd)
    {
        CurrentHealth.Value = Mathf.Max(0, CurrentHealth.Value - cmd.Amount);
        HitFx.Emit();
    }
}
```

---

## 🧩 Advanced Usage

- Bindings can target any property (getter) that returns an `ObservableData<T>`, whether in a `MonoBehaviour` or `ScriptableObject`.
- Property drawers and custom inspectors streamline the workflow—just select the model and property in the Unity Editor.
- Supports runtime and editor-time scenarios.

---

### ☝️ _One more thing:_ External Binding Example — Player Animator Is Sleepy If It's Evening

Servable bindings can reference properties from external objects, including ScriptableObjects, making it easy to build centralized services or game-wide state.

**Example:**  
Suppose you have a `DaytimeService` ScriptableObject with an observable enum property for the current day phase. You want your player animator to enter a "sleepy" state if it's evening.

**DaytimeService ScriptableObject:**
```csharp
using Servable.Runtime;
using Servable.Runtime.ObservableProperty;
using UnityEngine;

public enum DayPhase
{
    Morning,
    Afternoon,
    Evening,
    Night
}

[CreateAssetMenu(menuName = "Game/DaytimeService")]
public class DaytimeService : ScriptableObject
{
    public ObservableData<DayPhase> Phase = new(DayPhase.Morning);
}
```

**Binding Example: Player Animator Sleepy State**
```csharp
using Servable.Runtime.Bindings;
using UnityEngine;

public class BindAnimatorSleepy : ABindingData<DayPhase>
{
    Animator _animator;
    Animator animator => _animator ??= GetComponent<Animator>();
    public string sleepyParameter = "IsSleepy";

    public override void OnValue(DayPhase value)
    {
        if (animator)
            animator.SetBool(sleepyParameter, value == DayPhase.Evening);
    }
}
```

**How to bind to the external ScriptableObject:**
- Create an instance of `DaytimeService` in your project (Assets → Create → Game → DaytimeService).
- Add `BindAnimatorSleepy` to your player GameObject (with an `Animator`).
- **In the Inspector, bind `BindAnimatorSleepy.data` to the `Phase` property of the `DaytimeService` ScriptableObject instance**.
- In your Animator Controller, use the `IsSleepy` bool parameter to transition to a "sleepy" animation state.  
  For example, when `IsSleepy` is true (evening), the avatar might play a yawn, stretch, or drowsy idle animation.

**Gameplay logic example:**
```csharp
// From any system or MonoBehaviour:
daytimeService.Phase.Value = DayPhase.Evening; // Triggers sleepy state on all linked animators
```

---

## 🖥️ Editor Integration

- 🏷️ Custom inspectors and property drawers for easy, visual setup
- 🟩 Colored tags and labels in the Unity hierarchy for quick visualization of bindings
- These features support (but do not replace) robust runtime data binding

---

## 📦 Requirements

- Unity **2021.3+**
- [Newtonsoft.Json](https://www.newtonsoft.com/json)

---

## ⚡ Status

**Active development.**  
APIs may change. Feedback and contributions are welcome!

---

## 🤝 Contributing

Pull requests, issues, and suggestions are welcome!  
Star ⭐ the repo to support development.

---

## 📄 License

[MIT](LICENSE)

---

<p align="center">
  <b>Made with ❤️ by <a href="https://github.com/zenikode">zenikode</a></b>
</p>
