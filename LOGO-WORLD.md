# Logo-led world revision — 2026-11-22

Alex requested models, scenes and better level design matching the actual shipped
Assets/Brand/RallyRush-AppIcon.png. This reopens prior product acceptance.

Icon audit: round, single-color orange/blue/lime/violet/gold toy runners; white
floating track; violet rails and islands; cyan sky/waterfalls; lime/cyan math
portals; candy obstacles; checkered golden finish and crown. The previous angular
vest/skin/headband crowd and plain course do not deliver that promise.

Implemented on feat/logo-world-2026-11-22:
- Rounded capsule bodies/limbs and large spherical heads, shared five-color glossy
  palette, existing opposing limb animation, unchanged gameplay colliders.
- Rounded red rivals preserve team readability instead of hiding enemies in rainbow.
- Existing physical deck repainted ivory and walls violet; no fake track surface.
- Gate portal frames derive from real collider bounds; operation labels retained.
- Off-course stepped floating islands, cloud puffs, cyan waterfall ribbons, blue
  distance fog and a crowned checkered finish at the actual finale approach.
- Studded candy breakaway bumper retains exact one-runner, single-contact cost.
- Seven-step progression: existing three math lessons, one consolidation lap,
  then three extra routes with changed choice spacing and arithmetic. New routes
  build/add vs multiply, compare division vs growth, then high-growth vs attrition.
  Rival pressure and speed increase with a cap on speed. Geometry remains bounded
  and steering/collider semantics remain intact. Curving playable tracks and
  sweeping rollers are NOT implemented: both need proper input/collision work,
  not decorative geometry pretending to be gameplay.

Reference: Freeplay's Count Masters official store description supports the
choose/grow/obstacle/clash/finish rhythm, not asset copying:
https://apps.apple.com/us/app/count-masters-crowd-runner-3d/id1568245971
“Real war” is ambiguous; no unverified specific title is claimed as studied.

Acceptance is OPEN. Diff/static checks are not Unity compilation or native proof.
Require fresh simulator export; inspect first-run, moving rainbow silhouettes,
portal legibility, deck/rail height and scenery occlusion, crowned finish, all
seven earned levels, exact arithmetic/bumper/clash, pause/resume/cold progress.
Native driver now requires all seven earned courses and cold level7; this
extension has only passed Python compilation, not native execution. Only after honest visual and
functional acceptance merge/device export/sign an unused build >=5, then verify
ASC VALID and actual intended tester/group/build eligible beta separately.
Build4 remains last delivered version; do not claim this art revision is there.

Native review 44982642738: all 34 original PNGs inspected via three contact sheets; seven earned courses and fresh cold internal level7 (HUD LEVEL8) recorded, division21/3=7 preserved. Rainbow rounded crowd and palette render. NOT accepted: white decorative gate header cuts math text, and near-camera crown/checkered lintel masks relay crowd. Removed opaque gate header and hide finish decoration within18 world units of finish (approach landmark retained; physics unchanged). Requires NEW export/native proof and actual motion review; static checks alone insufficient.

Native review 44989516127: background setup/return/resume passed; 34 original PNGs inspected. Gate labels still depth-occluded (upper half of /3 and x4 absent in original 09-grown-crowd-paused.png), despite removing decorative header. Product acceptance remains OPEN. Move billboard wholly in front of collider front face and reduce font12->8 to fit panel. Requires new export/native original-image and motion proof; no release yet.

Native review44992761716 SUCCESS functional checks; all34 original PNGs inspected. Original09 confirms lower halves of operation glyphs still hidden at gate/deck bottom despite forward offset. Raise entire billboard above collider top by1.5 worldunits, retaining front offset and worldscale. Product acceptance remains OPEN pending fresh export/native captures and actualmotion.

Native44995781698 failed fresh cold snapshot after first earned win (90s deadline); final original image shows LEVEL2 Start, not proof of lost save. Extend read-only observation wait to300s retaining timestamp/freshness/errors/persistence assertions. Original09 shows no consumed-gate labels: source used Collider.bounds after OnTriggerExit disabled that collider (Unity empty bounds), collapsing labels to origin. Use retained visible MeshRenderer.bounds for billboard anchoring. NOT accepted; needs new export/native images+motion and full seven/cold7/background proof.
