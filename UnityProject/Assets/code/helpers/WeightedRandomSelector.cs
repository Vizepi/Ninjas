// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using System.Collections.Generic;

namespace vzp {
	public class WeightedRandomSelector {
		//=============================================================================================
		public static T Pick<T>( IEnumerable<T> _collection, Func<T, float> _weightGet, float _random ) {
			float totalWeight = 0.0f;

			foreach ( T element in _collection ) {
				totalWeight += _weightGet( element );
			}

			return Pick( _collection, _weightGet, totalWeight, _random );
		}

		//=============================================================================================
		public static T Pick<T>( IEnumerable< T > _collection, Func<T, float> _weightGet, float _totalWeight, float _random ) {
			float accumulatedWeight = 0.0f;
			float randomWeighted = _totalWeight * _random;

			foreach ( T element in _collection ) {
				float weight = _weightGet( element );
				if ( weight == 0 ) {
					continue;
				}
				accumulatedWeight += weight;

				if ( accumulatedWeight >= randomWeighted ) {
					return element;
				}
			}

			return default( T );
		}
	}
}
