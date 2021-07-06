using NaughtyAttributes;
using UnityEngine;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Fields
        public int maxLevelCount;

        // Input
        [ Foldout ( "Input Settings" ) ] public float input_threshold_DeadZone = 1;
        [ Foldout ( "Input Settings" ) ] public float input_threshold_Rotation = 5;
        [ Foldout ( "Input Settings" ) ] public float input_threshold_Stretch = 10;

        // UI
        [ Foldout( "UI Settings" ), Tooltip( "Duration of the movement for ui element" ) ] public float ui_Entity_Move_TweenDuration;
        [ Foldout( "UI Settings" ), Tooltip( "Duration of the fading for ui element" ) ] public float ui_Entity_Fade_TweenDuration;
		[ Foldout( "UI Settings" ), Tooltip( "Duration of the scaling for ui element" ) ] public float ui_Entity_Scale_TweenDuration;
		[ Foldout( "UI Settings" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
        [ Foldout( "UI Settings" ), Tooltip( "Agent WorldUI name Offset from Ragdoll " ) ] public Vector3 worldUI_AgentName_Offset;
        [ Foldout( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe" ) ] public int swipeThreshold;

        // Player
        [ Foldout( "Actor" ) ] public float actor_launchForce = 20000; // 10k
        [ Foldout( "Actor" ) ] public float actor_deStretchDuration = 0.1f; // Destretch duration before launching
        [ Foldout( "Actor" ) ] public float actor_attachPoint_Z = 0.85f; // Hand attach point Z Value
        [ Foldout( "Actor" ), MinMaxSlider( 0, 2 ) ] public Vector2 actor_resetWaitDuration; // Duration for an actor to reset itself to a waypoint

        [ Foldout( "Agent" ) ] public float agent_rotationSpeed = 25f; // Rotate speed for agent to alinged with launch direction
        [ Foldout( "Agent" ) ] public float agent_stretchDutation = 0.25f; // Stretch duration after Agent rotated to launch target point
        [ Foldout( "Agent" ) ] public float agent_armReach_Cofactor = 1f; // 
        [ Foldout( "Agent" ), Range( 0, 1 ) ] public float agent_failChance = 0.1f; // Fail change of a agent. Launches to wrong target on fail.
        [ Foldout( "Agent" ), MinMaxSlider( 0 , 5 ) ] public Vector2 agent_launchWaitDuration; // Duration between agent attached to platform and agent's next Launch sequence

        [ Foldout( "Level" ) ] public float level_fenceBottomOffset = 7.5f; // Follow offset of bottom fence object for following the last ranking actor

		[ Foldout( "Camera" ), MinMaxSlider( 10, 50 ) ] public Vector2 camera_Depth_FollowDistance; // Min and max distance for camera to follow player in depth( Z axis )
		[ Foldout( "Camera" ) ] public float camera_FollowSpeed = 10f; // Distance for camera to follow target in X-Y axis
		[ Foldout( "Camera" ) ] public float camera_Depth_FollowSpeed = 10f; // Distance for camera to follow target in Z axis
		[ Foldout( "Camera" ) ] public float camera_FollowZoneRadius = 2.5f; // Radius of the follow zone for the camera


		[ ShowNonSerializedField ] public const int actorCount = 4;
        
        [ Foldout( "Rectangular Platform Settings" ), Range( 1, 100 ), Label( "Rand. point inside: % of Margin" ) ]
		public int randomPointInside_MarginPercentage = 15;
        [ Foldout( "Rectangular Platform Settings" ), Range( 1, 100 ), Label( "Rand. point outside: % of Max Offset" ) ]
		public int randomPointOutside_MaxOffsetPercentage = 45;
        
        [ Foldout( "Circular Platform Settings" ), MinMaxSlider( 1, 100 ), Label( "Rand. point inside is in this range (% of Radius)" ) ]
		public Vector2 randomPointInside_betweenRadii = new Vector2( 10, 90 );
		public float RandomPointInside_Radius => Random.Range( randomPointInside_betweenRadii.x / 100.0f, randomPointInside_betweenRadii.y / 100.0f );
		[ Foldout( "Circular Platform Settings" ), MinMaxSlider( 100, 300 ), Label( "Rand. point outside is in this range (% of Radius)" ) ]
		public Vector2 randomPointOutside_betweenRadii = new Vector2( 100, 150 );
		public float RandomPointOutside_Radius => Random.Range( randomPointOutside_betweenRadii.x / 100.0f, randomPointOutside_betweenRadii.y / 100.0f );
		[ Foldout( "Circular Platform Settings" ), MinMaxSlider( 10, 90 ), Label( "Reset slot is in this range (% of Extents)" ) ]
		public Vector2 resetSlotOffsetRange = new Vector2( 30, 65 );
		public float RandomRadialResetSlotOffset => Random.Range( resetSlotOffsetRange.x / 100.0f, resetSlotOffsetRange.y / 100.0f );
#endregion

#region Singleton Fields
		private static GameSettings instance;

        private delegate GameSettings ReturnGameSettings();
        private static ReturnGameSettings returnInstance = LoadInstance;

        public static GameSettings Instance
        {
            get
            {
                return returnInstance();
            }
        }
#endregion

#region Implementation
        static GameSettings LoadInstance()
        {
            if (instance == null)
                instance = Resources.Load<GameSettings>("game_settings");

            returnInstance = ReturnInstance;

            return instance;
        }

        static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}
