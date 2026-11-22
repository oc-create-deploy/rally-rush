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

### Phase2 diagnostic failed — 20:05 UTC
Run44942950535 sourcebca1f9e/hostfbee5bb: raw +20 1->21 and x4 21->84,
actual spawned/model counts84 agree. Native positive run then lost at boss.
The supposed moving-grown image is **RUN ENDED**, not crowd evidence: saved
Game/21 JSON at19:59:29.776 precedes screenshot invocation20:00:48 by~79s.
Exact blocking/stall cause is unproven. Both images inspected in
rally-phase2-failure.png. No acceptance and no blind retry. Add real native
pause/frozen readable capture with fresh pre/post image state and retain actual
moving video; complete product course/clash/finale/progression/pause corrections.

20:12 source iteration: dedicated unobscured PAUSE/RESUME button uses real player
input and freezes Time.timeScale, including battle timers/clash coroutines; keeps
the original Game/MiniBattle/Battle state on resume without restarting the battle.
Backgrounding pauses, return stays paused. Read-only playerPaused telemetry and
native frozen-crowd capture now require actual pause, stable count/position across
screenshot and actual resume; image explicitly labelled paused, not moving.
Compile/native verification pending, full product redesign still pending.

20:41 native44946132951: actual pause/resume and visible21-person crowd now
verified in unobscured paused image, raw position/count stable across freshframes
3926->4014. +20 1->21 and x4 21->84;76 consistent count samples. Native win205
and replay passed. Aggregate FAIL at cold recorder discovery (snapshot missing),
not proven lost score; final image shows playable start. Host now invalidates
cached recorder path and re-resolves container on cold launch (prior noncached
harness cold passed). Must prove, not assume. Runtime log also has DOTween safe
mode missing/null target warnings on scene replay; fix lifecycle before final
acceptance. Clash/finale captures absent; level remains0 and gate text still clips.
Archive44946132951.zip CRC checked; all9PNG inspected rally-phase3-contact.png.
No full product acceptance/release.

21:04 diagnostic44947738339 SUCCESS: cold score205 verified after path rediscovery,
80 count-consistent samples, gate1->21->84, real pause/resume and visible21 crowd.
All10PNG inspected: first moving-labelled images actually show RUN ENDED (capture
delay), so not moving-visual acceptance. Replay image has simulator notification;
not a store screenshot. Clash/finale remain absent. Complete artifact CRC checked
in memory; original PNG/JSON/logs retained locally; large video retained in GitHub
artifact14724937312 (not copied locally due disk headroom).

Next product source iteration: three authored math-choice variants (+20/-10 then
/3/x4; +12/x2 then -15/x3; x3/+8 then /2/+15), modest escalating forward speed,
earned NEXT LEVEL advances saved level instead of clamping to0. Reuses course
geometry; NOT yet complete redesigned course/clash/finale. Camera keeps both lanes
centered during steering. Scene teardown kills old tweens to address null-target
warnings. Native harness now requires actual level+1 and cold-level persistence.
Syntax/diff checks pass; new Unity/native compilation and visual proof pending.

21:39 native44951269887 FAIL at initial real start: recorderframe5029 older
than10s rejected immediately after button input. Startup image inspected centered
track/both choices; final RUN ENDED. Actual decrease-gate log shows app progressed,
so no new crash proven. Host waits within existing90s observation budget for a
FRESH (<10s) snapshot, preserving stale raw JSON, never using stale state for
input/acceptance. New stages/next-level/cold-level still unproven. No weakened
gameplay gate or full product claim. Artifact14726295791 CRC checked in memory;
original PNG/JSON/logs retained locally, video retained in GitHub artifact.

22:07 diagnostic44952775563 SUCCESS: actual NEXT LEVEL0->1, different +12/x2
then -15/x3 labels, cold earnedlevel1/best205 verified.90 countconsistent samples,
real pause/resume visible21 crowd. All10PNG inspected rally-phase6-contact.png;
02 moving image clips runner/gates,03 already RUNENDED, so no full moving-visual
acceptance. Paused image still clips right edge of crowd. No missing-target tween
warning found in this runtime log. Clash/finale absent; full product pending.
Artifact14727232095 CRC verified in memory; original PNG/JSON/logs retained locally,
video retained remotely, avoiding duplicate large movie with low disk headroom.

New source iteration inserts a real full-width encounter at z82: original red
stylized humanoid rivals (6/12/18 by stage), labelled remaining enemy count,
visible individuals removed as actual paired attrition reduces both armies.
Existing guarded MiniBattle drives actual count/loss/survival and real pause
freezes it; optional particle handling supports this encounter without imported
cat effects. New native capture explicitly pauses the clash and labels it so.
Compile/native visuals/exact clash outcomes UNPROVEN. Finale remains old tap/roll
and still needs product correction, plus course dressing/moving capture/background.

22:43 native44956318336 SUCCESS, but NOT visual acceptance. Raw clash paused80
hero/2enemy stable before/after, result205 and next/coldlevel pass. All12PNG
inspected rally-phase7-contact.png: rivals are not readable red humans (ghostlike
white fragments); HUD105 is cumulative gate score, NOT actual80 crowd; right
crowd still clips. Finale-labelled image is already result, not roll proof.
Artifact14727874183 CRC checked in memory, original PNG/JSON/logs locally retained,
video remote. Do not mistake diagnostic booleans for visual proof.

Next source: bind rival and finale materials to actual runner prefab material
(not arbitrary scene MeshRenderer which may use text-atlas shader); visible top
HUD now shows actual crowd. Replace old unrelated boss/tap/ball finish with a
crowd relay: actual surviving army moves to original reward steps, multiplier
1..5 derives from surviving count, earned reward=count*multiplier saved in score,
readable world reward banner. Real pause freezes relay. Recorder exposes raw
relay count/multiplier/reward/stage; host checks arithmetic and captures real
paused relay. Compilation/native visuals/outcomes unproven. Full crowd bounds,
course dressing, moving capture, background and acceptance remain pending.

23:13 native44959314107 SUCCESS, all12PNG inspected rally-phase8-contact.png.
Red rivals now visibly humanoid; actual hero81/rivals3 paused stable, then
surviving78 runs relay x4=312; total417, realnextlevel/coldlevel/best persistence.
Actual paused relay banner/steps/army readable; no old ball/boss taps(0).
HUD81 matches actual crowd. Still right-edge clipped crowd and first moving
images clip hero/near gates, so NOT full visual/product acceptance. Artifact
14729088201 CRC checked in memory; original images/JSON/logs local/video remote.

New source constrains army center by actual formation half-width, with minimum
steering span for choices, exposes actual min/max to native route planner;
gate-side selection now uses player army-center x, not arbitrary first follower
exit across a paired gate. Camera widens/backtracks smoothly for full crowd.
New native proof required; no claim bounds fixed before inspection. Remaining:
course dressing, both math choices/division, complete stage playthroughs, hazards
and actual background/resume, moving visuals/full acceptance before release.

23:48 diagnostic44961814147 SUCCESS, all12PNG inspected phase9 contact. Crowd
now fits in growth/relay images, near gate choices legible at paused growth;
initial runner smaller, far gates hidden under HUD; moving-labelled images still
show RUNENDED. Clash-labelled image actually enemy0/hero78 (pause too late), so
NOT clash-capture acceptance despite diagnostic boolean. Relay/reward417 and
next/coldlevel pass. Artifact14730457903 CRC checked in memory; original PNG/JSON/
logs retained locally, large video retained in GitHub due tight disk.

Clash attrition now .6s per pair (3.6..10.8s authored encounter) for readable
player experience; native frozen capture requires actual remaining enemy>0
and stable before/after count, relay requires actual CrowRelay stage. Expanded
native path earns levels1/2/3, verifies every gate/clash/relay, actual division
21/3=7 route, earns level4 and cold-restores it. Then real paused Safari warmup
and genuine running-background/fresh paused return/stable position/manual resume
checks. No model writes. New compile/native proof required; still not full
product approval until rendered/moving evidence/course/hazards review passes.

Nov22 01:04: native44967897521 FAIL at level1 relay pause observation, after
level0 gate/clash/relay/win/next/cold and level1 gate/clash38hero/11enemy passed.
All13PNG inspected phase10 contact: final image actually shows paused relay
27x2=54 and RESUME, so missing immediate host flag != proof pause failed.
Extra pre-input state read removed; actual observed pause-button used directly.
Pause/resume proof now requires one fresh actual snapshot within20s bounded
host observation budget (not repeated separate val reads/5s), retains frame/UTC
request metadata. Enemy>0/relay-stage/stable count+position and real input gates
unchanged. Fullstage2/3/division/background not reached; no product acceptance.
Artifact14732414720 CRC checked in memory; original PNG/JSON/logs local/video
remote. New host proof pending, no source rebuild required.
