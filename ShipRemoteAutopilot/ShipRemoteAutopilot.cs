using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipRemoteAutopilot
{
    public class ShipRemoteAutopilot : ModBehaviour
    {
        bool CounterActive = false;
        bool isTimerDone = false;
        bool isShipAligned = false;
        bool isSunAvoided = true;
        RaycastHit hit;
        AstroObject planetaryBody = Locator.GetAstroObject(AstroObject.Name.GiantsDeep);

        //10 Seconds countdown for moving ship to air
        IEnumerator ThrusterTiming()
        {
            WaitForSeconds wait = new WaitForSeconds(10);
            yield return wait;

            isTimerDone = true;
        }
        //2 Second countdown for aligning the ship to the surface of the planet for landing
        IEnumerator ShipAlignmentTiming()
        {
            WaitForSeconds wait = new WaitForSeconds(2);
            yield return wait;

            isShipAligned = true;
        }


        //Links the Align and Land Ship functions to the Autopilot Arrival flag
        private void Start()
        {
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                var shipBody = FindObjectOfType<ShipBody>();

                shipBody.GetComponent<Autopilot>().OnArriveAtDestination += AlignShip;
            };
        }

        //Function to Align the ship to face the floor of the planet
        public void AlignShip(float arrivalError)
        {
            var ShipBody = FindObjectOfType<ShipBody>();
            var AligningReferenceFrame = planetaryBody.GetOWRigidbody().GetReferenceFrame();
            Locator.GetShipBody().GetComponent<AlignShipWithReferenceFrame>().OnEnterLandingMode(AligningReferenceFrame);
            StartCoroutine(ShipAlignmentTiming());
        }

        //Function to activate autopilot to location
        public void TravelToLocation()
        {
            //Sets the timer to be done
            isTimerDone = false;

            var ShipBody = Locator.GetShipBody();
            var ShipReferenceFrame = planetaryBody.GetOWRigidbody().GetReferenceFrame();
            ModHelper.Console.WriteLine("Ship Reference Frame Name : " + ShipReferenceFrame);
            Locator.GetPlayerTransform().GetComponent<ReferenceFrameTracker>().TargetReferenceFrame(ShipReferenceFrame);
            Locator.GetShipBody().GetComponent<Autopilot>().FlyToDestination(ShipReferenceFrame);
        }


        private void Update()
        {
            var ShipBody = Locator.GetShipBody();

            //Controls upward thrust when counter is counting
            if (CounterActive)
            {
                Locator.GetShipBody().GetComponent<ThrusterModel>().AddTranslationalInput(Vector3.up);
            }

            //When the ship is aligned 
            if (isShipAligned)
            {
                Locator.GetShipBody().GetComponent<AlignShipWithReferenceFrame>().OnExitLandingMode();
                isShipAligned = false;
            }

            //When the timer is done, activate the travelling from orbit
            if (isTimerDone)
            {
                CounterActive = false;
                isTimerDone = false;
                isSunAvoided = false;
            }

            //Only runs if it has not been checked that the sun is in the way
            if (!isSunAvoided)
            {
                Locator.GetShipBody().DisableCollisionDetection();
                var direction = planetaryBody.transform.position - ShipBody.transform.position;
                if (Physics.Raycast(ShipBody.transform.position, (planetaryBody.transform.position - ShipBody.transform.position).normalized , out hit))
                {
                    if (hit.transform.name == "Sun_Body")
                    {
                        Locator.GetShipBody().GetComponent<ThrusterModel>().AddTranslationalInput(Vector3.Cross(direction, Vector3.up));
                        isSunAvoided = false;
                    }else
                    {
                        isSunAvoided = true;
                        TravelToLocation();
                    }
                }else
                {
                    TravelToLocation();
                }
            }

            if (isSunAvoided)
            {
                Locator.GetShipBody().EnableCollisionDetection();
            }

            //Go to Location
            if (Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                if (ShipBody.GetComponent<LandingPadManager>()._isLanded == true)
                {
                    CounterActive = true;
                    StartCoroutine(ThrusterTiming());
                }else
                {
                    isSunAvoided = false;
                }
            }


            //Select the Planet from the Planet Index
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.TimberHearth);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to Timber Hearth");
            }

            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.GiantsDeep);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to Giant's Deep");
            }

            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.BrittleHollow);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to Brittle Hollow");
            }

            if (Keyboard.current.numpad4Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.DarkBramble);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to Dark Bramble");
            }

            if (Keyboard.current.numpad5Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.TowerTwin);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to AshTwin");
            }

            if (Keyboard.current.numpad6Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.CaveTwin);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to EmberTwin");
            }

            if (Keyboard.current.numpad7Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.Comet);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to the Interloper");
            }

            if (Keyboard.current.numpad8Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.RingWorld);
                ModHelper.Console.WriteLine("Changed Remote Autopilot location to the <REDACTED>");
            }
        }
    }
}
