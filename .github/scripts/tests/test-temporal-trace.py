"""Exercise fresh, duplicate, stale, missing and recovered native observations."""
import importlib.util
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import time
from types import SimpleNamespace

if len(sys.argv) > 1:
    root = Path(sys.argv[1])
    spec = importlib.util.spec_from_file_location('trace', Path(__file__).parents[1] / 'native-temporal-trace.py')
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    path = root / 'state.json'
    def write(frame, utc=None):
        path.write_text(json.dumps(dict(frame=frame, utcSeconds=utc or time.time(), values=[], errors=[])))
    write(1)
    bridge = SimpleNamespace(swipe=lambda: time.sleep(.3), tap_direct=lambda: None, action=lambda: None)
    emit = module.install(root, lambda: path, bridge)
    time.sleep(.2)
    bridge.swipe()
    time.sleep(2.1)  # Duplicate must not become a second observation.
    write(2, time.time()-20)  # New frame but stale: must not be accepted.
    time.sleep(.2)
    path.unlink()
    time.sleep(.2)
    write(3)
    time.sleep(.2)
else:
    with tempfile.TemporaryDirectory() as directory:
        subprocess.run([sys.executable, __file__, directory], check=True)
        rows = [json.loads(line) for line in (Path(directory)/'native-temporal-trace.jsonl').read_text().splitlines()]
        assert [r['frame'] for r in rows if r['kind']=='observation'] == [1, 3], rows
        assert any(r['kind']=='coverage-gap' for r in rows)
        assert any(r['kind']=='coverage-gap-end' for r in rows)
        assert [r['kind'] for r in rows if r['kind'].startswith('input-')] == ['input-start', 'input-end']
        assert rows[-1]['kind']=='trace-end'
    print('PASS: freshness, deduplication, missing-file recovery, explicit gaps and input tracing')
