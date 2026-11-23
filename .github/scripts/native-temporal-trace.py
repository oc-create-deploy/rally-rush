"""Observe existing native recorder continuously; never modify game state."""
import atexit, json, threading, time

def install(out, get_path, bridge, minimum_utc=lambda: 0):
    lock = threading.Lock()
    stop = threading.Event()
    stream = (out / 'native-temporal-trace.jsonl').open('w', buffering=1)
    def emit(event):
        with lock:
            stream.write(json.dumps(dict(hostUtc=time.time(), **event)) + '\n')
    def observe():
        previous = None
        last_fresh = time.monotonic()
        gap_started = None
        reason = 'no-path'
        last_report = 0
        while not stop.wait(.1):
            path = get_path()
            try:
                if path is None:
                    reason = 'no-path'
                else:
                    state = json.loads(path.read_text())
                    identity = (state['utcSeconds'], state['frame'])
                    age = time.time() - state['utcSeconds']
                    if state['utcSeconds'] < minimum_utc():
                        reason = 'previous-session'
                    elif age > 2:
                        reason = 'stale-snapshot'
                    elif identity == previous:
                        reason = 'unchanged-snapshot'
                    else:
                        now = time.monotonic()
                        if gap_started is not None:
                            emit(dict(kind='coverage-gap-end', durationSeconds=now-gap_started))
                            gap_started = None
                        previous = identity
                        last_fresh = now
                        emit(dict(kind='observation', frame=state['frame'], nativeUtc=state['utcSeconds'],
                                  ageSeconds=age, values=state['values'], errors=state['errors']))
                        continue
            except (OSError, ValueError, KeyError) as error:
                reason = type(error).__name__
            now = time.monotonic()
            if now - last_fresh >= 2 and (gap_started is None or now-last_report >= 2):
                if gap_started is None:
                    gap_started = last_fresh
                emit(dict(kind='coverage-gap', reason=reason, durationSeconds=now-gap_started))
                last_report = now
    for name in ('swipe', 'tap_direct', 'action'):
        original = getattr(bridge, name)
        def wrapped(*args, _original=original, _name=name, **kwargs):
            emit(dict(kind='input-start', method=_name, args=args))
            try:
                result = _original(*args, **kwargs)
            except Exception as error:
                emit(dict(kind='input-error', method=_name, error=str(error)))
                raise
            emit(dict(kind='input-end', method=_name))
            return result
        setattr(bridge, name, wrapped)
    worker = threading.Thread(target=observe, daemon=True)
    worker.start()
    def close():
        stop.set()
        worker.join(timeout=2)
        emit(dict(kind='trace-end', note='Recorder cadence ~350ms; not render FPS or visual acceptance'))
        stream.close()
    atexit.register(close)
    return emit
