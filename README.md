# Unity Core Utility Package

This package is the invisible backbone for scalable Unity projects. It supplies optimized, pre-built systems to handle repetitive background tasks. Engineered to prevent memory stutters, its independent architecture keeps your codebase clean and adaptable, freeing you to focus strictly on building core gameplay features.

## Core Principles

* **Zero GC Allocation:** Systems are designed to pre-allocate memory and prevent Garbage Collection spikes during the core gameplay loop.
* **SOLID Architecture:** Modules are heavily decoupled, relying on interfaces and ScriptableObjects to ensure testability and isolation.
* **Modularity:** Each utility acts as an independent building block, meaning you only use what you need without unnecessary overhead.

## Modules

### 1. Object Pooling System
A custom-built, highly optimized object pooling solution. 
* Completely independent of Unity's built-in pool.
* Decouples pool configuration from the spawner logic.
* Engineered for allocation-free runtime spawning and despawning.

### 2. API Manager
An asynchronous network management system.
* Built utilizing C# `async/await` for scalable and readable network requests.
* Uses the Facade pattern to hide complex HTTP headers and request formatting from game logic.
* Supports interface-based injection for easy offline mocking.
