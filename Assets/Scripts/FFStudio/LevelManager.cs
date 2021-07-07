using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
#region Fields
        [ Header( "Event Listeners" ) ]
        public EventListenerDelegateResponse levelLoadedListener;
        public EventListenerDelegateResponse levelRevealedListener;
        public EventListenerDelegateResponse levelStartedListener;
        public EventListenerDelegateResponse levelCompletedListener;

		// Actor Event Listeners
		public EventListenerDelegateResponse actor_Parcipated_RaceListener;
		public EventListenerDelegateResponse actor_Finished_RaceListener;

		[ Header( "Fired Events" ) ]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;

        [ Header( "Level Releated" ) ]
        public PlatformSet platformSet;
        public SharedFloatProperty levelProgress;
        public SharedReferenceProperty bottomFence_Property;

		// Private Fields

		// Level  
		private PlatformBase lastPlatform;
		private Transform bottomFence;

		// Rank
		[ReadOnly, SerializeField] private List< Actor > raceParticipants = new List< Actor >( GameSettings.actorCount );
		[ReadOnly, SerializeField] private List< Actor > currentRanks_WorldPoint = new List< Actor >( GameSettings.actorCount );
		[ReadOnly, SerializeField] private List< Actor > currentRanks_Waypoint = new List< Actor >( GameSettings.actorCount );
		[ReadOnly, SerializeField] private List< Actor > finishedParticipants = new List< Actor >( GameSettings.actorCount );

		// Unity Messages 
		UnityMessage actorsRankCheck;

#endregion

#region UnityAPI

		private void OnEnable()
        {
            // Level Releated
            levelLoadedListener   .OnEnable();
            levelRevealedListener .OnEnable();
            levelStartedListener  .OnEnable();
			levelCompletedListener.OnEnable();

			// Actor Releated
			actor_Parcipated_RaceListener.OnEnable();
			actor_Finished_RaceListener  .OnEnable();
		}

        private void OnDisable()
        {
            // Level Releated
            levelLoadedListener   .OnDisable();
            levelRevealedListener .OnDisable();
            levelStartedListener  .OnDisable();
			levelCompletedListener.OnDisable();


            // Actor Releated
			actor_Parcipated_RaceListener.OnDisable();
			actor_Finished_RaceListener  .OnDisable();

        }

        private void Awake()
        {
            levelLoadedListener.response     = LevelLoadedResponse;
            levelRevealedListener.response   = LevelRevealedResponse;
            levelStartedListener.response    = LevelStartedResponse;
            levelCompletedListener.response  = LevelCompletedResponse;

			actor_Parcipated_RaceListener.response = Actor_Participated_RaceResponse;
			actor_Finished_RaceListener.response   = Actor_Finished_RaceResponse;

			actorsRankCheck = ExtensionMethods.EmptyMethod;
		}

        private void Update()
        {
			actorsRankCheck();
		}

#endregion

#region Implementation
        void LevelLoadedResponse()
        {
            levelProgress.SetValue(0);
			raceParticipants.Clear();
			finishedParticipants.Clear();
		}

        void LevelRevealedResponse()
        {

        }

        void LevelStartedResponse()
        {
			var platformCount = platformSet.itemDictionary.Count / 2;
			platformSet.itemDictionary.TryGetValue( platformCount - 1, out lastPlatform );

			bottomFence = bottomFence_Property.sharedValue as Transform;
			actorsRankCheck = CheckActorRanks;
		}

        void LevelCompletedResponse()
        {
			actorsRankCheck = ExtensionMethods.EmptyMethod;
		}

        void Actor_Participated_RaceResponse()
        {
			var actor = (actor_Parcipated_RaceListener.gameEvent as ReferenceGameEvent).eventValue as Actor;
			raceParticipants.Add( actor );
		}

        void Actor_Finished_RaceResponse()
        {
			var actor = (actor_Finished_RaceListener.gameEvent as ReferenceGameEvent).eventValue as Actor;

			raceParticipants.Remove( actor );
			finishedParticipants.Add( actor );
		}

		void CheckActorRanks()
		{
			currentRanks_WorldPoint.Clear();
			currentRanks_Waypoint.Clear();

			for( var i = 0; i < raceParticipants.Count; i++ )
			{
				currentRanks_WorldPoint.Add( raceParticipants[ i ] );
				currentRanks_Waypoint.Add( raceParticipants[ i ] );
			}

			currentRanks_WorldPoint.Sort( ( x, y ) => CompareActors_WorldPoint( x, y ) );
			currentRanks_Waypoint.Sort( ( x, y ) => CompareActors_Waypoint( x, y ) );

			for( var i = 0; i < currentRanks_WorldPoint.Count; i++ )
			{
				currentRanks_WorldPoint[ i ].Rank = finishedParticipants.Count + i + 1;
			}

			if( currentRanks_Waypoint.Count > 0 )
			{
				var position = bottomFence.position;

				position.y = currentRanks_Waypoint[ currentRanks_Waypoint.Count - 1 ].ActorPlatform.transform.position.y - GameSettings.Instance.level_fenceBottomOffset;
				bottomFence.DOMove( position, 0.5f );
			}
		}

        int CompareActors_WorldPoint( Actor x, Actor y )
        {
			var x_Distance = lastPlatform.transform.position.y - x.ActorPosition.y;
			var y_Distance = lastPlatform.transform.position.y - y.ActorPosition.y;

            if( x_Distance < y_Distance  )
				return -1;
            else if( Mathf.Approximately( x_Distance, y_Distance ) )
				return 0;
            else
				return 1;
		}

        int CompareActors_Waypoint( Actor x, Actor y )
        {
            if( x.ActorWayPoint < y.ActorWayPoint  )
				return 1;
            else if( x.ActorWayPoint == y.ActorWayPoint )
				return 0;
            else
				return -1;
		}
#endregion
    }
}