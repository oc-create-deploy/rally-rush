"""Drive normal native touch input; QA snapshots are read-only observations."""
import json, os, pathlib, subprocess, time
out = pathlib.Path('screenshots'); out.mkdir(exist_ok=True)
udid = os.environ['UDID']; bundle = os.environ['APP_BUNDLE_ID']
container = pathlib.Path(subprocess.check_output(['xcrun','simctl','get_app_container',udid,bundle,'data'],text=True).strip())
seq = 0
snapshot_path = None
import importlib.util
spec=importlib.util.spec_from_file_location('touchbridge',pathlib.Path(__file__).with_name('native-touch-bridge.py'))
module=importlib.util.module_from_spec(spec);spec.loader.exec_module(module)
bridge=module.TouchBridge(out,udid,bundle)

def state():
    global snapshot_path
    deadline = time.time()+90
    while time.time()<deadline:
        for p in ([snapshot_path] if snapshot_path is not None else container.rglob('qa-state.json')):
            try:
                s=json.loads(p.read_text())
                assert not s['errors'],s['errors']
                if s["frame"] >= 30:
                    if time.time()-s['utcSeconds']>=10:
                        # Hosted simulator/file I/O may briefly stall. Never
                        # use this stale model for an input/acceptance decision.
                        (out/'last-stale-observation.json').write_text(json.dumps(s,indent=2))
                        time.sleep(.2)
                        continue
                    snapshot_path=p
                    return s
            except (json.JSONDecodeError,FileNotFoundError): pass
        time.sleep(.5)
    probe_errors=list(container.rglob('qa-recorder-error.txt'))
    if probe_errors:
        detail=probe_errors[0].read_text()
        (out/'qa-recorder-error.txt').write_text(detail)
        raise AssertionError('Native diagnostic recorder failed: '+detail)
    raise AssertionError('Read-only native snapshot missing')

def capture(label):
    (out/(label+'.json')).write_text(json.dumps(state(),indent=2))
    subprocess.run(['xcrun','simctl','io',udid,'screenshot',str(out/(label+'.png'))],check=True)

def frozen_capture(label):
    # Real player pause, not a QA model write. Keep the course unobscured.
    tap('PAUSE')
    deadline=time.time()+5
    while val('GameFlowManager.playerPaused')!='True' and time.time()<deadline:time.sleep(.1)
    assert val('GameFlowManager.playerPaused')=='True','Actual player pause missing'
    before=state();capture(label);after=state()
    def values(s):return {v['key']:v['value'] for v in s['values']}
    b,a=values(before),values(after)
    assert a['GameFlowManager.playerPaused']=='True' and a['GameFlowManager.state']=='Pause'
    for key in ['RadialFormation.amount','ArmyMovementController.position']:
        assert a[key]==b[key],('Pause did not freeze gameplay',key,b[key],a[key])
    (out/(label+'-after.json')).write_text(json.dumps(after,indent=2))
    tap('RESUME')
    deadline=time.time()+5
    while val('GameFlowManager.playerPaused')=='True' and time.time()<deadline:time.sleep(.1)
    assert val('GameFlowManager.playerPaused')=='False','Actual resume missing'

def point(item):
    assert 0<item['x']<1 and 0<item['y']<1,item
    return f"{round(item['x']*100)}%, {round(item['y']*100)}%"

def swipe_point(item):
    return f"{item['x']*100:.4f}%, {item['y']*100:.4f}%"

def action(body):
    global seq
    seq+=1
    p=out/f'input-{seq:03}.yaml'
    p.write_text(f'appId: {bundle}\n---\n'+body+'\n')
    if bridge.proc is not None and body.startswith("- tapOn:"):
        bridge.tap_direct(body.split('"')[1])
    else: bridge.action(body)
    time.sleep(.1)

def tap(text):
    choices=[i for i in state()['items'] if i['kind']=='button' and i['interactable'] and text.casefold() in (i.get('text','').strip().casefold(),i['name'].strip().casefold())]
    assert choices,('Button missing',text)
    action('- tapOn:\n    point: "'+point(choices[0])+'"')

def val(key):return next(v['value'] for v in state()['values'] if v['key']==key)
# Start the native driver without starting the run; ignore harmless track touch.
action('- tapOn:\n    point: "90%, 40%"')
capture('01-startup')
start=[i for i in state()['items'] if i['kind']=='button' and i['interactable'] and any(t in (i['name']+' '+i['text']).casefold() for t in ['start','play'])]
assert start,('Native start control missing',[(i['name'],i['text']) for i in state()['items'] if i['kind']=='button'])
action('- tapOn:\n    point: "'+point(start[0])+'"')
start_deadline=time.time()+3
while val('GameFlowManager.state')=='Start' and time.time()<start_deadline:time.sleep(.1)
assert val('GameFlowManager.state')=='Game','Native start must be observed during active gameplay, not after a terminal popup'
before_position=val('ArmyMovementController.position')
action('- swipe:\n    start: "50%, 75%"\n    end: "75%, 75%"\n    duration: 3000')
capture('02-gameplay')
capture('03-steered')
def xyz(value):return tuple(float(x.strip()) for x in value.strip('()').split(','))
before=xyz(before_position);after=xyz(val('ArmyMovementController.position'))
assert abs(after[0]-before[0])>.05,('Native swipe did not steer the army',before,after)
assert after[2]>before[2],('Army did not advance',before,after)
# Observe the real run outcome without changing the model or forcing a win.
deadline=time.time()+240
while time.time()<deadline:
    buttons=[i for i in state()['items'] if i['kind']=='button' and i['interactable']]
    terminal=[i for i in buttons if any(t in (i['name']+' '+i['text']).casefold() for t in ['restart','retry','next level'])]
    if terminal:break
    time.sleep(2)
else:
    capture('failure-outcome');raise AssertionError('Native run did not reach an outcome')
capture('04-outcome')
restart=[i for i in terminal if any(t in (i['name']+' '+i['text']).casefold() for t in ['restart','retry'])]
assert restart,('Need real retry path review',terminal)
action('- tapOn:\n    point: "'+point(restart[0])+'"');time.sleep(4)
assert val('GameFlowManager.state')=='Start','Retry did not return to a fresh run'
capture('05-retry')
(out/'result.json').write_text(json.dumps({'nativeStart':True,'realSteeringGesture':True,'observedLateralMovement':True,'observedForwardMovement':True,'runOutcomeObserved':True,'retry':True,'winAndProgressPersistence':'PENDING'},indent=2))
# A second real run chooses beneficial visible gate operations, then taps the boss.
# Reads model/geometry only; every change remains a normal native touch.
tap('StartButton')
start_deadline=time.time()+3
while val('GameFlowManager.state')=='Start' and time.time()<start_deadline:time.sleep(.1)
route=[];boss_taps=0;deadline=time.time()+240
gate_history={};captured_growth=False;captured_clash=False;captured_finale=False;consistent_samples=0
count_mismatch_since=None
count_mismatches=[]

def audit_crowd(snapshot, values):
    global consistent_samples,count_mismatch_since
    amount=int(values['RadialFormation.amount'])
    counts=(amount,int(values['GameFlowManager.playerCount']),int(values['ArmyController.VisibleUnitCount']))
    if len(set(counts))!=1:
        # Physics contacts/coroutines can change amount after that frame's
        # formation FixedUpdate. Require raw convergence within .5 seconds,
        # retaining every mismatch rather than accepting stale visible counts.
        if count_mismatch_since is None:count_mismatch_since=snapshot['utcSeconds']
        count_mismatches.append({'frame':snapshot['frame'],'utcSeconds':snapshot['utcSeconds'],'counts':counts})
        (out/'count-transitions.json').write_text(json.dumps(count_mismatches,indent=2))
        assert snapshot['utcSeconds']-count_mismatch_since<=.5,('Persistent visible/HUD/model count drift',counts)
    else:
        count_mismatch_since=None;consistent_samples+=1
    for gate in snapshot['items']:
        if gate['kind']!='gate' or not gate.get('consumed') or gate.get('before',0)<=0:continue
        op=gate['text'];n=int(op[1:]);before=gate['before']
        expected=before+n if op[0]=='+' else before-n if op[0]=='-' else before*n if op[0]=='x' else before//n
        assert gate['after']==max(0,expected),('Actual gate arithmetic mismatch',gate,expected)
        previous=gate_history.get(str(gate['id']))
        proof={'operation':op,'before':before,'after':gate['after'],'rowZ':gate['shape'][0]['y']}
        assert previous is None or previous==proof,('Gate applied repeatedly',previous,proof)
        gate_history[str(gate['id'])]=proof
    rows={}
    for proof in gate_history.values():
        row=round(proof['rowZ'],1);rows[row]=rows.get(row,0)+1
    assert all(n==1 for n in rows.values()),('Both paired gates applied',gate_history)
    (out/'gate-arithmetic.json').write_text(json.dumps(gate_history,indent=2))
while time.time()<deadline:
    snapshot=state();values={v['key']:v['value'] for v in snapshot['values']}
    audit_crowd(snapshot,values)
    if not captured_growth and count_mismatch_since is None and int(values['RadialFormation.amount'])>1 and values.get('GameFlowManager.state')=='Game':
        frozen_capture('09-grown-crowd-paused');captured_growth=True;continue
    if not captured_clash and values.get('GameFlowManager.state')=='MiniBattle':
        frozen_capture('10-clash-paused');captured_clash=True;continue
    if not captured_finale and values.get('BattleController.finaleStage')=='CrowdRelay':
        assert int(values['BattleController.finaleReward'])==int(values['BattleController.finaleCrowd'])*int(values['BattleController.finaleMultiplier'])
        frozen_capture('11-crowd-relay-paused');captured_finale=True;continue
    buttons=[i for i in snapshot['items'] if i['kind']=='button' and i['interactable']]
    if any(i.get('text')=='NEXT LEVEL' for i in buttons):
        time.sleep(.6) # Let the actual win popup animation finish before capture/input.
        break
    if any(i['name']=='Restart' for i in buttons):
        capture('failure-winning-route');raise AssertionError(('Positive-gate route lost',route,values))
    if values.get('BattleController.canStart')=='True':
        push=[i for i in buttons if 'TAP TO PUSH' in i.get('text','')]
        assert push,('Boss touch control missing',buttons)
        bridge.tap_direct(point(push[0]));boss_taps+=1;time.sleep(.1);continue
    if values.get('GameFlowManager.state')=='Game':
        x,_,z=xyz(values['ArmyMovementController.position'])
        gates=[i for i in snapshot['items'] if i['kind']=='gate' and i['interactable'] and i['shape'] and i['shape'][0]['y']>z+2]
        if gates:
            front=min(i['shape'][0]['y'] for i in gates)
            choices=[i for i in gates if abs(i['shape'][0]['y']-front)<2]
            amount=int(values['RadialFormation.amount'])
            def effect(gate):
                label=gate['text'];n=int(label[1:])
                return amount+n if label[0]=='+' else amount-n if label[0]=='-' else amount*n if label[0]=='x' else amount//max(n,1)
            gate=max(choices,key=effect);target=max(float(values.get('ArmyMovementController.minX',-2.2)),min(float(values.get('ArmyMovementController.maxX',2.2)),gate['shape'][0]['x']-.28));dx=target-x
            if abs(dx)>.35 and front-z<28:
                start_x=80 if dx<0 else 20
                end_x=max(2,min(98,start_x+dx/7.5*100))
                route.append({'atZ':z,'gateZ':front,'operation':gate['text'],'fromX':x,'targetX':target})
                bridge.swipe(f'{start_x}%, 70%',f'{end_x}%, 70%')
    time.sleep(.15)
else:
    capture('failure-win-timeout');raise AssertionError(('Real winning route timed out',route))
assert len(gate_history)>=2,('Need at least two actually applied gates',gate_history)
assert captured_growth,'Native moving grown crowd capture missing'
(out/'phase2-diagnostic.json').write_text(json.dumps({'gateArithmetic':gate_history,'consistentCountSamples':consistent_samples,'growthCapture':captured_growth,'clashCapture':captured_clash,'finaleCapture':captured_finale,'fullRedesignAcceptance':False},indent=2))
(out/'route.json').write_text(json.dumps(route,indent=2))
capture('06-win')
won_level=int(val('LevelController.currentLevel'))
won_best=int(val('ScoreManager.bestScore'))
assert won_best>0, 'Actual win must record a positive best score'
tap('NEXT LEVEL');time.sleep(4)
assert val('GameFlowManager.state')=='Start','Next level start failed'
assert int(val('LevelController.currentLevel'))==won_level+1,'Earned next level did not advance'
capture('07-win-replay')
result=json.loads((out/'result.json').read_text());result.update({'realPositiveGateRunWon':True,'bossNativeTouches':boss_taps,'winReplay':True,'winAndProgressPersistence':'PERSISTENCE_PENDING'})
(out/'result.json').write_text(json.dumps(result,indent=2))

subprocess.run(['xcrun','simctl','terminate',udid,bundle],check=True)
for snapshot_path in container.rglob('qa-state.json'):snapshot_path.unlink()
subprocess.run(['xcrun','simctl','launch',udid,bundle],check=True)
# Unity's persistent-data subpath can be recreated on cold launch. Do not
# pin discovery to the pre-termination recorder location.
container = pathlib.Path(subprocess.check_output(['xcrun','simctl','get_app_container',udid,bundle,'data'],text=True).strip())
snapshot_path = None
time.sleep(4)
assert int(val('ScoreManager.bestScore'))==won_best,'Earned best score did not survive cold launch'
assert val('GameFlowManager.state')=='Start','Cold launch did not restore a playable start'
assert int(val('LevelController.currentLevel'))==won_level+1,'Earned level did not survive cold launch'
capture('08-cold-restored')
result.update({'earnedBestScore':won_best,'earnedBestScoreColdLaunchVerified':True,'winAndProgressPersistence':True})
(out/'result.json').write_text(json.dumps(result,indent=2))
