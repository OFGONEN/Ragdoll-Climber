/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
	public class HammeringTween : MonoBehaviour
	{
#region Fields
		public Vector3 startAngle;
		public Vector3 endAngle;
		[ Label( "Foward Speed" ) ] public float angularSpeed_Forward;
		[ Label( "Backward Speed" ) ] public float angularSpeed_Backward;

		public float delayBeforeGoing_Up;
		public float delayBeforeGoing_Down;

		public bool playOnStart;

        [ ShowIf( "playOnStart" ) ]
		public bool hasDelay;

        [ ShowIf( "hasDelay" ) ]
		public float delayAmount;

		public bool loop;

        [ ShowIf( "loop" ) ]
        public LoopType loopType = LoopType.Restart;

		public bool customEasing_Foward;

        [ HideIf( "customEasing_Foward" ) ]
        public Ease easing_Foward = Ease.Linear;

        [ ShowIf( "customEasing_Foward" ) ]
        public AnimationCurve easingCurve_Foward;

		public bool customEasing_Backward;

        [ HideIf( "customEasing_Backward" ) ]
        public Ease easing_Backward = Ease.Linear;

        [ ShowIf( "customEasing_Backward" ) ]
        public AnimationCurve easingCurve_Backward;

        public GameEvent[] fireTheseOnComplete;

        [ field: SerializeField, ReadOnly ]
        public bool IsPlaying { get; private set; }

		// Private Fields
		private float deltaAngle;
		private float DurationForward => Mathf.Abs( deltaAngle / angularSpeed_Forward / 2f );
		private float DurationBackward => Mathf.Abs( deltaAngle / angularSpeed_Backward / 2f );

		Sequence sequence;
#endregion

#region Unity API
		private void OnEnable()
		{
			Play();
		}

		private void OnDisable()
		{
			Pause();
		}

		private void Start()
		{
			deltaAngle = Quaternion.Angle( Quaternion.Euler( startAngle ), Quaternion.Euler( endAngle ) );

			if( !enabled )
				return;

			if( playOnStart )
			{
				if( hasDelay )
					DOVirtual.DelayedCall( delayAmount, Play );
				else
					Play();
			}
		}

		private void OnDestroy()
		{
			KillTween();
		}
#endregion

#region API

		[Button()]
		public void Play()
		{
			transform.localEulerAngles = startAngle;

			if( sequence == null )
				CreateAndStartTween();
			else
				sequence.Play();

			IsPlaying = true;
		}

		[Button(), EnableIf( "IsPlaying" )]
		public void Pause()
		{
			if( sequence == null )
				return;

			sequence.Pause();

			IsPlaying = false;
		}

		[Button(), EnableIf( "IsPlaying" )]
		public void Stop()
		{
			if( sequence == null )
				return;

			sequence.Rewind();

			IsPlaying = false;
		}

		[Button(), EnableIf( "IsPlaying" )]
		public void Restart()
		{
			if( sequence == null )
				Play();
			else
			{
				sequence.Restart();

				IsPlaying = true;
			}
		}

#endregion

#region Implementation
		private void CreateAndStartTween()
		{
			sequence = DOTween.Sequence();

			var downTween = transform.DOLocalRotate( endAngle, DurationForward );
			var upTween   = transform.DOLocalRotate( startAngle, DurationBackward );

			if( customEasing_Foward )
				downTween.SetEase( easingCurve_Foward );
			else 
				downTween.SetEase( easing_Foward );

			if( customEasing_Backward )
				upTween.SetEase( easingCurve_Backward );
			else 
				upTween.SetEase( easing_Backward );

			sequence.Append( downTween );
			sequence.AppendInterval( delayBeforeGoing_Up );
			sequence.Append( upTween );
			sequence.AppendInterval( delayBeforeGoing_Down );

			sequence.SetLoops( loop ? -1 : 0, loopType )
					.OnComplete( SequenceComplete );
		}

		private void SequenceComplete()
		{
			IsPlaying = false;

			KillTween();

			for( var i = 0; i < fireTheseOnComplete.Length; i++ )
				fireTheseOnComplete[ i ].Raise();
		}

		private void KillTween()
		{
			IsPlaying = false;

			sequence.Kill();
			sequence = null;
		}

#endregion

#region EditorOnly
#if UNITY_EDITOR
#endif
#endregion
	}
}