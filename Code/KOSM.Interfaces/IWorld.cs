using System;
using System.Collections.Generic;

namespace KOSM.Interfaces
{
    public interface IWorld
    {
        ILog MissionLog { get; }
        ILog MissionPlanLog { get; }
        ILog DebugLog { get; }
        ILog LiveDebugLog { get; }
        void GameLog(object message);

        IRocket ActiveRocket { get; }
        List<IRocket> Rockets { get; }

        List<IBody> Bodies { get; }
        IBody FindBodyByName(string bodyName);

        
        void FinishUpdate();

        void StartGame(string profile, string filename);
        void LoadGame(string filename);
        void SaveGame(string filename);
        bool QuickLoad();
        bool QuickSave();

        double PointInTime { get; }
        double TimeOfNextManeuver { get; }
        
        bool IsTimeWarping { get; }
        bool WarpTime(double timespan);
        bool WarpTimeTo(double timeToWarpTo);
        void PreventTimeWarping();
    }
}
