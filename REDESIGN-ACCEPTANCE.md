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

## First source correction — 18:56 UTC (NOT accepted)

Cat component now builds an original vest/head/headband/limb/shoe runner over the
existing root collider/tag, hides the old primitive and animates opposing limbs
only in active game/battle states. This changes the referenced shipped prefab's
actual runtime rendering, not just an unused asset. Native visuals remain unproven.
Drag now maps normalized screen delta directly to track displacement without the
extra fixedDeltaTime multiplier; no movement persists after finger release.
RadialFormation distributes remainders among rings so requested and visible unit
counts agree even when crowd size is not divisible by ring count.

Diff checks pass; no Unity compiler is available on this host. Simulator export is
for compilation/visual iteration, NOT product acceptance. Still required: inspect
actual native humanoid/steering/camera and revise course/gates/clashes/finale/levels,
upgrade old smoke harness to exact real choices/growth/loss/battle/win/next/save gates,
then full fresh acceptance and incremented replacement distribution.

## Deeper shipped-flow audit — 19:03 UTC (compile still running)

- Level.unity overrides levels.Array.size to **1**. Stale data[1]/data[2]
  overrides do not establish playable levels; NextLevel clamps to the last index.
  Current replay therefore is not proof of designed escalating progression.
- MiniBattleController's loop tests the player count twice, never the enemy
  count; trigger lacks Cat/state filtering and count changes do not synchronize
  GameFlowManager/UI. It has a particle effect, not proven visible opposing crowd.
- Obstacle subtracts decreaseAmount yet removes exactly one spawned unit; it
  leaves count UI stale. Multiple root contacts and formation respawn need an
  honest, bounded per-unit loss contract with native before/after evidence.
- Finale is currently a tap meter initially fixed at .5, followed by a cat-ball
  roll whose force uses incoming crowd count. That alone does not establish the
  requested original human-crowd clash/finale presentation.
- Corridor arithmetic has no explicit consumed guard in the model, accepts zero
  division, and increments a separate cumulative score. Verify paired exclusivity,
  sane operands and exact once-only actual crowd changes, not just score movement.
- Old native harness explicitly expects RUN AGAIN, not next-level advancement;
  it never asserts each gate's exact crowd delta or visible unit consistency.

These are source-audit findings, not observed native failures. Phase1 export
44938365904 compiles b326846 only; all deeper fixes and acceptance remain pending.

## Phase1 native diagnostic — 19:33 UTC (full acceptance still FAIL/pending)

Run44939603351/export44938365904 sourceb326846 succeeded using actual native
start/drag/retry/positive-route/boss taps/replay/cold launch; best205 preserved.
Inspected all9 PNGs in rally-phase1-contact.png: original teal humanoid is now
visible on the real perspective track and moves laterally/forward. No pink shader
observed. However moving captures show only the initial one runner, not the grown
crowd. Win observed84 in telemetry, but no growth/clash/finale image was captured.
Gate labels crop at the screen edges/behind top HUD at distance; current course
is sparse. Result says RUN AGAIN, level remains0. These are NOT product approval.

Next source correction adds paired consumed guards before callbacks, raw gate
before/after telemetry, exact visible count telemetry and centralized HUD/model
count synchronization after gates/hazards/clashes. MiniBattle now filters real
runner contacts, engages once and stops when either side reaches zero, including
zero-enemy edge case. Recorder writes atomic timestamped snapshots. New source
must compile and be tested; redesigned escalating course/clash/finale and full
exact-arithmetic/native visual acceptance still required before release.

### Next native diagnostic harness — 19:41 UTC

The host now checks raw consumed gate before/after arithmetic, unchanged repeated
observations and one applied choice per row, requires at least two actual gates,
and compares actual spawned-unit count / formation / GameFlow count. Physics and
coroutine transitions get a bounded .5s raw convergence window; every mismatch is
retained and prolonged drift fails. A moving grown-crowd image is required. Clash
and finale-roll captures are attempted and explicitly reported missing if not
observed; this diagnostic does not claim full redesign acceptance or next levels.
Reuses Metro's proven no-input driver keepalive and longer initial installation
budget. Python syntax/diff checks pass; actual new native evidence still pending.
