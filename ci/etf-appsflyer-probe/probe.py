import os,sys,json,subprocess,urllib.request,hashlib,zipfile,pathlib,plistlib,time,platform

def run(args,**kw):return subprocess.run(args,check=True,**kw)
version=os.environ['SDK_VERSION']
if not __import__('re').fullmatch(r'\d+\.\d+\.\d+',version):raise SystemExit('Invalid SDK version')
root=pathlib.Path.cwd(); work=root/'probe-work';work.mkdir(exist_ok=True)
spec=json.load(urllib.request.urlopen('https://trunk.cocoapods.org/api/v1/pods/AppsFlyerFramework/specs/'+version))
url=spec['source']['http'];archive=work/'sdk.zip';urllib.request.urlretrieve(url,archive)
if spec['source'].get('sha256') and hashlib.sha256(archive.read_bytes()).hexdigest()!=spec['source']['sha256']:raise SystemExit('SDK integrity check failed')
with zipfile.ZipFile(archive) as z:z.extractall(work/'sdk')
framework=next((work/'sdk').glob('**/xcframework/full/AppsFlyerLib.xcframework'))
with (framework/'Info.plist').open('rb') as f:slices=plistlib.load(f)['AvailableLibraries']
arch='arm64' if platform.machine()=='arm64' else 'x86_64'
slice=next(s for s in slices if s['SupportedPlatform']=='ios' and s.get('SupportedPlatformVariant')=='simulator' and arch in s['SupportedArchitectures'])
framework_dir=framework/slice['LibraryIdentifier']
app=work/'AttributionProbe.app';app.mkdir(exist_ok=True)
sdk=subprocess.check_output(['xcrun','--sdk','iphonesimulator','--show-sdk-path'],text=True).strip()
command=['xcrun','--sdk','iphonesimulator','swiftc','-sdk',sdk,'-target',arch+'-apple-ios18.0-simulator','-F',str(framework_dir),'-framework','AppsFlyerLib','-framework','UIKit','-framework','StoreKit','-framework','AdSupport','-framework','AppTrackingTransparency','-framework','SystemConfiguration','-framework','Security','-framework','WebKit','-Xlinker','-ObjC','-lz','-lc++',str(root/'ci/etf-appsflyer-probe/AppDelegate.swift'),'-o',str(app/'AttributionProbe')]
if int(version.split('.')[0])>=7:command.extend(['-D','AF_SDK_V7'])
run(command)
with (app/'Info.plist').open('wb') as f:plistlib.dump({'CFBundleIdentifier':'com.etf.radar','CFBundleExecutable':'AttributionProbe','CFBundleName':'AttributionProbe','CFBundlePackageType':'APPL','CFBundleShortVersionString':'0.0.1','CFBundleVersion':'1','MinimumOSVersion':'18.0','LSRequiresIPhoneOS':True,'UIDeviceFamily':[1],'UILaunchScreen':{}},f)
run(['codesign','--force','--sign','-',str(app)])
devices=json.loads(subprocess.check_output(['xcrun','simctl','list','devices','available','--json']))['devices']
choices=[d for runtime,ds in devices.items() if 'iOS' in runtime for d in ds if d['name'].startswith('iPhone')]
if not choices:raise SystemExit('No iPhone simulator')
udid=choices[0]['udid']
run(['xcrun','simctl','boot',udid]);run(['xcrun','simctl','bootstatus',udid,'-b'])
run(['xcrun','simctl','install',udid,str(app)])
env=os.environ.copy();env['SIMCTL_CHILD_AF_DEV_KEY']=env['AF_DEV_KEY']
run(['xcrun','simctl','launch',udid,'com.etf.radar'],env=env)
container=pathlib.Path(subprocess.check_output(['xcrun','simctl','get_app_container',udid,'com.etf.radar','data'],text=True).strip())
file=container/'Documents/probe-result.json'
for _ in range(90):
 if file.exists():
  result=json.loads(file.read_text())
  if result.get('callback') in ['success','failure']:break
 time.sleep(1)
else:result=json.loads(file.read_text()) if file.exists() else {'error':'No probe result'}
result['requestedSDKVersion']=version
# A simulator callback is diagnostic only; no backend production row is created.
result['realUserAttribution']=False
result['productionBackendWritten']=False
out=root/'safe-results';out.mkdir(exist_ok=True)
(out/'result.json').write_text(json.dumps(result,indent=2))
print(json.dumps(result,indent=2))
