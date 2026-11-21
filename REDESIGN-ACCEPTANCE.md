# Rally Rush: requested product correction (2026-11-21)

User reports it does not resemble a 3D multiplier/crowd runner (Count Masters,
Number Runner 3D given as genre examples). Mechanical smoke tests are not product
acceptance. Keep an original presentation, characters, course art and UI.

Sequential queue: **after Metro Dash 3D replacement**, before returning to Merge
Foundry / Chroma / Cubic. No overlapping CI or release work between games.

Required player experience:

- A readable perspective 3D course and a visibly animated human/stylized runner
  crowd, not only a numeric counter or unrelated cat assets.
- Continuous forward motion; intuitive touch-drag horizontal steering with useful
  sensitivity, bounds and responsive camera. Tutorial demonstrates the real input.
- Clearly legible paired math gates (+, x, negative/division where designed).
  Choosing a gate changes the actual visible crowd once, with honest arithmetic.
- Crowd size and player number stay consistent. Hazard losses and enemy-clash
  outcomes are understandable, with actual crowd interaction, not invisible math.
- Designed, escalating levels with a clear start, choices, hazards/enemies, finish
  and crowd-dependent finale/reward; win/loss/retry/next-level all work.
- Readable UI/safe areas, visually inspect native first-run and moving gameplay,
  both gate choices, growth, collision/battle, finale, result and next level.
- Save/restart/pause consistency; no runtime errors. Audit current source and
  scenes first: imported code alone is not proof the shipped scenes render them.
- Preserve com.play.crowdrunner and signing identity. Fresh simulator acceptance,
  merge/push, incremented device build, ASC VALID and intended tester/group/eligible
  beta verification before claiming delivered. No physical device available.

## Source/scene audit — 2026-11-21 18:49 UTC

Metro replacement build 4 has now passed ASC VALID and independent intended
internal-group/tester/build/IN_BETA_TESTING checks. Rally is the sole active game.

- Enabled build scene is Assets/Scenes/Level.unity; its camera is perspective
  (orthographic=0, FOV60). Merely having 3D imports does not meet the brief.
- Scene army override references GUID e7ae3c5c7da8c4dadbafbabb187578c4,
  ExampleCat.prefab. Despite its name, current prefab is a single primitive mesh,
  no child rig: not a visibly animated stylized human crowd.
- ArmyController spawns Cat-tagged units via RadialFormation; retained Cat naming
  alone is not visual evidence. Replace actual rendered units/animation, not names.
- ArmyMovementController scales per-frame finger delta by fixedDeltaTime/width;
  current steering is frame-rate/sensitivity dependent and needs a real drag gate.
- Corridor operations update RadialFormation and disable paired colliders on exit;
  current score accumulates crowd counts, separate from actual crowd. Need exact
  before/after arithmetic, single application, visible unit count, hazards/clashes.
- Existing battle/finale/level flow must be audited against actual scene refs and
  native rendered captures; old mechanical acceptance is not sufficient.

Next implementation: original animated humanoid crowd and legible course/gates,
responsive normalized drag, designed escalating gate/hazard/clash/finish levels,
read-only telemetry and fresh real native input/capture gates. No Rally CI or
replacement deployment has been dispatched for this correction yet.
