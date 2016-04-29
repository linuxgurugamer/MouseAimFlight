﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MouseAimFlight.FlightModes
{
    class CruiseFlight : Flight
    {
        private static string flightMode = "Cruise Flight";

        public CruiseFlight()
        {

        }

        public override ErrorData Simulate(Transform vesselTransform, Vector3d targetDirection, Vector3d targetDirectionYaw, Vector3 targetPosition, Vector3 upDirection, float upWeighting, Vessel vessel)
        {
            float pitchError;
            float rollError;
            float yawError;

            float sideslip;

            sideslip = (float)Math.Asin(Vector3.Dot(vesselTransform.right, vessel.srf_velocity.normalized)) * Mathf.Rad2Deg;

            pitchError = -((float)Math.PI * 0.5f - (float)Math.Acos(Vector3.Dot(vesselTransform.up, vessel.upAxis))) * Mathf.Rad2Deg;
            yawError = (float)Math.Asin(Vector3d.Dot(Vector3d.right, VectorUtils.Vector3dProjectOnPlane(targetDirectionYaw, Vector3d.forward))) * Mathf.Rad2Deg;

            //roll
            Vector3 currentRoll = -vesselTransform.forward;
            Vector3 rollTarget;

            rollTarget = (targetPosition + Mathf.Clamp(upWeighting * (100f - Math.Abs(yawError * 2.5f)), 0, float.PositiveInfinity) * upDirection) - vessel.CoM;

            rollTarget = Vector3.ProjectOnPlane(rollTarget, vesselTransform.up);

            rollError = VectorUtils.SignedAngle(currentRoll, rollTarget, vesselTransform.right) - sideslip * (float)Math.Sqrt(vessel.srf_velocity.magnitude) / 5;

            float pitchDownFactor = pitchError * (10 / ((float)Math.Pow(yawError, 2) + 10f) - 0.1f);
            rollError -= Mathf.Clamp(pitchDownFactor, -15, 0);

            ErrorData behavior = new ErrorData(pitchError, rollError, yawError);

            return behavior;
        }

        public override string GetFlightMode()
        {
            return flightMode;
        }
    }
}
