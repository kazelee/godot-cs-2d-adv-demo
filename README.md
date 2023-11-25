# C# 版本 Godot 4 勇者传说 Demo

使用 C# 实现 Godot 4 教程《勇者传说》项目

视频链接：[合集·《勇者传说》Godot 4教程](https://space.bilibili.com/7092/channel/collectiondetail?sid=1304862)

作者的 GDS 版本项目：[timothyqiu/godot-2d-adventure-tutorial: Godot 4 横版动作游戏教程](https://github.com/timothyqiu/godot-2d-adventure-tutorial)

编辑器：Rider（支持较好，推荐；如果使用 vsc 或 vs 可参考下方和其他网络资料）

VSCode 可参考：[Godot + C# + Visual Studio Code 配置](https://www.bilibili.com/read/cv26511303/)（需要安装 C# 相关插件及 C# tools for godot 等插件）

Visual Studio 可参考：[【Godot】基础C#脚本入门以及vs调试设置 - 知乎](https://zhuanlan.zhihu.com/p/660009066)

## 前言

由于是跟着原教程做一遍项目后，重新用 C# 再自己过一遍，很多部分不一定会严格遵守教程中的顺序和细节，更多地是自己换一种语言重新熟悉一遍游戏的制作流程，加深印象。

我会对自己遇到的困难进行总结，但不会事无巨细地对每个细节和知识点都列举出来，所以系个人项目，仅供参考。

本项目一定程度上参考了[汐雨烟时](https://space.bilibili.com/102545357)的评论和[少年林克的奇幻漂流](https://space.bilibili.com/2641637)的 [Github 项目](https://github.com/haiyusun/godot-2d-hero)。

## 难点/知识点总结

### 00 环境部署

安装 Rider 和 Godot C# mono 版本，在「编辑器设置 - 文本编辑器 - 外部」勾选「使用外部编辑器」，可执行目录填写 Rider 的 exe 文件即可（在安装目录的 bin 文件中），然后创建新项目，新建 cs 脚本。

（确保电脑安装了 .Net 框架，Rider 在安装后的初始化会提示下载 .Net）使用 Rider 打开 cs 文件时，会自动检测到项目的 sln 文件，然后根据项目推荐安装 Godot 插件，确认安装即可（这一部分不需要额外操作，十分省心）

### 01 引用节点

GDS 支持用 $ 符号 + 节点相对地址定位到节点：

```gdscript
@onready var graphics: Node2D = $Graphics
@onready var animation_player: AnimationPlayer = $AnimationPlayer
```

C# 脚本必须写成如下形式（需要在 _ready 函数中使用 GetNode 函数获取节点）：

```C#
public partial class player : CharacterBody2D {
	private Node2D _graphics;
	private AnimationPlayer _animationPlayer;
	
	public override void _Ready() {
		_graphics = GetNode<Node2D>("Graphics");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
}
```

具体可以参考官方文档：[C# API 与 GDScript 的差异 — Godot Engine (4.x) 简体中文文档](https://docs.godotengine.org/zh-cn/4.x/tutorials/scripting/c_sharp/c_sharp_differences.html)

### 02 项目结构与节点状态机

[少年林克的奇幻漂流](https://space.bilibili.com/2641637)的 [Github 项目](https://github.com/haiyusun/godot-2d-hero)中使用的就是一种类似 Unity 的大项目结构，将场景与脚本分开，并通过节点状态机实现各状态切换，本项目也沿用这种模式，不过尽可能减少项目的复杂程度，遵守教程的实现思路。

节点状态机参考了：[Making a basic finite state machine (Godot4/C#) | by Mina Pêcheux | CodeX | Sep, 2023 | Medium](https://medium.com/codex/making-a-basic-finite-state-machine-godot4-c-fe5ccc0e8cd7)

出于简明的需要，不再考虑各种接口和复用，也不额外使用非节点的 C# 脚本，一律继承自 Godot 的 Node。由于能力有限，暂时只能做简单的修改，无法用大型项目的结构框架要求重写代码。

#### 节点状态机模板

基类状态 State 脚本（虚方法）

```C#
using Godot;
using System;

public partial class State : Node
{
    public StateMachine StateMachine;
 
    public virtual void Enter() {}
    public virtual void Exit() {}
 
    public new virtual void Ready() {} // Rider 建议添加 new 以避免混淆
    public virtual void Update(float delta) {}
    public virtual void PhysicsUpdate(float delta) {}
    public virtual void HandleInput(InputEvent @event) {}
}
```

状态机 StateMachine 脚本

```C#
using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    [Export] public NodePath InitialState;
    
    private Dictionary<string, State> _states;
    private State _currentState;
    
    public override void _Ready() {
        _states = new Dictionary<string, State>();
        foreach (var node in GetChildren()) {
            if (node is State s) {
                _states[node.Name] = s;
                s.StateMachine = this;
                s.Ready();
                s.Exit(); // reset
            }
        }
        
        _currentState = GetNode<State>(InitialState);
        _currentState.Enter();
    }
    
    public override void _Process(double delta) {
        _currentState.Update((float)delta);
    }
    
    public override void _PhysicsProcess(double delta) {
        _currentState.PhysicsUpdate((float)delta);
    }
    
    public override void _UnhandledInput(InputEvent @event) {
        _currentState.HandleInput(@event);
    }
    
    public void TransitionTo(string key) {
        if (!_states.ContainsKey(key) || _states[key] == _currentState) {
            return;
        }

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }
}
```

注意：代码中 Export 的内容不会立刻显示在检查器中，需要先编译！

#### 从 GDS 枚举状态机迁移

设置 PlayerState 继承自 State，其他玩家状态（如 PlayerIdleState）继承自 PlayerState

与枚举状态机的方法相似，我们也是要在 Player 的场景下加载一个 StateMachine 脚本，只不过节点状态机需要在脚本节点下方声明具体的状态，并为这些状态添加对应的状态脚本。脚本中各函数的实现如下：

| GDS 状态函数               | 节点状态函数         | 补充                  |
| -------------------------- | -------------------- | --------------------- |
| get_next_state(state)      | Update(delta)        | 判断条件/进入其他状态 |
| transition_state(from, to) | Enter()              | 进入新状态            |
| tick_physics(delta)        | PhysicsUpdate(delta) | 状态的运动逻辑        |

（注：GDS 中 match 前面的部分写在基类 PlayerState 中，match 后面的部分写在对应的状态类中）

【补充】关于 is_first_tick 的问题

原 GDS 项目中，改写成状态机需要引入这个变量确保跳跃的第一帧速度是满速，这是因为原本的逻辑是：刷新速度 -> 判断跳跃（如果跳跃，改变速度）-> move_and_slide -> 新一轮……所以速度改变的瞬间不会被重力影响；改成状态机后，逻辑就变成了：进入跳跃状态 -> 改变速度（跳跃速度）-> 进入物理帧处理函数 -> 调用 move 函数，改变速度（受到重力影响）-> move_and_slide……所以速度会受到影响。

使用节点状态机，并不能避免这个问题，所以也需要额外声明。节点状态机需要在进入状态的时候就给 is_first_tick 赋值为 true，然后在运行 move 函数的时候给与不同的参数，然后恢复成 false。

### 03 访问权限

GDS 的 set/get 语句，可以在 C# 中可以用类似的方式实现（不考虑枚举的实现）：

```c#
private int _direction;  
[Export] public int Direction {
    get => _direction;
    set {
        _direction = value;
        Graphics.Scale = new Vector2(-_direction, 1);
    }
}
```

当然这种方法解决不了 export 和 onready 的时序问题，而且臃肿没必要；可以不使用 export，而是在 ready 中赋值，也可以另外设置一个函数，用于赋值时改变 graphics，并在 ready 之后调用：

```c#
public void SetDirection(int dir) {
    Direction = dir;
    Graphics.Scale = new Vector2(-Direction, 1);
}
```

枚举也改用 const int 来实现，注意不要忘了 export 的 direction 值是 -1，默认是 0，会导致野猪无法显示（scale 为 0）

### 04 信号与事件委托

关于 C# 的事件委托机制，这里做一个粗浅的类比：

| GDS 的代码         | C# 的代码                              | 具体含义             |
| ------------------ | -------------------------------------- | -------------------- |
| `signal my_signal` | `public delegate void MyEventHandler;` | 事件/信号的“代号”    |
| `A.connect(_on_B)` | `A += OnB;`                            | 将代号与处理函数连接 |
| `func _on_B()`     | `public void OnB()`                    | 处理函数的实现       |

比如，对于 Hitbox 的代码，用 GDS 写的效果如下：

```gdscript
signal hit(hurtbox)

func _init() -> void:
	area_entered.connect(_on_area_entered) # "signal area_entered" in Area2D

func _on_area_entered(hurtbox: Hurtbox) -> void:
	hit.emit(hurtbox)
	hurtbox.hurt.emit(self)
```

用 C# 写的效果如下：

```c#
[GlobalClass]
public partial class Hitbox : Area2D {
    [Signal]
    public delegate void HitEventHandler(Hurtbox hurtbox);

    public override void _Ready() {
        AreaEntered += OnAreaEntered; // "public event ...EventHandler AreaEntered" in Area2D
    }

    public void OnAreaEntered(Area2D area) {
        Hurtbox hurtbox = area as Hurtbox;
        EmitSignal(SignalName.Hit, hurtbox);
        hurtbox?.EmitSignal(Hurtbox.SignalName.Hurt, this);
    }
}
```

值得注意的是，信号创建时必须以“EventHandler”结尾，调用的名称即创建名称去掉“EventHandler”的形式。事件和委托有所不同，事件是对委托的封装，而且只能在声明事件的类内部被调用；一般情况下使用委托就行了。

### 05 全局类

GDS 项目中，继承自节点的脚本只要声明了 class_name，便可以在节点列表中找到并引用；C# 项目就不行，一种替代方法就是添加这个脚本继承的父节点，然后给这个节点添加这个脚本。当然，也可以通过声明全局类来实现：

```C#
[GlobalClass]
public partial class Hurtbox : Area2D {
    [Signal]
    public delegate void HurtEventHandler(Hitbox hitBox);
}
```

这样，Hurtbox 这个节点就可以在场景中作为节点直接添加了！

## 附录：一些错误

0. 项目出现以下错误：

```
E 0:00:01:0260   can_instantiate: Cannot instance script because the associated class could not be found. Script: 'res://src/Player.cs'. Make sure the script exists and contains a class definition with a name that matches the filename of the script exactly (it's case-sensitive).
  <C++ 错误>       Method/function failed. Returning: false
  <C++ 源文件>      modules/mono/csharp_script.cpp:2388 @ can_instantiate()
```

这种问题很频繁，网络上也能找到各种反馈，目前没有一个非常确定的说法，可以考虑如下方法解决：

- 确保场景名称、脚本名称、类名是完全相同的（包括大小写），如 Player.tscn, Player.cs, `public partial class Player`
- 项目解决方案的名称最好不要有特殊字符和数字（用纯英文最保险）
- 如果无法解决，重新创建场景新建脚本，并确保位置不再更改；或者确保脚本在场景的同一级目录下，或者脚本在场景的同一级文件夹中（如，为 `res://scenes/World.tscn` 添加脚本，位置可以是 `res://scenes/World.cs` 或 `res://scenes/src/World.cs`，但如果是 `res://src/World.cs` 就可能会出错，原因不详）

---

1. `_graphics.Transform.Scale.X = direction < 0 ? -1 : 1;`

原因：①不需要使用 Transform 做“过渡”；② Godot C# 不能对类型为 Vector 的属性做单值更改，必须作为一个整体赋值

解决代码如下：

```C#
var newScale = Vector2.One;
newScale.X = direction < 0 ? -1 : 1;
_graphics.Scale = newScale;
```

或者简化成：

```C#
_graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
```

---

2. `System.NullReferenceException: Object reference not set to an instance of an object`

原因：子节点（状态机）加载的时候，父节点 Player 还没有准备好，所以状态中的语句执行必须**确保 Player 存在**

我们可以在基本状态 PlayerState 中判断，这样对具体的状态就不必再考虑这个问题了（以 Enter 为例）

```c#
public override void Enter() {
    if (Player == null) {
        return;
    }

    if (!Player.LeaveFromOnFloorState && IsOnFloorState) {
        Player.CoyoteTimer.Stop();
    }

    Player.AnimationPlayer.Play(AnimationName);
}
```

---

3. 野猪节点状态机的问题（承接错误问题 2）

之前解决 Player 的 ready 时序问题，是直接简单粗暴的在 Enter 函数中判断 Player 是否为空，为空时 return；但这样就需要依赖状态转换来更新，因为玩家默认在空中，第二帧会就会改变状态，进而调用 Enter，此时 Player 不为空。

野猪就不一样了，由于不区分空中和地面的状态，一旦第一帧为空，return 之后就再也不会进入新状态并调用 Enter 函数，此时野猪没有站立动画，直到 2s 过后进入 walk 状态才会开始运动。

当然，只有 2s，如果可以确保玩家 2s 内不会看到野猪，那也无妨，不过这里还是修复一下：

```c#
private bool _isBoarReady = false;

public override void Enter() {
    if (Boar == null) {
        // _isBoarReady = false;
        return;
    }
    base.Enter();
	...
}

public override void Update(float delta) {
    if (!_isBoarReady) {
        if (Boar == null) {
            return;
        }

        _isBoarReady = true;
        Enter();
    }
    base.Update(delta);
	...
}
```

Godot 的 ready 机制自底向上，很符合直觉，但对于需要引用父节点的节点状态机来说很麻烦；父节点一定会在子节点的 ready 函数调用后再获取，所以原则上需要在子节点初始化时获取父节点的操作，应该由父节点完成，否则就得像上面的代码一样进行繁琐的判断，避免 `NullReferenceException` 和 Enter 函数语句没有执行的问题。

## 补充：一些改进

1. 蹬墙跳版本的 Coyote Timer

如果左右移动的输入先于跳跃，那么哪怕两个键按的很近，也会使得先进入 Fall 状态，随后跳跃就没用了；和走出平台的瞬间跳跃需要的 Coyote Timer 一样，可以把离开墙面的瞬间跳跃也设置一个倒计时。

```C#
// PlayerState.cs
public override void Enter() {
    ...
    if (!Player.LeaveFromWall && this is PlayerWallSlidingState s) {
        Player.WallJumpTimer.Stop();
    }
    ...
}
public override void Exit() {
	...
    Player.LeaveFromWall = this is PlayerWallSlidingState;
}
public override void Update(float delta) {
    ...
	var canJump = Player.IsOnFloor() || Player.CoyoteTimer.TimeLeft > 0 || Player.WallJumpTimer.TimeLeft > 0;
    ...
}

// PlayerFallState.cs
public override void Enter() {
	...
    if (Player.LeaveFromWall) {
        Player.WallJumpTimer.Start();
    }
}

// PlayerWallJump.cs
public override void Enter() {
	...
    Player.WallJumpTimer.Stop();
    Player.JumpRequestTimer.Stop();
}

// PlayerWallSliding.cs
public override void Enter() {
    base.Enter();
    Player.WallJumpTimer.Stop();
}
```

（未完待续，项目截至课程第 12 课“状态面板”，暂时搁置）
