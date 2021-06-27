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
        [ Foldout( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe" ) ] public int swipeThreshold;

        // Player
        [ Foldout( "Actor" ) ] public float actor_launchForce = 20000; // 10k
        [ Foldout( "Actor" ) ] public float actor_deStretchDuration = 0.1f; // Destretch duration before launching
        [ Foldout( "Actor" ) ] public float actor_attachPoint_Z = 0.85f; // Hand attach point Z Value

        [ Foldout( "Agent" ) ] public float agent_rotationSpeed = 25f; // Rotate speed for agent to alinged with launch direction
        [ Foldout( "Agent" ) ] public float agent_stretchDutation = 0.25f; // Stretch duration after Agent rotated to launch target point

		[ ShowNonSerializedField ] public const int actorCount = 4;
        
        [ Foldout( "Rectangular Platform Settings" ), Range( 1, 100 ), Label( "Rand. point inside: % Margin" ) ]
		public int randomPointInside_MarginPercentage = 15;
        [ Foldout( "Rectangular Platform Settings" ), Range( 1, 100 ), Label( "Rand. point outside: % Max Offset" ) ]
		public int randomPointOutside_MaxOffsetPercentage = 45;
        
        [ Foldout( "Circular Platform Settings" ), MinMaxSlider( 1, 100 ), Label( "Rand. point inside is between (% Radius)" ) ]
		public Vector2 randomPointInside_betweenRadii = new Vector2( 10, 90 );
		public float RandomPointInside_Radius => Random.Range( randomPointInside_betweenRadii.x / 100.0f, randomPointInside_betweenRadii.y / 100.0f );
		[ Foldout( "Circular Platform Settings" ), MinMaxSlider( 100, 300 ), Label( "Rand. point outside is between (% Radius)" ) ]
		public Vector2 randomPointOutside_betweenRadii = new Vector2( 100, 150 );
		public float RandomPointOutside_Radius => Random.Range( randomPointOutside_betweenRadii.x / 100.0f, randomPointOutside_betweenRadii.y / 100.0f );
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
