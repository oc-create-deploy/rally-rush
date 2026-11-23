"""Observe existing native recorder continuously; never modify game state."""
import atexit, json, threading, time

def install(out, get_path, bridge):
    lock = threading.Lock()
    stop = threading.Event()
    stream = (out / 'native-temporal-trace.jsonl').open('w', buffering=1)
    def emit(event):
        with lock:
            stream.write(json.dumps(dict(hostUtc=time.time(), **event)) + '\n')
    def observe():
        previous = None
        while not stop.wait(.1):
            path = get_path()
            if path is None:
                continue
            try:
                state = json.loads(path.read_text())
                identity = (state['utcSeconds'], state['frame'])
                if identity == previous:
                    continue
                previous = identity
                emit(dict(kind='observation', frame=state['frame'], nativeUtc=state['utcSeconds'],
                          values=state['values'], errors=state['errors']))
            except (OSError, ValueError, KeyError):
                continue  # Atomic-file replacement can temporarily hide a snapshot.
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
