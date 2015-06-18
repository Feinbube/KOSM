using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public class Camera : WorldObject, ICamera
    {
        FlightCamera raw { get { return FlightCamera.fetch; } }

        public Camera(World world)
            : base(world)
        {
        }

        # region WorldObject

        public override string Identifier
        {
            get { throw new NotImplementedException(); }
        }

        # endregion WorldObject

        #region ICamera

        public void BodyBehindRocket(IBody body, IRocket rocket)
        {
            return;
            if (rocket == null)
                return;

            if ((rocket as Rocket).raw == null)
                return;

            raw.transform.position = (v3d(rocket.Position) - v3d(body.Position)).normalized * 1 + v3d(body.Position);
            raw.transform.rotation = UnityEngine.Quaternion.LookRotation((rocket as Rocket).raw.transform.position - raw.transform.position, UnityEngine.Vector3.up);
        }

        #endregion ICamera
    }
}
