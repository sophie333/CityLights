using UnityEngine;

namespace UnityTracking
{
	public interface ITrackingManager
	{
		float TargetScreenWidth { get; }
        float TargetScreenHeight { get; }

		float TrackingStageX { get; }
		float TrackingStageY { get; }

		Vector2 GetScreenPositionFromRelativePosition (float x, float y);
	}
}