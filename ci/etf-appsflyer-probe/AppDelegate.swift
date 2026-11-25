import UIKit
import AppsFlyerLib

final class AppDelegate: UIResponder, UIApplicationDelegate, AppsFlyerLibDelegate {
  var window: UIWindow?
  var result: [String:Any] = ["environment":"ios-simulator", "realUserAttribution":false, "callback":"waiting"]
  var started = false
  let key = ProcessInfo.processInfo.environment["AF_DEV_KEY"] ?? ""
  func save() {
    let file = FileManager.default.urls(for:.documentDirectory,in:.userDomainMask)[0].appendingPathComponent("probe-result.json")
    if let data = try? JSONSerialization.data(withJSONObject:result, options:[.prettyPrinted,.sortedKeys]) { try? data.write(to:file,options:.atomic) }
  }
  func clean(_ s: String) -> String {
    let redacted = key.isEmpty ? s : s.replacingOccurrences(of:key,with:"[redacted]")
    return String(redacted.replacingOccurrences(of:"https?://[^\\s]+",with:"[URL]",options:.regularExpression).prefix(500))
  }
  func application(_ application:UIApplication,didFinishLaunchingWithOptions options:[UIApplication.LaunchOptionsKey:Any]?) -> Bool {
    guard !key.isEmpty else {result["error"]="Missing probe credential";save();return true}
    window=UIWindow(frame:UIScreen.main.bounds)
    let view=UIViewController();view.view.backgroundColor = .white;window?.rootViewController=view;window?.makeKeyAndVisible()
    let af=AppsFlyerLib.shared();af.appsFlyerDevKey=key;af.appleAppID="6782401897";af.delegate=self;af.isDebug=false
    af.disableAdvertisingIdentifier=true
    af.setSharingFilterForPartners(["all"])
    af.setConsentData(AppsFlyerConsent(isUserSubjectToGDPR:NSNumber(value:true),hasConsentForDataUsage:NSNumber(value:true),hasConsentForAdsPersonalization:NSNumber(value:false),hasConsentForAdStorage:NSNumber(value:false)))
    result["sdkVersion"]=af.getSDKVersion();result["configured"]=true;save()
    return true
  }
  func applicationDidBecomeActive(_ application:UIApplication) {
    guard !key.isEmpty && !started else{return};started=true
    AppsFlyerLib.shared().isStopped=false
    AppsFlyerLib.shared().start(completionHandler:{ response,error in
      self.result["launchResponseReceived"]=response != nil
      if let e=error as NSError? {self.result["launchError"]=["domain":e.domain,"code":e.code,"description":self.clean(e.localizedDescription)]}
      else {self.result["launchAccepted"]=true}
      self.save()
    })
  }
  func onConversionDataSuccess(_ info:[AnyHashable:Any]) {
    result["callback"]="success";result["afStatus"]=info["af_status"] as? String ?? "missing";result["firstLaunch"]=String(describing:info["is_first_launch"] ?? "missing");save()
  }
  func onConversionDataFail(_ error:Error) {
    let e=error as NSError
    result["callback"]="failure";result["conversionError"]=["domain":e.domain,"code":e.code,"description":clean(e.localizedDescription),"parseReason":clean(e.userInfo[NSDebugDescriptionErrorKey] as? String ?? "")];save()
  }
}
UIApplicationMain(CommandLine.argc, CommandLine.unsafeArgv,nil,NSStringFromClass(AppDelegate.self))
