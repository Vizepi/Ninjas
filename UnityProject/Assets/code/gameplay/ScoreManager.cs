// Copyright 2020 J. KIEFFER - All Rights Reserved.
using UnityEngine;

namespace vzp {
	public class ScoreManager : GameSystemManager {
		[SerializeField, Tooltip( "Lifetime of a combo during wich it can be improved (s)" )]
		float m_comboLifetime = 5.0f;

		float m_comboTimer = 0.0f;
		long m_score = 0;
		float m_combo = 0.0f;

		//=============================================================================================
		public float ComboLifetime {
			get { return m_comboLifetime; }
		}

		//=============================================================================================
		public float ComboTimer {
			get { return m_comboTimer; }
		}

		//=============================================================================================
		public long Score {
			get { return m_score; }
		}

		//=============================================================================================
		public float Combo {
			get { return m_combo; }
		}

		//=============================================================================================
		public override void OnUpdate() {
			if ( m_comboTimer > 0.0f ) {
				m_comboTimer -= Time.deltaTime; // TODO jkieffer - Use GameTime
				if ( m_comboTimer <= Mathf.Epsilon ) {
					m_comboTimer = 0.0f;
					m_combo = 0.0f;
				}
			}
		}

		//=============================================================================================
		public void AddScore( int _score, float _comboAdded ) {
			if ( _score <= 0 ) {
				return;
			}

			m_score += ( long )( _score * ( 1.0f + m_combo ) );
			m_combo += _comboAdded;

			if ( _comboAdded >= Mathf.Epsilon ) {
				m_comboTimer = m_comboLifetime;
			}
		}
	}
}
