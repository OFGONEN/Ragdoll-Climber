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

		// Private Fields

		// Level  
		private PlatformBase lastPlatform;

		// Rank
		[ReadOnly, SerializeField] private List< Actor > raceParticipants = new List< Actor >( GameSettings.actorCount );
		[ReadOnly, SerializeField] private List< Actor > currentRanks = new List< Actor >( GameSettings.actorCount );
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
			var actor = (actor_Parcipated_RaceListener.gameEvent as ReferenceGameEvent).eventValue as Actor;

			raceParticipants.Remove( actor );
			finishedParticipants.Add( actor );
		}

		void CheckActorRanks()
		{
			currentRanks.Clear();

			for( var i = 0; i < raceParticipants.Count; i++ )
			{
				currentRanks.Add( raceParticipants[ i ] );
			}

			currentRanks.Sort( ( x, y ) => CompareActors( x, y ) );

			for( var i = 0; i < currentRanks.Count; i++ )
			{
				currentRanks[ i ].Rank = finishedParticipants.Count + i + 1;
			}
		}

        int CompareActors( Actor x, Actor y )
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
#endregion
    }
}