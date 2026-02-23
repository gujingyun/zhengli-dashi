# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# 🎮 整理大师 - Claude Code 项目指南

## 项目概述

- **项目名称**: 整理大师 (ZhengLi Master)
- **引擎**: 团结引擎 (Tuanjie Engine) 1.8.2
- **语言**: C#
- **目标平台**: 微信小游戏 / 抖音小游戏

## 项目结构

```
Assets/
├── Scripts/           # C# 脚本
│   ├── Core/         # 核心系统
│   ├── Gameplay/     # 游戏玩法
│   ├── UI/          # UI系统
│   └── Utils/        # 工具类
├── Scenes/           # 场景文件
├── Prefabs/          # 预制体
├── UI/               # UI资源
├── Resources/        # 运行时资源
├── Audio/            # 音效
└── Art/              # 美术资源
    ├── Sprites/     # 精灵图
    ├── Animations/  # 动画
    └── Atlas/       # 图集
```

## 代码架构

### 核心类层次

- `TuanjieMonoBehaviour`: 所有组件的基类，提供 `FindChild<T>()` 和 `GetOrAddComponent<T>()` 工具方法
- `TuanjieSingleton<T>`: 单例基类，继承自 `TuanjieMonoBehaviour`，自动实现 `DontDestroyOnLoad`
- 命名空间: `ZhengLiMaster` (游戏代码) 和 `TuanjieFramework` (框架代码)

### 核心系统

| 类 | 职责 | 关键方法 |
|---|---|---|
| `GameManager` | 游戏状态管理、步数/分数计算、消除逻辑 | `StartGame()`, `StartLevel()`, `EliminateBox()` |
| `DragManager` | 触摸/鼠标输入处理、射线检测目标盒子 | `StartDrag()`, `Dragging()`, `EndDrag()` |
| `Item` | 物品数据与拖拽状态 | `OnDragStart()`, `OnDragEnd()`, `ReturnToOriginalPosition()` |
| `Box` | 收纳盒容量管理、物品槽位布局 | `CanAddItem()`, `AddItem()`, `GetSameTypeCount()` |

### 状态机

`GameState` 枚举: `Waiting` → `Playing` → `Paused`/`LevelComplete`/`GameOver`

### 事件系统

使用 C# `System.Action` 委托进行事件通信:
- `onMovesChanged(int)` - 步数变化
- `onScoreChanged(int)` - 分数变化
- `onLevelComplete()` - 关卡完成
- `onGameOver()` - 游戏结束

## 游戏核心功能

1. **拖拽系统** - 单指拖拽，物品跟随手指移动
2. **盒子容量系统** - 3格/4格/5格容量
3. **消除系统** - 3个相同物品自动消除
4. **步数系统** - 步数用完游戏结束
5. **关卡系统** - 100+关卡配置
6. **存档系统** - 本地存储 + 云端同步
7. **好友排行榜** - 微信/抖音关系链
8. **登录系统** - 微信/抖音登录

## 开发规范

- 遵循 C# 编码规范
- 使用 MVC 架构模式
- 所有公共方法需添加 XML 注释
- 提交信息格式: `feat: 功能描述` / `fix: 修复描述`

## 团结引擎特定

- 使用 `TuanjieEngine` 命名空间 (别名 `TuanjieFramework`)
- UI 使用 UGUI 系统
- 发布使用 Cocos Creator 类似的发布流程

## 命令行构建

团结引擎支持命令行构建，但需要图形界面初始化。
建议开发时在本地使用团结引擎编辑器，构建时使用命令行或CI/CD。
