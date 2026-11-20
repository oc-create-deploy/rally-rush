"""Drive normal native touch input; QA snapshots are read-only observations."""
import json, os, pathlib, subprocess, time
out = pathlib.Path('screenshots'); out.mkdir(exist_ok=True)
udid = os.environ['UDID']; bundle = os.environ['APP_BUNDLE_ID']
container = pathlib.Path(subprocess.check_output(['xcrun','simctl','get_app_container',udid,bundle,'data'],text=True).strip())
seq = 0
import importlib.util
spec=importlib.util.spec_from_file_location('touchbridge',pathlib.Path(__file__).with_name('native-touch-bridge.py'))
module=importlib.util.module_from_spec(spec);spec.loader.exec_module(module)
bridge=module.TouchBridge(out,udid,bundle)

def state():
    deadline = time.time()+90
    while time.time()<deadline:
        for p in container.rglob('qa-state.json'):
            try:
                s=json.loads(p.read_text())
                assert not s['errors'],s['errors']
                if s["frame"] >= 30: return s
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
