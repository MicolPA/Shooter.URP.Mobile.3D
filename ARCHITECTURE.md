# ARCHITECTURE.md

## 1. Project summary

Co-op FPS for PC and mobile, developed in Unity 6 with URP.

The player can:

- Single Player.
<!-- - Create an online match. -->
<!-- - Allow another player to join through an identifier or room code. -->
- Survive enemy waves.
- Shoot with a single weapon.
- Face both ground and flying enemies.

The project uses a minimalist aesthetic based on:

- Simple geometric shapes.
- Solid colors.
- Gradients.
- Emissive materials.
- Bloom and neon lighting.
- Few textures or none at all.

## 2. Main technologies

- Unity 6
- Universal Render Pipeline
- New Input System
- Joystick Pack for mobile joysticks
- AI Navigation
- CharacterController
- TextMeshPro
- ScriptableObjects for wave configuration
- Netcode for GameObjects, Lobby, and Relay in a later stage

## 3. Architecture principles

- Each script has a clear responsibility.
- No abstractions are created unless they solve a real need.
- The player uses CharacterController, not Rigidbody.
- The weapon only applies damage; the enemy decides when it dies.
- Enemies notify their death through events.
- WaveManager controls the waves, but does not instantiate them directly.
- EnemySpawner instantiates enemies, but does not decide which wave is active.
- The UI only displays information; it does not control gameplay.
- In future online mode, the host will have authority over waves and enemies.

## 4. Recommended folder structure

```text
Assets
├── Art
├── Audio
├── Materials
├── Models
├── Prefabs
│   ├── Enemies
│   ├── Effects
│   ├── Player
│   └── UI
├── Scenes
├── ScriptableObjects
│   └── Waves
├── Settings
│   └── Input
├── UI
└── Scripts
    ├── Audio
    ├── Core
    ├── Enemies
    ├── Input
    ├── Managers
    ├── Player
    ├── UI
    ├── Utilities
    ├── Waves
    └── Weapons
```

## 5. Main scene hierarchy

```text
Scene
├── GameManager
├── AudioManager
├── Navigation
├── EnemySpawnSystem
├── WaveSystem
├── Player
│   └── CameraPivot
│       └── Main Camera
├── Canvas
│   ├── GameplayHUD
│   ├── MobileControls
│   ├── DamageOverlay
│   ├── WaveAnnouncement
│   └── LoseCanvas
├── EventSystem
├── Global Volume
└── Environment
```

## 6. Input system

### 6.1 FPSInputActions

Recommended location:

```text
Assets/Settings/Input/FPSInputActions.inputactions
```

Action map:

```text
Player
├── Move
├── Look
├── Jump
├── Fire
├── Reload
├── Sprint
├── Pause
└── Restart
```

PC bindings:

```text
Move    → WASD
Look    → Mouse Delta
Jump    → Space
Fire    → Left Mouse Button
Reload  → R
Sprint  → Left Shift
Pause   → Escape
Restart → F5
```

### 6.2 InputReader

Location:

```text
Assets/Scripts/Input/InputReader.cs
```

Responsibility:

- Receive events from the Player Input component.
- Store movement, camera, shooting, and sprinting input.
- Store jump, reload, pause, and restart button presses.
- Receive values from mobile controls.

Main data:

```csharp
MoveInput
LookInput
IsFiring
IsSprinting
```

Consumable methods:

```csharp
ConsumeJump()
ConsumeReload()
ConsumePause()
```

Mobile methods:

```csharp
SetMobileMoveInput(Vector2 value)
SetMobileLookInput(Vector2 value)
SetMobileFire(bool pressed)
PressMobileJump()
PressMobileReload()
```

### 6.3 MobileJoystickInput

Location:

```text
Assets/Scripts/Input/MobileJoystickInput.cs
```

Responsibility:

- Read the left joystick from Joystick Pack.
- Read the right joystick from Joystick Pack.
- Send those values to InputReader.

It must be added as a component of the Player.

### 6.4 MobileActionButton

Location:

```text
Assets/Scripts/Input/MobileActionButton.cs
```

Responsibility:

- Detect PointerDown and PointerUp.
- Trigger Fire, Jump, or Reload in InputReader.
- Allow Fire to be held down.
- Execute Jump and Reload only once per press.

Configuration:

```text
FireButton   → Action: Fire
JumpButton   → Action: Jump
ReloadButton → Action: Reload
```

## 7. Player

### 7.1 Components

```text
Player
├── CharacterController
├── PlayerInput
├── InputReader
├── MobileJoystickInput
├── PlayerMovement
├── PlayerLook
├── PlayerWeapon
├── PlayerHealth
├── PlayerDamageFeedback
└── CameraPivot
    └── Main Camera
```

It must not have a Rigidbody or CapsuleCollider.

### 7.2 PlayerMovement

Responsibility:

- Horizontal movement.
- Sprinting.
- Jumping.
- Gravity.
- Calling CharacterController.Move().

Suggested initial settings:

```text
Walk Speed: 5
Sprint Speed: 8
Acceleration: 12
Deceleration: 16
Jump Height: 1.5
Gravity: -20
Grounded Gravity: -2
```

### 7.3 PlayerLook

Responsibility:

- Rotate the Player horizontally.
- Rotate CameraPivot vertically.
- Limit pitch.
- Apply different sensitivities for mouse and joystick.
- Lock the cursor in PC mode.
- Do not lock it in mobile mode.

### 7.4 PlayerHealth

Responsibility:

- Store maximum and current health.
- Receive damage.
- Heal.
- Detect death.
- Play damage sound.
- Notify damage and death through events.

Events:

```csharp
OnDamaged
OnDeath
```

### 7.5 PlayerDamageFeedback

Responsibility:

- Listen to PlayerHealth.OnDamaged.
- Slightly move CameraPivot.
- Show a red overlay.
- Fade the overlay.

It must be a component of the Player.

## 8. Weapon

### PlayerWeapon

Location:

```text
Assets/Scripts/Weapons/PlayerWeapon.cs
```

Responsibility:

- Shoot with raycast.
- Apply damage.
- Control fire rate.
- Control ammo.
- Reload.
- Play sounds.
- Read Fire and Reload from InputReader.

Flow:

```text
InputReader
→ PlayerWeapon
→ Physics.Raycast
→ IDamageable.TakeDamage()
```

## 9. Damage system

### IDamageable

Location:

```text
Assets/Scripts/Core/IDamageable.cs
```

Contract:

```csharp
void TakeDamage(float damage);
```

## 10. Enemies

### 10.1 EnemyHealth

Responsibility:

- Store health.
- Receive damage.
- Detect death.
- Play impact and death sounds.
- Create explosion particles.
- Emit OnDeath.
- Destroy the GameObject.

The event should run before destroying the enemy.

### 10.2 EnemyAI

Used by ground enemies.

Responsibility:

- Find the Player.
- Chase it with NavMeshAgent.
- Stop when entering range.
- Look at the Player.
- Apply damage according to cooldown.
- Stop if the Player dies.

### 10.3 FlyingEnemyAI

Responsibility:

- Find the Player.
- Move with Vector3.MoveTowards.
- Maintain a preferred height.
- Rotate toward the target.
- Attack when entering range.
- Do not use NavMeshAgent.

### 10.4 Prefabs and variants

```text
GroundEnemyBase
├── GroundEnemyVer01
└── GroundEnemyVer02

FlyingEnemy
```

Variants can override health, speed, damage, scale, material, and death effect.


## 11. Navegación

`Navigation` contiene un `NavMeshSurface`.

Los puntos terrestres deben quedar sobre el NavMesh. Los voladores no lo necesitan.

## 12. EnemySpawner

Ubicación:

```text
Assets/Scripts/Waves/EnemySpawner.cs
```

Responsabilidad:

- Elegir un punto aleatorio.
- Validar posiciones terrestres con `NavMesh.SamplePosition`.
- Instanciar enemigos terrestres y voladores.
- Devolver el `EnemyHealth` creado.

Jerarquía:

```text
EnemySpawnSystem
├── GroundSpawnPoints
└── FlyingSpawnPoints
```

## 13. Oleadas

### 13.1 WaveData

ScriptableObject ubicado en:

```text
Assets/Scripts/Waves/WaveData.cs
```

Assets:

```text
Assets/ScriptableObjects/Waves/Wave01.asset
Assets/ScriptableObjects/Waves/Wave02.asset
Assets/ScriptableObjects/Waves/Wave03.asset
```

Datos actuales:

```text
GroundEnemyVer01 Prefab / Count
GroundEnemyVer02 Prefab / Count
FlyingEnemy Prefab / Count
Spawn Interval
Delay Before Next Wave
Total Enemy Count
```

### 13.2 WaveManager

Responsabilidad:

- Leer `WaveData`.
- Crear solicitudes de spawn.
- Mezclar el orden.
- Pedir enemigos al `EnemySpawner`.
- Registrar enemigos vivos.
- Suscribirse a `EnemyHealth.OnDeath`.
- Esperar a que `aliveEnemies.Count` sea cero.
- Iniciar la siguiente ola.
- Actualizar la UI.

Propiedades:

```csharp
CurrentWaveNumber
AliveEnemyCount
IsWaveRunning
AllWavesCompleted
```

## 14. UI

### 14.1 UIManager

Ubicación:

```text
Assets/Scripts/UI/UIManager.cs
```

Responsabilidad:

- Actualizar barra de vida.
- Actualizar número de ola.
- Actualizar enemigos vivos.
- Mostrar anuncio de próxima ola.
- Mostrar `LoseCanvas`.

### 14.2 HUD

Muestra:

```text
Vida del Player
Ola actual
Enemigos vivos
```

### 14.3 WaveAnnouncement

Usa `CanvasGroup` y `TMP_Text`.

```text
Alpha: 0
Interactable: false
Blocks Raycasts: false
```

El objeto debe permanecer activo.

### 14.4 DamageOverlay

Imagen roja a pantalla completa:

```text
Alpha inicial: 0
Raycast Target: false
```

### 14.5 LoseCanvas

Comienza desactivado.

Su botón llama:

```csharp
GameManager.RestartGame()
```

## 15. GameManager

Ubicación:

```text
Assets/Scripts/Managers/GameManager.cs
```

Responsabilidad:

- Escuchar `PlayerHealth.OnDeath`.
- Mostrar `LoseCanvas`.
- Liberar y mostrar el cursor.
- Pausar la partida.
- Reiniciar la escena.

Flujo:

```text
PlayerHealth.Die()
→ OnDeath
→ GameManager
→ UIManager.ShowLose()
→ Cursor visible
→ Time.timeScale = 0
```

## 16. Audio

### AudioManager

Ubicación:

```text
Assets/Scripts/Audio/AudioManager.cs
```

Responsabilidad:

- Reproducir música de fondo.
- Reproducir efectos mediante `PlayOneShot`.

Clips actuales:

```text
Background Music
Gunshot
Reload
Player Damage
Enemy Damage
Enemy Death
```

## 17. Reinicio de pruebas

Puede hacerse mediante F5 o con el botón del `LoseCanvas`.

La escena debe estar en la lista del Build Profile.

## 18. Estilo visual

Piso sugerido:

```text
#2B2F54
```

Cubos sugeridos:

```text
#454C7A
```

Neón cian:

```text
#29D8FF
```

Neón naranja:

```text
#FF633D
```

Materiales flat:

```text
URP/Lit
Metallic: 0
Smoothness: 0–0.1
Specular Highlights: Off
Environment Reflections: Off
```

Materiales neón:

```text
Emission: On
HDR Intensity: 3–8
```

Global Volume inicial:

```text
Bloom: Threshold 1.1 / Intensity 0.35 / Scatter 0.65
Tonemapping: ACES
Color Adjustments: Exposure -0.15 / Contrast 12 / Saturation 5
Vignette: Intensity 0.18 / Smoothness 0.55
```

Para móvil, intentar mantener entre 6 y 10 Point Lights visibles simultáneamente, sin sombras.

## 19. Futuro multijugador

Flujo objetivo:

```text
Jugar solo
Jugar online
├── Crear sala
└── Unirse con código
```

Tecnologías previstas:

```text
Netcode for GameObjects
Unity Lobby
Unity Relay
```

El host tendrá autoridad sobre:

```text
Inicio de oleadas
Spawn de enemigos
Vida y muerte de enemigos
Contador de enemigos
Cambio de ola
Estado de partida
```

Cada cliente controlará únicamente su propio Player.

## 20. Notas de mantenimiento

1. Al modificar un script en otra conversación, compartir su versión actual.
2. No asumir que coincide exactamente con versiones anteriores.
3. Mantener este documento actualizado tras cambios importantes.
4. Añadir nuevos eventos, dependencias y responsabilidades.
5. Evitar reintroducir sistemas descartados sin una necesidad real.

Este documento describe el estado actual de la arquitectura, pero los nombres exactos de campos y algunas implementaciones pueden cambiar durante el desarrollo.
