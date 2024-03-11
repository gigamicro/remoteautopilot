using OWML.Common;//IModConfig
using OWML.ModHelper;//ModBehaviour
// using OWML.Utils;
// using System;
// using System.Collections;
// using System.Diagnostics;
using UnityEngine;//Vector3
using UnityEngine.InputSystem;//Keyboard
// rember ilspycmd "$(locate '*OuterWilds_Data/Managed/Assembly-CSharp.dll')" | less

namespace RemoteAutopilot
{
    public class RemoteAutopilot : ModBehaviour
    {
        bool doAlignment;
        bool doCollisionDisable;

        public override void Configure(IModConfig config)
        {
            doAlignment = config.GetSettingsValue<bool>("Align landing gear down while decelerating");
            doCollisionDisable = config.GetSettingsValue<bool>("Disable collision en route");
        }

        private ReferenceFrameTracker lockTracker = null;
        private OWRigidbody shipBody = null;
        bool active = false;

        public void OnFail()
        {
            if (active)
                shipBody?.GetComponent<Autopilot>().StartMatchVelocity(lockTracker?.GetReferenceFrame());
        }
        public void OnInit()
        {
            if (active && doCollisionDisable && Vector3.Distance((Vector3)shipBody?.transform.position, Locator.GetPlayerTransform().position) > 10)
                shipBody?.DisableCollisionDetection();
        }
        public void OnRetro()
        {
            if (active && doAlignment)
                shipBody?.GetComponent<AlignShipWithReferenceFrame>().OnEnterLandingMode(lockTracker?.GetReferenceFrame());
        }
        public void OnArrive(float arrivalError)
        {
            if (active) {
                shipBody?.EnableCollisionDetection();
                shipBody?.GetComponent<AlignShipWithReferenceFrame>().OnExitLandingMode();
                active = false;
            }
        }
        public void OnAbort() => OnArrive(1/0f);

        private void Update()
        {
            if (shipBody==null) {
                shipBody = Locator.GetShipBody();
                active=false;
                if (shipBody!=null) {
                    Autopilot ap = shipBody?.GetComponent<Autopilot>();
                    ap.OnAlreadyAtDestination   += OnFail;
                    ap.OnInitFlyToDestination   += OnInit;
                    ap.OnFireRetroRockets       += OnRetro;
                    ap.OnArriveAtDestination    += OnArrive;
                    ap.OnAbortAutopilot         += OnAbort;
                }
            }
            if (lockTracker==null && Locator.GetPlayerTransform()!=null)
                lockTracker = Locator.GetPlayerTransform().GetComponent<ReferenceFrameTracker>();

            if (Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                /*if (shipBody?.GetComponent<LandingPadManager>()._isLanded == true)
                {
                    state=AutonomyState.Leaving;
                    StartCoroutine(Timing(4, AutonomyState.AutoBegin));
                }
                else { state=AutonomyState.AutoBegin; }*/
                active=true;
                shipBody?.GetComponent<Autopilot>().FlyToDestination(lockTracker?.GetReferenceFrame());
            }

            /*
            if (active)
            {
                shipBody?.DisableCollisionDetection();
                RaycastHit hit;
                if (Physics.Raycast(shipBody?.transform.position, (lockTracker?.transform.position - shipBody?.transform.position).normalized, out hit, 10))
                {
                    if (Vector3.Distance(hit.transform.position, lockTracker?.transform.position) > 800)
                    {
                        ModHelper.Console.WriteLine("hit:"+hit.transform.name);
                        Vector3 vec = Vector3.Cross(
                            hit.transform.position-shipBody?.transform.position,
                            hit.transform.position-lockTracker?.transform.position);
                        ModHelper.Console.WriteLine("vec:"+vec);
                        ModHelper.Console.WriteLine("fin:"+// shipBody?.GetComponent<ThrusterModel>().AddTranslationalInput(
                            shipBody?.transform.InverseTransformPoint(vec
                            +shipBody?.transform.position).normalized
                        );
                    }
                }
                shipBody?.EnableCollisionDetection();
            } //*/

            // if (Keyboard.current.numpadPeriodKey.wasPressedThisFrame)
            //     lockTracker?.TargetReferenceFrame(Locator.GetPlayerTransform().GetComponent<OWRigidbody>().GetReferenceFrame());
            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .Sun            ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .TowerTwin      ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .CaveTwin       ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .TimberHearth   ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad4Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .BrittleHollow  ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad5Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .GiantsDeep     ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad6Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .DarkBramble    ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad7Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .Comet          ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad8Key.wasPressedThisFrame)
                lockTracker?.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .RingWorld      ).GetOWRigidbody().GetReferenceFrame());
        }
    }
}
