﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface ICamera
    {
        void BodyBehindRocket(IBody body, IRocket rocket);
    }
}
