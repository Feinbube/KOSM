using System;
using System.Collections.Generic;

namespace KOSM.Interfaces
{
    public interface IWorld
    {
        ILog MissionLog { get; }
        ILog MissionPlanLog { get; }
        ILog PersistentDebugLog { get; }
        ILog LiveDebugLog { get; }
        void ToGameLog(object message);

        ICamera Camera { get; }

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

        List<string> RocketDesigns { get; }
        void Launch(string vabName);

        double PointInTime { get; }
        double TimeOfNextManeuver { get; }
        
        bool IsTimeWarping { get; }
        bool WarpTime(double timespan);
        bool WarpTimeTo(double timeToWarpTo);
        void PreventTimeWarping();

        void ClearLogs();
    }
}
