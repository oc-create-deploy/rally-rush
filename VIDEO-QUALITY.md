# Video quality revision — Nov 22, 10:43 UTC user override

Alex rejected Rally Rush as ugly. Build 5 distribution remains a technical fact,
NOT current product acceptance. This revision is PRIORITY before remaining games.
No screenshot/contact sheet/sparse sampled frame sequence may satisfy video acceptance.

## Web references consulted
- Count Masters official developer listing: https://apps.apple.com/us/app/count-masters-crowd-runner-3d/id1568245971
  Gate choice, crowd growth, obstacle, rival clash, finish rhythm.
- Mob Control publisher description and media/trailer page: https://www.nintendo.com/us/store/products/mob-control-switch/
  Clear opposing teams, readable multiplication and tactical encounter variety.
- Official Steam trailer listing: https://store.steampowered.com/app/2736490/Mob_Control/
These are research references, not a claim that trailers were played or measured.
Do not copy proprietary assets or claim equivalent quality from descriptions.

## First source pass (not native verified)
- Turning pose derives from lateral velocity, clamped to 28 degrees, damped rather
  than per-frame drag pixels; steering displacement/collision envelope unchanged.
- Formation reshaping uses exponential settling; children already inherit root
  translation (the earlier suspected forward-drift diagnosis was incorrect).
- Camera follows in LateUpdate with exponential interpolation, not FixedUpdate.
- Reduce repeated islands 14 to 8 per side, lower their height, reduce clouds,
  soften sky and violet/cyan/lime palette. Preserve original logo direction.
- Native QA emits full, continuous 30fps review MP4 alongside original MOV and
  ffprobe metadata, with a full decode check. No elapsed-time cuts or speedup.

## Required acceptance, still OPEN
Watch actual continuous video at real elapsed time, not just extracted stills.
Compare player scale/silhouette, floor/rail/scenery hierarchy, gate readability
through approach, steering latency/overshoot, crowd settling, run/turn animation,
clash feedback, pause/resume, win/loss/retry pacing and finish clarity. Review
both small and large crowds, all seven earned courses and cold level selection.
Use runtime timing evidence for performance; 30fps encoding is NOT proof of 30fps
rendering. If this environment cannot play/review the video, explicitly block
visual acceptance; do not substitute sparse screenshots. Deliver the playable
video to Alex in Telegram. Continue actual gameplay changes based on review,
not just recolor until a build passes. Existing arithmetic, hazard, clash,
seven-earned/cold-save and background checks remain mandatory.
Only after honest video/gameplay acceptance merge, fresh device export, sign
unused build >=6 and independently verify ASC VALID + intended beta distribution.
