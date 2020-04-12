// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;

namespace vzp {
	public class RandomGenerator {
		//=============================================================================================
		const int kMultiplier = 0x08200001;
		const int kAdditive = 0x00004801;
		//=============================================================================================
		public int Seed { get; set; }

		//=============================================================================================
		public RandomGenerator() {
			Seed = new Random().Next();
		}

		//=============================================================================================
		public RandomGenerator( int _seed ) {
			Seed = _seed;
		}

		//=============================================================================================
		public int NextInt() {
			Seed = Seed * kMultiplier + kAdditive;
			return Seed;
		}

		//=============================================================================================
		public int NextInt( int _min, int _max ) {
			return NextInt() % ( _max - _min ) + _min;
		}

		//=============================================================================================
		public float NextFloat() {
			return ( ( float )NextInt() ) / int.MaxValue;
		}

		//=============================================================================================
		public float NextFloat( float _min, float _max ) {
			return NextFloat() * ( _max - _min ) + _min;
		}
	}
}
