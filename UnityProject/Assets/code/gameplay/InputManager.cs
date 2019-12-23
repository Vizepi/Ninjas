// © Copyright 2019 J. KIEFFER - All Rights Reserved.
using System;
using UnityEngine;

namespace vzp {
	public class InputManager : Singleton< InputManager > {
		//=============================================================================================
		public enum ActionName {
			Left,
			Right,
			Up,
			Down,
			Jump,
			Attack,
			Object1,
			Object2,
			Hide,
			Teleport,
			Pause,
			Confirm,
			Cancel
		}

		//=============================================================================================
		public enum ActionPolicy {
			KeyMouseOnly,
			GamepadOnly,
			KeyMouseAndGamepad
		}

		//=============================================================================================
		[Serializable]
		public struct ActionDeviceMapping {
			[Tooltip( "Enable if the input is an axis" )]
			public bool isAxis;
			[Tooltip( "The key code associated to the input. Ignored if axis" )]
			public KeyCode keyCode;
			[Tooltip( "The axis associated to the input. Ignored if not an axis" )]
			public string axisName;
			[Tooltip( "Enable for a negative direction (min/max and threshold test are inverted)" )]
			public bool invertAxis;
			[Tooltip( "The minimal value of the axis (deadzone). Ignored if not an axis" )]
			public float axisMinValue;
			[Tooltip( "The maximal value of the axis. Ignored if not an axis" )]
			public float axisMaxValue;
			[Tooltip( "The input is considered pressed if the value is above this threshold. Ignored if not an axis" )]
			public float axisPressThreshold;
		}

		//=============================================================================================
		[Serializable]
		public struct ActionMapping {
			[Tooltip( "Mapping for keyboard and mouse" )]
			public ActionDeviceMapping keyMouse;
			[Tooltip( "Mapping for gamepad" )]
			public ActionDeviceMapping gamepad;
		}

		//=============================================================================================
		public struct KeyState {
			public bool isPressed;
			public bool wasPressed;
			public bool justPressed;
			public bool justReleased;
			public float value;
		}

		//=============================================================================================
		public struct ActionState {
			public KeyState keyMouse;
			public KeyState gamepad;
			public KeyState state;
		}

		//=============================================================================================
		[Serializable]
		public class Action {
			public ActionMapping mapping;
			[NonSerialized]
			public ActionState state;
		}

		//=============================================================================================
		[Header( "Mappings" )]
		[SerializeField, Tooltip( "Left action mapping" )]
		ActionMapping m_leftMapping;
		[SerializeField, Tooltip( "Right action mapping" )]
		ActionMapping m_rightMapping;
		[SerializeField, Tooltip( "Up action mapping" )]
		ActionMapping m_upMapping;
		[SerializeField, Tooltip( "Down action mapping" )]
		ActionMapping m_downMapping;
		[SerializeField, Tooltip( "Jump action mapping" )]
		ActionMapping m_jumpMapping;
		[SerializeField, Tooltip( "Attack action mapping" )]
		ActionMapping m_attackMapping;
		[SerializeField, Tooltip( "Object1 action mapping" )]
		ActionMapping m_object1Mapping;
		[SerializeField, Tooltip( "Object2 action mapping" )]
		ActionMapping m_object2Mapping;
		[SerializeField, Tooltip( "Hide action mapping" )]
		ActionMapping m_hideMapping;
		[SerializeField, Tooltip( "Teleport action mapping" )]
		ActionMapping m_teleportMapping;
		[SerializeField, Tooltip( "Pause action mapping" )]
		ActionMapping m_pauseMapping;
		[SerializeField, Tooltip( "Confirm action mapping" )]
		ActionMapping m_confirmMapping;
		[SerializeField, Tooltip( "Cancel action mapping" )]
		ActionMapping m_cancelMapping;

		//=============================================================================================
		[SerializeField, HideInInspector]
		Action[] m_actions = new Action[ Enum.GetNames( typeof( ActionName ) ).Length ];
		ActionPolicy m_policy = ActionPolicy.KeyMouseAndGamepad;

		//=============================================================================================
		public Action this[ ActionName _index ] {
			get {
				return m_actions[ ( int )_index ];
			}
		}

		//=============================================================================================
		void Awake() {
			InitSingleton();

			m_actions[ ( int )ActionName.Left ] = new Action { mapping = m_leftMapping };
			m_actions[ ( int )ActionName.Right ] = new Action { mapping = m_rightMapping };
			m_actions[ ( int )ActionName.Up ] = new Action { mapping = m_upMapping };
			m_actions[ ( int )ActionName.Down ] = new Action { mapping = m_downMapping };
			m_actions[ ( int )ActionName.Jump ] = new Action { mapping = m_jumpMapping };
			m_actions[ ( int )ActionName.Attack ] = new Action { mapping = m_attackMapping };
			m_actions[ ( int )ActionName.Object1 ] = new Action { mapping = m_object1Mapping };
			m_actions[ ( int )ActionName.Object2 ] = new Action { mapping = m_object2Mapping };
			m_actions[ ( int )ActionName.Hide ] = new Action { mapping = m_hideMapping };
			m_actions[ ( int )ActionName.Teleport ] = new Action { mapping = m_teleportMapping };
			m_actions[ ( int )ActionName.Pause ] = new Action { mapping = m_pauseMapping };
			m_actions[ ( int )ActionName.Confirm ] = new Action { mapping = m_confirmMapping };
			m_actions[ ( int )ActionName.Cancel ] = new Action { mapping = m_cancelMapping };
		}

		//=============================================================================================
		void OnDestroy() {
			ShutdownSingleton();
		}

		//=============================================================================================
		void UpdateKeyState( ref ActionDeviceMapping _deviceMapping, ref KeyState _newState, ref KeyState _oldState ) {
			if ( _deviceMapping.isAxis ) {
				if ( _deviceMapping.invertAxis ) {
					_newState.value = Mathf.Clamp( Input.GetAxis( _deviceMapping.axisName ), _deviceMapping.axisMaxValue, _deviceMapping.axisMinValue );
					_newState.isPressed = _newState.value < _deviceMapping.axisPressThreshold;
				} else {
					_newState.value = Mathf.Clamp( Input.GetAxis( _deviceMapping.axisName ), _deviceMapping.axisMinValue, _deviceMapping.axisMaxValue );
					_newState.isPressed = _newState.value > _deviceMapping.axisPressThreshold;
				}
			} else {
				_newState.isPressed = Input.GetKey( _deviceMapping.keyCode );
				_newState.value = _newState.isPressed ? _deviceMapping.axisMaxValue : _deviceMapping.axisMinValue;
			}
			_newState.wasPressed = _oldState.isPressed;
			_newState.justPressed = _newState.isPressed && !_newState.wasPressed;
			_newState.justReleased = !_newState.isPressed && _newState.wasPressed;
		}

		//=============================================================================================
		void Update() {
			foreach ( Action action in m_actions ) {
				ActionState newState = new ActionState();

				if ( m_policy != ActionPolicy.GamepadOnly ) {
					UpdateKeyState( ref action.mapping.keyMouse, ref newState.keyMouse, ref action.state.keyMouse );
				}

				if ( m_policy != ActionPolicy.KeyMouseOnly ) {
					UpdateKeyState( ref action.mapping.gamepad, ref newState.gamepad, ref action.state.gamepad );
				}

				if ( m_policy == ActionPolicy.KeyMouseAndGamepad ) {
					newState.state.isPressed = newState.keyMouse.isPressed || newState.gamepad.isPressed;
					newState.state.wasPressed = newState.keyMouse.wasPressed || newState.gamepad.wasPressed;

					if ( action.mapping.keyMouse.isAxis == action.mapping.gamepad.isAxis ) {
						newState.state.value = ( newState.keyMouse.value + newState.gamepad.value ) * 0.5f;
					} else if ( action.mapping.keyMouse.isAxis ) {
						newState.state.value = newState.keyMouse.value;
					} else { // action.mapping.gamepad.isAxis
						newState.state.value = newState.gamepad.value;
					}
				} else if ( m_policy == ActionPolicy.KeyMouseOnly ) {
					newState.state = newState.keyMouse;
				} else { // m_policy == ActionPolicy.GamepadOnly
					newState.state = newState.gamepad;
				}

				newState.state.justPressed = newState.state.isPressed && !newState.state.wasPressed;
				newState.state.justReleased = !newState.state.isPressed && newState.state.wasPressed;

				action.state = newState;
			}
		}

		//=============================================================================================
		Action GetAction( ActionName _name ) {
			return m_actions[ ( int )_name ];
		}
	}
}
