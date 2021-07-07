/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using FFStudio;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

public abstract class Actor : MonoBehaviour
{
	#region Fields
	[ Header( "Event Listener" ) ]
	public EventListenerDelegateResponse levelStartedListener;

	[ Header( "Fired Events" ) ]
	public ReferenceGameEvent actor_Participate_Race;
	public ReferenceGameEvent actor_Finished_Race;
	public ParticleSpawnEvent particleSpawnEvent;

	public int actor_Index;
	public UIWorldSpace actorNameDisplay;
	public SkinnedMeshRenderer modelRenderer;
	public PlatformSet platformSet;

	// Public Properties
	public int Rank
	{
		set 
		{
			actorRank = value;
			actorNameDisplay.entityName.text = actorName + " #" + value;
		}
	}
	public Vector3 ActorPosition => parentRigidbody.transform.position;
	public int ActorWayPoint => currentWayPoint;
	public PlatformBase ActorPlatform => currentPlatform;

	// Protected/Private Properties
	protected Vector3 ActorRotation => parentRigidbody.transform.eulerAngles;
	protected Vector3 LeftShouldPos => arm_left_limbs[ 0 ].transform.position;
	protected Vector3 RightShouldPos => arm_right_limbs[ 0 ].transform.position;
	protected float ArmReachDistance => armReachDistance;

	// Body Related
	[Foldout( "Body" )] protected Rigidbody parentRigidbody;  // Most parent rigidbody. Has jointed to Spine of the ragdoll via FixedJoint.
	[Foldout( "Body" ), SerializeField] private Rigidbody[] rotatingLimbs_rigidbodies; // Torso, hip and leg limbs

	// Hand Related 
	[Foldout( "Hand" ), SerializeField] private Rigidbody hand_rb_left; // Left Hand Rigidbody
	[Foldout( "Hand" ), SerializeField] private Rigidbody hand_rb_right; // Right hand Rigidbody
	[Foldout( "Hand" ), SerializeField] private Transform hand_optimalPos_left;  // left hand optimal position
	[Foldout( "Hand" ), SerializeField] private Transform hand_optimalPos_right; // right hand optimal position
	[Foldout( "Hand" ), SerializeField] private HandJoint handJoint_Left; // Fixed joint for fixing position and rotation of left hand
	[Foldout( "Hand" ), SerializeField] private HandJoint handJoint_Right; // Fixed joint for fixing position and rotation of left hand

	// Arm Related
	[Foldout( "Arm" ), SerializeField] private Rigidbody[] arm_left_limbs; // Left arm rigidbodies
	[Foldout( "Arm" ), SerializeField] private Rigidbody[] arm_right_limbs; // Right arm rigidbodies

	// Configure 
	[Foldout( "Configure" ), SerializeField ] private TransformData[] arm_holdingPositions_Left; // Left arm's holdings position and rotations
	[Foldout( "Configure" ), SerializeField ] private TransformData[] arm_holdingPositions_Right; // Right arm's holding position and rotations
	[Foldout( "Configure" ), SerializeField ] private TransformData[] rotatingLimbs_holdingPositions; // Rotating limbs holding position and rotations
	[Foldout( "Configure" ), SerializeField ] private TransformData[] limbs_holdPositions_TPose; // T-Pose position and rotation data of the ragdoll

	// WayPoint
	[ SerializeField, ReadOnly ] protected int currentWayPoint = 0;
	private PlatformBase currentPlatform;

	// Private Fields
	private Rigidbody[] limbs_rigidbodies; // Every rigidbody in the ragdoll

	private Transform attached_Hand; // Hand that is attached to a platform
	private HandJoint attached_HandJoint; // Hand Joint that hand attached to
	private GameObject handTargetObject; // World point that hand will attached to
	private Vector3 handTargetPoint; // World point that hand will attached to
	private UnityMessage attachHand; // Delegate for attaching hand, gets set by checking checking attach points in the space
	private UnityMessage applyHandPosition; // Delegate for modifying attached hand's limbs rotation and position for rotating the ragdoll
	private UnityMessage update; 
	private Tween resetActorWaitTween;

	protected string actorName; // Actor's name and rank
	protected int actorRank; // Actor's rank in the race
	private float armReachDistance; // An arm's reach distance from a shoulder
	private const int collisionLayer = 27;
	private Collider[] castTarget = new Collider[ 1 ]; // Used for OverlapSphereNonAlloc 

#endregion

#region Unity API
	protected virtual void OnEnable()
	{
		levelStartedListener.OnEnable();
	}

	protected virtual void OnDisable()
	{
		levelStartedListener.OnDisable();
		KillTweens();
	}

	protected virtual void Awake()
	{
		parentRigidbody   = GetComponentInChildren< Rigidbody >();
		limbs_rigidbodies = parentRigidbody.GetComponentsInChildren< Rigidbody >(); // Get every rigidbody that ragdoll has
		
		// Search distance for searching a point for a hand to attached to. Search origin is shoulder
		armReachDistance  = Vector3.Distance( arm_left_limbs[ arm_left_limbs.Length - 1 ].transform.position, arm_left_limbs[ 0 ].transform.position );

		levelStartedListener.response = LevelStartResponse;
		actorNameDisplay.followTarget = parentRigidbody.transform;

		update = ExtensionMethods.EmptyMethod;
	}

	protected virtual void Start()
	{
		platformSet.itemDictionary.TryGetValue( currentWayPoint, out currentPlatform );
		ResetActorToWayPoint();

		actor_Participate_Race.eventValue = this;
		actor_Participate_Race.Raise();
	}

	private void Update()
	{
		update();
	}

#endregion

#region API
	public virtual void ResetActor()
	{
		var waitRange = GameSettings.Instance.actor_resetWaitDuration;
		var randomWaitDuration = Random.Range( waitRange.x, waitRange.y );

		// ReleaseHands();
		DefaultTheRagdoll();
		TPoseTheRagdoll();

		actorNameDisplay.gameObject.SetActive( false );
		parentRigidbody.gameObject.SetActive( false );

		resetActorWaitTween = DOVirtual.DelayedCall( randomWaitDuration, ResetActorToWayPoint );
		resetActorWaitTween.OnComplete( () => resetActorWaitTween = null );
	}
#endregion

#region Implementation
	[ Button() ] // Try to find world points for a hand to hold onto. Checks optimal points first if there is no collider on optimal points 
	// An SphereCast is casted from shoulders to find a collider then a holding point calculated for a hand
	protected void TryToAttachHands()
	{
		if( HasOptimalPoints() )
			attachHand();
		else if (HasClosestPoint())
		{
			attachHand();
		}
	}

	private bool HasOptimalPoints()
	{
		// Cast a raycast from behind the optimal points to prevent if optimal points are already inside a collider, since ragdoll can move freely and 
		// Optimal points moves with it
		var castOrigin_Left = hand_optimalPos_left.position;
		castOrigin_Left.z   = -1;

		var castOrigin_Right = hand_optimalPos_right.position;
		castOrigin_Right.z   = -1;


		RaycastHit raycastHit_Left;
		RaycastHit raycastHit_Right;

		// Raycast in obstacle layer to find collider
		var left_OptimalPoint  = Physics.Raycast( castOrigin_Left, Vector3.forward, out raycastHit_Left, 10, LayerMask.GetMask( "Obstacle" ) );
		var right_OptimalPoint = Physics.Raycast( castOrigin_Right, Vector3.forward, out raycastHit_Right, 10, LayerMask.GetMask( "Obstacle" ) );

		bool hasOptimalPoint = false;

		if( left_OptimalPoint && right_OptimalPoint ) // If both left and right optimal points are found, choose a random one
		{
			var random = Random.Range( 0, 2 );

			if( random == 0 )
			{
				attachHand = AttachLeftHand;
				handTargetPoint = raycastHit_Left.point;

				UpdateWayPointIndex( raycastHit_Left.collider.gameObject );
			}
			else 
			{
				attachHand = AttachRightHand;
				handTargetPoint = raycastHit_Right.point;
				
				UpdateWayPointIndex( raycastHit_Right.collider.gameObject );
			}

			hasOptimalPoint = true;
		}
		else if ( left_OptimalPoint || right_OptimalPoint )
		{
			if( left_OptimalPoint )
			{
				attachHand = AttachLeftHand;
				handTargetPoint = raycastHit_Left.point;

				UpdateWayPointIndex( raycastHit_Left.collider.gameObject );
			}
			else 
			{
				attachHand = AttachRightHand;
				handTargetPoint = raycastHit_Right.point;

				UpdateWayPointIndex( raycastHit_Right.collider.gameObject );
			}

			hasOptimalPoint = true;
		}

		// FFLogger.Log( "Found Optimal Point:" + hasOptimalPoint );
		// Since ragdoll can hold onto any point in space, we should manually set Z point of the point the hand is holding onto.
		// This is because we may want ragdoll to have a little bit of distance with the obstacle to have it look properly hanging 
		handTargetPoint.z = GameSettings.Instance.actor_attachPoint_Z; 
		return hasOptimalPoint;
	}

	// Cast Sphere to find nearest obstacles. If found calculate a point where the farthest point that arm can reach to obstacle's origin
	private bool HasClosestPoint()
	{
		// Cache shoulders positions
		var left_ShoulderPosition  = arm_left_limbs[ 0 ].transform.position;
		var right_ShoulderPosition = arm_right_limbs[ 0 ].transform.position;

		castTarget[ 0 ] = null;
		Physics.OverlapSphereNonAlloc( left_ShoulderPosition, armReachDistance, castTarget, LayerMask.GetMask( "Obstacle" ) );

		// If an obstacle is found edit the distance so that we can decide if a collider is found without having an bool variable to cache the information
		float armDistance_Left  = -1;
		float armDistance_Right = -1;

		Transform target_Left  = null;
		Transform target_Right = null;

		if( castTarget[ 0 ] != null  )
		{
			armDistance_Left = Vector3.Distance( castTarget[ 0 ].transform.position, left_ShoulderPosition );
			target_Left = castTarget[ 0 ].transform;
		}

		castTarget[ 0 ] = null;
		Physics.OverlapSphereNonAlloc( right_ShoulderPosition, armReachDistance, castTarget, LayerMask.GetMask( "Obstacle" ) );

		if( castTarget[ 0 ] != null )
		{
			armDistance_Right = Vector3.Distance( castTarget[ 0 ].transform.position, right_ShoulderPosition );
			target_Right = castTarget[ 0 ].transform;
		}

		bool hasPoint = false;

		if( armDistance_Left > 0 && armDistance_Right > 0 ) // Both of the arms can reach to a obstacle 
		{
			if( armDistance_Left <= armDistance_Right ) // Choose the closest one
			{
				attachHand = AttachLeftHand;
				handTargetPoint = left_ShoulderPosition + (target_Left.position - left_ShoulderPosition).normalized * armReachDistance;

				UpdateWayPointIndex( target_Left.gameObject );
			}
			else 
			{
				attachHand = AttachRightHand;
				handTargetPoint = right_ShoulderPosition + (target_Right.position - right_ShoulderPosition).normalized * armReachDistance;

				UpdateWayPointIndex( target_Right.gameObject );
			}

			hasPoint = true;
		}
		else if ( armDistance_Left > 0 || armDistance_Right > 0 ) // One of the arm's can reach to a obstacle
		{
			if( armDistance_Left > 0 )
			{
				attachHand = AttachLeftHand;
				handTargetPoint = left_ShoulderPosition + (target_Left.position - left_ShoulderPosition).normalized * armReachDistance;

				UpdateWayPointIndex( target_Left.gameObject );
			}
			else 
			{
				attachHand = AttachRightHand;
				handTargetPoint = right_ShoulderPosition + (target_Right.position - right_ShoulderPosition).normalized * armReachDistance;

				UpdateWayPointIndex( target_Right.gameObject );
			}

			hasPoint = true;
		}

		// FFLogger.Log( "Found Closest Point:" + hasPoint );
		handTargetPoint.z = GameSettings.Instance.actor_attachPoint_Z;
		return hasPoint;
	}

	protected void ReadyToLaunch()
	{
		DefaultTheRagdoll();
		ReleaseHands();

		// DeStretches the model in a duration to make the model look like its been let go from stretching and launching with destretch momentum
		var deStretchTween = DOTween.To( () => modelRenderer.GetBlendShapeWeight( 0 ), x => modelRenderer.SetBlendShapeWeight( 0, x ), 0, GameSettings.Instance.actor_deStretchDuration );
		deStretchTween.OnComplete( Launch );
	}

	protected virtual void Launch()
	{
		parentRigidbody.AddForce( parentRigidbody.transform.up * GameSettings.Instance.actor_launchForce );
	}

	protected void RotateToTargetAngle( float targetAngle ) // Rotates the parent rigidbody around holding hand's position by angle
	{
		var currentAngle = parentRigidbody.transform.eulerAngles.z;
		var rotateAmount = targetAngle - currentAngle;

		// //! This is relative, i.e it rotates the object angle more around the point
		parentRigidbody.transform.RotateAround( attached_HandJoint.transform.position, Vector3.forward, rotateAmount );
	}

	protected void Rotate( float rotateAmount )
	{
		parentRigidbody.transform.RotateAround( attached_HandJoint.transform.position, Vector3.forward, rotateAmount );
		OffsetTheRagdoll();
	}

	protected void Stretch( float ratio ) // Modifies the **SkinnedMeshRenderer**'s blend shape key to make model look stretched
	{
		modelRenderer.SetBlendShapeWeight( 0, ratio * 100 );
	}

	private void AttachLeftHand() // Attaches the left hand into a target point
	{
		DefaultTheRagdoll(); // Make ragdoll dynamic and zero out velocities 

		hand_rb_left.transform.position         = handTargetPoint; // Move ragdoll's hand into target position
		hand_rb_left.transform.forward          = Vector3.right; // Rotate the hand into a holding rotation
		handJoint_Left.Attach( handTargetPoint, handTargetObject, hand_rb_left ); // Attach hand to joint

		applyHandPosition  = ApplyArmPosition_Left; // Cache with arm to position when holding
		attached_Hand      = hand_rb_left.transform; // Cache the attached hand
		attached_HandJoint = handJoint_Left;

		OnHandsAttached();

		// Removes the actor from collision Layer to its own layer for its to be only interacting with itself
		// So that a launched actor wouldn't collide with a holding actor
		ChangeActorCollisionLayer( 31 - actor_Index );
	}

	private void AttachRightHand()
	{
		DefaultTheRagdoll();

		hand_rb_right.transform.position         = handTargetPoint;
		hand_rb_right.transform.forward          = Vector3.left;
		handJoint_Right.Attach( handTargetPoint, handTargetObject, hand_rb_right );

		applyHandPosition  = ApplyArmPosition_Right;
		attached_Hand      = hand_rb_right.transform; 
		attached_HandJoint = handJoint_Right;

		OnHandsAttached();

		// Removes the actor from collision Layer to its own layer for its to be only interacting with itself
		// So that a launched actor wouldn't collide with a holding actor
		ChangeActorCollisionLayer( 31 - actor_Index );
	}

	protected virtual void ResetActorToWayPoint()
	{
		var position = currentPlatform.GetResetSlot( actor_Index ); // Get a reset slot for our actor based on actor index

		ReleaseHands(); // Release the hands from fixed joints, If any hand is attached
		DefaultTheRagdoll(); // Zero out velocities and make every limb's rigidbody as dynamic
		TPoseTheRagdoll(); // Return the limb's back to T-Pose rotation and position

		parentRigidbody.transform.position = position;  // Set the reset position
		TryToAttachHands();
	}

	[ Button() ]
	protected void StraightenUpRagdoll() // Straighten Ups the ragdoll for its to be stretched or launched
	{
		// Put arm limbs into holding position and rotation
		applyHandPosition();

		// Make necessary rigidbodies kinematic, i.e parent rigidbody, spines, legs
		parentRigidbody.MakeKinematic( true ); // make parent rigidbody kinematic
		ChangeKinematicRigidbody( rotatingLimbs_rigidbodies, true ); // make spines and legs kinematic

		// Put rotating limbs into holding position and rotation
		for( var i = 0; i < rotatingLimbs_rigidbodies.Length; i++ )
		{
			rotatingLimbs_rigidbodies[ i ].transform.SetLocalTransformData( rotatingLimbs_holdingPositions[ i ] );
		}

		update = OffsetTheRagdoll; // Since every limb is kinematic we need to correctly offset the ragdoll every frame

		OffsetTheRagdoll();
	}

	// Correctly positions the ragdoll and rotates around hand joint
	protected void OffsetTheRagdoll()
	{
		var lastRotation_Z = parentRigidbody.transform.localEulerAngles.z; // cache parent rigidbody's rotation
		parentRigidbody.transform.localEulerAngles = Vector3.zero; // zero out the rotation of the parent rigidbody

		// Since the ragdoll is in a straight position, attached hand and torso has a fixed distance. If this distance is applied to 
		// hand's attached point we will have the point where parent rigidbody should be
		var parentRigidbody_Position       = attached_HandJoint.transform.position - ( attached_Hand.position - parentRigidbody.transform.position );
		parentRigidbody.transform.position = parentRigidbody_Position; 
		RotateToTargetAngle( lastRotation_Z ); // Rotate the parent rigidbody back to its first rotation
	}

	// Make every rigidbody in the ragdoll dynamic and zero out every velocity 
	protected void DefaultTheRagdoll()
	{
		update = ExtensionMethods.EmptyMethod; // Since every limb is back to being dynamic there is no need to offset the ragdoll anymore 

		parentRigidbody.gameObject.SetActive( true );
		actorNameDisplay.gameObject.SetActive( true );

		for( var i = 0; i < limbs_rigidbodies.Length; i++ )
		{
			limbs_rigidbodies[ i ].velocity         = Vector3.zero;
			limbs_rigidbodies[ i ].angularVelocity  = Vector3.zero;
			limbs_rigidbodies[ i ].MakeKinematic( false );
		}

		// Returns the actor back to collision layer for it to collide with other actors in the air etc.
		DOVirtual.DelayedCall( GameSettings.Instance.actor_changeCollisionLayer_WaitDuration, () => ChangeActorCollisionLayer( collisionLayer ) );
	}

	// Returns the ragdoll limbs to its T-Pose position and rotation
	private void TPoseTheRagdoll()
	{
		for( var i = 0; i < limbs_rigidbodies.Length; i++ )
		{
			limbs_rigidbodies[ i ].transform.SetLocalTransformData( limbs_holdPositions_TPose[ i ] );
		}
	}

	protected virtual void KillTweens()	
	{
		if( resetActorWaitTween != null )
		{
			resetActorWaitTween.Kill();
			resetActorWaitTween = null;
		}
	}
	
	// Nulls the connected body of FixedJoints for hands to release hands rigidbodies to act freely 
	protected virtual void ReleaseHands()
	{
		handJoint_Left.Release();
		handJoint_Right.Release();
	}

	protected abstract void LevelStartResponse();

	protected virtual void OnHandsAttached()
	{
		var spawnPosition = handTargetPoint;
		spawnPosition.z   = GameSettings.Instance.particle_spawnPosition_Z;

		particleSpawnEvent.changePosition = true;
		particleSpawnEvent.spawnPoint     = spawnPosition;
		particleSpawnEvent.particleAlias  = "actor_attached";
		// particleSpawnEvent.Raise();
	}

	private void ApplyArmPosition_Left()
	{
		ApplyArmPosition( arm_left_limbs, arm_holdingPositions_Left );
	}

	private void ApplyArmPosition_Right()
	{
		ApplyArmPosition( arm_right_limbs, arm_holdingPositions_Right );
	}

	// Make arm limbs kinematic and put them into holding position and rotation
	private void ApplyArmPosition( Rigidbody[] armLimbs, TransformData[] armPositions )
	{
		// for( var i = 0; i < armLimbs.Length - 1; i++ )
		for( var i = 0; i < armLimbs.Length ; i++ )
		{
			armLimbs[ i ].transform.SetLocalTransformData( armPositions[ i ] );
			// armLimbs[ i ].MakeKinematic( true );
		}

		ChangeKinematicRigidbody( armLimbs, true );
	}

	private void ChangeKinematicRigidbody( Rigidbody[] rigidbodies, bool isKinematic )
	{
		for( var i = 0; i < rigidbodies.Length; i++ )
		{
			rigidbodies[ i ].MakeKinematic( isKinematic );
		}
	}

	private void ChangeActorCollisionLayer(int index)
	{
		for( var i = 0; i < limbs_rigidbodies.Length; i++ )
		{
			limbs_rigidbodies[ i ].gameObject.layer = index;
		}
	}

	private void UpdateWayPointIndex( GameObject platformObject )
	{
		handTargetObject = platformObject;

		platformSet.itemDictionary.TryGetValue( platformObject.GetInstanceID(), out currentPlatform );

		currentWayPoint = currentPlatform.platformIndex;

		if( currentWayPoint + 1 == platformSet.itemDictionary.Count / 2 )
		{
			actor_Finished_Race.eventValue = this;
			actor_Finished_Race.Raise();
		}
	}
#endregion

#region EditorOnly
#if UNITY_EDITOR
	// Gets called in the editor. Serializes current position and rotation of **ARMS** into arrays to use it in run time. Serialization saved into scene data which is .unity file
	private void SerializeArmPositions()
	{
		arm_holdingPositions_Left  = new TransformData[ arm_left_limbs.Length ];
		arm_holdingPositions_Right = new TransformData[ arm_right_limbs.Length ];

		for( var i = 0; i < arm_left_limbs.Length; i++ )
		{
			arm_holdingPositions_Left[ i ]  = arm_left_limbs[ i ].transform.GetLocalTransformData();
			arm_holdingPositions_Right[ i ] = arm_right_limbs[ i ].transform.GetLocalTransformData();
		}
	}

	// Gets called in the editor. Serializes current position and rotation of **ROTATION LIMBS** into arrays to use it in run time. Serialization saved into scene data which is .unity file
	[ Button() ]
	protected void SerializeRotatingLimbsPosition()
	{
		rotatingLimbs_holdingPositions = new TransformData[ rotatingLimbs_rigidbodies.Length ];

		for( var i = 0; i < rotatingLimbs_rigidbodies.Length; i++ )
		{
			rotatingLimbs_holdingPositions[ i ]  = rotatingLimbs_rigidbodies[ i ].transform.GetLocalTransformData();
		}
	}

	// Gets called in the editor. Serializes current position and rotation of every **Rigidbody** that **Ragdoll** has into arrays to use in run time. Serialization saved into scene data which is .unity file
	private void SerializeTPoseLimbsPosition()
	{
		var limbs_rigidbodies = parentRigidbody.GetComponentsInChildren< Rigidbody >(); // Get every rigidbody that ragdoll has

		limbs_holdPositions_TPose = new TransformData[ limbs_rigidbodies.Length ];

		for( var i = 0; i < limbs_rigidbodies.Length; i++ )
		{
			limbs_holdPositions_TPose[ i ] = limbs_rigidbodies[ i ].transform.GetLocalTransformData();
		}
	}

	private void ApplyArmPositions()
	{
		for( var i = 0; i < arm_left_limbs.Length; i++ )
		{
			arm_left_limbs[ i ].transform.SetLocalTransformData( arm_holdingPositions_Left[ i ] );
			arm_right_limbs[ i ].transform.SetLocalTransformData( arm_holdingPositions_Right[ i ] );
		}	
	}

	private void ApplyRotatingLimbPositions()
	{
		for( var i = 0; i < rotatingLimbs_rigidbodies.Length; i++ )
		{
			rotatingLimbs_rigidbodies[ i ].transform.SetLocalTransformData( rotatingLimbs_holdingPositions[ i ] );
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere( arm_left_limbs[ 0 ].transform.position, armReachDistance );
		Gizmos.DrawWireSphere( arm_right_limbs[ 0 ].transform.position, armReachDistance );
	}

#endif
#endregion
}