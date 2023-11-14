using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
// rember ilspycmd "$(locate '*OuterWilds_Data/Managed/Assembly-CSharp.dll')" | less

namespace ShipRemoteAutopilot
{
    public class ShipRemoteAutopilot : ModBehaviour
    {
        ReferenceFrameTracker lockTracker = null;
        OWRigidbody shipBody = null;
        // enum AutonomyState
        // {
        //     Inactive,
        //     // Leaving,
        //     // AutoBegin,
        //     Auto,
        //     // AutoEnd,
        //     // Aligning,
        //     // Landing
        // }
        // private AutonomyState _state = AutonomyState.Inactive;
        // AutonomyState state {
        //     get { return _state; }
        //     set {
        //         if (_state==AutonomyState.Landing) shipBody.GetComponent<AlignShipWithReferenceFrame>().OnExitLandingMode();
        //         if (_state==AutonomyState.Auto) shipBody.EnableCollisionDetection();
        //         _state=value;
        //         ModHelper.Console.WriteLine("state=" + _state);
        //     }
        // }
        // AutonomyState state = AutonomyState.Inactive;
        bool active = false;

        // IEnumerator Timing(int seconds, AutonomyState toset)
        // {
        //     ModHelper.Console.WriteLine("state=" + toset + " in " + seconds + "s");
        //     WaitForSeconds wait = new WaitForSeconds(seconds);
        //     yield return wait;
        //     state=toset;
        // }

        public void OnFail()
        {
            if (active)
                shipBody.GetComponent<Autopilot>().StartMatchVelocity(lockTracker.GetReferenceFrame());
        }
        public void OnInit()
        {
            if (active && Vector3.Distance(shipBody.transform.position, Locator.GetPlayerTransform().position) > 10)
                shipBody.DisableCollisionDetection();
        }
        public void OnRetro()
        {
            if (active)
                shipBody.GetComponent<AlignShipWithReferenceFrame>().OnEnterLandingMode(lockTracker.GetReferenceFrame());
                // state=AutonomyState.Aligning;
        }
        public void OnArrive(float arrivalError)
        {
            if (active)
                shipBody.EnableCollisionDetection();
                // state=AutonomyState.AutoEnd;
            active = false;
        }
        public void OnAbort()
        {
            // state=AutonomyState.Inactive
            shipBody.EnableCollisionDetection();
            active=false;
        }

        private void Update()
        {
            if (shipBody==null) {
                shipBody = Locator.GetShipBody();
                // state = AutonomyState.Inactive;
                active=false;
                if (shipBody!=null) {
                    Autopilot ap = shipBody.GetComponent<Autopilot>();
                    ap.OnAlreadyAtDestination   += OnFail;
                    ap.OnInitFlyToDestination   += OnInit;
                    ap.OnFireRetroRockets       += OnRetro;
                    ap.OnArriveAtDestination    += OnArrive;
                    ap.OnAbortAutopilot         += OnAbort;
                }
            }
            if (lockTracker==null && Locator.GetPlayerTransform()!=null)
                lockTracker = Locator.GetPlayerTransform().GetComponent<ReferenceFrameTracker>();
            // if (shipBody==null || lockTracker==null) return;
            if (lockTracker==null) return;

            if (Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                /*if (shipBody.GetComponent<LandingPadManager>()._isLanded == true)
                {
                    state=AutonomyState.Leaving;
                    StartCoroutine(Timing(4, AutonomyState.AutoBegin));
                }
                else { state=AutonomyState.AutoBegin; }*/
                // state=AutonomyState.AutoBegin;
                // state=AutonomyState.Auto;
                active=true;
                shipBody.GetComponent<Autopilot>().FlyToDestination(lockTracker.GetReferenceFrame());
            }

            // if (state==AutonomyState.Leaving)
            // {
            //     shipBody.GetComponent<ThrusterModel>().AddTranslationalInput(Vector3.up);
            // }
            // if (activeBegin)
            // {
            //     // ModHelper.Console.WriteLine("AutoBegin");
            //     state=AutonomyState.Auto;
            //     // shipBody.GetComponent<Autopilot>().FlyToDestination(lockTracker.GetReferenceFrame());
            //     // isSunAvoided = false;
            //     if (Vector3.Distance(shipBody.transform.position, Locator.GetPlayerTransform().position) > 10)
            //     shipBody.DisableCollisionDetection();
            // }
            /*if (active)
            {
                shipBody.DisableCollisionDetection();
                bool collide = true;
                RaycastHit hit;
                if (Physics.Raycast(shipBody.transform.position, (lockTracker.transform.position - shipBody.transform.position).normalized, out hit, 10))
                {
                    if (Vector3.Distance(hit.transform.position, lockTracker.transform.position) > 800)//350) // (hit.transform.name == "Sun_Body")
                        {ModHelper.Console.WriteLine("hit:"+hit.transform.name);
                        collide = false;
                            Vector3 vec = Vector3.Cross(
                                hit.transform.position-shipBody.transform.position,
                                hit.transform.position-lockTracker.transform.position)
                            ;ModHelper.Console.WriteLine("vec:"+vec);
                        shipBody.GetComponent<ThrusterModel>().AddTranslationalInput(
                            shipBody.transform.InverseTransformPoint(vec
                            +shipBody.transform.position).normalized
                        );}
                }
                if (collide)
                shipBody.EnableCollisionDetection();
            }*/
            // if (activeEnd)
            // {
            //     shipBody.EnableCollisionDetection();
            //     // ModHelper.Console.WriteLine("AutoEnd");
            //     // state=AutonomyState.Aligning;
            //     // shipBody.GetComponent<AlignShipWithReferenceFrame>().OnEnterLandingMode(lockTracker.GetReferenceFrame());
            //     // StartCoroutine(Timing(2, AutonomyState.Landing));
            //     // StartCoroutine(Timing(25, AutonomyState.Inactive, AutonomyState.Landing));
            //     // StartCoroutine(Timing(2, AutonomyState.Inactive));
            // }
            // if (state==AutonomyState.Landing && shipBody.GetComponent<LandingPadManager>()._isLanded)
            //     StartCoroutine(Timing(2, AutonomyState.Inactive));

            // Planet Selection
            if (Keyboard.current.numpadPeriodKey.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetPlayerTransform().GetComponent<OWRigidbody>().GetReferenceFrame());
            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .Sun            ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .TowerTwin      ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .CaveTwin       ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .TimberHearth   ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad4Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .BrittleHollow  ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad5Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .GiantsDeep     ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad6Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .DarkBramble    ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad7Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .Comet          ).GetOWRigidbody().GetReferenceFrame());
            if (Keyboard.current.numpad8Key.wasPressedThisFrame)
                lockTracker.TargetReferenceFrame(Locator.GetAstroObject(AstroObject.Name    .RingWorld      ).GetOWRigidbody().GetReferenceFrame());
        }
    }
}
